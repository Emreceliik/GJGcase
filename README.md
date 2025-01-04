# GJGcase - Game Concept and Mechanics

## Overview
In **GJGcase**, the goal is to collapse or blast groups of same-colored blocks. The minimum number of blocks required to form a collapsible or blastable group is **2**. The game offers a dynamic and engaging experience with different levels of complexity.

### Game Features
- **Number of Colors (K)**: The game can support between **1 and 6 different colors**, each color having a unique icon for easier recognition.
- **Board Dimensions**: The game board can range from **2 to 10 rows (M)** and **2 to 10 columns (N)**.
- **Filling Vacant Spaces**: Extra blocks to fill vacant areas are added from the top of the corresponding column.

### Icon Representation Based on Group Size
To help players identify larger groups, blocks will change their icons based on the group size:
- **Default Icon**: All blocks in a group show the default icon.
- **First Condition (A)**: If the group has more blocks than the minimum condition, it will display the **first icon**.
- **Second Condition (B)**: Groups that exceed the second threshold will show the **second icon**.
- **Third Condition (C)**: Groups that exceed the third threshold will show the **third icon**.

### Handling Deadlock Situations
Occasionally, the game might reach a **deadlock** where no collapsible or blastable group exists. Instead of relying on random shuffling, the game will implement a smart **shuffling solution** to resolve such situations without the need for repeated blind shuffling.

### Similar Games for Reference
For a better understanding of the collapse/blast mechanics and overall gameplay, you can check out the following games:
- **Toon Blast**
- **Lilly’s Garden**
- **Pet Rescue Saga**
<p align="center">
  
[Project Image](https://github.com/Emreceliik/GJGcase/raw/main/Assets/Assets/Images/Telefon.png)
</p>
### How to Play
1. Select a group of same-colored blocks.
2. Collapse or blast them for points.
3. Try to find larger groups for more rewards.
4. Watch out for deadlock situations and let the game handle it!

---

By adding these rules and mechanics, we aim to create a unique and enjoyable experience for players while keeping the gameplay fresh and challenging. Stay tuned for more updates!
