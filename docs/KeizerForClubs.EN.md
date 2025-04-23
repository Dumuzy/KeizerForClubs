
<br />

## KeizerForClubs

<br />

### General

The KeizerForClubs software allows to manage e.g. chess tournaments
using the Keizer system with up to 100 players. 
<br />

### How is a tournament conducted?

The essential steps in brief:

-   Open the program, select *Start...* from the menu. Each
    tournament is stored in a small database: open an existing file to
    continue a tournament, choose a new file name to start a new
    one.
-   For a new tournament enter the names of all participating players
    (if possible with rating, but it will work without
    also).
-   You can also enter players who are not present at the first round,
    but will come later to running tournaments or add new players in
    later rounds.
-   Before drawing the first or next round: set the status of all
    players (present, excused etc.).
-   Now create the next round: it can be done by the program or
    manually.
-   If necessary, create lists: ranking, pairings, current participants
    list ...
-   Exit the program until the next round.

<br />

### Short description of the GUI

The program is quite self-explanatory, just a few
lines...

-   In the beginning with *Start...* a tournament shall be created.
    Later you can switch to another tournament with
    *Start...*
-   Then at the left the three tabs can be selected: *Players*, *Pairings*,
    *Settings*.
-   In order to add participants, just write the name and rating into
    the table. Choose the status in the last column by clicking the
    *down arrow* and choose from the list. ID is assigned
    automatically.
-   To draw a new round, select one of the menu items in the
    *Pairing* menu. The instructions for manual pairing are listed
    below.
-   The pairings menu is active only when the tab pairings is
    selected.
-   You can display all rounds played so far using the arrow up/down
    buttons beside the round number.
-   Set game results by selecting from the list (again by clicking the
    *down arrow*).
-   Results can be corrected anytime (when e.g. *1-0* entered instead
    of *0-1*; not required for retired participants).
-   The Lists menu is for exporting rankings etc.

<br />

### Instructions for the window *Manual pairing*

The menu item opens a new window, listing the names of all available
players (status *present*, has to be set before) on the left and *free
boards* on the right.

-   Click a name on he left and an empty space on the right: the player
    is set to this *board*.
-   Click on a board with a name: the player returns to the
    left.
-   Click OK to accept entries - they will be checked before
    accepted.
-   If the number of players was odd, the list on the left may contain
    one name who will get the bye. On the right any *board* has to
    have 2 players (or 0).

<br />

### Description of some specific scenarios

#### No PC in the playing hall?

Then the tournament director determines the pairings for that round
using the current ranking. Rules are described below. Later the results
of the round can be entered using *manual pairing*. 

#### New players entering the tournament

Just add the name to the list: the player is immediately taken into
account for pairing and ranking.

#### Players leaving the tournament

Set the status of the player to *retired*. The player will no longer be taken into account for paring and ranking,
his counterparts of already played games get a bonus for their
games. The status can be changed back to active anytime.

If a player cancels before the first round, you can set his state to
*deleted*, then he'll be deleted completely.

#### Rules for pairing

-   Pairing is done using the players who have shown up.
-   Go top-down trough the ranking: the highest ranked plays against
    the 2nd ranked, 3rd - 4th and so on.
-   The ranking is sorted by Keizer points in descending order; for the
    first round the rating is used.
-   Colour distribution is *fair* - the one with less white games gets
    white.
-   If both equally often had white and the players already had played
    aginst each other, the colours of their last game are reversed.
    Otherwise white is given to the lower ranked. 
-   Unlike in the Swiss system three times consecutively the same colour
    might happen.
-   If an odd number of players show up: the lowest-ranked player gets
    a bye.
-   Options:
    -   The bye can go to the second lowest ranked player instead (etc.)
        when double-byes shall be avoided. Also, a player who has missed a
        game won't get a bye before all other players have gotten a bye or
        missed a game. 
    -   Pairings may be repeated after n rounds.

<br />

### Evaluation parameters

In the Keizer system one can also get points when not playing. The
credit varies according to the reason for absence. For the calculation
it is assumed that you play against yourself using a percentage factor.
100% corresponds to a victory, 50% a draw and 0% to a loss.

The percentage values can be chosen on the settings tab. By default,
the following values are set:

-   70% for absence due to *club commitments*
-   35% for excused absence 
-   35% for unexcused absence
-   75% for games against withdrawn players.
-   50% for bye


#### Toughness Bonus 

In KeizerForClubs, you can get some fraction of points for playing, even if you lose. You get the toughness bonus times the Keizer-Rank-Points of your opponent for losing a game. I recommend values at 2-5 %.  

*Reasoning:*

1. A defeat being always 0 feels a bit unfair, because if you loose against the tournament winner, that means something different than if you lose against the last of the tournament. 
Therefore in swiss system you have what in german is called *Buchholz*, in english it is called *SOS* or *sum of opponent scores*, in french *SPA* or *somme des points des adversaires*. It is the sum of all the points of your opponents. You get more *SOS* if you've lost against the winner than if you've lost against the last. The toughness bonus is the KeizerForClubs equivalent to  *Buchholz*, *SOS* or *SPA*.

