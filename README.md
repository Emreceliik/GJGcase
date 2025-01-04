# GJGcase
The minimum number of same colored blocks to create a collapsible / blastable group is 2. 
Total number of colors (K) in a game can be varied between 1 to 6, all colors should have a 
different icon for easier recognition by the player. Board can have 2 to 10 rows (M) and 2 to 
10 columns (N). Extra blocks needed to fill vacant areas should be created at the outside of 
the board and drop from the top of the corresponding column.
Additionally, we want players to find bigger groups easier, therefore we want to have different 
icons on blocks based on the number of items in corresponding groups. In other words;
● All the blocks in a group should display default icons by default, if not changed by the 
following rules.
● If the group has more blocks than first condition(A) it should display first icon,
● If the group has more blocks than second condition(B) it should display second icon,
● If the group has more blocks than the third condition(C) it should display the third 
icon.
Occasionally this mechanic can create a deadlock situation due to lack of any collapsible / 
blastable group on the board. We want the game to detect these situations and implement 
a shuffling solution which doesn’t rely on “blindly shuffle N times until deadlock is resolved”.
You can find examples for the above definitions and rules at the end of the document.
Finally for better understanding of the collapse/blast mechanic and gameplay, you can play 
Toon Blast, Lilly’s Garden or Pet Rescue Saga games .

![Project Image](https://github.com/Emreceliik/GJGcase/raw/main/Assets/Assets/Images/Telefon.png)
