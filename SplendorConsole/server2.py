import asyncio
import websockets
import json

from real_simulation_try2 import *

ifFirst = True

async def handle_connection(websocket: websockets.WebSocketServerProtocol, path: str) -> None:
    try:
        async for message in websocket:
            
            data = json.loads(message)
            id = data.get("Id")

            global ifFirst

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
                ifFirst = True
                # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
                
                rewards = data.get("Rewards")
                last_feedback = data.get("LastFeedback")
                print(f"[Python] Serwer odebrał wygraną {rewards} i ostatni feedback {last_feedback}")
                #trainer.update_with_final_rewards(rewards)
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

                #trainer.update_with_final_rewards(rewards)

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


asyncio.run(start_server())
