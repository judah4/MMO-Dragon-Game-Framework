# MMO Game Framework

Proof of concept for SpatialOS style entities and load balancing with distributed server workers.

## Features
Central server that handles message passing between the client and server workers.  
A Entity, dynamic object, data store. Entities have a list of data including position, type, and any custom data needed.  
Protobuf messages for message passing to the client and server.  
