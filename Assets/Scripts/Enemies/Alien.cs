using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Controlador del enemigo "Alien"
public class Alien : NetworkBehaviour
{
    // Vida del alien
    private NetworkVariable<int> health = new NetworkVariable<int>();

    // Velocidad de disparo
    [SerializeField] private float fireRate;
    // Variable de control para la velocidad de disparo
    private float timeSinceLastFire;

    // Rango máximo de disparo
    [SerializeField] private float distanceThreshold = 25f;

    // Fuerza y dirección de las balas
    private Vector2 Direction;
    [SerializeField] private float force;

    // Referencia al objeto que maneja las balas de red
    private GameObject bullethandler;

    private GameObject target;

    //Knockback variables
    private float knockbackForce = 15f;
    private float knockbackDuration = 0.2f;
    private Rigidbody2D rb;

    void Start () {
        bullethandler = GameObject.FindWithTag("BulletHandler");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        rb = GetComponent<Rigidbody2D>();
        // Inicializar vida
        if (NetworkManager.Singleton.IsServer) {
            health.Value = 7;
        }
    }

    // Al entrar en contacto con una bala, recibir daño
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "PlayerBullet")
        {
            if (col.gameObject.GetComponent<PlayerBullet>() != null) {
                AlienGetHitServerRpc(col.gameObject.GetComponent<PlayerBullet>().bulletDamage, col.gameObject.GetComponent<PlayerBullet>().bulletDirection);
            } else if (col.gameObject.GetComponent<CheeseBullet>() != null) {
                AlienGetHitServerRpc(col.gameObject.GetComponent<CheeseBullet>().bulletDamage, new Vector2(0,0));
            }
        }
    }

    // Al entrar en contacto con un jugador, dañarlo
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerController>().GetHit();
        }
    }

    // Actualizar la vida en la red
    [ServerRpc(RequireOwnership = false)]
    public void AlienGetHitServerRpc(int damage, Vector2 direction) {
        StartCoroutine(ApplyKnockback(direction));
        health.Value -= damage;
        if (health.Value <= 0) {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
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
        }

        // Si hay target, apuntarle con el cañón y disparar
        if (target != null) {
            Direction = target.transform.position - transform.position;
            Direction.Normalize();

            // Revisar si ya pasó el tiempo de espera de la velocidad de disparo
            if ((Time.time - timeSinceLastFire) > (fireRate)) {
                timeSinceLastFire = Time.time;
                Shoot();
            }
        }
        
    }

    // Llamar al Bullet Handler para que cree una bala de red
    void Shoot(){
        if (NetworkManager.Singleton.IsServer) {
            bullethandler.GetComponent<BulletHandler>().SpawnEnemyBulletServerRpc(force, Direction, transform.position.x, transform.position.y);
        }
    }

    //Knockback
    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        // Knockback direction
        Vector2 knockbackDirection = direction;

        // Apply a force to the enemy's Rigidbody2D
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        // Stop the knockback force after a duration
        rb.velocity = Vector2.zero;
    }
}
