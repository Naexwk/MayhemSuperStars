using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Controlador de escenas en GameRoom
// Helper de LanBehaviour
public class LocalPlayButton : MonoBehaviour
{
    [SerializeField] private GameObject relayMiniManager;
    private void Awake() {
        relayMiniManager = GameObject.FindWithTag("RelayMiniManager");
    }

	// Cambiar de escena
    public void ChangeScene(string scene){
        relayMiniManager.GetComponent<LanMiniBehaviour>().ChangeScene(scene);
	}
}