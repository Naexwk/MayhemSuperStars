using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class moveWaypoint : MonoBehaviour
{
    public float speed = 5f;
    private int change = 0;
    private SpriteRenderer spriteRenderer;
    List<Action> functions = new List<Action>();
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        functions.Add(waypointLeft);
        functions.Add(waypointUp);
        functions.Add(waypointRight);
        functions.Add(waypointDown);
    }
    void Update(){
        functions[change]();
    }
    void waypointRight(){
        transform.Translate(speed*Time.deltaTime,0,0);
    }
    void waypointLeft(){
        transform.Translate(-speed*Time.deltaTime,0,0);
    }
    void waypointUp(){
        transform.Translate(0,speed*Time.deltaTime,0);
    }
    void waypointDown(){
        transform.Translate(0,-speed*Time.deltaTime,0);
    }
     void OnTriggerEnter2D(Collider2D otherCollider)
    {
        change++;
        if(change > 3){
            change = 0;
        }
    }
}
