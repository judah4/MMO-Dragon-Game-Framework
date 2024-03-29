
# MMO Dragon Game Framework

<a href="https://matrix.to/#/#dragongf:matrix.org" target="_blank" ><img src="https://img.shields.io/badge/chat-matrix-blue" ></a>

Proof of concept networking entities and load balancing with distributed server workers. 

### Warning  

Slowly and surely improving but probably not ready for a big game yet. Under heavy development.

## Tech
* C# Core
* [Lidgren Networking](https://github.com/lidgren/lidgren-network-gen3)
* MessagePack for Serialization

## Features

* Central server that handles message passing between the client and server workers.  
* An entity data store. Entities are the dynamic objects in the server. Entities have a list of data including position, type, and any game data needed.  
* MessagePack  for message passing to the client and server.  
* Example Unity3d client and server.  

## Future Work
* Better area entity checkouts.
* Multiple server workers for distributed processing.

# Getting Started

1. Open the Unity Project at `Workers/UnityMmo`.
2. Open the scene `Scenes/SampleScene.unity`.
3. Run the sever from the tool bar `Dragon GF/Run Server (Local)`.

![Server and Clients setup](https://img.youtube.com/vi/f6h_A6oPgyM/0.jpg)  

[Watch the video](https://youtu.be/f6h_A6oPgyM)  

# Important Links!

https://github.com/neuecc/MessagePack-CSharp#aot-code-generation-support-for-unityxamarin

## Setting up MessagePack Codegen
-i 
../packages/games.cookiedragon.mmogf.core/MmogfMessages
-o 
../packages/games.cookiedragon.mmogf.core/Scripts/Generated
-r
MmogfCoreResolver


### and

-i
MmogfMessages
-o
Scripts/Generated


# Docker Build

Build Server build in the `Builds/Linux` folder.  
Run in the main folder.  

## Game Build
```SHELL
docker build -f Docker/Worker/Dockerfile -t judah4/dragongf-testgame:v0.1.2 -t judah4/dragongf-testgame:latest .  
```
Example build file for the test game.

## Run On Docker
```SHELL
docker compose -f Docker/Compose/docker-compose.yml up -d  
```

## Push
```SHELL
docker push judah4/dragongf-testgame:latest  
docker push judah4/dragongf-testgame:v0.1.2  
```
# Agones and Kubernetes

`kubectl create -f Agones/dragongf-gameserver.yaml`

# Core Build

Run in the main folder.  

## Main Server Build
```SHELL
docker build -f Docker/MainServer/Dockerfile -t judah4/dragongf:v0.1.2 -t judah4/dragongf:latest .
```
Only required for core dev.

## Push

```SHELL
docker push judah4/dragongf:latest  
docker push judah4/dragongf:v0.1.2  
``` 
