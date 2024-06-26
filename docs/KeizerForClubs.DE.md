
<br />

## KeizerForClubs

<br />

### Allgemeines

Die KeizerForClubs-Software erm�glicht die Verwaltung von z.B. Schachturnieren mit bis zu 100 Spielern nach dem Keizer-System.

### Wie wird ein Turnier durchgef�hrt?

Die wesentlichen Schritte in Kurzform:

- Nach dem Programm-Start als erstes im Men� *Start...* ausw�hlen. Jedes Turnier
    wird in einer kleinen Datenbank abgespeichert: wenn Sie eine bestehende Datei
    ausw�hlen, wird ein laufendes Turnier fortgesetzt; w�hlen Sie einen neuen Dateinamen um ein neues Turnier zu starten. 
- Bei einem neuen Turnier tragen Sie jetzt die Namen aller teilnehmenden Spieler
    ein (m�glichst mit Rating, aber es geht zur Not auch ohne).
- Sie k�nnen auch Spieler eintragen, die zur ersten Runde nicht anwesend sind, aber
    sp�ter dazu kommen werden oder bei laufenden Turnieren neue Spieler
    hinzuf�gen. In sp�teren Runden k�nnen auch Nachz�gler aufgenommen werden.
- Vor der Auslosung der ersten oder n�chsten Runde: setzen Sie den Status aller
    Spieler (anwesend, entschuldigt etc.).
- Jetzt wird die n�chste Runde ausgelost: das kann automatisch �ber das Programm
    erfolgen oder manuell. Die Auslosung selbst ist einfach; wenn kein PC im
    Spiellokal zur Verf�gung steht, kann das ganz simpel anhand der aktuellen Tabelle
    gemacht werden, die Paarungen samt Ergebnissen k�nnen dann sp�ter am PC
    nachgetragen werden.
- Ergebnisse eintragen.
- Falls erforderlich Listen erstellen: Rangliste, Paarungen, aktuelle Teilnehmerliste, ...
- Programm beenden, f�r die n�chste Runde geht's wieder oben los:-)

### Kurzbeschreibung der Benutzeroberfl�che

Das Programm sollte selbsterkl�rend sein, hier nur einige kurze Hinweise...

- Am Anfang ist alles deaktiviert, zuerst muss mit *Start...* ein Turnier gew�hlt
    oder begonnen werden.
- Dann sind die drei Reiter unten ausw�hlbar: *Spieler*, *Paarungen*,
    *Einstellungen*.
- Um Spieler einzutragen, schreibt man Name und Rating in die Felder der Tabelle.
    Den Status w�hlt man in der letzten Spalte durch Anklicken von *Pfeil nach
    unten* und Auswahl aus der Liste. ID wird automatisch vergeben.
- Um eine neue Runde auszulosen, einen der beiden Men�punkte im Men�
    *Paarung* w�hlen. Die Anleitung f�r manuelle Paarungen steht weiter unten.
- Das Men� Paarungen ist nur aktiv, wenn der Reiter Paarungen ausgew�hlt ist.
- Auf dem Reiter Paarungen kann man alle Runden ausw�hlen (Pfeil nach
    oben/unten neben der Rundennummer).
- Spielergebnisse w�hlt man wieder aus einer Liste (nach Anklicken von *Pfeil nach
    unten*).
- Ergebnisse k�nnen auch nachtr�glich f�r l�ngst beendete Runden ge�ndert werden
    (zum Beispiel, weil versehentlich *1-0* statt *0-1* eingetragen wurde; nicht
    erforderlich f�r zur�ckgetretene Teilnehmer, das wird �ber den Status des
    Teilnehmers automatisch ber�cksichtigt).
- Im Men� Listen kann man Tabellenst�nde etc. anzeigen lassen.

<br />

### Anleitung f�r das Fenster *Manuelle Paarungen*

Der Men�punkt �ffnet ein neues Fenster, das links eine Liste mit Spielernamen und
rechts *freie Bretter* f�r die Paarungen hat. In die Liste der Spieler kommen alle mit
Status *anwesend*, das muss also entsprechend vorbereitet sein.

- Links einen Namen anklicken und rechts in ein freies Feld klicken: der Spieler
    wird an dieses Brett gesetzt.
