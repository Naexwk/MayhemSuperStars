using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Controlador del prop "Turret"
public class TurretScriptV2 : MonoBehaviour
{
    // Referencias a partes de la torreta
    [SerializeField] private GameObject alertLight;
    [SerializeField] private GameObject canonGun;
    [SerializeField] private Transform shootPoint;

    private GameObject[] players;
    private GameObject target;

    // Rango máximo de disparo
    [SerializeField] private float distanceThreshold = 25f;

    // Velocidad de disparo
    [SerializeField] private float fireRate;

    // Variable de control para la velocidad de disparo
    private float timeSinceLastFire;

    // Referencia al objeto que maneja las balas de red
    private GameObject bullethandler;

    // Fuerza y dirección de las balas
    private Vector2 Direction;
    [SerializeField] private float force;

    private bool canShoot = false;

    private void Awake() {
        GameManager.state.OnValueChanged += StateChange;
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        if (curr == GameState.Round || curr == GameState.StartGame) {
            canShoot = true;
        } else {
            canShoot = false;
        }
    }


    void Start () {
        bullethandler = GameObject.FindWithTag("BulletHandler");
    }

    void Update()
    {
        if (canShoot) {
            float closestDistance = Mathf.Infinity;
            // Actualizar lista de jugadores
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            // Revisando uno por uno, encontrar al más cercano y designarlo como target
            foreach(GameObject player in players){
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance < closestDistance && distance < distanceThreshold){
                    closestDistance = distance;
                    target = player.gameObject;
                }
            }

            // Si no se encontró a un jugador, borrar el target y apagar la luz de alerta
            if (closestDistance == Mathf.Infinity) {
                target = null;
                if (alertLight != null) {
                    alertLight.GetComponent<SpriteRenderer>().color = Color.black;
                }
            }

            // Si hay target, apuntarle con el cañón y disparar
            if (target != null) {
                alertLight.GetComponent<SpriteRenderer>().color = Color.red;
                Direction = target.transform.position - transform.position;
                Direction.Normalize();
                canonGun.transform.up = Direction;

                // Revisar si ya pasó el tiempo de espera de la velocidad de disparo
                if ((Time.time - timeSinceLastFire) > (fireRate)) {
                    timeSinceLastFire = Time.time;
                    Shoot();
                }
            }
        }
    }

    // Llamar al Bullet Handler para que cree una bala de red
    void Shoot(){
        if (bullethandler.GetComponent<NetworkObject>().IsOwner) {
            bullethandler.GetComponent<BulletHandler>().SpawnEnemyBulletServerRpc(force, Direction, shootPoint.position.x, shootPoint.position.y);
        }
    }
}
