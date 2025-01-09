import torch
import torch.nn as nn
import torch.optim as optim
import os
from collections import deque
import pickle
from torch.utils.data import DataLoader, TensorDataset
import random
import torch.cuda.amp as amp
import numpy as np

# Hyperparameters
learning_rate = 0.001
gamma = 0.99
epsilon = 1.0
epsilon_decay = 0.995
epsilon_min = 0.1
BUFFER_SIZE = 50000
batch_size = 256
MIN_REPLAY_SIZE = 1000
torch.backends.cudnn.benchmark = True
torch.set_num_threads(6)  # Match this to your CPU's core count


class DQNAgent:
    def __init__(self, state_dim, action_dim, device):
        self.device = device
        self.state_dim = state_dim
        self.action_dim = action_dim
        self.epsilon = epsilon
        self.replay_buffer = deque(maxlen=BUFFER_SIZE)
        self.global_step = 0

        self.scaler = amp.GradScaler()

        # Neural networks
        self.eval_net = self._build_network().to(device)
        self.target_net = self._build_network().to(device)
        self.update_target_network()

        # Optimizer and loss
        self.optimizer = optim.Adam(self.eval_net.parameters(), lr=learning_rate)
        self.loss_fn = nn.MSELoss()

    def _build_network(self):
        # Define a conditional batch normalization layer
        def safe_batch_norm(size):
            return nn.BatchNorm1d(size) if len(self.replay_buffer) > MIN_REPLAY_SIZE else nn.Identity()

        return nn.Sequential(
            nn.Linear(self.state_dim, 256),
            safe_batch_norm(256), 
            nn.ReLU(),
            nn.Linear(256, 256),
            safe_batch_norm(256),
            nn.ReLU(),
            nn.Linear(256, self.action_dim)
        )

    

    def update_target_network(self):
        """Update the target network to match the evaluation network."""
        self.target_net.load_state_dict(self.eval_net.state_dict())

    def create_dataloader(self):
        # Convert replay buffer to tensors
        if len(self.replay_buffer) < MIN_REPLAY_SIZE:
            return None  # Skip training if insufficient samples

        states, actions, rewards, next_states, dones = zip(*self.replay_buffer)

        # Convert to GPU-supported tensors
        dataset = TensorDataset(
            torch.FloatTensor(states),
            torch.LongTensor(actions),
            torch.FloatTensor(rewards),
            torch.FloatTensor(next_states),
            torch.FloatTensor(dones)
        )

        # Create DataLoader
        dataloader = DataLoader(
            dataset,
            batch_size=batch_size,
            shuffle=True,
            pin_memory=True,
            num_workers=4
        )
        return dataloader

    def store_transition(self, state, action, reward, next_state, done):
        if len(state) != self.state_dim or len(next_state) != self.state_dim:
            print(f"Invalid transition dimensions: {len(state)}, {len(next_state)}")
            return
        self.replay_buffer.append((state, action, reward, next_state, done))
       


    def learn(self, update_target_every=100):
        if len(self.replay_buffer) < MIN_REPLAY_SIZE:
            return

        # Sample replay buffer
        states, actions, rewards, next_states, dones = self.sample_replay_buffer(batch_size)

        # Ensure all tensors are on the same device
        states = states.to(self.device)  # [batch_size, state_dim]
        actions = actions.to(self.device).view(-1, 1)  # [batch_size, 1]
        rewards = rewards.to(self.device).view(-1, 1)  # [batch_size, 1]
        next_states = next_states.to(self.device)  # [batch_size, state_dim]
        dones = dones.to(self.device).view(-1, 1)  # [batch_size, 1]

        with amp.autocast():  # Enable mixed precision on CUDA
            # Compute Q-values
            q_values = self.eval_net(states).gather(1, actions)  # [batch_size, 1]

            # Compute target Q-values
            next_q_values = self.target_net(next_states).max(1, keepdim=True)[0].detach()  # [batch_size, 1]
            q_targets = rewards + gamma * next_q_values * (1 - dones)  # [batch_size, 1]

            # Compute loss
            loss = self.loss_fn(q_values, q_targets)  # Ensure q_values and q_targets have the same shape

        # Perform gradient descent with mixed precision
        self.optimizer.zero_grad()
        self.scaler.scale(loss).backward()
        self.scaler.step(self.optimizer)
        self.scaler.update()

        # Update target network periodically
        self.global_step += 1
        if self.global_step % update_target_every == 0:
            self.update_target_network()

    def sample_replay_buffer(self, batch_size):
    # Use random.sample for GPU-compatible sampling
        indices = torch.randint(0, len(self.replay_buffer), (batch_size,), device=self.device)
        batch = [self.replay_buffer[i] for i in indices.tolist()]
        #batch = random.sample(self.replay_buffer, batch_size)
        states, actions, rewards, next_states, dones = zip(*batch)
        return (
            torch.FloatTensor(states).to(self.device),
            torch.LongTensor(actions).unsqueeze(1).to(self.device),
            torch.FloatTensor(rewards).unsqueeze(1).to(self.device),
            torch.FloatTensor(next_states).to(self.device),
            torch.FloatTensor(dones).unsqueeze(1).to(self.device),
        )




    def generate_action_values(self, state):
        """Generate sorted action values for debugging or evaluation."""
        self.epsilon = max(epsilon_min, self.epsilon * epsilon_decay)
        state_tensor = torch.tensor(state, dtype=torch.float32, device=self.device).unsqueeze(0)

        with torch.no_grad():
            q_values = self.eval_net(state_tensor).squeeze()

        sorted_actions = torch.argsort(q_values, descending=True).tolist()
        return sorted_actions

    def save_checkpoint(self, filepath):
        """Save the current state of the agent to a checkpoint."""
        checkpoint = {
            "eval_net_state_dict": self.eval_net.state_dict(),
            "target_net_state_dict": self.target_net.state_dict(),
            "optimizer_state_dict": self.optimizer.state_dict(),
            "epsilon": self.epsilon,
        }
        os.makedirs(os.path.dirname(filepath), exist_ok=True)
        torch.save(checkpoint, filepath)
        print("checkpoint was saved !")

        replay_buffer_path = filepath + "_replay.pkl"
        with open(replay_buffer_path, "wb") as f:
            pickle.dump(list(self.replay_buffer), f)
     

    def load_checkpoint(self, filepath):
        """Load a saved state from a checkpoint."""
        if not os.path.exists(filepath):
            print(f"No checkpoint found at {filepath}")
            return False

        checkpoint = torch.load(filepath, weights_only=True)
        self.eval_net.load_state_dict(checkpoint["eval_net_state_dict"])
        self.target_net.load_state_dict(checkpoint["target_net_state_dict"])
        self.optimizer.load_state_dict(checkpoint["optimizer_state_dict"])
        self.epsilon = checkpoint["epsilon"]
        print("Checkpoint was loaded dudes :)")

        replay_buffer_path = filepath + "_replay.pkl"
        if os.path.exists(replay_buffer_path):
            with open(replay_buffer_path, "rb") as f:
                self.replay_buffer = deque(pickle.load(f), maxlen=BUFFER_SIZE)
        else:
            print(f"No replay buffer found at {replay_buffer_path}")

        return True

    def update_with_final_rewards(self, final_reward, reward_decay=gamma):
        """Distribute the final reward across all transitions with a decay factor."""
        n = len(self.replay_buffer)
        discounted_reward = final_reward
        total_discount = sum(reward_decay**i for i in range(n))

        updated_buffer = []
        for i, (state, action, reward, next_state, done) in enumerate(self.replay_buffer):
            reward_contribution = (reward_decay**i / total_discount) * final_reward
            updated_reward = reward + reward_contribution
            updated_buffer.append((state, action, updated_reward, next_state, done))

        self.replay_buffer = deque(updated_buffer, maxlen=BUFFER_SIZE)
