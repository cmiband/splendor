import numpy as np
import torch
import torch.nn as nn
import torch.optim as optim


import os
from collections import deque


learning_rate=0.001
gamma=0.99
epsilon=1.0
epsilon_decay=0.995
epsilon_min=0.1
BUFFER_SIZE=50000 # OK
batch_size=64
MIN_REPLAY_SIZE = 1000

class DQNAgent:
    def __init__(self, state_dim, action_dim):
        
        # Parameters
        self.state_dim = state_dim
        self.action_dim = action_dim
        self.epsilon = epsilon
        # Replay memory
        self.replay_buffer = deque(maxlen=BUFFER_SIZE) # OK
        
        # Neural networks
        self.eval_net = self._build_network() # OK
        self.target_net = self._build_network() # OK
        self.update_target_network() # OK

        # Optimizer and loss
        self.optimizer = optim.Adam(self.eval_net.parameters(), lr=learning_rate)
        self.loss_fn = nn.MSELoss()

    def _build_network(self): # OK
        return nn.Sequential(
            nn.Linear(self.state_dim, 128),
            nn.ReLU(),
            nn.Linear(128, 128),
            nn.ReLU(),
            nn.Linear(128, self.action_dim)
        )

    def update_target_network(self): # OK
        """Update the target network to match the evaluation network."""
        self.target_net.load_state_dict(self.eval_net.state_dict())

    def choose_action(self, state):
        """Choose an action using epsilon-greedy policy."""
        
        if np.random.rand() < self.epsilon:
            return np.random.randint(1, self.action_dim + 1)  # Random action (1 to action_dim)
        else:
            state_tensor = torch.FloatTensor(state).unsqueeze(0)
            q_values = self.eval_net(state_tensor)
            return torch.argmax(q_values).item() + 1  # Convert to 1-based indexing

    def store_transition(self, state, action, reward, next_state, done):

    # Ensure the replay buffer doesn't exceed its maximum size
        if len(self.replay_buffer) >= BUFFER_SIZE:
            self.replay_buffer.pop(0)  # Remove the oldest transition to make space
            
    # Add the new transition to the replay buffer
        self.replay_buffer.append((state, action, reward, next_state, done))



    def learn(self):
        """Sample from memory and perform a learning step."""
        if len(self.replay_buffer) < MIN_REPLAY_SIZE:
            return  # Not enough samples to learn

        # Sample a batch
        batch = np.random.choice(len(self.replay_buffer), BUFFER_SIZE, replace=False)
        states, actions, rewards, next_states, dones = zip(*[self.memory[i] for i in batch])

        states = torch.FloatTensor(states)
        actions = torch.LongTensor(actions).unsqueeze(1) - 1  # Convert to 0-based
        rewards = torch.FloatTensor(rewards).unsqueeze(1)
        next_states = torch.FloatTensor(next_states)
        dones = torch.FloatTensor(dones).unsqueeze(1)

        # Q-value computation
        q_values = self.eval_net(states).gather(1, actions)
        next_q_values = self.target_net(next_states).max(1, keepdim=True)[0].detach()
        q_targets = rewards + gamma * next_q_values * (1 - dones)

        # Loss and optimization
        loss = self.loss_fn(q_values, q_targets)
        self.optimizer.zero_grad()
        loss.backward()
        self.optimizer.step()


    def generate_action_values(self, state):

        """
        Generate and sort action values for a given state.
        """
        state_tensor = torch.FloatTensor(state).unsqueeze(0)
        action_values = self.policy_net(state_tensor).detach().numpy().squeeze()  # For DQN, use eval_net
        sorted_actions = np.argsort(action_values)[::-1]  # Sort actions by value (descending)
        return sorted_actions
    
    def punish_illegal_moves(self, state, illegal_moves):
        """
        Punish the agent for recommending illegal moves.
        """
        for move in illegal_moves:
            penalty = -1.0  # Adjust penalty value based on severity
            # Create a dummy next_state and done for punishment purposes
            dummy_next_state = np.zeros_like(state)
            done = False
            # Add penalty as a transition
            self.store_transition(state, move, penalty, dummy_next_state, done)
        self.learn()  # Update the model with penalties


        self.epsilon = max(epsilon_min, self.epsilon * epsilon_decay)
        if np.random.rand() < self.epsilon:
            # Random exploration: shuffle actions
            return np.random.permutation(range(1, self.action_dim + 1)).tolist()  # 1-based indexing
        else:
            state_tensor = torch.FloatTensor(state).unsqueeze(0)
            with torch.no_grad():
                q_values = self.eval_net(state_tensor).squeeze().numpy()

            sorted_actions = np.argsort(q_values)[::-1]  # Descending order
            return (sorted_actions + 1).tolist()  # Convert to 1-based indexing

    def save_checkpoint(self, filepath):
        """
        Save the current state of the agent to a checkpoint.
        """
        checkpoint = {
            "eval_net_state_dict": self.eval_net.state_dict(),
            "target_net_state_dict": self.target_net.state_dict(),
            "optimizer_state_dict": self.optimizer.state_dict(),
            "epsilon": self.epsilon,
            "memory": self.memory
        }
        os.makedirs(os.path.dirname(filepath), exist_ok=True)
        torch.save(checkpoint, filepath)
        print(f"Checkpoint saved at {filepath}")

    def load_checkpoint(self, filepath):
        """
        Load a saved state from a checkpoint.
        """
        if not os.path.exists(filepath):
            print(f"No checkpoint found at {filepath}")
            return False

        checkpoint = torch.load(filepath)
        self.eval_net.load_state_dict(checkpoint["eval_net_state_dict"])
        self.target_net.load_state_dict(checkpoint["target_net_state_dict"])
        self.optimizer.load_state_dict(checkpoint["optimizer_state_dict"])
        self.epsilon = checkpoint["epsilon"]
        self.memory = checkpoint["memory"]
        print(f"Checkpoint loaded from {filepath}")
        return True


