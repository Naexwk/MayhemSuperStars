using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ZombieSpawner : NetworkBehaviour
{
    // DEV: Variable de control, implementar
    public bool isInPlay;

    public GameObject zombiePrefab;
    public float timeToSpawn;

    private bool hasCoroutines = false;

    // Empezar a instanciar zombies
    // DEV: Debería parar si no está en juego (!isInPlay)


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameManager.State.OnValueChanged += StateChange;
        
    }

    public override void OnDestroy()
    {
        GameManager.State.OnValueChanged -= StateChange;
    }

    private void StateChange(GameState prev, GameState curr){
        if (curr == GameState.Round || curr == GameState.StartGame) {
            if (hasCoroutines) {
                StopAllCoroutines();
                hasCoroutines = false;
            }
            if (IsServer) {
                StartCoroutine(spawnZombie());
            }
            GetComponent<Rigidbody2D>().simulated = false;
        } else {
            if (hasCoroutines) {
                StopAllCoroutines();
                hasCoroutines = false;
            }
            GetComponent<Rigidbody2D>().simulated = true;
        }
    }

    // Aparecer un zombie cada timeToSpawn segundos
    IEnumerator spawnZombie() {
        hasCoroutines = true;
        yield return new WaitForSeconds(timeToSpawn);
        spawnZombieServerRpc();
        StartCoroutine(spawnZombie());
    }

    // Llamar al server para spawner al zombie en la red
    [ServerRpc(RequireOwnership = false)]
    public void spawnZombieServerRpc(){
        GameObject clone;
        clone = Instantiate(zombiePrefab, transform.position, transform.rotation);
        clone.GetComponent<NetworkObject>().Spawn();
    }


    /*[ClientRpc]
    public void disableHitboxClientRpc(){
        this.game
    }*/
}
