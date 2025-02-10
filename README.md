# Opdrachten Dapper (Software Engineering 3)

De opgave staan in Assignment1.cs, Assignment2.cs en Assignment3.cs.
Onder iedere opgave staat een test die je kunt gebruiken om te controleren of je oplossing werkt.

In ``DBHelper.cs`` staat de connectonstring. Deze moet je aanpassen naar jouw eigen database.
De tests maken de database automatisch aan. **Onderstaande hoeft je dus niet te doen.** 

Een database met de naam ``DapperBeer`` kan je zelf aanmaken in MySQL m.b.v.:
```sql
CREATE DATABASE DapperBeer;
```

Een database user met de naam ``DapperBeer`` en wachtwoord ``Test@1234!`` kan je aanmaken in MySQL m.b.v.:
```sql
CREATE USER 'DapperBeer'@'localhost' IDENTIFIED BY 'Test@1234!'; 
GRANT ALL ON DapperBeer.* TO 'DapperBeer'@'localhost'; 
FLUSH PRIVILEGES;
```
Normaal gesproken is het een slecht idee om een user alle rechten te geven op een database (DapperBeer).

De test zullen de database tabellen aanmaken en vullen met de juiste data.
Het is nuttig om eens een test te runnen en te kijken en je database te bekijken
(zie opdracht 1 in Assignments1.cs).

De code om een database en de tabellen aan te maken staat in de directory ``SQL\``.
**Het is belangrijk om eerste DapperBrewer.sql toe te voegen en daarna pas DapperBeer.sql.**
Daar staat ook een afbeelding van het databaseontwerp ``DapperBeerDatabaseDiagram.png``.
En het klassendiagram ``DapperBeerClassDiagram.png``.
En het klassendiagram ``DapperBeerClassDiagram.png``.

Het is raadzaam om Rider te gebruiken. Deze kan beter overweg met SQL en C# dan Visual Studio.
Als je dan fouten maakt in je SQL ziet Rider dit en kan je direct de fouten verbeteren.
Ook kan je direct de queries uitvoeren in Rider en de resultaten bekijken.
Je kan Rider gratis gebruiken met je een studenten account (https://www.jetbrains.com/community/education/#students).


# Bronnen

Op https://slides.com/jorislops/dapper kan je de slides vinden die ik heb gebruikt tijdens de les.
Op https://www.learndapper.com/ kan je meer informatie vinden over Dapper.
Opmerking: de pagina van LearnDapper is m.b.t. Relaties niet volledig en gebruikt niet de dictionary methode
die ik heb uitgelegd. Deze methoden wordt in de opdrachten gebruikt en ook door andere Dapper gebruikers aanbevolen.

# Voorbeelden van Relaties

Omdat de relatie tussen de tabellen in de database belangrijk is, heb ik een aantal voorbeelden gemaakt.
Deze staan in de directory ``ExampleFromSheets``.

Veel succes met de opdrachten!

Joris Lops
