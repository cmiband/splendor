from multiagentDQN import MultiAgentDQN
from multiagentPPO import MultiAgentPPO
import os
import torch

class MultiAgentTrainer:
    def __init__(self, algorithm, num_agents, state_dim, action_dim, **agent_kwargs):
        """
        Initialize a multi-agent system with the specified algorithm (DQN or PPO).
        :param algorithm: "DQN" or "PPO"
        :param num_agents: Number of agents.
        :param state_dim: Dimension of the state space.
        :param action_dim: Dimension of the action space.
        """
        self.algorithm = algorithm
        if algorithm == "DQN":
            self.agent_manager = MultiAgentDQN(num_agents, state_dim, action_dim, **agent_kwargs)
        elif algorithm == "PPO":
            self.agent_manager = MultiAgentPPO(num_agents, state_dim, action_dim, **agent_kwargs)
        else:
            raise ValueError("Unsupported algorithm: choose 'DQN' or 'PPO'.")

    def choose_actions(self, states):
        actions = self.agent_manager.choose_actions(states)
        if self.algorithm == "DQN":
            return actions, None  # Add a placeholder for probabilities
        return actions  # PPO already returns both actions and probabilities

    def store_transitions_or_trajectories(self, *args):
        """Store transitions for DQN or trajectories for PPO."""
        if self.algorithm == "DQN":
            self.agent_manager.store_transitions(*args)
        elif self.algorithm == "PPO":
            self.agent_manager.update_agents(*args)

    def learn_or_update(self, trajectories=None):
        """Trigger learning or policy update."""
        if self.algorithm == "DQN":
            self.agent_manager.learn()
        elif self.algorithm == "PPO":
            self.agent_manager.update_agents(trajectories)

    def update_target_networks(self):
        """Update target networks (DQN only)."""
        if self.algorithm == "DQN":
            self.agent_manager.update_target_networks()


    def save_all_agents(self, directory):
        """
        Save checkpoints for all agents.
        """
        os.makedirs(directory, exist_ok=True)
        for i, agent in enumerate(self.agent_manager.agents):
            agent.save_checkpoint(os.path.join(directory, f"agent_{i}_checkpoint.pth"))

    def load_all_agents(self, directory):
        """
        Load checkpoints for all agents.
        """
        for i, agent in enumerate(self.agent_manager.agents):
            filepath = os.path.join(directory, f"agent_{i}_checkpoint.pth")
            agent.load_checkpoint(filepath)

    def update_with_final_rewards(self, final_rewards):
        """
        Update agents' Q-values directly with the final rewards at the end of the game.
        :param trainer: The multi-agent trainer object.
        :param final_rewards: List of final rewards for each agent.
        """
        for i, reward in enumerate(final_rewards):
            agent = self.agent_manager.agents[i]
            # Get the states and actions from the replay buffer
            states, actions = [], []

            for transition in agent.replay_buffer:
                state, action, _, _, _ = transition  # Ignore reward, next_state, and done
                states.append(state)
                actions.append(action)

            # Convert to tensors
            states = torch.FloatTensor(states)
            actions = torch.LongTensor(actions).unsqueeze(1) - 1  # 0-based indexing

            # Compute Q-values for the actions taken
            q_values = agent.eval_net(states).gather(1, actions)

            # Use the final reward as the target for all transitions
            targets = torch.FloatTensor([reward] * len(states)).unsqueeze(1)

            # Compute loss and update
            loss = agent.loss_fn(q_values, targets)
            agent.optimizer.zero_grad()
            loss.backward()
            agent.optimizer.step()



