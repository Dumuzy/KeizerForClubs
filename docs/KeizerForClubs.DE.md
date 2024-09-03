
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

#### Härtebonus
 
 In KeizerForClubs kannst man einen Bruchteil der Punkte für das Antreten bekommen, auch wenn man verliert. Man bekommt den Härtebonus mal die Keizer-Rang-Punkte des Gegners, wenn man ein Spiel verliert. Ich empfehle Werte von 2-5 %.  

*Begründung:*

1. Eine Niederlage, die immer 0 zählt, fühlt sich ein wenig unfair an, denn wenn man gegen den Turniersieger verliert, bedeutet das etwas anderes, als wenn man gegen den Letzten des Turniers verliert. 
Deshalb gibt es im Schweizer System das, was man auf Deutsch *Buchholz* nennt, auf Englisch *SOS* oder *sum of opponent scores*, auf Französisch *SPA* oder *somme des points des adversaires*. Es ist die Summe aller Punkte der Gegner. Man bekommt mehr *Buchholz*, wenn man gegen den Sieger verloren hat, als wenn man gegen den Letzten verloren hat. Der Härtebonus ist das KeizerForClubs-Äquivalent zu *Buchholz*, *SOS* oder *SPA*.

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

#### Sieg-Normalisierung

Wenn dieses Kästchen angekreuzt ist, werden alle 
Keizer-Punkte so normalisiert, dass ein Sieg gegen den letzten der Rangliste 1 Keizer-Punkt zählt. 

Das ändert nicht wirklich etwas an der Berechnung der Ranglisten oder an der Paarung. 
Aber es macht die Keizer-Punkte irgendwie viel greifbarer.

(Im ursprünglichen Keizer-System sind die angegebenen Keizer-Punkte in der Regel große ganze Zahlen.
Ursprünglich war dies wahrscheinlich der Fall, um die Berechnungen zu vereinfachen. 
Es ist einfacher, mit ganzen Zahlen zu rechnen, wenn man keinen Computer hat.

Heutzutage werden alle Berechnungen vom Computer durchgeführt, und so können wir viel 
kleinere, aber gebrochene Zahlen verwenden.)



<br />

### Menü Listen

Durch Auswahl eines Eintrags im Listenmenü werden Ausgabelisten im Ordner _export_ erzeugt. Die Art der zu erzeugenden Ausgaben kann 
auf der Registerkarte _Einstellungen_ ausgewählt werden.

#### Html Ausgabe
Die Html-Ausgabe besteht aus allgemeinem Css, das für jede Tabelle gleich ist, und Html, das die eigentlichen Daten enthält. 
Das Css wird aus der Datei _export/keizer.css_ übernommen. Wenn Sie möchten, können Sie das Css an Ihre eigenen Bedürfnisse anpassen. 

Die Datentabellen erhalten die folgenden Tags, wobei die Angaben in _{}_ durch echte Daten ersetzt werden.
```
id='kfc-{tournament name}-{table type}-{current round}'
class='my-wrapper kfc-wrapper kfc-wrapper-{number of columns}'
/* Die Kreuztabelle kann sehr breit werden, daher erhält sie andere Css-Klassen: */
class='my-exwrapper kfc-exwrapper kfc-exwrapper-{number of columns}'
```

#### Kreuztabelle Details

##### Rank-Pb

Die Spalte _Rank-Pb_ steht für "Keizer-*Rank*-*P*oints *b*efore der aktuellen Runde". Wenn Sie mit der Maus über die Spaltenüberschrift fahren, sehen Sie einen Tooltip. 

Ich werde versuchen, das zu erklären.

Die Keizer-Punkte, die ein Spieler A für einen Sieg gegen Spieler B erhält, sind genau dieser Wert, Rank-Pb.    Ich habe die Spalte eingeführt, um die Keizer-Punkte manuell nachrechnen zu können.  Es gab Fragen von Vereinskameraden...

Vgl. den Screenshot unten, der einen Teil einer Tabelle eines Beispielturniers zeigt.

