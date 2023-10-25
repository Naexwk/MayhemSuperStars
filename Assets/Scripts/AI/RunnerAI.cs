using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class RunnerAI : NetworkBehaviour
{
    // Referencia a el objetivo del agente de navegación
    public Transform target;
    // Referencia al agente de navegación
    private NavMeshAgent agent;

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
            agent.SetDestination(target.position);
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
