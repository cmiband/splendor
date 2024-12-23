from ppoAgent import *


class MultiAgentPPO:
    def __init__(self, num_agents, state_dim, action_dim, **agent_kwargs):
        self.num_agents = num_agents
        self.agents = [
            PPOAgent(state_dim, action_dim, **agent_kwargs) for _ in range(num_agents)
        ]

    def choose_actions(self, inputs):
        """
        Processes inputs sequentially for each agent and chooses actions.
        Args:
            inputs: List of inputs where each entry corresponds to the input for one agent.
                    Each input is an array containing standardized states of the game.
        Returns:
            actions: List of actions taken sequentially by each agent.
            probs: List of probabilities associated with the chosen actions.
        """
        actions = []
        probs = []

        for agent, input_state in zip(self.agents, inputs):
            # Each agent processes its respective input_state sequentially
            action, prob = agent.choose_action(input_state)
            actions.append(action)
            probs.append(prob)

        return actions, probs

    def update_agents(self, trajectories):
        for agent, agent_trajectories in zip(self.agents, trajectories):
            agent.update(agent_trajectories)
