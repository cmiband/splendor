import numpy as np
from dqnAgent import DQNAgent

# Initialize trainer and environment configuration for a single agent
STATE_DIM = 348
ACTION_DIM = 43

agent = DQNAgent(state_dim=STATE_DIM, action_dim=ACTION_DIM)

# Initialize to track previous states and actions
previous_state = None

done = False

def step(state, previous_reward, previous_action):

    global previous_state, done

    try:
        if len(state) != STATE_DIM:
            return "Invalid state dimension", 400

        if previous_state is not None:
            agent.store_transition(
                previous_state,
                previous_action,
                previous_reward,
                state,
                done
            )
    
        #action = agent.choose_action(state)  # Choose action using epsilon-greedy policy
        sorted_actions = agent.generate_action_values(state)
        
        # Save the current state and action for the next call
        previous_state = state

        # Learn for the agent if memory size is sufficient
        agent.learn()

        return sorted_actions
    except Exception as e:
        return {"error": str(e)}, 500
