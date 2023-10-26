using UnityEngine;

public class MoveBackAndForth : MonoBehaviour
{
    public float speed = 2.0f;     
    public float timeToMove = 5.0f;
    private float timeBeforeChange = 0.0f;     
    private bool movingRight = true;
    private float newX = 0.0f;
    void Start(){
        timeBeforeChange = timeToMove;
    }
    void OnTriggerStay2D(Collider2D other){
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().GetHit();
        }
    }
    void Update()
    {
        if(timeBeforeChange > 0){
            newX = transform.position.x + (movingRight ? speed * Time.deltaTime : -speed * Time.deltaTime);
            timeBeforeChange -= Time.deltaTime;
        } else{
            timeBeforeChange  = timeToMove;
            movingRight = !movingRight;
        }
        // Update the object's position
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
