using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletSpawner : NetworkBehaviour
{
    public Rigidbody BulletPrefab;
    private float bulletSpeed = 80f;

    public float timeBetweenBullets = .5f;
    private float shotCountDown = 0f;

    private void Update()
    {
        if (IsServer)
        {
            if(shotCountDown > 0)
            {
                shotCountDown -= Time.deltaTime;
            }
        }
    }

    [ServerRpc]
    public void FireServerRpc(ServerRpcParams rpcParams = default)
    {
        if(shotCountDown > 0)
        {
            return;
        }

        Rigidbody newBullet = Instantiate(BulletPrefab, transform.position, transform.rotation);
        newBullet.velocity = transform.forward * bulletSpeed;
        newBullet.gameObject.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        Destroy(newBullet.gameObject, 3);

        shotCountDown = timeBetweenBullets;
    }
}
