
<br />

## KeizerForClubs

<br />

### Général

Le logiciel KeizerForClubs permet de gérer des tournois d'échecs utilisant le système Keizer jusqu'à 100 joueurs. 
<br />

### Comment se déroule un tournoi?

Les étapes essentielles en bref:

-   Ouvrez le programme, sélectionnez *Start...* dans le menu. Chaque tournoi est enregistré dans une petite base de données: ouvrez un fichier existant pour continuer un tournoi, choisissez un nouveau nom de fichier pour en commencer un nouveau.
-   Pour un nouveau tournoi, entrez les noms de tous les joueurs participants (si possible avec un classement élo, mais cela fonctionnera aussi sans).
-   Vous pouvez également inscrire des joueurs qui ne seront pas présents lors de la première ronde, mais qui joueront lors de rondes ultérieures du tournoi. Vous pouvez aussi ajouter de nouveaux joueurs en tout temps lors des rondes suivantes.
-   Avant d'apparier la première ronde ou la ronde suivante: définissez le statut de tous les joueurs (présents, excusés, etc.).
-   À présent, appariez la prochaine ronde: cela peut réalisé automatiquement par le programme ou manuellement.
-   Si nécessaire, créez des fichiers des listes: classement, appariements, liste actuelle des joueurs, etc.
-   Quitter le programme jusqu'à la prochaine ronde.

<br />

### Brève description de l'interface graphique

L'interface du programme est assez explicite, quelques lignes d'explication suffisent...

-   Pour commencer, un tournoi doit être créé avec *Start...*. Après, vous pourrez passer à un autre tournoi avec *Start...*.
-   Les trois onglets en bas de l'interface peuvent ensuite être sélectionnés (*Joueurs*, *Appariements*, *Paramètres*).
-   Pour ajouter des joueurs, il suffit d'inscrire leur nom et leur classement élo dans le tableau. Choisissez le statut dans la dernière colonne en cliquant sur *la flèche vers le bas* et sélectionnez une option dans la liste. L'ID est attribué automatiquement.
-   Pour apparier une nouvelle ronde, sélectionnez l'une des possibilités du menu *Appariements*. Les explications concernant l'appariement manuel se trouvent plus bas.
-   Le menu pour les appariements n'est actif que lorsque l'onglet des appariements est sélectionné.
-   Vous pouvez afficher et naviguer dans toutes les rondes jouées jusqu'à présent en utilisant les boutons fléchés haut/bas qui se trouvent à côté du numéro de la ronde.
-   Inscrivez les résultats des parties en les sélectionnant dans la liste (à nouveau en cliquant sur *la flèche vers le bas*).
-   Les résultats peuvent être corrigés à n'importe quel moment (lorsque, par exemple, *1-0* est saisi au lieu de *0-1*. Cela n'est nécessaire pour les joueurs qui se sont retirés du tournoi).
-   Le menu *Listes* permet d'exporter des fichiers du classement, des appariements, etc.

<br />

### Explications pour la fenêtre *Appariements manuels*

L'option du menu ouvre une nouvelle fenêtre avec, à gauche, la liste des noms de tous les joueurs disponibles (c'est-à-dire des joueurs dont le statut a été défini sur "Présent" auparavant) et, à droite, un *tableau vide*.

