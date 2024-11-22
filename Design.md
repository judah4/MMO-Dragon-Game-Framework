
# Architecture

## Terminology

* Main Server
* Dragon Wing (Server Worker)

## Main Server

The main hub that is the source of truth for entity data.

##  Mesh Server

Distributed servers that connect to the main server and handle messages from server workers.

## Server Worker

The server the game client messages. This has authority over the connect game client and updates entity states to the mesh server. This is built with the game engine.

# Client (Worker)

The game client that communicates with the server worker. It also has a connection with the mesh worker in case it needs to switch to a different server worker.

# Single Server Dev Mode

Dev Mode has the main server also be the mesh server to send message to workers.


# Connection Flow

## Login

Game Client connects to the main server. (Maybe start with a server locator to directly connect to a mesh server?)
Main server hands off game client to a mesh server.
Mesh Server gives authority of client to a server worker.
Server worker creates player entities. The player is officially in the game.

## Server Boundary Transfer

Mesh Server sees client is on boundary.
Mesh Server tells server worker and client to connect. (Tell client to connect to server? Not sure on the exact flow yet)
Once client is in other worker's area of interest, swap authority to the second worker.
Disconnect client and first worker from each other once far enough away.

## Server Worker Crash

Mesh Server tells server worker and client to connect to closest server worker. Client should have some secondary connections in case of needing to take over.
Swap authority to the second worker.

# Server Worker Redundancy

Each entity has only a single worker with authority over it, but multiple server workers will see the entity. 
If the entity crosses a boundary or if a worker crashes, another server worker will gain authority and handle the entity instead.
