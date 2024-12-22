import numpy as np
from multiagentTrainer import MultiAgentTrainer

# Initialize trainer and environment configuration
NUM_AGENTS = 4
STATE_DIM = 348
ACTION_DIM = 43

trainer = MultiAgentTrainer("DQN", num_agents=NUM_AGENTS, state_dim=STATE_DIM, action_dim=ACTION_DIM)


# Initialize to track previous states and actions
# Initialize to track previous states and actions
previous_state = None
previous_action = None
done = False

def step(current_agent_index, state, previous_reward):
    """
    API endpoint for receiving the game state from Unity, processing actions for the current agent,
    and returning the sorted action values.
    """
    global previous_state, previous_action, done

    try:
        if len(state) != STATE_DIM:
            return "Invalid state dimension", 400

        # Update the replay buffer for the previous agent
        if previous_state is not None:
            previous_agent_index = (current_agent_index - 1) % NUM_AGENTS
            trainer.agent_manager.agents[previous_agent_index].store_transition(
                previous_state,
                previous_action,
                previous_reward,
                state,
                done
            )

        # Process the current agent's action
        current_agent = trainer.agent_manager.agents[current_agent_index]
        action = current_agent.choose_action(state)  # Choose action using epsilon-greedy policy
        sorted_actions = current_agent.generate_action_values(state)

        # Save the current state and action for the next call
        previous_state = state
        previous_action = action

        # Learn for the previous agent if memory size is sufficient
        trainer.agent_manager.agents[current_agent_index].learn()

        return sorted_actions
    except Exception as e:
        return {"error": str(e)}, 500



