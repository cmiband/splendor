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

    def choose_actions(self, states):
        """
        Let all agents choose their actions based on their respective states.
        :param states: List of states, one per agent.
        :return: List of actions chosen by each agent.
        """
        actions = [agent.choose_action(state) for agent, state in zip(self.agents, states)]
        return actions

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

    def format_actions_for_api(self, actions):
        """
        Format the actions into a JSON-compatible structure for the API server.
        :param actions: List of actions chosen by each agent.
        :return: JSON string of actions.
        """
        return json.dumps({"actions": actions})

    def parse_states_from_api(self, json_data):
        """
        Parse the JSON input from the API server into a list of states.
        :param json_data: JSON string containing the states.
        :return: List of states, one per agent.
        """
        data = json.loads(json_data)
        states = data.get("states", [])
        return states


# Testing MultiAgentDQN class with mock data
multi_agent_dqn = MultiAgentDQN(num_agents=3, state_dim=348, action_dim=43)
mock_states = [np.random.rand(348) for _ in range(3)]
mock_actions = multi_agent_dqn.choose_actions(mock_states)

# Mock transition data
mock_next_states = [np.random.rand(348) for _ in range(3)]
mock_rewards = [1.0, -1.0, 0.5]
mock_dones = [False, False, True]

# Store transitions and perform learning
multi_agent_dqn.store_transitions(mock_states, mock_actions, mock_rewards, mock_next_states, mock_dones)
multi_agent_dqn.learn()

# Update target networks
multi_agent_dqn.update_target_networks()

# Format actions for API
actions_json = multi_agent_dqn.format_actions_for_api(mock_actions)
actions_json
