# lottery
A simplified lottery game as a console application using
C#. Which follows the specifications below closely.

## Ticket Purchase
- The user (Player 1) will be prompted via the console to purchase their
desired number of tickets.
- The remaining participants are computer-generated players (CPU),
labelled sequentially as Player 2, Player 3, etc. Their ticket purchases
are determined randomly by the system.

## Player Limits and Ticket Cost
- The total number of players in the lottery game should range
between 10 and 15.
- Each player is allowed to purchase between 1 and 10 tickets.
- Each player starts with a balance of $10.
- Each ticket costs $1.
- The system should ensure that no player can purchase more tickets
than their balance allows. If a player attempts to purchase more
tickets, only the number of tickets they can afford will be purchased

## Prize Determination
The lottery program should distribute prizes according to the following
rules:

- Grand Prize: A single ticket will win 50% of the total ticket revenue.
- Second Tier: 10% of the total number of tickets (rounded to the
nearest whole number) will share 30% of the total ticket revenue
equally.
- Third Tier: 20% of the total number of tickets (rounded to the
nearest whole number) will share 10% of the total ticket revenue
equally.

## Important

- Tickets that have already won a prize are excluded from further prize tiers.
- Any remaining revenue after all prizes have been distributed will be the
house profit.
- If the number of winners for a prize tier is not exactly divisible by the number of winners of
that tier, the closest equal split should be calculated, and any remaining amount should be added to
the house profit.

## Output Requirements
The program should output the following information to the console:
1. A list of all players and the number of tickets each purchased.
2. The list of winning players, including:
a. The prize tier they won (Grand, Second, or Third).
b. The amount they won.
3. The total casino (house) profit