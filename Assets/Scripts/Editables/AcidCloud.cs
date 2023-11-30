using UnityEngine;

public class AcidCloud : MonoBehaviour
{
    public float speed = 2.0f;     
    public float timeToMove = 5.0f;
    private float timeBeforeChange = 0.0f;     
    private bool movingRight = true;
    private float newX = 0.0f;
    private Vector2 initialPosition;
    private bool canMove = false;

    int owner;

    private void Awake() {
        initialPosition = transform.position;
        GameManager.state.OnValueChanged += StateChange;
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        // Si empieza una ronda de juego, parar todas las corutinas de spawns de zombies e
        // iniciar una corutina nueva de spawn de zombies
        if (this != null) {
            if (curr == GameState.Round || curr == GameState.StartGame) {
                owner = GetComponent<propOwner>().owner;
                GetComponent<damageSource>().owner = owner;
                canMove = true;
            } else {
                canMove = false;
                transform.position = initialPosition;
            }
        }
    }

    void Start(){
        timeBeforeChange = timeToMove;
    }
    void OnTriggerStay2D(Collider2D other){
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().GetHit(owner);
        }
    }
    void Update()
    {
        if (canMove) {
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
}
