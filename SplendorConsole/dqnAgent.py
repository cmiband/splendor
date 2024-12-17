import numpy as np
import torch
import torch.nn as nn
import torch.optim as optim


class DQNAgent:
    def __init__(
        self, 
        state_dim, 
        action_dim, 
        learning_rate=0.001, 
        gamma=0.99, 
        epsilon=1.0, 
        epsilon_decay=0.995, 
        epsilon_min=0.1, 
        memory_size=5000, 
        batch_size=64
    ):
        # Parameters
        self.state_dim = state_dim
        self.action_dim = action_dim
        self.lr = learning_rate
        self.gamma = gamma
        self.epsilon = epsilon
        self.epsilon_decay = epsilon_decay
        self.epsilon_min = epsilon_min
        self.batch_size = batch_size

        # Replay memory
        self.memory = []  # List of transitions (s, a, r, s', done)
        self.memory_size = memory_size

        # Neural networks
        self.eval_net = self._build_network()
        self.target_net = self._build_network()
        self.update_target_network()

        # Optimizer and loss
        self.optimizer = optim.Adam(self.eval_net.parameters(), lr=self.lr)
        self.loss_fn = nn.MSELoss()

    def _build_network(self):
        # Simple feedforward network
        return nn.Sequential(
            nn.Linear(self.state_dim, 128),
            nn.ReLU(),
            nn.Linear(128, 128),
            nn.ReLU(),
            nn.Linear(128, self.action_dim)
        )

    def update_target_network(self):
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
        """Store a transition in replay memory."""
        if len(self.memory) >= self.memory_size:
            self.memory.pop(0)  # Remove the oldest experience
        self.memory.append((state, action, reward, next_state, done))

    def learn(self):
        """Sample from memory and perform a learning step."""
        if len(self.memory) < self.batch_size:
            return  # Not enough samples to learn

        # Sample a batch
        batch = np.random.choice(len(self.memory), self.batch_size, replace=False)
        states, actions, rewards, next_states, dones = zip(*[self.memory[i] for i in batch])

        states = torch.FloatTensor(states)
        actions = torch.LongTensor(actions).unsqueeze(1) - 1  # Convert to 0-based
        rewards = torch.FloatTensor(rewards).unsqueeze(1)
        next_states = torch.FloatTensor(next_states)
        dones = torch.FloatTensor(dones).unsqueeze(1)

        # Q-value computation
        q_values = self.eval_net(states).gather(1, actions)
        next_q_values = self.target_net(next_states).max(1, keepdim=True)[0].detach()
        q_targets = rewards + self.gamma * next_q_values * (1 - dones)

        # Loss and optimization
        loss = self.loss_fn(q_values, q_targets)
        self.optimizer.zero_grad()
        loss.backward()
        self.optimizer.step()

        # Decay epsilon
        self.epsilon = max(self.epsilon_min, self.epsilon * self.epsilon_decay)

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


# Testing the DQNAgent class
agent = DQNAgent(state_dim=348, action_dim=43)
state = np.random.rand(348)
action = agent.choose_action(state)
agent.store_transition(state, action, 1.0, state, False)
agent.learn()

action  # Return the action chosen for inspection
