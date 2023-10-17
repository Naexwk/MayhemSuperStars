using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Controlador del movimiento de la cámara
public class CameraMovement : NetworkBehaviour
{
    // El objetivo de la cámara: el objeto que va a seguir
    private Transform target;

    // Tiempo de suavizamiento de la cámara
   [SerializeField] private float smoothTime = 0.15f;

    private Vector3 velocity = Vector3.zero;

    GameObject[] cameraTargets;

    // Ejecutar la función changeZoomByGameState cada vez que cambie el estado de juego
    void Awake(){
        GameManager.state.OnValueChanged += ChangeZoomByGameState;
    }

    // Fijar objetivo de cámara
    public void SetCameraTarget(Transform _target){
        target = _target;
    }

    // Moverse hacia el objetivo de cámara suavemente
    void FixedUpdate()
    {
        if (target != null){
            Vector3 targetPosition = target.TransformPoint(new Vector3(0, 0, -10));
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

    }

    // Cambiar la visión de la cámara para ver al jugador de cerca durante la ronda,
    // o para ver todo el mapa en el resto de estados de juego.
    private void ChangeZoomByGameState(GameState prev, GameState curr){
        if (curr == GameState.Round || curr == GameState.StartGame) {
            Camera.main.orthographicSize = 9;
        } else {
            Camera.main.orthographicSize = 17;
        }
    }
}
