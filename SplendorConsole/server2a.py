import asyncio
import websockets
import json
import numpy as np
from dqnAgent import DQNAgent

STATE_DIM = 348
ACTION_DIM = 43
done = False
gra1000 = 0
agent = DQNAgent(state_dim=STATE_DIM, action_dim=ACTION_DIM)
previous_state = None

async def handle_connection(websocket: websockets.WebSocketServerProtocol, path: str) -> None:

    global done, gra1000, previous_state

    try:
        async for message in websocket:
            


            data = json.loads(message)
            id = data.get("Id")
            
            if id == 1:

                done = False

                previous_reward = data.get("Feedback")
                previous_action = data.get("PreviousMove")
                state = data.get("GameState")

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
                
                # Save the current state and action for the next call
                previous_state = state
                
                #TRENING
                output = agent.generate_action_values(state)

                agent.learn()
                
                response_object = {
                    "MovesList": output,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == 2:

                last_previous_reward = data.get("LastFeedback")
                last_previous_action = data.get("LastMove")
                last_state = data.get("LastGameState")
                done = True
                gra1000 += 1 

                if previous_state is not None:
                    agent.store_transition(
                        previous_state,
                        last_previous_action,
                        last_previous_reward,
                        last_state,
                        done
                    )

                if gra1000 % 100 == 0:
                    agent.save_checkpoint("./checkpoints/agent_0_checkpoint.pth")


                # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
                
                rewards = data.get("Rewards")
                #last_feedback = data.get("LastFeedback")
                print(f"[Python] Serwer odebrał wygraną {rewards}")
                
                
                agent.update_with_final_rewards(rewards[0])
                
                #TRENING

                response_object = {
                    "ResponseCode": 0,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == -1:

                last_previous_reward = data.get("LastFeedback")
                last_previous_action = data.get("LastMove")
                last_state = data.get("LastGameState")
                done = True
                gra1000 += 1 

                if previous_state is not None:
                    agent.store_transition(
                        previous_state,
                        last_previous_action,
                        last_previous_reward,
                        last_state,
                        done
                    )
                
                # REQUEST Z ZAKOŃCZONEJ GRY Z REMISEM LUB BŁĘDEM
                rewards = data.get("Rewards")
                last_feedback = data.get("LastFeedback")
                print(f"[Python] Serwer odebrał remis lub błąd {rewards} i ostatni feedback {last_feedback}")

                #TRENING

                agent.update_with_final_rewards(rewards[0])

                response_object = {
                    "ResponseCode": 0,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)
            


    except websockets.exceptions.ConnectionClosedError as e:
        print(f"[Python] Połączenie zamknięte: {e}")
    except Exception as e:
        print(f"[Python] Wystąpił błąd: {e}")


async def start_server() -> None:
    async with websockets.serve(handle_connection, "localhost", 8765, ping_timeout=None):
        print("[Python] Serwer WebSocket działa na ws://localhost:8765")
        await asyncio.Future()

agent.load_checkpoint("./checkpoints/agent_0_checkpoint.pth")
asyncio.run(start_server())
