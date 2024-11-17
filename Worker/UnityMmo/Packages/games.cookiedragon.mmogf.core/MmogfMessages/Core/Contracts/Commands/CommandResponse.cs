﻿using System.Runtime.Serialization;

namespace Mmogf.Core.Contracts.Commands
{
    [DataContract]
    public struct CommandResponse
    {
        [DataMember(Order = 0)]
        public CommandResponseHeader Header { get; set; }

        [DataMember(Order = 1)]
        public byte[] Payload { get; set; }

        public CommandResponse(CommandResponseHeader header, byte[] payload)
        {

            Header = header;
            Payload = payload;
        }
    }
}
