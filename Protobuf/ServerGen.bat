protoc -I=Server --csharp_out=../MessageProtocols Server/ClientConnect.proto
protoc -I=Server --csharp_out=../MessageProtocols Server/EntityInfo.proto
protoc -I=Server --csharp_out=../MessageProtocols Server/GameData.proto
protoc -I=Server --csharp_out=../MessageProtocols Server/ServerConnect.proto
protoc -I=Server --csharp_out=../MessageProtocols Server/SimpleMessage.proto