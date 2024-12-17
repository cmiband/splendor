import numpy as np
import json
import torch
from multiagentTrainer import *

class EnvironmentSimulator:
    """
    Simulates communication with the C# game environment.
    Acts as a stand-in for the actual API server.
    """
    def __init__(self, num_agents, state_dim):
        self.num_agents = num_agents
        self.state_dim = state_dim

    def get_game_state(self):
        """
        Simulates receiving the game state as a JSON string.
        :return: JSON string with states for all agents.
        """
        states = [np.random.rand(self.state_dim).tolist() for _ in range(self.num_agents)]
        return json.dumps({"states": states})

    def send_actions(self, actions_json):
        """
        Simulates sending actions back to the game environment.
        Prints the actions for inspection.
        """
        print(f"Actions sent to environment: {actions_json}")


# Initialize environment simulator and multi-agent DQN
num_agents = 4
state_dim = 348
action_dim = 43

env_simulator = EnvironmentSimulator(num_agents=num_agents, state_dim=state_dim)
multi_agent_dqn = MultiAgentDQN(num_agents=num_agents, state_dim=state_dim, action_dim=action_dim)

# Simulated environment loop
trainer = MultiAgentTrainer("PPO", num_agents=4, state_dim=348, action_dim=43)

for episode in range(1, 11):
    print(f"\n=== Episode {episode} ===")
    states = env_simulator.get_game_state()
    states = trainer.agent_manager.parse_states_from_api(states)
    
    trajectories = [[] for _ in range(num_agents)]  # For PPO

    for t in range(100):  # Example: 100 steps per episode
        actions, probs = trainer.choose_actions(states)
        actions_json = trainer.agent_manager.format_actions_for_api(actions)
        env_simulator.send_actions(actions_json)

        next_states = [np.random.rand(state_dim) for _ in range(num_agents)]
        rewards = [np.random.uniform(-1, 1) for _ in range(num_agents)]
        dones = [np.random.choice([True, False]) for _ in range(num_agents)]

        if trainer.algorithm == "DQN":
            trainer.store_transitions_or_trajectories(states, actions, rewards, next_states, dones)
        elif trainer.algorithm == "PPO":
            for i in range(num_agents):
                value = trainer.agent_manager.agents[i].value_net(torch.FloatTensor(states[i])).item()
                trajectories[i].append((states[i], actions[i], rewards[i], dones[i], probs[i], value))

        states = next_states
        if all(dones):
            break
        
    # for t in range(max_steps):
        # actions = trainer.choose_actions(states)
    
        # # Send actions to C# and receive feedback
        # actions_json = trainer.agent_manager.format_actions_for_api(actions)
        # env_simulator.send_actions(actions_json)
    
        # # Get next states, rewards, and illegal moves from C#
        # next_states = ...  # Fetch from C# (API or simulation)
        # rewards = ...      # Fetch rewards
        # dones = ...        # Fetch done flags
        # illegal_moves_list = ...  # Fetch illegal moves from C#
    
        # # Handle illegal moves
        # trainer.handle_illegal_moves(states, illegal_moves_list)

        # # Continue with training
        # trainer.store_transitions_or_trajectories(states, actions, rewards, next_states, dones)
        # ...


    if trainer.algorithm == "PPO":
        trainer.learn_or_update(trajectories)
    else:
        trainer.learn_or_update()

    if episode % 2 == 0 and trainer.algorithm == "DQN":
        trainer.update_target_networks()

