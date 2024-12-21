import numpy as np
from multiagentTrainer import MultiAgentTrainer

# Initialize trainer and environment configuration
NUM_AGENTS = 4
STATE_DIM = 348
ACTION_DIM = 43

trainer = MultiAgentTrainer("DQN", num_agents=NUM_AGENTS, state_dim=STATE_DIM, action_dim=ACTION_DIM)
current_agent_index = 0  # Tracks which agent's turn it is
last_rewards = [0.0] * NUM_AGENTS  # Store the last reward for each agent
trainer.load_all_agents("./checkpoints")


def step(current_agent_index, state, previous_reward):
    """
    API endpoint for receiving the game state from Unity, processing actions for the current agent,
    and returning the sorted action values.
    """
    try:
        if len(state) != STATE_DIM:
            return "Invalid state dimension", 400

        # Update reward for the previous agent
        previous_agent_index = (current_agent_index - 1) % NUM_AGENTS
        trainer.agent_manager.agents[previous_agent_index].store_transition(
            state=np.zeros(STATE_DIM),  # Placeholder state, no effect
            action=0,  # Placeholder action
            reward=previous_reward,
            next_state=np.zeros(STATE_DIM),  # Placeholder state, no effect
            done=False  # Not a terminal state
        )

        # Process the current agent's action
        current_agent = trainer.agent_manager.agents[current_agent_index]
        sorted_actions = current_agent.generate_action_values(state)

        # Learn for the previous agent if memory size is sufficient
        trainer.agent_manager.agents[previous_agent_index].learn()

        return sorted_actions
    except Exception as e:
        return e, 500
    


trainer.save_all_agents("./checkpoints")


