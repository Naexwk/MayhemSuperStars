using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Dropship : NetworkBehaviour
{
    [SerializeField] private int spawnRange;
    [SerializeField] private float spawnTime;
    [SerializeField] private float speed;
    [SerializeField] private GameObject alienPrefab;
    [SerializeField] private GameObject alienRay, alienSpawn;
    private Vector3 spawnPosition;
    private Vector3 helperSpawnPosition;
    private Vector3 targetPosition;
    private float timeSinceLastSpawn;
    private Vector2 initialPosition;
    private bool isMoving = false;
    private bool canMove = true;
    private bool WaitingToSpawn = false;

    // Escuchar al cambio de estado de juego
    private void Awake() {
        GameManager.state.OnValueChanged += StateChange;
        initialPosition = new Vector2(transform.position.x, transform.position.y);
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        // Si empieza una ronda de juego, parar todas las corutinas de spawns de zombies e
        // iniciar una corutina nueva de spawn de zombies
        if (this != null && IsServer) {
            if (curr == GameState.Round || curr == GameState.StartGame) {
                StopAllCoroutines();
                isMoving = false;
                canMove = true;
                WaitingToSpawn = false;
                StartCoroutine(SearchNewSpawnPosition());
                // Asegurarse que no tenga colisiones
                GetComponent<Rigidbody2D>().simulated = false;
            } else {
                StopAllCoroutines();
                isMoving = false;
                canMove = false;
                transform.position = initialPosition;
                GetComponent<Rigidbody2D>().simulated = true;
            }
        }
    }

    IEnumerator SearchNewSpawnPosition () {
        bool foundValidPosition = false;
        Vector2 helperSpawnPosition = new Vector3 (initialPosition.x + UnityEngine.Random.Range(-spawnRange, spawnRange), 
        initialPosition.y + UnityEngine.Random.Range(-spawnRange, spawnRange));
        RaycastHit2D hit = Physics2D.Raycast(helperSpawnPosition, Vector2.zero);
        if (hit.collider == null)
        {
            spawnPosition = helperSpawnPosition;
            targetPosition = new Vector3 (spawnPosition.x, spawnPosition.y + 10f, 0f);
            foundValidPosition = true;
        } else if (hit.collider.tag != "Wall" && hit.collider.tag != "MapBorders") {
            spawnPosition = helperSpawnPosition;
            targetPosition = new Vector3 (spawnPosition.x, spawnPosition.y + 10f, 0f);
            foundValidPosition = true;
        }

        if (foundValidPosition) {
            isMoving = true;
        } else {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SearchNewSpawnPosition());
        }
    }

    private void Update() {
        if (IsServer) {
            if (isMoving && canMove) {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            }

            if (transform.position == targetPosition) {
                if (!WaitingToSpawn) {
                    WaitingToSpawn = true;
                    StartCoroutine(WaitToSpawn());
                }
            }
        }
    }

    IEnumerator WaitToSpawn(){
        isMoving = false;
        canMove = false;
        helperSpawnPosition = spawnPosition;
        spawnPosition = new Vector3(0f,0f,0f);
        targetPosition = new Vector3(0f,0f,0f);
        StartCoroutine(SearchNewSpawnPosition());
        Instantiate(alienRay, transform.position, Quaternion.Euler(90f,-90f,0f));
        Instantiate(alienSpawn, helperSpawnPosition, Quaternion.Euler(0f,0f,0f));
        yield return new WaitForSeconds(2f);
        SpawnAlien();
        StartCoroutine(WaitToMove());
        WaitingToSpawn = false;
    }

    IEnumerator WaitToMove(){
        yield return new WaitForSeconds(3);
        canMove = true;
    }

    private void SpawnAlien(){
        if (this != null && IsServer) {
            GameObject clone;
            clone = Instantiate(alienPrefab, helperSpawnPosition, transform.rotation);
            clone.GetComponent<NetworkObject>().Spawn();
        }
    }
}
