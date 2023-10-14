using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletSpawner : NetworkBehaviour
{
    public Rigidbody BulletPrefab;
    private float bulletSpeed = 20f;

    [ServerRpc]
    public void FireServerRpc(ServerRpcParams rpcParams = default)
    {
        Rigidbody newBullet = Instantiate(BulletPrefab, transform.position, transform.rotation);
        newBullet.velocity = transform.forward * bulletSpeed;
        newBullet.gameObject.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        Destroy(newBullet.gameObject, 3);
    }
}