Sie sehen zum Beispiel, dass der Rank-Pb von Spieler 4 (Sascha) 22 ist. Das bedeutet, dass Sascha in der Runde vor dieser Runde so in der Tabelle platziert war, dass sein "Wert" 22 Keizer-Rang-Punkte beträgt. Das bedeutet, dass du für jeden Sieg gegen Sascha 22 Punkte bekommst. Und weil Andreas in Runde 1 gegen Sascha gewonnen hat, hat er dafür 22 Keizer-Punkte bekommen. Das können Sie in der Tabelle unter R1 sehen.

Noch einer: Rank-Pb von Spieler 2 (Andreas) ist 23. Deshalb: Axel, der in Runde 2 gegen Andreas unentschieden gespielt hat, hat dafür 11,5 (11,5 = 23 / 2) Keizer-Punkte bekommen.  

![Example Tournament](./Rank-Pb.png)

<br />

### Menü Spieler

#### Spieler importieren
Man kann eine Liste mit Spielern im csv-Format importieren. Importiert werden Namen und Ratings der Spieler. 
Das Format der csv-Datei wird bestimmt durch die erste Zeile der Datei. In der ersten Zeile werden Trennzeichen (Strichpunkt, Doppelpunkt oder Komma), 
der Text _Name_ und der Text _Rating_ gesucht. Entsprechend dem Format der ersten Zeile wird der Rest der Datei eingelesen. 
Beispiel: Die erste Zeile der csv-Datei ist 
```
x;x;;xxx;Name;;Rating
``` 
Dann wird Trennzeichen der Strichpunkt, die Spielernamen sind in der 5. Spalte, die Ratings der Spieler sind in der 7. Spalte. 

##### Test-Skript ausführen
Das geht auch über *Spieler importieren*. Wenn das ausgewählte csv nur den Text `testscript` in der ersten Zeile hat, wird alles was folgt als Anweisungen für das Test-Skripting interpretiert. Folgende Anweisungen sind möglich:

```
# Leerzeilen und Zeilen, die mit einem # beginnen, werden ignoriert. 
nn delete-round  # Ruft nn mal die Funktion *Runde löschen* auf. 
kk create-player  # Es werden kk Test-Spieler erzeugt. 
ii create-round create-results # Es wird ii mal eine neue Runde automatisch erzeugt und automatisch Ergebnisse dazu. 
delete-all-rounds  # Lösche alle Runden.
delete-all-players # Lösche alle Spieler. 
create-results-2   # Ergebnisse mit vielen (- +), (+ -), (- -), (vertagt) automatisch erzeugen. 
```

#### Alle löschen
Löscht alle Spieler. Dies ist nützlich, wenn Sie ein Turnier B erstellen möchten, das genau die gleichen Einstellungen wie das vorherige Turnier A haben soll. 
Kopieren Sie in diesem Fall A.s3db nach B.s3db, löschen Sie dann alle gespielten Runden und anschließend alle Spieler. 
Um sicherzustellen, dass Sie nicht versehentlich Ihr aktuelles Turnier löschen, ist das Löschen aller Spieler nur möglich, wenn keine Partien in der Datenbank vorhanden sind.   

#### Spieler-IDs neu vergeben
Nach dem Importieren einer Spielerliste, z. B. der Vereinsmitglieder, und dem anschließenden Löschen der Spieler, die nicht am Turnier teilnehmen, kann die Spielerliste 
in KeizerForClubs viele Löcher in der Reihenfolge der Spielernummern haben. Das ist ein bisschen ärgerlich. Zumal man an der höchsten Spieler-ID nicht erkennen kann, wie viele Teilnehmer am Turnier teilnehmen.  
Sie können _Spieler-IDs neu vergeben_ verwenden, um die Löcher in der Reihenfolge der IDs zu entfernen. Nur möglich am Anfang eines Turniers. 


<br />

### Keizer System und Schweizer System

#### Vergleich von Keizer- und Schweizer Paarungssystem