2. With the Keizer system, players who don't play may get considerable amount of points. It feels unfair against those who play and lose, that the losers all the time get zero but the avoiders still get points.


<br />

### Explanation of options

#### \# rounds before pairings repeat

This is the minimum number of rounds before two players may compete
against each other again. 

*   0 means two players may be paired against each other again in the very next round.
*   99 (or any other high number) ensures two players will meet only once in a tournament.
*   If a tournament has 7 rounds and this number is set to 2, it could be that some pairings take place 3 times in the tournament, namely in
the 1st, 4th and 7th round. 

#### Allocate bye's even
When this option is not set, the bye is always given to the lowest
ranked player when an odd number of players show up for a round.


#### First round random
In the Keizer system, the first round would be each time the same with
the same players. That may be annoying for small clubs. When setting
this value not to 0, the first round colours are random and a random
number between 0 and the value is added or subtracted from each players
rating for determining the first round pairings.

#### Random board
In chess, usually the strongest players play on the front boards in tournaments. This is not good for Novuss (*disc billiards*), a game played mainly in Latvia and Estonia.

Why? With Novuss, every table is slightly different because it is made of wood. In the tournament, random tables should be assigned because when a player plays at the same table for the second time, he knows the specific table - so it is important that he plays at a different table in the next game.

If *Random board* is checked, the board or table numbers will be randomly assigned.

#### Ratio win against first to last

In the Keizer system, a win against the first-ranked player gets more
points than a win against the other players. 

This value is the points you'd get for a win against the first ranked
player divided by the points you'd get against the last ranked player.
The lower this number, the closer are Keizer system and Swiss system. 3
is the default. The higher the number, the more the better players stay
separated from the others. I recommend lower values for tournaments with
less than 20 players. 

#### Normalization of victory

If this checkbox is checked, all the Keizer points 
are normalized so that a win against the last in the rank counts 1 Keizer point. 

This doesn't really change anything in the calculation of the rankings or the pairing.
But it somehow makes the Keizer points much more graspable.

(In the original Keizer system the given Keizer points usually are big whole numbers.
Originally, this has probably been the case to make the caclculations easier. 
It's easier to calculate with whole numbers if you don't have a computer.

Nowadays, all the calculation is done by the computer and so we can use much 
smaller but broken numbers.) 

#### Categories
Imagine you've got a tournament where you want to have separate standings tables for different 
categories of players, e.g. youngsters or below rating 1500. In this case, you can use the 
categories option. Here you have to define your categories together with an abbreviation for 
every category. Say you want to have the categories "Rating < 1500" and "Female".
Then you could define these categories and their abbreviations here as
```
r15=Rating < 1500, f=Females
``` 
You must use commas here to separate different categories and you must use the equality sign to separate
abbreviation from full category name. The abbreviations shall be used in the players table to assign one 
or more categories to a player. Also the abbreviations are used in the filenames for the reports. 
The full category names are used in the header of the reports. The abbreviations must not contain any
spaces or commas or other special characters. Only a-z, A-Z and 0-9 are allowed. 

<br />

### Lists menu
By selecting an entry in the lists menu, output lists will be generated in the folder _export_. The type of outputs to be generated can be 
selected on the _Settings_ tab. 

#### Html Output
The html output consists of general css which is the same for every table and html which contains the real data. 
The css is taken from the file _export/keizer.css_ which should stay untouched. 
You can change the user.css to match your own needs if you want to. 

The data tables get the following tags, where the stuff in _{}_ is replaced by real data:
```
id='kfc-{tournament name}-{table type}-{current round}'
class='my-wrapper kfc-wrapper kfc-wrapper-{number of columns}'
/* The cross table can get very wide, so it gets the different css classes: */
class='my-exwrapper kfc-exwrapper kfc-exwrapper-{number of columns}'
```

#### Cross table details

##### Rank-Pb
The column _Rank-Pb_ stands for "Keizer-*Rank*-*P*oints *b*efore the current round". If you hover over the column header of the html with a mouse, you'll see a tooltip. 

I'll try to explain.

The Keizer points a player A gets for a victory against Player B is just this value, Rank-Pb.    I've introduced the column for purposes of manually recalculating the Keizer points.  There were questions by club mates...

Cf. the screenshot below, which is a part of a table of an example tournament.

You see for example Rank-Pb of player 4 (Sascha) is 22. This means, in the round before this one, Sascha was placed such in the table, that his "value" is 22 Keizer-Rank-Points. Which means, that you get 22 points for every victory against Sascha. And, because Andreas has won against Sascha in round 1, he's got 22 Keizer-points for that. Which you can see in the table under R1.

Another one: Rank-Pb of player 2 (Andreas) is 23. Therefore: Axel, who drew in round 2 against Andreas, got 11.5 (11.5 = 23 / 2) Keizer points for that draw.  

