import numpy as np
from multiagentTrainer import MultiAgentTrainer
import json

# Initialize trainer and environment configuration
NUM_AGENTS = 4
STATE_DIM = 348
ACTION_DIM = 43

trainer = MultiAgentTrainer("PPO", num_agents=NUM_AGENTS, state_dim=STATE_DIM, action_dim=ACTION_DIM)


previous_data = {
    i: {"state": None, "action": None, "action_prob": None, "done": False}
    for i in range(NUM_AGENTS)
}

trajectory_buffer = {i: [] for i in range(NUM_AGENTS)}

def store_transition(agent_index, state, action, reward, next_state, done, action_prob):
    trajectory_buffer[agent_index].append((state, action, reward, next_state, done, action_prob))


def step(current_agent_index, state, previous_reward):
    """
    Processes the current agent's step and stores the previous agent's transition.
    """
    try:
        # Validate the state dimensions
        if len(state) != STATE_DIM:
            return "Invalid state dimension", 400

        # Previous agent index
        previous_agent_index = (current_agent_index - 1) % NUM_AGENTS

        # Complete the transition for the previous agent
        if previous_data[previous_agent_index]["state"] is not None:
            transition = (
                previous_data[previous_agent_index]["state"],  # Previous state
                previous_data[previous_agent_index]["action"],  # Action taken
                previous_reward,  # Reward received
                state,  # Current state becomes the next state for the previous agent
                previous_data[previous_agent_index]["done"],  # Done flag
                previous_data[previous_agent_index]["action_prob"],  # Action probability
            )
            store_transition(previous_agent_index, *transition)

        # Generate action for the current agent
        current_agent = trainer.agent_manager.agents[current_agent_index]
        action, action_prob = current_agent.generate_action(state)

        # Update the buffer with the current agent's data
        previous_data[current_agent_index] = {
            "state": state,
            "action": action,
            "action_prob": action_prob,
            "done": False,  # You may modify this based on your termination logic
        }
        # previous_data[current_agent_index]["done"] = True
        return action.tolist()
    except Exception as e:
        return {"error": str(e)}, 500


    
with open ("przy.json") as file:
    przyklasowy_json = json.load(file)

przyklasowy_input = przyklasowy_json['state']
przyklasowy_reward = przyklasowy_json['reward']

print(step(0, przyklasowy_input, przyklasowy_reward))