-   Cliquez sur un nom de joueur dans la liste de gauche puis sur une cellule vide du tableau à droite: le nom du joueur s'inscrit dans cette *cellule*.
-   Cliquez dans une cellule contenant déjà le nom d'un joueur: le nom du joueur est renvoyé dans la liste de gauche et la cellule redevient vide.
-   Cliquez sur le bouton "OK" pour enregistrer les appariements. Ils seront vérifiés avant d'être acceptés.
-   Si le nombre de joueurs est impair, la liste des noms des joueurs à gauche doit contenir à la fin seulement le nom du joueur qui doit obtenir le bye. *Chaque ligne* du tableau de droite dont contenir les noms de deux joueurs (ou d'aucun joueur).

<br />

### Description de quelques scénarios particuliers

#### Pas d'ordinateur dans la salle de jeu?

Dans ce cas, le directeur du tournoi déterminera les appariements pour la ronde en utilisant le classement actuel du tournoi. Les règles pour les appariements sont expliquées plus bas. Ensuite, les résultats de la ronde pourront être entrés dans le logiciel en utilisant *l'appariement manuel*.

#### Nouveaux joueurs entrant dans un tournoi en cours

Il suffit juste d'ajouter leurs noms dans la liste des joueurs: ces nouveaux joueurs seront immédiatement pris en compte pour les appariements des rondes suivantes et dans le classement du tournoi.

#### Joueurs qui se retirent du tournoi

Mettez le statut de ces joueurs sur *Retiré*.

Ces joueurs ne seront plus pris en compte pour les appariements et le classement du tournoi. Les adversaires de ces joueurs retirés reçoivent un bonus pour leurs parties jouées contre eux.

Le statut des joueurs retirés peut être remis à actif (présent) à n'importe quel moment.

Si un joueur renonce à participer au tournoi avant le début de la première ronde, vous pouvez mettre son statut sur *Effacer* et il sera alors complètement supprimé de la liste des joueurs.

#### Règles pour les appariements

-   L'appariement d'une ronde se fait en fonction des joueurs présents.
-   Parcourez le classement du tournoi du premier au dernier: le premier classé affronte le 2e, le 3e affronte le 4e et ainsi de suite.
-   Le classement du tournoi est établi par ordre décroissant des points Keizer. Pour la première ronde, ce sont les points de classement élo des joueurs qui sont utilisés.
-   La distribution des couleurs doit être *équitable*: le joueur qui a jusqu'ici joué avec le moins de fois avec les Blancs joue avec les Blancs.
-   Si les deux joueurs ont eu un même nombre de fois les Blancs et qu'ils ont déjà joué l'un contre l'autre lors d'une ronde précédente, alors les couleurs sont inversées par rapport à cette partie précédente, sinon les Blancs sont attribués au joueur le moins bien classé dans le tournoi. 
-   Contrairement au système suisse, la même couleur peut être attribuée à un joueur trois fois de suite.
-   Si un nombre impair de joueurs est présent, le joueur le moins bien classé reçoit un bye.
-   Options:
    -   Le bye peut être attribué à l'avant-dernier joueur le moins bien classé, etc. afin d'éviter qu'un même joueur reçoive deux fois le bye. En outre, un joueur qui a manqué une ronde ne recevra pas de bye avant que tous les autres joueurs aient reçu un bye ou manqué une ronde.
    -   Les mêmes appariements peuvent être répétés après n rondes.

<br />

### Les paramètres

Dans le système Keizer, il est également possible d'obtenir des points lorsqu'on ne joue pas. Le nombre de points varie en fonction de la raison de l'absence. Pour le calcul, on considère que vous jouez contre vous-même en utilisant un coefficient en pourcentage. 100% correspondent à une victoire, 50% à un match nul et 0% à une défaite.

Les valeurs en pourcentage peuvent être choisies dans l'onglet des paramètres. Par défaut, les valeurs suivantes sont définies:

-   70% en cas d'absence en raison *d'un engagement pour le club*.
-   35% pour une absence excusée.
-   35% pour une absence non excusée.
-   75% pour les parties contre des joueurs qui se sont retirés.
-   50% pour un bye.

<br />

### Explication des options

#### \# rondes avant répét. appariement

Il s'agit du nombre minimum de rondes avant que deux joueurs puissent s'affronter à nouveau.

*   0 signifie que deux joueurs peuvent être de nouveau appariés ensemble lors de la ronde suivante.
*   99 (ou tout autre nombre élevé) garantit que deux joueurs ne se rencontreront qu'une seule fois durant tout le tournoi.
*   Si un tournoi comporte 7 rondes et que ce nombre est fixé à 2, il se peut que certains mêmes appariements se répètent trois fois dans le tournoi, à savoir aux 1re, 4e et 7e rondes.

#### Distribuer le bye équitablement
Si cette option n'est pas activée, le bye est toujours attribué au joueur le moins bien classé du tournoi lorsqu'un nombre impair de joueurs se présente à une ronde.

#### Degré d'aléatoire pour la première ronde
Dans le système Keizer, l'appariement de la première ronde est à chaque fois identique avec les mêmes joueurs au départ. Cela peut être gênant pour des tournois dans des petits clubs où ce sont toujours les mêmes joueurs qui jouent les tournois. Lorsque cette valeur est réglée sur un nombre différent de 0, l'attribution des couleurs lors de la première ronde devient aléatoire et un nombre aléatoire compris entre 0 et la valeur est ajouté ou soustrait du classement élo de chaque joueur pour déterminer les appariements de la première ronde.

#### Ratio du gain entre 1er et dernier

Dans le système Keizer, une victoire contre le joueur classé premier du tournoi rapporte plus de points qu'une victoire contre les autres joueurs.

Cette valeur correspond aux points que vous obtiendriez pour une victoire contre le joueur le mieux classé du tournoi, divisés par les points que vous obtiendriez contre le joueur le moins bien classé du tournoi. Plus ce chiffre est petit, plus le système Keizer et le système suisse sont proches. 3 est la valeur par défaut. Plus le chiffre est élevé, plus les joueurs les mieux classés du tournoi se distinguent des autres. Je recommande des valeurs plus basses pour les tournois de moins de 20 joueurs.


<br />

### Menu Listes
En sélectionnant une entrée dans le menu des listes, des listes de sorties seront générées dans le dossier _export_. Le type de sorties à générer peut être sélectionné 
être sélectionné dans l'onglet _Paramètres_. 

#### Sortie Html
La sortie html se compose d'un code css général qui est le même pour chaque tableau et d'un code html qui contient les données réelles. 
Les css proviennent du fichier _export/keizer.css_. Vous pouvez modifier les css pour les adapter à vos besoins si vous le souhaitez. 

Les tableaux de données reçoivent les balises suivantes, où les éléments dans _{}_ sont remplacés par de vraies données :
```
id='kfc-{tournament name}-{table type}-{current round}'
class='my-wrapper kfx-wrapper kfc-wrapper-{number of columns}'
/* The cross table can get very wide, so it gets the different css classes: */
class='my-exwrapper kfx-exwrapper kfc-exwrapper-{number of columns}'
```


<br />

### Menu Joueurs

#### Importer des joueurs
Il est possible d'importer une liste de joueurs au format csv. Le nom et le classement du joueur sont importés. 
Le format du fichier csv est déterminé par la première ligne du fichier. Dans la première ligne, les séparateurs (point-virgule, deux-points ou virgule), 
le texte _Name_ et le texte _Rating_. Le reste du fichier est lu en fonction du format de la première ligne. 
Exemple : la première ligne du fichier csv est 
```
x;x;;xxx;Name;;Rating
``` 
Ensuite, le séparateur est le point-virgule, les noms des joueurs sont dans la 5e colonne, les ratings des joueurs sont dans la 7e colonne. 


### Remerciements

*   Le programme a été développé à l'origine avec SharpDevelop, mais j'utilise à présent Microsoft Visual Studio Community 2022. 
*   La base de données est [SQLite](https://www.SQLite.org).
*   Le logiciel est hébergé sur [Github](https://github.com/Dumuzy/KeizerForClubs/releases). 

Remerciements particuliers à

*   Thomas Schlapp pour le développement initial de ce logiciel.
*   Jürgen Kehr pour ses tests et ses autres précieuses informations.
*   Pascal Golay pour ses tests et la traduction en français. 

### Licence

Le programme est mis à disposition gratuitement. Il est fourni sans aucune garantie; l'utilisation se fait à ses propres risques et périls. Cf. aussi le fichier License.txt.