- Rechts ein Brett mit einem Namen anklicken: der Spieler kommt zur�ck in die
    linke Liste.
- Ok klicken zum Eingaben �bernehmen - wird aber vorher kontrolliert.
- Links darf h�chstens ein Name stehen bleiben, bei ungerader Spielerzahl
    bekommt der das Freilos. Rechts sind keine Paarungen mit nur einem Spieler
    erlaubt.

<br />

### Beschreibung einiger spezieller Szenarien

#### Kein PC im Spiellokal?

Dann legt der Turnierleiter die Paarungen am Spielabend anhand der Tabelle fest (Regeln
daf�r weiter unten). Sp�ter gibt er die Paarungen zu Hause �ber *manuelle Paarung* ein,
tr�gt die Ergebnisse ein und druckt den Tabellenstand f�r den n�chsten Spielabend aus.

#### Nachz�gler

Einfach den neuen Spieler in die Liste einf�gen: er wird sofort bei Auslosung und
Tabellenstand ber�cksichtigt.

#### R�cktritte

Den Status f�r den Spieler auf *abgemeldet* setzen.
Der Spieler wird bei Auslosung und Tabellenstand nicht mehr ber�cksichtigt, f�r bereits
gespielte Partien bekommen die Gegner eine Wertungsgutschrift.

Falls ein Spieler sich vor der ersten Runde abmeldet, kann man seinen Status auf *gel�scht* setzen, er wird dann komplett aus dem Turnier enfernt. 

#### Regeln f�r die Paarung

- Es spielen alle anwesenden Teilnehmer; bei ungerader Anzahl gibt es ein Freilos.
- Man geht einfach die aktuelle Tabelle von oben nach unten durch: von den
    Anwesenden spielt der am besten platzierte gegen den zweitplatzierten, 3. - 4.
    und so weiter.
- Der Tabellenstand wird sortiert nach Keizer-Wertungspunkten und bei
    Gleichstand nach Rating - letzteres vor allem in der 1. Runde.
- Farbverteilung erfolgt *fair* - wei� bekommt, wer es seltener hatte.
- Wenn beide gleich oft Wei� hatten und die Spieler bereits gegeneinander 
    gespielt haben, werden die Farben des letzten Spiels vertauscht.
    Andernfalls erh�lt der schlechter platzierte Spieler wei�.
- Anders als beim Schweizer System ist drei mal dieselbe Farbe hintereinander
    erlaubt. Kommt aber in der Praxis kaum vor. 
- Bei ungerader Anzahl Spieler bekommt der am schlechtesten platzierte anwesende
    Spieler ein Freilos.
- Optionen:
    1. das Freilos kann stattdessen an den 2. schlechtest platzierten Spieler (usw.)
       �bergehen, wenn Doppel-Freilose vermieden werden sollen. 
       Auch ein Spieler, der ein Spiel verpasst hat, erh�lt erst dann ein Freilos, wenn alle anderen Spieler ein Freilos erhalten oder
        ein Spiel verpasst haben.
    2. Paarungen k�nnen wiederholt werden, ggf. mit der Einschr�nkung *aber
       fr�hestens nach n Runden*.

<br />

### Erkl�rung Wertungsparameter

Beim Keizer-System kann man auch Wertungspunkte bekommen, wenn man nicht spielt.
Die Gutschrift ist je nach Grund unterschiedlich hoch. F�r die Berechnung wird
angenommen, das man gegen jemanden spielt, der genauso stark ist wie man selbst. Ein
Prozent-Faktor bestimmt dann die Gutschrift: 100% entspricht einem Sieg, 50% einem
Remis und 0% einer Niederlage.

�ber die Schieber auf dem Reiter Einstellungen kann man die Prozentwerte festlegen.
Voreingestellt sind folgende Werte:

- 70% f�r Abwesenheit wg. *Vereinsverpflichtungen*
- 35% f�r Entschuldigtes Fehlen
- 35% f�r unentschuldigtes Fehlen
- 75% f�r Spiele gegen Zur�ckgetretene.
- 50% f�r Freilos

<br />

### Erkl�rung der Optionen

#### Anzahl Runden bis Paarung wiederholbar

