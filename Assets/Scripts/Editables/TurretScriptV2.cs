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
    [SerializeField] private GameObject turretSmoke;
    [SerializeField] private GameObject turretHit;

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
    private bool disabled = false;

    private int health;

    private void Awake() {
        GameManager.state.OnValueChanged += StateChange;
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        if (curr == GameState.Round || curr == GameState.StartGame) {
            canShoot = true;
            disabled = false;
        } else {
            canShoot = false;
        }
    }


    void Start () {
        bullethandler = GameObject.FindWithTag("BulletHandler");
        health = 10;
    }

    void Update()
    {
        if (canShoot && !disabled) {
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

    void OnTriggerEnter2D(Collider2D collision) {
        // Destruir al tocar un muro
        if (collision.gameObject.tag == "PlayerBullet")
        {
            if (disabled) {
                return;
            }
            Instantiate(turretHit, transform.position, transform.rotation);
            if (collision.gameObject.GetComponent<CheeseBullet>() != null) {
                health -= collision.gameObject.GetComponent<CheeseBullet>().bulletDamage;
            }

            if (collision.gameObject.GetComponent<PlayerBullet>() != null) {
                health -= collision.gameObject.GetComponent<PlayerBullet>().bulletDamage;
            }
            if (health <= 0) {
                Instantiate(turretSmoke, transform.position, Quaternion.Euler(-90f,0f,0f));
                StartCoroutine(disableTurret());
                health = 10;
            }
        }
    }

    IEnumerator disableTurret(){
        disabled = true;
        yield return new WaitForSeconds(5f);
        disabled = false;
    }
}
