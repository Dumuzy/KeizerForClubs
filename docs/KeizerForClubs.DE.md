
<br />

## KeizerForClubs

<br />

### Allgemeines

Die KeizerForClubs-Software ermöglicht die Verwaltung von z.B. Schachturnieren mit bis zu 100 Spielern nach dem Keizer-System.

### Wie wird ein Turnier durchgeführt?

Die wesentlichen Schritte in Kurzform:

- Nach dem Programm-Start als erstes im Menü *Start...* auswählen. Jedes Turnier
    wird in einer kleinen Datenbank abgespeichert: wenn Sie eine bestehende Datei
    auswählen, wird ein laufendes Turnier fortgesetzt; wählen Sie einen neuen Dateinamen um ein neues Turnier zu starten. 
- Bei einem neuen Turnier tragen Sie jetzt die Namen aller teilnehmenden Spieler
    ein (möglichst mit Rating, aber es geht zur Not auch ohne).
- Sie können auch Spieler eintragen, die zur ersten Runde nicht anwesend sind, aber
    später dazu kommen werden oder bei laufenden Turnieren neue Spieler
    hinzufügen. In späteren Runden können auch Nachzügler aufgenommen werden.
- Vor der Auslosung der ersten oder nächsten Runde: setzen Sie den Status aller
    Spieler (anwesend, entschuldigt etc.).
- Jetzt wird die nächste Runde ausgelost: das kann automatisch über das Programm
    erfolgen oder manuell. Die Auslosung selbst ist einfach; wenn kein PC im
    Spiellokal zur Verfügung steht, kann das ganz simpel anhand der aktuellen Tabelle
    gemacht werden, die Paarungen samt Ergebnissen können dann später am PC
    nachgetragen werden.
- Ergebnisse eintragen.
- Falls erforderlich Listen erstellen: Rangliste, Paarungen, aktuelle Teilnehmerliste, ...
- Programm beenden, für die nächste Runde geht's wieder oben los:-)

### Kurzbeschreibung der Benutzeroberfläche

Das Programm sollte selbsterklärend sein, hier nur einige kurze Hinweise...

- Am Anfang ist alles deaktiviert, zuerst muss mit *Start...* ein Turnier gewählt
    oder begonnen werden.
- Dann sind die drei Reiter unten auswählbar: *Spieler*, *Paarungen*,
    *Einstellungen*.
- Um Spieler einzutragen, schreibt man Name und Rating in die Felder der Tabelle.
    Den Status wählt man in der letzten Spalte durch Anklicken von *Pfeil nach
    unten* und Auswahl aus der Liste. ID wird automatisch vergeben.
- Um eine neue Runde auszulosen, einen der beiden Menüpunkte im Menü
    *Paarung* wählen. Die Anleitung für manuelle Paarungen steht weiter unten.
- Das Menü Paarungen ist nur aktiv, wenn der Reiter Paarungen ausgewählt ist.
- Auf dem Reiter Paarungen kann man alle Runden auswählen (Pfeil nach
    oben/unten neben der Rundennummer).
- Spielergebnisse wählt man wieder aus einer Liste (nach Anklicken von *Pfeil nach
    unten*).
- Ergebnisse können auch nachträglich für längst beendete Runden geändert werden
    (zum Beispiel, weil versehentlich *1-0* statt *0-1* eingetragen wurde; nicht
    erforderlich für zurückgetretene Teilnehmer, das wird über den Status des
    Teilnehmers automatisch berücksichtigt).
- Im Menü Listen kann man Tabellenstände etc. anzeigen lassen.

<br />

### Anleitung für das Fenster *Manuelle Paarungen*

Der Menüpunkt öffnet ein neues Fenster, das links eine Liste mit Spielernamen und
rechts *freie Bretter* für die Paarungen hat. In die Liste der Spieler kommen alle mit
Status *anwesend*, das muss also entsprechend vorbereitet sein.

- Links einen Namen anklicken und rechts in ein freies Feld klicken: der Spieler
    wird an dieses Brett gesetzt.
- Rechts ein Brett mit einem Namen anklicken: der Spieler kommt zurück in die
    linke Liste.
- Ok klicken zum Eingaben übernehmen - wird aber vorher kontrolliert.
- Links darf höchstens ein Name stehen bleiben, bei ungerader Spielerzahl
    bekommt der das Freilos. Rechts sind keine Paarungen mit nur einem Spieler
    erlaubt.

<br />

### Beschreibung einiger spezieller Szenarien

#### Kein PC im Spiellokal?

Dann legt der Turnierleiter die Paarungen am Spielabend anhand der Tabelle fest (Regeln
dafür weiter unten). Später gibt er die Paarungen zu Hause über *manuelle Paarung* ein,
trägt die Ergebnisse ein und druckt den Tabellenstand für den nächsten Spielabend aus.

#### Nachzügler

Einfach den neuen Spieler in die Liste einfügen: er wird sofort bei Auslosung und
Tabellenstand berücksichtigt.

#### Rücktritte

Den Status für den Spieler auf *abgemeldet* setzen.
Der Spieler wird bei Auslosung und Tabellenstand nicht mehr berücksichtigt, für bereits
gespielte Partien bekommen die Gegner eine Wertungsgutschrift.

Falls ein Spieler sich vor der ersten Runde abmeldet, kann man seinen Status auf *gelöscht* setzen, er wird dann komplett aus dem Turnier enfernt. 

#### Regeln für die Paarung

- Es spielen alle anwesenden Teilnehmer; bei ungerader Anzahl gibt es ein Freilos.
- Man geht einfach die aktuelle Tabelle von oben nach unten durch: von den
    Anwesenden spielt der am besten platzierte gegen den zweitplatzierten, 3. - 4.
    und so weiter.
