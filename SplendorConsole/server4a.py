import asyncio
import websockets
import json
import torch
from multiagentDQN import MultiAgentDQN

STATE_DIM = 348
ACTION_DIM = 43
NUM_AGENTS = 4

gra1000 = 0
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

trainer = MultiAgentDQN(num_agents=NUM_AGENTS, state_dim=STATE_DIM, action_dim=ACTION_DIM, device=device)

previous_state = None
cur = None

async def handle_connection(websocket: websockets.WebSocketServerProtocol, path: str) -> None:
    try:
        async for message in websocket:
            
            data = json.loads(message)
            id = data.get("Id")

            global gra1000, previous_state, cur
            

            if id == 1:

                
                done = False
                # REQUEST Z WEWNĄTRZ
                current_player = data.get("CurrentPlayer")
                cur = current_player
                previous_reward = data.get("Feedback")
                previous_action = data.get("PreviousMove")
                state = data.get("GameState")

                if previous_state is not None:
                    trainer.agents[(current_player-1)%4].store_transition(
                        previous_state,
                        previous_action,
                        previous_reward,
                        state,
                        done
                    )

                previous_state = state
    
                #TRENING
                output = trainer.agents[current_player].generate_action_values(state)

                trainer.agents[current_player].learn()

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
                winner = data.get("Winner")
                gra1000 += 1 
   
                #if previous_state is not None:
                trainer[cur].store_transition(
                    previous_state,
                    last_previous_action,
                    last_previous_reward,
                    last_state,
                    done
                )

                if gra1000 % 200 == 0:
                    trainer.save_all_agents("./checkpoints")

                if winner>=4:
                    trainer.save_best_agents("./checkpoints")
                # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
                
                rewards = data.get("Rewards")

                print(f"[Python] Serwer odebrał wygraną {rewards}")
                trainer.update_rewards(rewards)
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
                winner = data.get("Winner")
                gra1000 += 1 


                trainer[cur].store_transition(
                    previous_state,
                    last_previous_action,
                    last_previous_reward,
                    last_state,
                    done
                )
                if winner<=-2:
                    trainer.save_best_agents("./checkpoints")
                rewards = data.get("Rewards")

                print(f"[Python] Serwer odebrał remis lub błąd {rewards}")

                trainer.update_rewards(rewards)

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

trainer.load_all_agents("./checkpoints")
asyncio.run(start_server())
