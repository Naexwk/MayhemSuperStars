using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPlayer : MonoBehaviour
{
    public int bulletDamage = 5;
    public float distanceThreshold = 25f;
    public float fireRate = 0.33f;
    public float force;
    public float bulletSpeed = 5f;
    public Transform waypoint;
    private bool runAway = false;
    private float timeSinceLastFire;
    private float closestDistance = Mathf.Infinity;
    private float distance; 
    private GameObject bullethandler;
    private Vector2 Direction;
    private GameObject[] enemies;
    private GameObject target;
    private NavMeshAgent agent;

    void Start () {
        bullethandler = GameObject.FindWithTag("BulletHandler");
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Shooting
        closestDistance = Mathf.Infinity;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies){
            
            distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance < distanceThreshold){
                closestDistance = distance;
                //Debug.Log("Closest distance: " + closestDistance);
                target = enemy.gameObject;
            }
        }

        if (closestDistance == Mathf.Infinity) {
            target = null;
        }

        if (target != null) {
            Direction = target.transform.position - transform.position;
            Direction.Normalize();

            if ((Time.time - timeSinceLastFire) > (fireRate)) {
                timeSinceLastFire = Time.time;
                shoot();
            }
        }
        //Movement
        /* Comprtamiento Scared, it also needs the triggers to have no condition
        if(runAway){
            transform.rotation = Quaternion.identity;
        } else {
            agent.destination = Direction;
            transform.rotation = Quaternion.identity;
        }*/
        if(runAway){
            agent.destination = Direction;
            transform.rotation = Quaternion.identity;
        } else { // else move towards target waypoint
            agent.destination = waypoint.position;
            transform.rotation = Quaternion.identity;
        }
    }

    void shoot(){
        GameObject clone;
        clone = Instantiate(bullethandler.GetComponent<BulletHandler>().prefabBullet, transform.position, Quaternion.identity);
        clone.GetComponent<PlayerBullet>().bulletDamage = bulletDamage;
        clone.GetComponent<PlayerBullet>().bulletSpeed = bulletSpeed;
        clone.GetComponent<PlayerBullet>().bulletDirection = Direction;
        clone.GetComponent<Rigidbody2D>().velocity = (Direction) * (bulletSpeed);
        /*if (bullethandler.GetComponent<NetworkObject>().IsOwner) {
            bullethandler.GetComponent<BulletHandler>().spawnEnemyBulletServerRpc(force, Direction, shootPoint.position.x, shootPoint.position.y);
        }*/
    }
    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag == "Enemy"){
            runAway = true;
        }
        
    }
    void OnCollisionExit2D(Collision2D other){
        if(other.gameObject.tag == "Enemy"){
            runAway = false;
        }
    }
}
