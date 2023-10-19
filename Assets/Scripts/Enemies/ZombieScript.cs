using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

// Controlador del enemigo "Zombie"
public class ZombieScript : NetworkBehaviour
{

    // Referencia a el objetivo del agente de navegación
    private Transform target;
    // Referencia al agente de navegación
    private NavMeshAgent agent;

    // Vida del zombie
    private NetworkVariable<int> health = new NetworkVariable<int>();

    private Animator animator;
    private SpriteRenderer m_SpriteRenderer;

    // Valores iniciales
    void Start()
    {
        animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // Aleatorizar el color del zombie
        m_SpriteRenderer.color = new Color(1f, 1f, Random.Range(0f, 1f));
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // Inicializar vida
        if (NetworkManager.Singleton.IsServer) {
            health.Value = 5;
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }

    // Moverse a su target
    void Update()
    {
        FindPlayer();
        if (target != null){
            agent.SetDestination(target.position);
        }
    }

    // Al entrar en contacto con una bala, recibir daño
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "PlayerBullet")
        {
            if (col.gameObject.GetComponent<PlayerBullet>() != null) {
                ZombieGetHitServerRpc(col.gameObject.GetComponent<PlayerBullet>().bulletDamage);
            } else if (col.gameObject.GetComponent<CheeseBullet>() != null) {
                ZombieGetHitServerRpc(col.gameObject.GetComponent<CheeseBullet>().bulletDamage);
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
    
    // Buscar al jugador más cercano
    void FindPlayer()
    {
        if (IsOwner) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            // Si no hay jugadores, no moverse
            if (players.Length == 0) {
                target = transform;
                animator.SetBool("isMoving", false);
            } else {
                float closestDistance = Mathf.Infinity;
                foreach(GameObject p in players){
                    float distance = Vector2.Distance(transform.position, p.transform.position);
                    animator.SetBool("isMoving", true);
                    if (distance < closestDistance){
                        closestDistance = distance;
                        target = p.transform;
                    }
                }
            }
        }
        
        
    }

    // Actualizar la vida en la red
    [ServerRpc(RequireOwnership = false)]
    public void ZombieGetHitServerRpc(int damage) {
        health.Value -= damage;
        if (health.Value <= 0) {
            Destroy(this.gameObject);
        }
    }
}
