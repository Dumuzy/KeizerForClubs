
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


