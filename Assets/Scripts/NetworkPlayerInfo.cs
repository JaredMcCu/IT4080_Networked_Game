using UnityEngine;
using UnityEngine.Serialization;
using Unity.Netcode;
using System.Collections;

public struct NetworkPlayerInfo : INetworkSerializable, System.IEquatable<NetworkPlayerInfo>
{
    public ulong clientId;
    public bool ready;
    public Color color;
    //public FixedString32Bytes playerName;
    

    public NetworkPlayerInfo(ulong id)
    {
        clientId = id;
        ready = false;
        color = Color.magenta;
        //playerName = "not set";
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref ready);
        serializer.SerializeValue(ref color);
        //serializer.SerializeValue(ref playerName);
    }

    public bool Equals(NetworkPlayerInfo other) {
        return clientId == other.clientId;
    }
}
