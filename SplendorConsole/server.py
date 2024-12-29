from flask import Flask, request
from flask_socketio import SocketIO, emit
import json
from random import shuffle
from real_simulation_try1 import *
import logging
import eventlet

gra1000 = 0

# Set up logging
logging.basicConfig(level=logging.DEBUG)  # Set the logging level to DEBUG
logger = logging.getLogger(__name__)

# Set up Flask
app = Flask(__name__)
#socketio = SocketIO(app, cors_allowed_origins="*")
#socketio = SocketIO(app, cors_allowed_origins="*", async_mode='threading')
socketio = SocketIO(app, async_mode='eventlet')
#socketio = SocketIO(app)
                    
@app.route('/')
def index():
    return "Flask WebSocket Server is running!"

# Obsługa połączenia WebSocket
@socketio.on('message')
def handle_message(message):
    try:
        global gra1000
        data = json.loads(message)
        id = data.get("Id")

        if id == 1:
            # REQUEST Z WEWNĄTRZ
            current_player = data.get("CurrentPlayer")
            feedback = data.get("Feedback")
            game_state = data.get("GameState")

            gra1000 += 1 

            if gra1000 % 10 == 0:
                #trainer.save_all_agents("./checkpoints")
                trainer.save_all_agents("C:/Users/macie/Documents/GitHub/splendor/SplendorConsole")

            # LOSOWY OUTPUT, NALEŻY ZASTĄPIĆ OUTPUTEM Z MODELU
            output = [i for i in range(1, 44)]
            shuffle(output)
            # output = step(current_player, game_state, feedback)

            response_object = {
                "MovesList": output,
            }
            emit('response', json.dumps(response_object))

        elif id == 2:
            # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
            rewards = data.get("Rewards")
            last_feedback = data.get("LastFeedback")
            print(f"[Python] Serwer odebrał wygraną {rewards} i ostatni feedback {last_feedback}")

            gra1000 += 1 

            if gra1000 % 10 == 0:
                #trainer.save_all_agents("./checkpoints")
                trainer.save_all_agents("C:/Users/macie/Documents/GitHub/splendor/SplendorConsole")

            response_object = {
                "ResponseCode": 0,
            }
            emit('response', json.dumps(response_object))

        elif id == -1:
            # REQUEST Z ZAKOŃCZONEJ GRY Z REMISEM LUB BŁĘDEM
            gra1000 += 1 

            if gra1000 % 10 == 0:
                #trainer.save_all_agents("./checkpoints")
                trainer.save_all_agents("C:/Users/macie/Documents/GitHub/splendor/SplendorConsole")

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
    trainer.load_all_agents("./checkpoints")
    socketio.run(app, host='localhost', port=8765, debug=True)  #8999 8765
