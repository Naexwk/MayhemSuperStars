using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class ZombieScript : NetworkBehaviour
{
    public float updateTimer = 1f;

    public float speed;
    private Transform target;
    private NavMeshAgent agent;

    public NetworkVariable<int> health = new NetworkVariable<int>();

    private Animator animator;
    //public int health;

    // Valores iniciales
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.Value = 5;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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

    // Al entrar en contacto con una bala de queso, recibir daño
    /*void OnCollisionEnter2D(Collision2D col)
    {

        // Funciona exclusivamente con la bala de queso porque es la única con Collider, no trigger
        if (col.gameObject.tag == "PlayerBullet")
        {
            ZombieGetHitServerRpc(col.gameObject.GetComponent<CheeseBullet>().bulletDamage);
        }
    }*/
    
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

    [ServerRpc(RequireOwnership = false)]
    public void ZombieGetHitServerRpc(int damage) {
        health.Value -= damage;
        if (health.Value <= 0) {
            Destroy(this.gameObject);
        }
    }
}
