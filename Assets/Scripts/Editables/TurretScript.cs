using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TurretScript : MonoBehaviour
{
    private GameObject Target;
    private List<GameObject> playersIn = new List<GameObject>();
    private bool detected = false;
    public GameObject alertLight;
    public GameObject canonGun;
    public GameObject bullets;
    public Transform shootPoint;
    public float force;
    private float timer;
    Vector2 Direction;
    public float fireRateTimer;

    private GameObject bullethandler;

    private GameObject nearestPlayer;
    private float distanceToNearestPlayer = -1;
    private float distanceToPlayer;
    // Buscar bullet handler, que mandará las señales al server
    void Start () {
        bullethandler = GameObject.FindWithTag("BulletHandler");
    }


    // Al entrar un jugador, añadirlo a los posibles objetivos
    void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player") && !playersIn.Contains(other.gameObject)){
            playersIn.Add(other.gameObject);
        }
    }

    // Al salir un jugador (por movimiento o muerte), 
    // eliminarlo de los posibles objetivos
    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") || other.CompareTag("Dead Player")){
            playersIn.Remove(other.gameObject);
        }
    }


    void Update()
    {
        if (playersIn.Count > 0){

            // Cambio visual de la torreta
            detected = true;
            alertLight.GetComponent<SpriteRenderer>().color = Color.red;

            distanceToNearestPlayer = -1f;

            // Buscar al jugador más cercano y seleccionarlo como objetivo
            foreach (GameObject player in playersIn){

                    distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
                    if ((distanceToPlayer < distanceToNearestPlayer) || distanceToNearestPlayer == -1f) {
                        distanceToNearestPlayer = distanceToPlayer;
                        nearestPlayer = player;
                    }

            }
            Target = nearestPlayer;
        }
        // Si no hay objetivos, desactivarse
        else{
            distanceToNearestPlayer = -1f;
            Target = null;
            detected = false;
            alertLight.GetComponent<SpriteRenderer>().color = Color.black;
        }

        // Obtener la dirección hacia el objetivo, mover el cañón
        // y si se cumple el firerate, disparar
        if (detected){
            Direction = Target.transform.position - transform.position;
            Direction.Normalize();
            canonGun.transform.up = Direction;
            timer += Time.deltaTime;
            if (timer > fireRateTimer){
                timer = 0;
                shoot();
            }
        }
    }

    // Llamar al bulletHandler para que instancie una bala
    void shoot(){
        if (bullethandler.GetComponent<NetworkObject>().IsOwner) {
            //bullethandler.GetComponent<BulletHandler>().SpawnEnemyBulletServerRpc(force, Direction, shootPoint.position.x, shootPoint.position.y);
        }
    }
}
