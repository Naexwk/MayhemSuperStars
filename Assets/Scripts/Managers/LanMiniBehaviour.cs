using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

// Controlador de escenas en GameRoom
// Helper de LanBehaviour
public class LanMiniBehaviour : NetworkBehaviour
{
	// Cambiar de escena
    public void ChangeScene(string sceneName){
		NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}
}
