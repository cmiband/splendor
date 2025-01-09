from agent_gpu import *

class MultiAgentDQN:
    def __init__(self, num_agents, state_dim, action_dim, device):
        self.num_agents = num_agents
        self.agents = [
            DQNAgent(state_dim, action_dim, device=device) for _ in range(num_agents)
        ]

    def __getitem__(self, index):
        return self.agents[index]

    def store_transitions(self, state, action, reward, next_state, done):
        for i in range(self.num_agents):
            self.agents[i].store_transition(state[i], action[i], reward[i], next_state[i], done[i])

    def learn(self):
        for agent in self.agents:
            agent.learn()

    def update_rewards(self, rewards):
        for i in range(len(self.agents)):
            self.agents[i].update_with_final_rewards(rewards[i])

    def load_all_agents(self, directory):
        for i, agent in enumerate(self.agents):
            filepath = os.path.join(directory, f"agent_{i}_checkpoint.pth")
            agent.load_checkpoint(filepath)

    def save_all_agents(self, directory):
        # Normalize the directory path
        directory = os.path.abspath(directory)
        os.makedirs(directory, exist_ok=True)
      
        
        for i, agent in enumerate(self.agents):
            filepath = os.path.join(directory, f"agent_{i}_checkpoint.pth")
            agent.save_checkpoint(filepath)

    def save_best_agents(self, directory):
        # Normalize the directory path
        directory = os.path.abspath(directory)
        os.makedirs(directory, exist_ok=True)
      
        
        for i, agent in enumerate(self.agents):
            filepath = os.path.join(directory, f"agent_best_{i}_checkpoint.pth")
            agent.save_checkpoint(filepath)