<style>
th, td {
    padding: 5px;
}
tr:hover {background-color: coral;}
tr:nth-child(odd) {background-color: #f2f2f2;}
</style>

<table style="max-width: 800px">
<colgroup>
<col span="1" style="Breite: 20%;">
<col span="1" style="Breite: 40%;">
<col span="1" style="Breite: 40%;">
</colgroup>
<tr><th>Eigenschaft</th><th>Keizer</th><th>Schweizer</th></tr>
<tr><th>Paarungen</th><td>werden in der Regel kurz vor der Runde gebildet.</td><td>können Tage oder Wochen vor der Runde gebildet werden.</td></tr>
<tr><th>Starke Spieler werden gegeneinander gepaart</th><td>von der ersten Runde an.</td><td>nur später.</td></tr>
<tr><th>Der Spieler hat keinen Gegner</th><td>Da die Paarung kurz vor der Runde erfolgt, kommt dies nicht oft vor.</td><td>Kann oft vorkommen.</td></tr>
<tr><th>Punkte für Sieg</th><td>Mehr Punkte für Sieg gegen bessere Spieler.</td><td>Ein Punkt für jeden Sieg, 
  plus Feinwertung wie *Summe der Gegnerpunkte* aka Buchholz.</td></tr>
<tr><th>Punkte für Niederlagen</th><td>Im originalen Keizer einfach null. In KeizerForClubs ist es möglich, einen Härtebonus zu vergeben.
Dieser ersetzt die Buchholz-Feinwertung. </td><td>Null, zählt aber für die Buchholz-Feinwertung.</td></tr>
<tr><th>Farbmanagement </th><td>Farbe spielt bei der Paarung keine Rolle.  Wer relativ weniger Weiß hatte, bekommt Weiß. In der Praxis ist der Unterschied zum Schweizer Farbmanagement marginal.</td><td>Idealerweise jede Runde anders für jeden Spieler, niemand bekommt jemals dreimal die gleiche Farbe in Folge.  Dies wird bei der Paarung erzwungen. </td></tr>
<tr><th>Fehlende Spieler</th><td>Erhalten in der Regel Bruchteile von Punkten. </td><td> Ein oder zwei *bye-remis* sind oft erlaubt.</td></tr>
<tr><th>Spiele gegen zurückgetretene Spieler</th><td>Spezialbehandlung angewendet.</td><td>Ergebnisse können zur Feinwertung annulliert werden.</td></tr>
<tr><th>Gleicher Gegner in späterer Runde</th><td>kann erlaubt sein.</td><td>ist nie erlaubt.</td></tr>
</table>

#### Keizer-Swiss Hybrid Systeme

Man kann sich viele verschiedene Keizer-Swiss-Hybridsysteme vorstellen. Mit KeizerForClubs können Sie viele davon erstellen. Zum Beispiel:

* Ein Sieg gegen jemanden zählt einen Punkt plus Feinwertung. Ähnlich wie bei Swiss.
* Farben werden wie in Keizer verwaltet.
* Spieler, die nicht da sind, können Bruchteile von Punkten bekommen, wie in Keizer oder Schweizer mit Bye's.
* Spieler, die Spiele gegen ausgeschiedene Spieler haben, erhalten volle Punkte wie im Schweizer.
* Grundsätzlich kann ein Spieler zweimal gegen einen anderen spielen, wie in Keizer - oder nicht, wie in der Schweiz. Dies kann komplett von der Turnierleitung gesteuert werden mit der Einstellung *Anz. Runden vor Paarungs-Wiederholung*.


<br/>

### Danksagungen

*   Das Programm wurde ursprünglich mit SharpDevelop entwickelt, jetzt verwende ich Microsoft Visual Studio Community 2022. 
*   Die Datenbank ist [SQLite](https://www.SQLite.org).
*   Die Software wird gehostet auf [Github](https://github.com/Dumuzy/KeizerForClubs/releases). 

Besonderer Dank geht 

*   Thomas Schlapp für die ursprüngliche Entwicklung dieser Software.
*   Jürgen Kehr für Tests und andere wertvolle Informationen.
*   Pascal Golay für Tests, Diskussionen und die Übersetzung ins Französische. 

### Lizenz

Das Programm ist *Freeware*, wurde nach bestem Wissen und Gewissen entwickelt und
steht kostenlos zur Verfügung. Die Benutzung ist u.a. an die Bedingung geknüpft, das
jedwede Garantie, Haftung und/oder Gewährleistung ausgeschlossen ist und keinerlei
Ansprüche welcher Art auch immer gegen die Autoren erhoben werden können.
Ausführlich nachzulesen in der License.txt.


