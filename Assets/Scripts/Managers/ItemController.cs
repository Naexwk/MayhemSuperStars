using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TriggerEditModeController;

// Controlador de botones que crean objetos fantasmas, como parte del modo edición
public class ItemController : MonoBehaviour
{
    // id del prop
    private int id;

    // Cantidad a colocar de props
    private int quantity = 1;
    private LevelEditorManager editor; 
    private OptionsSelector optionsSelector; 
    public GameObject tempObject;
    private Renderer tempRend;

    // Escuchar cambios de estado de juego
    private void Awake() {
        GameManager.state.OnValueChanged += StateChange;
    }

    // Encontrar al LevelEditorManager
    void Start()
    {
        editor = GameObject.FindWithTag("LevelEditorManager").GetComponent<LevelEditorManager>();
        optionsSelector = GameObject.FindWithTag("OptionsSelector").GetComponent<OptionsSelector>();
    }

    // Función para designar la cantidad de props a colocar
    public void setQuantity(int _quantity){
        quantity = _quantity;
    }


    public void StartNewPlacement(int _id)
    {   
        id = optionsSelector.propOptions[_id][0];
        quantity = optionsSelector.propOptions[_id][1];
        PlaceProp();
    }

    private void PlaceProp(){
        if(quantity > 0 ){
            // Encontrar posición para spawnear
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            // Instancia el objeto temporal
            tempObject = Instantiate(editor.ItemPrefabs[id], new Vector3(worldPosition.x, worldPosition.y,0), Quaternion.identity);
            
            // Desactiva el Circle Collider (si existe) para que no interfiera con
            // los permisos de colocación
            
            CircleCollider2D circleCollider = tempObject.GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                circleCollider.enabled = false;
            }

            // Convierte el collider a trigger
            BoxCollider2D boxCollider = tempObject.GetComponent<BoxCollider2D >();
            if(boxCollider != null){
                boxCollider.isTrigger = true; 
            }

            // Añadir el script TriggerEditModeController para que el objeto cambie de color al 
            // entrar en contacto con otro objeto, indicando que no tiene permiso de colocación
            if(tempObject.tag != "Bomb"){
                TriggerEditModeController triggerEditModeController = tempObject.AddComponent<TriggerEditModeController>();
            } 
            
            // Convirte al objeto en transparente
            tempRend = tempObject.GetComponent<Renderer>();
            Color currentColor = tempRend.material.color; 
            float newAlpha = 0.5f;
            tempRend.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

            // Cambiar el objeto seleccionado del LevelEditorManager
            editor.id = id;

            // Reducir la cantidad de props a colocar
            quantity--;
            
        } else {
            GameObject gm;
            gm = GameObject.FindGameObjectWithTag("GameManager");
            gm.GetComponent<GameManager>().done.Value++;
        }
    }

    void Update(){
        if(tempObject != null){
            if(tempObject.tag == "Bomb"){
                Debug.Log(tempObject.GetComponent<BombEditModeScript>().placeable);
            }
            if(tempObject != null){
                // Seguir el mouse con el objeto fantasma
                Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                tempObject.transform.position = worldPosition;

                // Al hacer clic con el mouse, aparecer el objeto si es colocable
                if(tempObject.tag != "Bomb" && Input.GetMouseButtonDown(0) && tempObject.GetComponent<TriggerEditModeController>().placeable){
                    editor.SpawnProp();
                    Destroy(tempObject);
                    PlaceProp();
                } else if(tempObject.tag == "Bomb" && Input.GetMouseButtonDown(0) && tempObject.GetComponent<BombEditModeScript>().placeable){
                    editor.SpawnProp();
                    Destroy(tempObject);
                    GameObject gm;
                    gm = GameObject.FindGameObjectWithTag("GameManager");
                    gm.GetComponent<GameManager>().done.Value++;
                }
            }
        }
    }

    // Terminar modo edición al acabar el tiempo
    private void StateChange(GameState prev, GameState curr){
        if (this != null) {
            if (curr == GameState.Round || curr == GameState.StartGame) {
                Destroy(tempObject);
            }
        }
    }
}