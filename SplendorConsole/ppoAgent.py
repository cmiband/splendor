import torch
import torch.nn as nn
import torch.optim as optim
import numpy as np

class PPOAgent:
    def __init__(
        self, 
        state_dim, 
        action_dim, 
        learning_rate=0.001, 
        gamma=0.99, 
        lambda_=0.95, 
        clip_ratio=0.2, 
        policy_epochs=10, 
        value_epochs=10, 
        entropy_coef=0.01
    ):
        self.state_dim = state_dim
        self.action_dim = action_dim
        self.gamma = gamma
        self.lambda_ = lambda_
        self.clip_ratio = clip_ratio
        self.policy_epochs = policy_epochs
        self.value_epochs = value_epochs
        self.entropy_coef = entropy_coef

        # Policy and Value networks
        self.policy_net = self._build_policy_network()
        self.value_net = self._build_value_network()
        
        # Optimizers
        self.policy_optimizer = optim.Adam(self.policy_net.parameters(), lr=learning_rate)
        self.value_optimizer = optim.Adam(self.value_net.parameters(), lr=learning_rate)

    def _build_policy_network(self):
        return nn.Sequential(
            nn.Linear(self.state_dim, 128),
            nn.ReLU(),
            nn.Linear(128, 128),
            nn.ReLU(),
            nn.Linear(128, self.action_dim),
            nn.Softmax(dim=-1)  # Action probabilities
        )

    def _build_value_network(self):
        return nn.Sequential(
            nn.Linear(self.state_dim, 128),
            nn.ReLU(),
            nn.Linear(128, 128),
            nn.ReLU(),
            nn.Linear(128, 1)  # State value
        )

    def choose_action(self, state):
        state_tensor = torch.FloatTensor(state).unsqueeze(0)
        action_probs = self.policy_net(state_tensor).detach().numpy().squeeze()
        action = np.random.choice(self.action_dim, p=action_probs)
        return action, action_probs[action]  # Action and its probability

    def compute_advantages(self, rewards, values, dones):
        advantages = []
        gae = 0
        values = np.append(values, 0)

        for t in reversed(range(len(rewards))):
            delta = rewards[t] + self.gamma * (1 - dones[t]) * values[t + 1] - values[t]
            gae = delta + self.gamma * self.lambda_ * (1 - dones[t]) * gae
            advantages.insert(0, gae)
        return advantages

    def update(self, trajectories):
        states, actions, rewards, dones, old_probs, values = zip(*trajectories)

        # Convert to tensors
        states = torch.FloatTensor(states)
        actions = torch.LongTensor(actions).unsqueeze(1)
        old_probs = torch.FloatTensor(old_probs).unsqueeze(1)
        rewards = torch.FloatTensor(rewards).unsqueeze(1)
        dones = torch.FloatTensor(dones).unsqueeze(1)
        values = torch.FloatTensor(values).unsqueeze(1)

        # Compute advantages
        advantages = self.compute_advantages(rewards, values, dones)
        advantages = torch.FloatTensor(advantages).unsqueeze(1)

        # Policy updates
        for _ in range(self.policy_epochs):
            logits = self.policy_net(states)
            action_probs = logits.gather(1, actions)
            ratios = action_probs / old_probs

            clipped_ratios = torch.clamp(ratios, 1 - self.clip_ratio, 1 + self.clip_ratio)
            surrogate_loss = torch.min(ratios * advantages, clipped_ratios * advantages)
            entropy = -torch.sum(logits * torch.log(logits + 1e-8), dim=-1).mean()

            loss = -surrogate_loss.mean() + self.entropy_coef * entropy
            self.policy_optimizer.zero_grad()
            loss.backward()
            self.policy_optimizer.step()

        # Value updates
        for _ in range(self.value_epochs):
            value_preds = self.value_net(states)
            value_loss = nn.MSELoss()(value_preds, rewards)
            self.value_optimizer.zero_grad()
            value_loss.backward()
            self.value_optimizer.step()

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
