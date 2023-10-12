using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Controlador del prop "Zombie Grave"
public class ZombieSpawner : NetworkBehaviour
{

    // Referencia al prefab de zombies
    [SerializeField] private GameObject zombiePrefab;
    // Tiempo en el que se spawnea un zombie (en segundos)
    [SerializeField] private float timeToSpawn;

    private bool hasCoroutines = false;

    // Escuchar al cambio de estado de juego
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameManager.state.OnValueChanged += StateChange;
        
    }

    public override void OnDestroy()
    {
        GameManager.state.OnValueChanged -= StateChange;
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        // Si empieza una ronda de juego, parar todas las corutinas de spawns de zombies e
        // iniciar una corutina nueva de spawn de zombies
        if (curr == GameState.Round || curr == GameState.StartGame) {
            if (hasCoroutines) {
                StopAllCoroutines();
                hasCoroutines = false;
            }
            if (IsServer) {
                StartCoroutine(SpawnZombie());
            }
            // Asegurarse que no tenga colisiones
            GetComponent<Rigidbody2D>().simulated = false;
        } else {
            // Detener todas las corutinas de spawns de zombies
            if (hasCoroutines) {
                StopAllCoroutines();
                hasCoroutines = false;
            }
            // Asegurarse que tenga colisiones (para detectar otros props colocados
            // en el modo editor)
            GetComponent<Rigidbody2D>().simulated = true;
        }
    }

    // Aparecer un zombie cada timeToSpawn segundos
    IEnumerator SpawnZombie() {
        hasCoroutines = true;
        yield return new WaitForSeconds(timeToSpawn);
        GameObject clone;
        clone = Instantiate(zombiePrefab, transform.position, transform.rotation);
        clone.GetComponent<NetworkObject>().Spawn();
        StartCoroutine(SpawnZombie());
    }

}
