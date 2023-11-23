using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class Catnado : NetworkBehaviour
{

    [SerializeField] private float succccc;
    [SerializeField] private float fireRate;
    [SerializeField] private float force;
    Vector2 startingPos;
    bool enableControl = false;
    float timeSinceLastFire;
    // Referencia al objeto que maneja las balas de red
    private GameObject bullethandler;

    private void Awake() {
        GameManager.state.OnValueChanged += StateChange;
        bullethandler = GameObject.FindWithTag("BulletHandler");
        startingPos = transform.position;
        transform.rotation = Quaternion.Euler(-90f,0f,0f);
    }

    void Start () {
        bullethandler = GameObject.FindWithTag("BulletHandler");
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        // Si empieza una ronda de juego, parar todas las corutinas de spawns de zombies e
        // iniciar una corutina nueva de spawn de zombies
        if (this != null) {
            if (curr == GameState.Round || curr == GameState.StartGame) {

                enableControl = true;
                transform.position = startingPos;
                gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = false;
            } else {
                enableControl = false;
                transform.position = startingPos;
                gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
                
            }
        }
    }

    // Al entrar un jugador, añadirlo a los posibles objetivos
    void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")){
            Vector2 direction = (transform.position - other.transform.position) ;
            direction.Normalize();
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * succccc);
        }
    }

    void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Player"){
            other.gameObject.GetComponent<PlayerController>().GetHit();
        }
    }

    private void Update() {
        if ((Time.time - timeSinceLastFire) > fireRate && enableControl) {
            timeSinceLastFire = Time.time;
            Shoot();
        }
    }

    private void Shoot(){
        Vector2 direction = new Vector2 (UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        direction.Normalize();
        if (NetworkManager.Singleton.IsServer) {
            bullethandler.GetComponent<BulletHandler>().SpawnEnemyBulletServerRpc(force, direction, transform.position.x, transform.position.y + 4f);
        }
    }

}
