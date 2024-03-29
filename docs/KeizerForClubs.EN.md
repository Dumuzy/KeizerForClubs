
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
-   Then at the bottom the three tabs can be selected: *Players*, *Pairings*,
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

#### Ratio win against first to last

In the Keizer system, a win against the first-ranked player gets more
points than a win against the other players. 

This value is the points you'd get for a win against the first ranked
player divided by the points you'd get against the last ranked player.
The lower this number, the closer are Keizer system and Swiss system. 3
is the default. The higher the number, the more the better players stay
separated from the others. I recommend lower values for tournaments with
less than 20 players. 

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

### Acknowledgements

*   The program originally was developed with SharpDevelop, now I'm using Microsoft Visual Studio Community 2022. 
*   The database is [SQLite](https://www.SQLite.org).
*   The software is hosted on [Github](https://github.com/Dumuzy/KeizerForClubs/releases). 

Special thanks to 

*   Thomas Schlapp for the original development of this software.
*   Jürgen Kehr for tests and other valuable information.
*   Pascal Golay for tests and translation to french. 

### License

The program is available for free. It comes without any warranty or
guarantee, use at own risk. See also the License.txt.
