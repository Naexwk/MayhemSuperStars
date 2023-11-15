using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LocalDestroyer : NetworkBehaviour
{
    void Start(){
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>() != null) {
                player.GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
