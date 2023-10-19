using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;

// Controlador del objetivo de cámara
public class CameraTarget : NetworkBehaviour
{

    private GameObject[] players;

    // Jugador a seguir
    private GameObject chosenPlayer;

    // Variable de control que informa si se debe seguir al jugador o centrar la cámara
    public bool lockOnPlayer = false;
    bool lastLockOnValue = false;

    // Variables de posición para el movimiento de la cámara respecto al mouse
    Vector3 worldMousePos;
    Vector3 pos;
    float posX;
    float posY;
    float helper;

    private Camera mainCamera;
    
    // Máximos de distancia de la cámara con respecto al jugador
    [SerializeField] private float verticalOffset = 1f;
    [SerializeField] private float horizontalOffset = 3f;

    // Escuchar los cambios de escena
    void Awake(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Destruir este objeto al llegar a la escena GameRoom
    // Se regenerará al iniciar la escena de juego
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { 
        if (scene.name == "GameRoom" && this != null) {
            Destroy(this.gameObject);
        }
    }

    // Buscar al jugador cuyo ID corresponda al del cameraTarget
    public void StartCam () {
        players = GameObject.FindGameObjectsWithTag("Player");
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == gameObject.GetComponent<NetworkObject>().OwnerClientId) {
                chosenPlayer = player;
            }
        }
        
    }

    // Seguir al jugador seleccionado
    void Update()
    {
        // Teletransportarse instantáneamente al jugador seleccionado al cambiar el estado del juego
        if (lockOnPlayer != lastLockOnValue) {
            if (lockOnPlayer) {
                gameObject.transform.position = chosenPlayer.transform.position;
            }
            lastLockOnValue = lockOnPlayer;
        }

        if (chosenPlayer != null && lockOnPlayer) {

            worldMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Encontrar las coordenadas a colocarse (entre la cámara y el jugador)
            posX = (chosenPlayer.transform.position.x + worldMousePos.x)/2;
            posY = (chosenPlayer.transform.position.y + worldMousePos.y)/2;

            // Revisar si las coordenadas de X están fuera del máximo
            // Actualiza posX si está fuera del máximo, ya sea a la derecha o a la izquierda
            helper = Math.Abs((chosenPlayer.transform.position.x) - (posX));
            if (helper > horizontalOffset) {
                if (posX > chosenPlayer.transform.position.x) {
                    posX = chosenPlayer.transform.position.x + horizontalOffset;
                } else {
                    posX = chosenPlayer.transform.position.x - horizontalOffset;
                }
            }
            
            // Revisar si las coordenadas de Y están fuera del máximo
            // Actualiza posY si está fuera del máximo, ya sea hacia arriba o hacia abajo
            helper = Math.Abs((chosenPlayer.transform.position.y) - (posY));
            if (helper > verticalOffset) {
                if (posY > chosenPlayer.transform.position.y) {
                    posY = chosenPlayer.transform.position.y + verticalOffset;
                } else {
                    posY = chosenPlayer.transform.position.y - verticalOffset;
                }
            }

            // Cambiar posición a las nuevas coordenadas
            pos = new Vector3(posX,posY,0f);
            gameObject.transform.position = pos;
        } else {
            // Si no debe seguir al jugador, o no existe, quedarse al centro
            gameObject.transform.position = new Vector3(50.8f, 31.8f, 0f);
        }
    }

}
