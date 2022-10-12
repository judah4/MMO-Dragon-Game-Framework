// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters.Mmogf.Core
{
    using global::System.Buffers;
    using global::MessagePack;

    public sealed class EventDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Mmogf.Core.EventData>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Mmogf.Core.EventData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(4);
            writer.WriteNil();
            writer.Write(value.EntityId);
            writer.Write(value.ComponentId);
            writer.Write(value.Payload);
        }

        public global::Mmogf.Core.EventData Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Mmogf.Core.EventData();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 1:
                        ____result.EntityId = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.ComponentId = reader.ReadInt16();
                        break;
                    case 3:
                        ____result.Payload = reader.ReadBytes()?.ToArray();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name
