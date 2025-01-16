# splendor
Implementacja gry Splendor przy użyciu silnika Unity i języka programowania C#. W repozytorium
znajduje się tylko kod gry, nie ma końcowych plików .exe i .bat służących do włączania aplikacji.
Repozytorium ma w sobie 2 podprojekty:
1. wersję konsolową gry, użytą do treningu modelu AI
2. wersję końcową, zawierającą implementację w Unity
   
Gra odbywa się w systemie turowym, użytkownik zaczyna i po nim następują ruchy jego przeciwników, czyli 3 botów. 
Co turę gra wysyła zapytanie do serwera, które zawiera w sobie informacje o stanie gry. Na podstawie tych informacji
serwer zwraca listę ruchów, które chce podjąć agent sztucznej inteligencji. Następnie gra interpretuje i wykonuje
ten ruch.
