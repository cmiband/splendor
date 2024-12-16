import asyncio
import websockets
import json
from random import shuffle

async def handle_connection(websocket: websockets.WebSocketServerProtocol, path: str) -> None:
    """Handles the connection from a client."""
    try:
        async for message in websocket:
            # print(f"Otrzymano wiadomość od klienta: {message}")

            data = json.loads(message)
            #print(f"Otrzymano dane jakieś")

            output = [i for i in range(1, 44)]
            shuffle(output)

            response_object = {
                "MovesList": output,
            }
            response_json = json.dumps(response_object)

            await websocket.send(response_json)
            #print(f"Serwer wysłał odpowiedź: {response_json}")

    except websockets.exceptions.ConnectionClosedError as e:
        print(f"Połączenie zamknięte: {e}")
    except Exception as e:
        print(f"Wystąpił błąd: {e}")

async def start_server() -> None:
    """Starts the WebSocket server."""
    async with websockets.serve(handle_connection, "localhost", 8765, ping_timeout=None):
        print("Serwer WebSocket działa na ws://localhost:8765")
        await asyncio.Future()


asyncio.run(start_server())
