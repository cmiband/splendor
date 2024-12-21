import asyncio
import websockets
import json
from random import shuffle

async def handle_connection(websocket: websockets.WebSocketServerProtocol, path: str) -> None:
    try:
        async for message in websocket:
            
            data = json.loads(message)
            id = data.get("Id")

            if id == 1:
                # REQUEST Z WEWNĄTRZ
                current_player = data.get("CurrentPlayer")
                feedback = data.get("Feedback")
                game_state = data.get("GameState")

                #LOSOWY OUTPUT, NALEŻY ZASTĄPIĆ OUTPUTEM Z MODELU
                output = [i for i in range(1, 44)]
                shuffle(output)

                #TRENING

                response_object = {
                    "MovesList": output,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == 2:
                # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
                rewards = data.get("Rewards")
                last_feedback = data.get("LastFeedback")
                print(f"[Python] Serwer odebrał wygraną {rewards} i ostatni feedback {last_feedback}")

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
