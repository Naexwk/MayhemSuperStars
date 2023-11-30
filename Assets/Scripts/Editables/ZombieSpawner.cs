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
    [SerializeField] private GameObject zombieSpawnParticles;

    int owner;

    private bool hasCoroutines = false;

    // Escuchar al cambio de estado de juego
    private void Awake() {
        GameManager.state.OnValueChanged += StateChange;
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        // Si empieza una ronda de juego, parar todas las corutinas de spawns de zombies e
        // iniciar una corutina nueva de spawn de zombies
        if (this != null) {
            if (curr == GameState.Round || curr == GameState.StartGame) {
                owner = GetComponent<propOwner>().owner;
                if (hasCoroutines) {
                    StopAllCoroutines();
                    hasCoroutines = false;
                }
                if (NetworkManager.Singleton.IsServer) {
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
    }

    // Aparecer un zombie cada timeToSpawn segundos
    IEnumerator SpawnZombie() {
        if (this != null) {
            hasCoroutines = true;
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
            Instantiate(zombieSpawnParticles, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            GameObject clone;
            clone = Instantiate(zombiePrefab, transform.position, Quaternion.identity);
            clone.GetComponent<damageSource>().owner = owner;
            clone.GetComponent<NetworkObject>().Spawn();
            StartCoroutine(SpawnZombie());
        }
    }
}
