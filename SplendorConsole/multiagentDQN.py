import json
from dqnAgent import *

class MultiAgentDQN:
    def __init__(self, num_agents, state_dim, action_dim, **agent_kwargs):
        """
        Multi-Agent DQN Manager to train and coordinate multiple agents.
        :param num_agents: Number of agents in the environment.
        :param state_dim: Dimension of the state representation.
        :param action_dim: Dimension of the action space (number of discrete actions).
        :param agent_kwargs: Additional arguments for each individual agent.
        """
        self.num_agents = num_agents
        self.agents = [
            DQNAgent(state_dim, action_dim, **agent_kwargs) for _ in range(num_agents)
        ]


    def store_transitions(self, states, actions, rewards, next_states, dones):
        """
        Store transitions for each agent.
        :param states: List of current states, one per agent.
        :param actions: List of actions taken by each agent.
        :param rewards: List of rewards received by each agent.
        :param next_states: List of next states for each agent.
        :param dones: List of done flags for each agent.
        """
        for i in range(self.num_agents):
            self.agents[i].store_transition(states[i], actions[i], rewards[i], next_states[i], dones[i])

    def learn(self):
        """
        Trigger the learning process for all agents.
        """
        for agent in self.agents:
            agent.learn()

    def update_target_networks(self):
        """
        Update the target networks of all agents.
        """
        for agent in self.agents:
            agent.update_target_network()