Minimale Rundenzahl, bis zwei Spieler erneut gegeneinander antreten d�rfen.

*   0 bedeutet daher: zwei Spieler k�nnen gegeneinander gepaart werden, auch wenn sie
schon in der vorhergehenden Runde gegeneinander spielten.
*   99 bedeutet, zwei Spieler k�nnen erst nach 99 Runden (praktisch also nie im ganzen
Turnier) nochmal gegeneinander kommen.
*   Wenn ein Turnier 7 Runden hat und diese Zahl auf 2 gesetzt ist, kann es sein, dass einige Paarungen dreimal im Turnier stattfinden, n�mlich in
der 1., 4. und 7. Runde.

#### Freilose gleich verteilen
Betrifft bei einer ungeraden Anzahl anwesender Spieler die Vergabe des Freiloses. Wenn nicht angehakt, geht das Freilos in jedem Fall an den
schlechtest plazierten (auch wenn der schon drei und andere Spieler gar kein Freilos hatten).

#### Erste Runde Zufall
Im Keizer-System w�re die erste Runde jedes Mal die gleiche mit
denselben Spielern. Das kann f�r kleine Vereine l�stig sein. Wenn der
Wert nicht auf 0 gesetzt wird, sind die Farben der ersten Runde zuf�llig und eine 
eine Zufallszahl zwischen 0 und dem Wert wird zum Rating jedes Spielers addiert oder subtrahiert, um die Erstrundenpaarungen zu bestimmen.

#### Verh�ltnis Sieg gegen den Ersten zum Letzten

Im Keizer-System gibt es f�r einen Sieg gegen den Erstplatzierten mehr
Punkte als f�r einen Sieg gegen die anderen Spieler. 

Dieser Wert ist gleich dem Verh�ltnis der Punkte, die Sie f�r einen Sieg gegen den Erstplatzierten erhalten w�rden geteilt durch die Punkte, die man f�r einen Sieg gegen den letztplatzierten Spieler erhalten w�rde.
Je niedriger dieser Wert ist, desto n�her sind sich das Keizer-System und das Schweizer System. Drei ist der Standardwert. Je h�her die Zahl ist, desto mehr bleiben die besseren Spieler unter sich. Ich empfehle niedrigere Werte f�r Turniere mit
weniger als 20 Spielern. 

<br />

### Men� Listen

Durch Auswahl eines Eintrags im Listenmen� werden Ausgabelisten im Ordner _export_ erzeugt. Die Art der zu erzeugenden Ausgaben kann 
auf der Registerkarte _Einstellungen_ ausgew�hlt werden.

#### Html Ausgabe
Die Html-Ausgabe besteht aus allgemeinem Css, das f�r jede Tabelle gleich ist, und Html, das die eigentlichen Daten enth�lt. 
Das Css wird aus der Datei _export/keizer.css_ �bernommen. Wenn Sie m�chten, k�nnen Sie das Css an Ihre eigenen Bed�rfnisse anpassen. 

Die Datentabellen erhalten die folgenden Tags, wobei die Angaben in _{}_ durch echte Daten ersetzt werden.
```
id='kfc-{tournament name}-{table type}-{current round}'
class='my-wrapper kfc-wrapper kfc-wrapper-{number of columns}'
/* Die Kreuztabelle kann sehr breit werden, daher erh�lt sie andere Css-Klassen: */
class='my-exwrapper kfc-exwrapper kfc-exwrapper-{number of columns}'
```

#### Kreuztabelle Details

##### Rank-Pb

Die Spalte _Rank-Pb_ steht f�r "Keizer-*Rank*-*P*oints *b*efore der aktuellen Runde". Wenn Sie mit der Maus �ber die Spalten�berschrift fahren, sehen Sie einen Tooltip. 

Ich werde versuchen, das zu erkl�ren.

Die Keizer-Punkte, die ein Spieler A f�r einen Sieg gegen Spieler B erh�lt, sind genau dieser Wert, Rank-Pb.    Ich habe die Spalte eingef�hrt, um die Keizer-Punkte manuell nachrechnen zu k�nnen.  Es gab Fragen von Vereinskameraden...

Vgl. den Screenshot unten, der einen Teil einer Tabelle eines Beispielturniers zeigt.

