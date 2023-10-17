using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controlador de boton de inicio de juego 
// Deprecated
public class PlayButton : MonoBehaviour
{
    public GameObject gameManagerInstance;

    // Busca al GameManager y le añade la función Start Game a este botón
    void Update()
    {
        if (gameManagerInstance != null) {
            return;
        }
        gameManagerInstance = GameObject.FindWithTag("GameManager");

        if (gameManagerInstance != null) {
            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(gameManagerInstance.GetComponent<GameManager>().StartGame);
        }
    }
}
