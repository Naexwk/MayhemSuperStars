using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

// Controlador de recarga de escena de juego
public class SceneReload : NetworkBehaviour
{
    // Carga la escena de nombre sceneName
    public void RestartGame(string sceneName){
        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
