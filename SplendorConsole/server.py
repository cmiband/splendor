import asyncio
import websockets
import json
from random import shuffle

from real_simulation_try1 import *

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
                output = step(current_player, game_state, feedback)

                response_object = {
                    "MovesList": output,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == 2:
                gra1000+= 1 
                if gra1000 % 1000 == 0:
                    trainer.save_all_agents("./checkpoints")


                # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
                
                rewards = data.get("Rewards")
                last_feedback = data.get("LastFeedback")
                print(f"[Python] Serwer odebrał wygraną {rewards} i ostatni feedback {last_feedback}")
                trainer.update_with_final_rewards(rewards)
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

                trainer.update_with_final_rewards(rewards)

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
