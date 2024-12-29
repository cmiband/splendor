from flask import Flask #, request
from flask_socketio import SocketIO, emit
import json
import logging
from random import shuffle
from real_simulation_try1 import *

# Obsługa logowania
logging.basicConfig(level=logging.DEBUG, 
                    format='%(asctime)s - %(levelname)s - %(message)s')

# Inicjalizacja zmiennych
gra1000 = 0

app = Flask(__name__)
app.debug=True
#socketio = SocketIO(app, cors_allowed_origins="*")
#socketio = SocketIO(app, cors_allowed_origins="*", async_mode="threading")
socketio = SocketIO(app, async_mode="threading")

@app.route('/')
def index():
     app.logger.info('[Python] Flask WebSocket Server is running!')
     return "Flask WebSocket Server is running!"

# Obsługa błędów połączenia
@socketio.on('connect')
def handle_connect():
    print("[Python] Klient połączony")

@socketio.on('disconnect')
def handle_disconnect():
    print("[Python] Klient rozłączony")

# Obsługa połączenia WebSocket
@socketio.on('message')
def handle_message(message):
    app.logger.debug(f'[Python] Otrzymano komunikat: {data}')
    print(f'[Python] Otrzymano komunikat: {data}')
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
            #print("STEP 1")

            if gra1000 % 10 == 0:
                #trainer.save_all_agents("./checkpoints")
                trainer.save_all_agents("./checkpoints")    #C:/Users/macie/Documents/GitHub/splendor/SplendorConsole

            # LOSOWY OUTPUT, NALEŻY ZASTĄPIĆ OUTPUTEM Z MODELU
            #output = [i for i in range(1, 44)]
            #shuffle(output)
            output = step(current_player, game_state, feedback)

            response_object = {
                "MovesList": output,
            }
            emit('response', json.dumps(response_object))

        elif id == 2:
            # REQUEST Z ZAKOŃCZONEJ GRY Z WYGRANYM
            rewards = data.get("Rewards")
            last_feedback = data.get("LastFeedback")
            print(f"[Python] Serwer odebrał wygraną {rewards} i ostatni feedback {last_feedback}")
            #print("STEP 2")
            gra1000 += 1 

            if gra1000 % 10 == 0:
                #trainer.save_all_agents("./checkpoints")
                trainer.save_all_agents("./checkpoints")    #C:/Users/macie/Documents/GitHub/splendor/SplendorConsole

            response_object = {
                "ResponseCode": 0,
            }
            emit('response', json.dumps(response_object))

        elif id == -1:
            # REQUEST Z ZAKOŃCZONEJ GRY Z REMISEM LUB BŁĘDEM
            gra1000 += 1 

            if gra1000 % 10 == 0:
                #trainer.save_all_agents("./checkpoints")
                trainer.save_all_agents("./checkpoints")    #C:/Users/macie/Documents/GitHub/splendor/SplendorConsole

            #print("STEP 3")
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

@socketio.on_error()  # Handles the default namespace
def error_handler(e):
    app.logger.error(f'[Python] ERROR: {e}')

if __name__ == '__main__':
    trainer.load_all_agents("./checkpoints")
    socketio.run(app, host='localhost', port=8765, debug=True)  #8999 8765
