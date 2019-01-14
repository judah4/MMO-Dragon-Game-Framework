
# MMO Game Framework

Proof of concept for SpatialOS style entities and load balancing with distributed server workers. 

### Warning  

Probably should not try to use. This was all written quickly over the weekend after the whole issue with Unity and Improbable.  Might not even cover 1% of what a full mmo networking framework can do like SpatialOS.


## Features

* Central server that handles message passing between the client and server workers.  
* An entity data store. Entities are the dynamic objects in the server. Entities have a list of data including position, type, and any custom data needed.  
* Protobuf messages for message passing to the client and server.  
* Example Unity3d client and server.  

## Future Work
* Add rotations.  
* Permissions for who can control what.
* Spread out the server workers.
* C# worker example without Unity3d.


![Server and Clients setup](https://img.youtube.com/vi/wfTIpBYMjlk/0.jpg)  

[Watch the video](https://youtu.be/wfTIpBYMjlk)  