Sie sehen zum Beispiel, dass der Rank-Pb von Spieler 4 (Sascha) 22 ist. Das bedeutet, dass Sascha in der Runde vor dieser Runde so in der Tabelle platziert war, dass sein "Wert" 22 Keizer-Rang-Punkte betr�gt. Das bedeutet, dass du f�r jeden Sieg gegen Sascha 22 Punkte bekommst. Und weil Andreas in Runde 1 gegen Sascha gewonnen hat, hat er daf�r 22 Keizer-Punkte bekommen. Das k�nnen Sie in der Tabelle unter R1 sehen.

Noch einer: Rank-Pb von Spieler 2 (Andreas) ist 23. Deshalb: Axel, der in Runde 2 gegen Andreas unentschieden gespielt hat, hat daf�r 11,5 (11,5 = 23 / 2) Keizer-Punkte bekommen.  

![Example Tournament](./Rank-Pb.png)

<br />

### Men� Spieler

#### Spieler importieren
Man kann eine Liste mit Spielern im csv-Format importieren. Importiert werden Namen und Ratings der Spieler. 
Das Format der csv-Datei wird bestimmt durch die erste Zeile der Datei. In der ersten Zeile werden Trennzeichen (Strichpunkt, Doppelpunkt oder Komma), 
der Text _Name_ und der Text _Rating_ gesucht. Entsprechend dem Format der ersten Zeile wird der Rest der Datei eingelesen. 
Beispiel: Die erste Zeile der csv-Datei ist 
```
x;x;;xxx;Name;;Rating
``` 
Dann wird Trennzeichen der Strichpunkt, die Spielernamen sind in der 5. Spalte, die Ratings der Spieler sind in der 7. Spalte. 


#### Alle l�schen
L�scht alle Spieler. Dies ist n�tzlich, wenn Sie ein Turnier B erstellen m�chten, das genau die gleichen Einstellungen wie das vorherige Turnier A haben soll. 
Kopieren Sie in diesem Fall A.s3db nach B.s3db, l�schen Sie dann alle gespielten Runden und anschlie�end alle Spieler. 
Um sicherzustellen, dass Sie nicht versehentlich Ihr aktuelles Turnier l�schen, ist das L�schen aller Spieler nur m�glich, wenn keine Partien in der Datenbank vorhanden sind.   

#### Spieler-IDs neu vergeben
Nach dem Importieren einer Spielerliste, z. B. der Vereinsmitglieder, und dem anschlie�enden L�schen der Spieler, die nicht am Turnier teilnehmen, kann die Spielerliste 
in KeizerForClubs viele L�cher in der Reihenfolge der Spielernummern haben. Das ist ein bisschen �rgerlich. Zumal man an der h�chsten Spieler-ID nicht erkennen kann, wie viele Teilnehmer am Turnier teilnehmen.  
Sie k�nnen _Spieler-IDs neu vergeben_ verwenden, um die L�cher in der Reihenfolge der IDs zu entfernen. Nur m�glich am Anfang eines Turniers. 

<br />


### Danksagungen

*   Das Programm wurde urspr�nglich mit SharpDevelop entwickelt, jetzt verwende ich Microsoft Visual Studio Community 2022. 
*   Die Datenbank ist [SQLite](https://www.SQLite.org).
*   Die Software wird gehostet auf [Github](https://github.com/Dumuzy/KeizerForClubs/releases). 

Besonderer Dank geht 

*   Thomas Schlapp f�r die urspr�ngliche Entwicklung dieser Software.
*   J�rgen Kehr f�r Tests und andere wertvolle Informationen.
*   Pascal Golay f�r Tests und die �bersetzung ins Franz�sische. 

### Lizenz

Das Programm ist *Freeware*, wurde nach bestem Wissen und Gewissen entwickelt und
steht kostenlos zur Verf�gung. Die Benutzung ist u.a. an die Bedingung gekn�pft, das
jedwede Garantie, Haftung und/oder Gew�hrleistung ausgeschlossen ist und keinerlei
Anspr�che welcher Art auch immer gegen die Autoren erhoben werden k�nnen.
Ausf�hrlich nachzulesen in der License.txt.


