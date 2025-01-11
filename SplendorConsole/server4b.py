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
                state = data.get("GameState")
                current_player = data.get("CurrentPlayer")
    
                #TRENING
                output = trainer.agents[current_player].generate_action_values(state)
                response_object = {
                    "MovesList": output,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == 2:

                print(f"[Python] Serwer odebrał wygraną")

                response_object = {
                    "ResponseCode": 0,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == -1:
                
                print(f"[Python] Serwer odebrał remis lub błąd")

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