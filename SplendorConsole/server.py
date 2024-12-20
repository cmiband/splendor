import asyncio
import websockets
import json
from random import shuffle

async def handle_connection(websocket: websockets.WebSocketServerProtocol, path: str) -> None:
    try:
        async for message in websocket:
            #print(f"Otrzymano wiadomość od klienta: {message}")

            data = json.loads(message)
            id = data.get("Id")

            if id == 1:
                # REQUEST Z WEWNĄTRZ
                feedback = data.get("Feedback")
                game_state = data.get("GameState")

                output = [i for i in range(1, 44)]
                shuffle(output)

                response_object = {
                    "MovesList": output,
                }

                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == 2:
                # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
                rewards = data.get("Rewards")
                print(f"[Python] Serwer odebrał wygraną {rewards}")
                response_object = {
                    "ResponseCode": 0,
                }
                response_json = json.dumps(response_object)
                await websocket.send(response_json)

            elif id == -1:
                # REQUEST Z ZAKOŃCZONEJ GRY Z REMISEM
                rewards = data.get("Rewards")
                print(f"[Python] Serwer odebrał remis lub błąd {rewards}")
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
