from ppoAgent import *
import json

class MultiAgentPPO:
    def __init__(self, num_agents, state_dim, action_dim, **agent_kwargs):
        self.num_agents = num_agents
        self.agents = [
            PPOAgent(state_dim, action_dim, **agent_kwargs) for _ in range(num_agents)
        ]

    def choose_actions(self, states):
        actions = []
        probs = []
        for agent, state in zip(self.agents, states):
            action, prob = agent.choose_action(state)
            actions.append(action)
            probs.append(prob)
        return actions, probs

    def update_agents(self, trajectories):
        for agent, agent_trajectories in zip(self.agents, trajectories):
            agent.update(agent_trajectories)

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
