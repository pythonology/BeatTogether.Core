﻿using System.Collections.Generic;
using System.Net;
using System.Threading;
using BeatTogether.Core.Messaging.Abstractions;
using BeatTogether.Core.Messaging.Models;

namespace BeatTogether.Core.Messaging.Implementations
{
    public abstract class BaseSession : ISession
    {
        public EndPoint EndPoint { get; }
        public uint Epoch { get; set; }
        public EncryptionParameters? EncryptionParameters { get; set; }

        private uint _lastSentSequenceId = 0;
        private uint _lastSentRequestId = 0;
        private object _handledRequestsLock = new object();
        private HashSet<uint> _handledRequests = new HashSet<uint>();
        private HashSet<uint> _cachedHandledRequests = new HashSet<uint>();

        public BaseSession(EndPoint endPoint)
        {
            EndPoint = endPoint;
        }

        public uint GetNextSequenceId()
            => unchecked(Interlocked.Increment(ref _lastSentSequenceId));

        public uint GetNextRequestId()
            => (unchecked(Interlocked.Increment(ref _lastSentRequestId)) % 64) | Epoch;

        public bool ShouldHandleRequest(uint requestId)
        {
            lock (_handledRequestsLock)
            {
                if (_cachedHandledRequests.Contains(requestId))
                    return false;
                if (!_handledRequests.Add(requestId))
                    return false;
                if (_handledRequests.Count >= 32)
                {
                    _cachedHandledRequests = _handledRequests;
                    _handledRequests = new HashSet<uint>();
                }
                return true;
            }
        }
    }
}