- Der Tabellenstand wird sortiert nach Keizer-Wertungspunkten und bei
    Gleichstand nach Rating - letzteres vor allem in der 1. Runde.
- Farbverteilung erfolgt *fair* - weiß bekommt, wer es seltener hatte.
- Wenn beide gleich oft Weiß hatten und die Spieler bereits gegeneinander 
    gespielt haben, werden die Farben des letzten Spiels vertauscht.
    Andernfalls erhält der schlechter platzierte Spieler weiß.
- Anders als beim Schweizer System ist drei mal dieselbe Farbe hintereinander
    erlaubt. Kommt aber in der Praxis kaum vor. 
- Bei ungerader Anzahl Spieler bekommt der am schlechtesten platzierte anwesende
    Spieler ein Freilos.
- Optionen:
    1. das Freilos kann stattdessen an den 2. schlechtest platzierten Spieler (usw.)
       übergehen, wenn Doppel-Freilose vermieden werden sollen. 
       Auch ein Spieler, der ein Spiel verpasst hat, erhält erst dann ein Freilos, wenn alle anderen Spieler ein Freilos erhalten oder
        ein Spiel verpasst haben.
    2. Paarungen können wiederholt werden, ggf. mit der Einschränkung *aber
       frühestens nach n Runden*.

<br />

### Erklärung Wertungsparameter

Beim Keizer-System kann man auch Wertungspunkte bekommen, wenn man nicht spielt.
Die Gutschrift ist je nach Grund unterschiedlich hoch. Für die Berechnung wird
angenommen, das man gegen jemanden spielt, der genauso stark ist wie man selbst. Ein
Prozent-Faktor bestimmt dann die Gutschrift: 100% entspricht einem Sieg, 50% einem
Remis und 0% einer Niederlage.

Über die Schieber auf dem Reiter Einstellungen kann man die Prozentwerte festlegen.
Voreingestellt sind folgende Werte:

- 70% für Abwesenheit wg. *Vereinsverpflichtungen*
- 35% für Entschuldigtes Fehlen
- 35% für unentschuldigtes Fehlen
- 75% für Spiele gegen Zurückgetretene.
- 50% für Freilos

<br />

### Erklärung der Optionen

#### Anzahl Runden bis Paarung wiederholbar

Minimale Rundenzahl, bis zwei Spieler erneut gegeneinander antreten dürfen.

*   0 bedeutet daher: zwei Spieler können gegeneinander gepaart werden, auch wenn sie
schon in der vorhergehenden Runde gegeneinander spielten.
*   99 bedeutet, zwei Spieler können erst nach 99 Runden (praktisch also nie im ganzen
Turnier) nochmal gegeneinander kommen.
*   Wenn ein Turnier 7 Runden hat und diese Zahl auf 2 gesetzt ist, kann es sein, dass einige Paarungen dreimal im Turnier stattfinden, nämlich in
der 1., 4. und 7. Runde.

#### Freilose gleich verteilen
Betrifft bei einer ungeraden Anzahl anwesender Spieler die Vergabe des Freiloses. Wenn nicht angehakt, geht das Freilos in jedem Fall an den
schlechtest plazierten (auch wenn der schon drei und andere Spieler gar kein Freilos hatten).

#### Erste Runde Zufall
Im Keizer-System wäre die erste Runde jedes Mal die gleiche mit
denselben Spielern. Das kann für kleine Vereine lästig sein. Wenn der
Wert nicht auf 0 gesetzt wird, sind die Farben der ersten Runde zufällig und eine 
eine Zufallszahl zwischen 0 und dem Wert wird zum Rating jedes Spielers addiert oder subtrahiert, um die Erstrundenpaarungen zu bestimmen.

#### Verhältnis Sieg gegen den Ersten zum Letzten

Im Keizer-System gibt es für einen Sieg gegen den Erstplatzierten mehr
Punkte als für einen Sieg gegen die anderen Spieler. 

Dieser Wert ist gleich dem Verhältnis der Punkte, die Sie für einen Sieg gegen den Erstplatzierten erhalten würden geteilt durch die Punkte, die man für einen Sieg gegen den letztplatzierten Spieler erhalten würde.
Je niedriger dieser Wert ist, desto näher sind sich das Keizer-System und das Schweizer System. Drei ist der Standardwert. Je höher die Zahl ist, desto mehr bleiben die besseren Spieler unter sich. Ich empfehle niedrigere Werte für Turniere mit
weniger als 20 Spielern. 

<br />

### Danksagungen

*   Das Programm wurde ursprünglich mit SharpDevelop entwickelt, jetzt verwende ich Microsoft Visual Studio Community 2022. 
*   Die Datenbank ist [SQLite](https://www.SQLite.org).
*   Die Software wird gehostet auf [Github](https://github.com/Dumuzy/KeizerForClubs/releases). 

Besonderer Dank geht 

*   Thomas Schlapp für die ursprüngliche Entwicklung dieser Software.
*   Jürgen Kehr für Tests und andere wertvolle Informationen.
*   Pascal Golay für Tests und die Übersetzung ins Französische. 

### Lizenz

Das Programm ist *Freeware*, wurde nach bestem Wissen und Gewissen entwickelt und
steht kostenlos zur Verfügung. Die Benutzung ist u.a. an die Bedingung geknüpft, das
jedwede Garantie, Haftung und/oder Gewährleistung ausgeschlossen ist und keinerlei
Ansprüche welcher Art auch immer gegen die Autoren erhoben werden können.
Ausführlich nachzulesen in der License.txt.


