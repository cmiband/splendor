import asyncio
import websockets
import json
from random import shuffle

from simulation_1_model import *

gra1000 = 0

async def handle_connection(websocket: websockets.WebSocketServerProtocol, path: str) -> None:
    try:
        async for message in websocket:
            
            data = json.loads(message)
            id = data.get("Id")

            global gra1000

            if id == 1:

                
                # REQUEST Z WEWNĄTRZ
                current_player = data.get("CurrentPlayer")
                feedback = data.get("Feedback")
                game_state = data.get("GameState")

                
                #TRENING
                output = step(game_state, feedback)#np.random.permutation(range(0, 42)).tolist()#

                response_object = {
                    "MovesList": output,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == 2:
                gra1000 += 1 
                if gra1000 % 100 == 0:
                    agent.save_checkpoint("./checkpoints/agent_0_checkpoint.pth")


                # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
                
                rewards = data.get("Rewards")
                last_feedback = data.get("LastFeedback")
                print(f"[Python] Serwer odebrał wygraną {rewards} i ostatni feedback {last_feedback}")
                
                
                agent.update_with_final_rewards(rewards[0])
                
                #TRENING

                response_object = {
                    "ResponseCode": 0,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == -1:
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
