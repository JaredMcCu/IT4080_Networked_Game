using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct NetworkPlayerInfo : INetworkSerializable, System.IEquatable<NetworkPlayerInfo>
{
    public ulong clientId;
    public bool ready;
    public NetworkPlayerInfo(ulong id)
    {
        clientId = id;
        ready = false;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref ready);
    }

    public bool Equals(NetworkPlayerInfo other) {
        return clientId == other.clientId;
    }
}
