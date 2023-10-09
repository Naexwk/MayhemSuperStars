using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SceneReload : NetworkBehaviour
{
    public void restartGame(string sceneName){
        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
