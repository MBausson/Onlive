# Onlive - Online Game Of Life

This project includes a server and a client app for an online implementation of [John Conway&#39;s game of life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life).
Language used: **C#**
Libraries / technologies: **SFML**, `TcpClient`

## Installation

### Server

This project provides a `Dockerfile` file, allowing you to easily deploy a server.
The default port used is `8001` : this can be changed in the `Dockerfile`, via the application arguments.

### Client

You will need `.NET 9.0` in order to build this application.
In the `Online` project directory, run this command: `dotnet build --configuration Release`. The build output should be in `bin\Release\net9.0`.
In that directory, run this command: `.\Onlive.exe <IP> <PORT>` (IP & Port refer to the server, default values are `127.0.0.1` & `8001`)

## How to play

Switch a single cell

- Left Click

Toggle Stash mode

- Left Shift

Switch every cells in the Stash

- Enter

Move around the map

- Z ; Q ; S ; D

Zoom-in and Zoom-out

- Scroll up ; Scroll down
