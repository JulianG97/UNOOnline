# UNOOnline #
An console implementation of the classic card game UNO with online multiplayer written in C#

## Protocol ##
```
|  UNO   |   TP   |  Content   |    !   |
| Header |  Type  |  Content   |   End  |
| 3 byte | 2 byte | 0 - n byte | 1 byte |
```

### Protocol Types ###

| Type | Full Name | Value | Length | Description |
| ---- | --------- | ----- | ------ | ----------- |
| CG | Create Game | AmountOfPlayers | 1 (+ 6) | Client sends create game request to the server with the amount of players |
| JG | Join Game | GameID | n (+ 6) | Client sends join game request to the server with the game ID |
| RL | Room List | RoomID-JoinedPlayers-NeededPlayers-... | n (+ 6) | Server sends list of all open rooms to the client |
| RR | Request Rooms | - | 0 (+ 6) | Client requests room list from the server |
| OK | OK | - | 0 (+ 6) | Server verifies client request |
| IN | Invalid | - | 0 (+ 6) | Server declines client request |
| GS | Game Start | LobbyID-PlayerID | 3 (+ 6) | Server sends game start with lobby ID and player ID to all clients of a game |
| IA | Is Alive | - | 0 (+ 6) | Server sends is alive to client and client responses |
| RI | Round Information | LastCardColor-LastCardValue-PlayerWhoIsOnTurn-Player1AmountOfCards-Player2AmountOfCards-... | 9-13 (+ 6) | Information sent every round from the server to the client |
| PC | Player Cards | Card1Color-Card1Value-Card2Color-Card2Value-... | n (+ 6) | Cards of the player sent from the server to the client when game starts or player is on turn |
| SC | Set Card | LobbyID-PlayerID-CardColor-CardValue-UnoYesOrNo | 9 (+ 6) | Card set by the player sent from the client to the server. UnoYesOrNo = 0 if player didn't call UNO; 1 if player call UNO |
| GO | Game Over | PlayerID | 1 (+ 6) | Game end with winner ID sent from the server to the client when game is over |

### Card Protocol ###
| Color | Protocol | Description |
| ----- | -------- | ----------- |
| Red | R-0 | - |
| Red | R-1 | - |
| Red | R-2 | - |
| Red | R-3 | - |
| Red | R-4 | - |
| Red | R-5 | - |
| Red | R-6 | - |
| Red | R-7 | - |
| Red | R-8 | - |
| Red | R-9 | - |
| Red | R-T | Draw Two |
| Red | R-R | Reverse |
| Red | R-S | Skip |
| Blue | B-0 | - |
| Blue | B-1 | - |
| Blue | B-2 | - |
| Blue | B-3 | - |
| Blue | B-4 | - |
| Blue | B-5 | - |
| Blue | B-6 | - |
| Blue | B-7 | - |
| Blue | B-8 | - |
| Blue | B-9 | - |
| Blue | B-T | Draw Two |
| Blue | B-R | Reverse |
| Blue | B-S | Skip |
| Green | G-0 | - |
| Green | G-1 | - |
| Green | G-2 | - |
| Green | G-3 | - |
| Green | G-4 | - |
| Green | G-5 | - |
| Green | G-6 | - |
| Green | G-7 | - |
| Green | G-8 | - |
| Green | G-9 | - |
| Green | G-T | Draw Two |
| Green | G-R | Reverse |
| Green | G-S | Skip |
| Yellow | Y-0 | - |
| Yellow | Y-1 | - |
| Yellow | Y-2 | - |
| Yellow | Y-3 | - |
| Yellow | Y-4 | - |
| Yellow | Y-5 | - |
| Yellow | Y-6 | - |
| Yellow | Y-7 | - |
| Yellow | Y-8 | - |
| Yellow | Y-9 | - |
| Yellow | Y-T | Draw Two |
| Yellow | Y-R | Reverse |
| Yellow | Y-S | Skip |
| Wild | W-C | Wild |
| Wild | W-F | Wild Draw Four |

### Amount of Cards ###
In total: 108 cards
![picture alt](https://raw.githubusercontent.com/JulianG97/UNOOnline/master/UNO%20Card%20Deck.png "UNO Card Deck")
