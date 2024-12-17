from multiagentDQN import MultiAgentDQN
from multiagentPPO import MultiAgentPPO

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

    def handle_illegal_moves(self, states, illegal_moves_list):
        """
        Handle illegal moves by punishing agents.
        :param states: List of states for all agents.
        :param illegal_moves_list: List of illegal moves for each agent.
        """
        for i, illegal_moves in enumerate(illegal_moves_list):
            if illegal_moves:
                self.agent_manager.agents[i].punish_illegal_moves(states[i], illegal_moves)
