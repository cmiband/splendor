# Flask SocketIO 
W tej wersji standardowy websocket został zastąpiony przez Flask SocketIO.

## Problemy w komunikacji klient-serwer
Po zmianie na Flask i SocketIO pojawiły się następujące kwestie:

1. W standardowym WebClient zmienić URL-a 
- URL : 'ws://localhost:8765/socket.io/?EIO=3&transport=websocket'
- EIO odnosi się do wersji 
- transport = websocket  - odnosi się do rodzaju transportu

2. Nie udało mi się pożenić klienta z SocketIO dla wersji 3 ani 4
- Zmienić klienta na bibliotekę C# SocketIoClientDotNet
- dotnet add package SocketIoClientDotNet
- Uwaga w tej wersji nie wolno dodawać w URL-u /socket.io/?EIO ... bo sam dodaje te parametry

3. Ale C# SocketIOClientDotNet - niestety nie wspiera EIO v.4 
- Zatem po stronie serwera wersja EIO zmienić z 4 na 3
- Jak to działa : 
	Flask SocketIO v.5.x : EIO = 4
	Flask SocketIO v.4.x : EIO = 3
- Zatem żeby działało;
	Zmienić wersje Flask-SocketIO na 4.3.2 : pip install flask-socketio==2.0.0
	Zmienić Flask na 1.1.3 :pip install flask==1.1.3
	Zmienić markupsafe na 2.0.0 : pip install markupsafe==2.0.0

4. Ew. zostajemy ze standardowym WebServiceClient / ConnectToWebSocket 
- Musi być URL jak w punkcie 1 powyżej
- Proponuję na razie użyć flask 1.1.3 i socketIO w wersji 4.3.2
- ustawić EIO=3