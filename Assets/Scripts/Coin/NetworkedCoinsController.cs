using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkedCoinsController : NetworkBehaviour
{
    [SerializeField] private GameObject Coin;
    [SerializeField] private float radius = 8.0f;
    [SerializeField] private int coinsAmount = 10;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        StartCoroutine(SpawnCoins());
    }

    IEnumerator SpawnCoins()
    {
        for (int i = 0; i < coinsAmount; i++)
        {
            Vector3 spawnLoc = (Vector3.forward * Random.Range(-radius, radius)) + (Vector3.right * Random.Range(-radius, radius));
            GameObject coin = Instantiate(Coin, spawnLoc, Quaternion.identity);
            coin.GetComponent<NetworkObject>().Spawn();
            yield return new WaitForSeconds(.5f);
        }
    }
}
