using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LanMiniBehaviour : NetworkBehaviour
{
    public void ChangeScene(string sceneName){
		NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}
}
