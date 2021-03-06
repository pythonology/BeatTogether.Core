﻿using BeatTogether.Core.Messaging.Abstractions;
using Krypton.Buffers;

namespace BeatTogether.Core.Messaging.Messages
{
    public class AcknowledgeMessage : IEncryptedMessage, IReliableResponse
    {
        public uint SequenceId { get; set; }
        public uint ResponseId { get; set; }
        public bool MessageHandled { get; set; }

        public void WriteTo(ref SpanBufferWriter bufferWriter)
        {
            bufferWriter.WriteBool(MessageHandled);
        }

        public void ReadFrom(ref SpanBufferReader bufferReader)
        {
            MessageHandled = bufferReader.ReadBool();
        }
    }
}
