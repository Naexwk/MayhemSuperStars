using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public GameObject gameManagerInstance;
    // Busca al Game Manager y le asigna la función
    // StartGame al boton
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
