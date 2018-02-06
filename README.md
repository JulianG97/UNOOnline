# UNOOnline
An console implementation of the classic card game UNO with online multiplayer written in C#

## Protocol
```
|  UNO   |   TP   |  Content   |
| Header |  Type  |  Content   |
| 3 byte | 2 byte | 0 - n byte |
```

### Protocol Types

| Type | Full Name | Value | Length | Description |
| ---- | --------- | ----- | ------ | ----------- |
| CG | Create Game | AmountOfPlayers | 1 (+ 5) | Client sends create game request to the server with the amount of players |
| JG | Join Game | GameID | n (+5) | Client sends join game request to the server with the game id |
| GL | Game List | GameID-JoinedPlayers-NeededPlayers-... | n (+5) | Server sends list of all open games to the client |
| OK | OK | - | 0 (+5) | Server verifies client request |
| IN | Invalid | - | 0 (+5) | Server declines client request |
| GS | Game Start | - | 0 (+5) | Server sends game start to all clients of a game |
