# Cyberprzepych - splendor

   



Cyberprzepych jest komputerową adaptacją gry planszowej "Splendor" w aranżacji cyberpunkowej. 
Do stworzenia projektu użyliśmy silnika Unity w wersji 2022.3.50f1 oraz języka C# do skryptów obsługujących grę.
Jest to gra jednoosobowa, a przeciwnicy są obsługiwani przez SI, uczoną przy pomocy symulacyjnej, konsolowej wersji gry. Model trenowaliśmy na bazie algorytmu Deep Q Network (DQN) - jednej z technik Reinforcement Learning.

**W grze**

W grze znajdują się dwie sceny:

1. Menu główne
![image](https://github.com/user-attachments/assets/3741e91a-067b-499c-8355-0dc6285aecfb)

   Posiada przyciski które służą do:
   1. Rozpoczęcia nowej gry
   2. Otwarcia menu ustawień:
   
   ![image](https://github.com/user-attachments/assets/57b8fb95-9d3d-4ab9-8029-a70ec6f6f129)
      
   3. Otwarcia okna z zasadami gry:
   
   ![image](https://github.com/user-attachments/assets/e23f6aa7-0cc1-4768-8177-3527cdacbef6)

2. Ekran gry

![image](https://github.com/user-attachments/assets/02e6c539-c9ec-4c10-a820-3f11132f0279)

Po lewej stronie znajdują się awatary przeciwników oraz informacje o ich zasobach i zarezerwowanych kartach. Na górze ekranu znajdują się Cyberlordzi wraz z wymaganymi bonusami. Pośrodku znajdują się karty, a obok nich chipy. Poniżej są informacje o zasobach gracza.
Podczas tury gracza dostępne są przyciski:
1. Pas:
   Pomija turę bez podejmowania żadnej akcji.
2. Kup kartę:
   Usuwa kartę ze stołu i dodaje ją do ręki gracza; jeśli gracz nie ma wystarczającej ilości zasobów na ekranie pojawia się alert.
3. Rezerwuj kartę:
   Usuwa kartę ze stołu i dodaje ją do ręki zarezerwowanych kart gracza. Od tego momentu tylko gracz ma możliwość jej zakupienia. Ponadto jeśli są dostępne, przy rezerwacji gracz otrzymuje złoty chip, którym można zapłacić jak dowolnym kolorem.

   Dodatkowo dostępne są również przyciski ustawień, zasad gry, wyjścia z gry oraz spisu kart:
      Otwiera on okno, gdzie widoczne są zakupione karty wszystkich graczy.

Gra odbywa się w systemie turowym, użytkownik zaczyna i po nim następują ruchy jego przeciwników, czyli 3 botów. 
Co turę gra wysyła zapytanie do serwera, które zawiera w sobie informacje o stanie gry. Na podstawie tych informacji
serwer zwraca listę ruchów, które chce podjąć agent sztucznej inteligencji. Następnie gra interpretuje i wykonuje
ten ruch.

Podczas tury graczy SI, gracz jest o tym informowany i nie ma możliwości podejmowania żadnej akcji.

**Repozytorium**
W repozytorium znajduje się tylko kod gry, nie ma końcowych plików .exe i .bat służących do włączania aplikacji.
Repozytorium ma w sobie 2 podprojekty:
1. wersję konsolową gry, użytą do treningu modelu AI
2. wersję końcową, zawierającą implementację w Unity
