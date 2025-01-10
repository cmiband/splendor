import asyncio
import websockets
import json
import numpy as np
#from dqnAgent import DQNAgent
from agent_gpu import DQNAgent
import torch

STATE_DIM = 348
ACTION_DIM = 43
gra1000 = 0
#agent = DQNAgent(state_dim=STATE_DIM, action_dim=ACTION_DIM)
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
agent = DQNAgent(state_dim=STATE_DIM, action_dim=ACTION_DIM, device=device)
previous_state = None

async def handle_connection(websocket: websockets.WebSocketServerProtocol, path: str) -> None:

    try:
        async for message in websocket:
            
            global gra1000

            data = json.loads(message)
            id = data.get("Id")
            
            if id == 1:

                state = data.get("GameState")

                if len(state) != STATE_DIM:
                    return "Invalid state dimension", 400
                
                #TRENING
                output = agent.generate_action_values(state)
                
                response_object = {
                    "MovesList": output,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == 2:

                gra1000 += 1 

                #if gra1000 % 100 == 0:
                #    agent.save_checkpoint("./checkpoints/agent_0_checkpoint.pth")


                # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
                #last_feedback = data.get("LastFeedback")
                print(f"[Python] Serwer odebrał wygraną")
                
                #TRENING

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

agent.load_checkpoint("./checkpoints/agent_0_checkpoint.pth")
asyncio.run(start_server())
