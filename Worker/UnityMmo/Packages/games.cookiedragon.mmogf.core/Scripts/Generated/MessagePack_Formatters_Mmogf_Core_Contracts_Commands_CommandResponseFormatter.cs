// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters.Mmogf.Core.Contracts.Commands
{
    public sealed class CommandResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Mmogf.Core.Contracts.Commands.CommandResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Mmogf.Core.Contracts.Commands.CommandResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Mmogf.Core.Contracts.Commands.CommandResponseHeader>(formatterResolver).Serialize(ref writer, value.Header, options);
            writer.Write(value.Payload);
        }

        public global::Mmogf.Core.Contracts.Commands.CommandResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __Header__ = default(global::Mmogf.Core.Contracts.Commands.CommandResponseHeader);
            var __Payload__ = default(byte[]);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __Header__ = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Mmogf.Core.Contracts.Commands.CommandResponseHeader>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        __Payload__ = global::MessagePack.Internal.CodeGenHelpers.GetArrayFromNullableSequence(reader.ReadBytes());
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Mmogf.Core.Contracts.Commands.CommandResponse(__Header__, __Payload__);
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
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name