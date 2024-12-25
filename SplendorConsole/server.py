from flask import Flask, request
from flask_socketio import SocketIO, emit
import json
from random import shuffle

app = Flask(__name__)
#socketio = SocketIO(app, cors_allowed_origins="*")
socketio = SocketIO(app, cors_allowed_origins="*", async_mode='threading')

@app.route('/')
def index():
    return "Flask WebSocket Server is running!"

# Obsługa połączenia WebSocket
@socketio.on('message')
def handle_message(message):
    try:
        data = json.loads(message)
        id = data.get("Id")

        if id == 1:
            # REQUEST Z WEWNĄTRZ
            current_player = data.get("CurrentPlayer")
            feedback = data.get("Feedback")
            game_state = data.get("GameState")

            # LOSOWY OUTPUT, NALEŻY ZASTĄPIĆ OUTPUTEM Z MODELU
            output = [i for i in range(1, 44)]
            shuffle(output)

            response_object = {
                "MovesList": output,
            }
            emit('response', json.dumps(response_object))

        elif id == 2:
            # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
            rewards = data.get("Rewards")
            last_feedback = data.get("LastFeedback")
            print(f"[Python] Serwer odebrał wygraną {rewards} i ostatni feedback {last_feedback}")

            response_object = {
                "ResponseCode": 0,
            }
            emit('response', json.dumps(response_object))

        elif id == -1:
            # REQUEST Z ZAKOŃCZONEJ GRY Z REMISEM LUB BŁĘDEM
            rewards = data.get("Rewards")
            last_feedback = data.get("LastFeedback")
            print(f"[Python] Serwer odebrał remis lub błąd {rewards} i ostatni feedback {last_feedback}")

            response_object = {
                "ResponseCode": 0,
            }
            emit('response', json.dumps(response_object))

    except Exception as e:
        print(f"[Python] Wystąpił błąd: {e}")
        emit('error', {'error': str(e)})

# Obsługa błędów połączenia
@socketio.on('connect')
def handle_connect():
    print("[Python] Klient połączony")

@socketio.on('disconnect')
def handle_disconnect():
    print("[Python] Klient rozłączony")

if __name__ == '__main__':
    socketio.run(app, host='localhost', port=8765, debug=True)
