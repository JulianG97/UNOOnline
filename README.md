# UNOOnline #
An console implementation of the classic card game UNO with online multiplayer written in C#

## Protocol ##
```
|  UNO   |   TP   |  Content   |
| Header |  Type  |  Content   |
| 3 byte | 2 byte | 0 - n byte |
```

### Protocol Types ###

| Type | Full Name | Value | Length | Description |
| ---- | --------- | ----- | ------ | ----------- |
| CG | Create Game | AmountOfPlayers | 1 (+ 5) | Client sends create game request to the server with the amount of players |
| JG | Join Game | GameID | n (+5) | Client sends join game request to the server with the game id |
| GL | Game List | GameID-JoinedPlayers-NeededPlayers-... | n (+5) | Server sends list of all open games to the client |
| OK | OK | - | 0 (+5) | Server verifies client request |
| IN | Invalid | - | 0 (+5) | Server declines client request |
| GS | Game Start | - | 0 (+5) | Server sends game start to all clients of a game |
| IA | Is Alive | - | 0 (+5) | Server sends is alive to client and client responses |
| RI | Round Information | PlayerWhoIsOnTurn-LastCardColor-LastCardValue-Player1AmountOfCards-Player2AmountOfCards-... | 9-13 (+5) | Information sent every round from the server to the client |

### Card Protocol ###
| Color | Protocol | Description |
| ----- | -------- | ----------- |
| Red | R-1 | - |
| Red | R-2 | - |
| Red | R-3 | - |
| Red | R-4 | - |
| Red | R-5 | - |
| Red | R-6 | - |
| Red | R-7 | - |
| Red | R-8 | - |
| Red | R-9 | - |
| Red | R-D | Draw Two |
| Red | R-R | Reverse |
| Red | R-S | Skip |
| Blue | B-1 | - |
| Blue | B-2 | - |
| Blue | B-3 | - |
| Blue | B-4 | - |
| Blue | B-5 | - |
| Blue | B-6 | - |
| Blue | B-7 | - |
| Blue | B-8 | - |
| Blue | B-9 | - |
| Blue | B-D | Draw Two |
| Blue | B-R | Reverse |
| Blue | B-S | Skip |
| Green | G-1 | - |
| Green | G-2 | - |
| Green | G-3 | - |
| Green | G-4 | - |
| Green | G-5 | - |
| Green | G-6 | - |
| Green | G-7 | - |
| Green | G-8 | - |
| Green | G-9 | - |
| Green | G-D | Draw Two |
| Green | G-R | Reverse |
| Green | G-S | Skip |
| Yellow | Y-1 | - |
| Yellow | Y-2 | - |
| Yellow | Y-3 | - |
| Yellow | Y-4 | - |
| Yellow | Y-5 | - |
| Yellow | Y-6 | - |
| Yellow | Y-7 | - |
| Yellow | Y-8 | - |
| Yellow | Y-9 | - |
| Yellow | Y-D | Draw Two |
| Yellow | Y-R | Reverse |
| Yellow | Y-S | Skip |
| Wild | W-C | Wild |
| Wild | W-D | Wild Draw Four |
