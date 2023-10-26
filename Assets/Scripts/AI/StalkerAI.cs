using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class StalkerAI : NetworkBehaviour
{
    // Referencia a el objetivo del agente de navegación
    public Transform target;
    // Referencia al agente de navegación
    private NavMeshAgent agent;
    // Distancia a detenerse
    [SerializeField] private float stalkingDistance;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (NetworkManager.Singleton.IsServer) {
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
            if (Vector2.Distance(transform.position, target.position) > stalkingDistance) {
                agent.SetDestination(target.position);
            } else {
                agent.SetDestination(transform.position);
            }
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
            } else {
                float closestDistance = Mathf.Infinity;
                foreach(GameObject p in players){
                    float distance = (Vector2.Distance(transform.position, p.transform.position)) * (1/p.GetComponent<PlayerController>().aiPriority);
                    if (distance < closestDistance){
                        closestDistance = distance;
                        target = p.transform;
                    }
                }
            }
        }
    }
}
