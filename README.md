# MMO Game Framework

Proof of concept for SpatialOS style entities and load balancing with distributed server workers.

## Features

* Central server that handles message passing between the client and server workers.  
* An entity data store. Entities are the dynamic objects in the server. Entities have a list of data including position, type, and any custom data needed.  
* Protobuf messages for message passing to the client and server.  
* Example Unity3d client and server.  

## Future Work
* Permissions for who can control what.
* Spread out the server workers.
* C# worker example without unity.


[![Watch the video](https://youtu.be/wfTIpBYMjlk)]