![Example Tournament](./Rank-Pb.png)

<br />

### Players menu

#### Import players
You can import a list of players in csv format. The name and rating of the players are imported. 
The format of the csv file is determined by the first line of the file. In the first line separators (semicolon, colon or comma), 
the text _Name_ and the text _Rating_ are searched for. The rest of the file is read according to the format of the first line. 
Example: The first line of the csv file is 
```
x;x;;xxx;Name;;Rating
``` 
Then the separator is the semicolon, the player names are in the 5th column, the ratings of the players in the 7th.

#### Delete All
Deletes all players. This is useful if you want to create a tournament B which shall have exactly the same settings as previous tournament A. 
In this case, copy A.s3db to B.s3db, then delete all played rounds, then delete all players. 
To make sure that you don't accidentally delete your current tournament, deleting all players is only possible without any games in the database.   

#### Rebase player ids
After importing a list of players, e.g. the club members, and then deleting those not playing in the tournament, the player list 
in KeizerForClubs may have a lot of holes in the sequence of player ids. This is a bit annoying. Especially as you can't see from the 
highest player id the number of participants in the tournament.  
You can use _Rebase player ids_ to remove the holes in the sequence of ids. 

<br />

### Automated post-processing of tables
It may be desirable to do something automatically with generated tables immediately after they have been created. For example, you may want to upload the tables somewhere or you may want to change or add to the tables in one way or another. You can imagine many things. 

In KeizerForClubs there is an interface for this: If there is a `script` directory next to the `export` directory and there is a file called `kfcpost.cmd` in the directory, this file is called as a batch file and the path to the table just created is passed as a parameter. 
You can of course call all other possible commands from this cmd file. 

<br />

### Keizer System and Swiss System 

#### Comparison of Keizer and Swiss Pairing Systems

<style>
th, td {
    padding: 5px;
}
tr:hover {background-color: coral;}
tr:nth-child(odd) {background-color: #f2f2f2;}
</style>

<table style="max-width: 800px">
<colgroup>
<col span="1" style="width: 20%;">
<col span="1" style="width: 40%;">
<col span="1" style="width: 40%;">
</colgroup>
<tr><th>Property</th><th>Keizer</th><th>Swiss</th></tr>
<tr><th>Pairing </th><td>is done usually shortly before the round is played.</td><td>may be done days or weeks before the round is played.</td></tr>
<tr><th>Strong players get paired against each other</th><td>from round one.</td><td>only later.</td></tr>
<tr><th>Player has no opponent</th><td> Because pairing is done shortly before the round, this is not happening often.</td><td>May happen often.</td></tr>
<tr><th>Points for victory</th><td>More points for victory against better players.</td><td>One point for every victory, 
  plus fine evaluation like *sum of opponent scores* or SOS.</td></tr>
<tr><th>Points for loss</th><td>In the original Keizer just zero. In KeizerForClubs it's possible  to award a toughness bonus.
This replaces the SOS fine evaluation in Swiss. </td><td>Zero, but counts for SOS fine evaluation.</td></tr>
<tr><th> Color management </th><td>Color plays no role in pairing.  Who had relatively less white gets white. In parctice, the difference to Swiss color management is marginal.</td><td>Ideally each round different for every player, nobody gets ever three times the same color in a row.  This is enforced during pairing. </td></tr>
<tr><th>Lacking players</th><td> Usually get fractions of points. </td><td> One or two *bye-draws* are often allowed. </td></tr>
<tr><th>Games against resigned players</th><td>Special handling used.</td><td> Results may be cancelled for fine evaluation.</td></tr>
<tr><th>Same opponent in later round</th><td>may be allowed.</td><td>is never allowed.</td></tr>
<tr><th></th><td></td><td></td></tr>
</table>


#### Keizer-Swiss Hybrid Systems

One can think of many different Keizer-Swiss hybrid systems. With KeizerForClubs, you can create lots of them. For example:

* A victory against anybody counts one point plus fine evaluation. Similar to Swiss.
* Colors are managed like in Keizer.
* Players that are not there can get fractions of points like in Keizer or Swiss with bye's.
* Players which have games against resigned players get full points like in Swiss.
* Principally, a player can play against another one twice, like in Keizer - or not, like in Swiss. This can be managed completely
by the tournament director by increasing the setting "# rounds before pairing repeat" beyond the
number of rounds of the tournament.



### Acknowledgements

*   The program originally was developed with SharpDevelop, now I'm using Microsoft Visual Studio Community 2022. 
*   The database is [SQLite](https://www.SQLite.org).
*   The software is hosted on [Github](https://github.com/Dumuzy/KeizerForClubs/releases). 

Special thanks to 

*   Thomas Schlapp for the original development of this software.
*   Jürgen Kehr for tests and other valuable information.
*   Pascal Golay for tests, discussions and translation to french. 

### License

The program is available for free. It comes without any warranty or
guarantee, use at own risk. See also the License.txt.
