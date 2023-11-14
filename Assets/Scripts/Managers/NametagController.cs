using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NametagController : MonoBehaviour
{
   void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       if (scene.name == "SampleScene") {
            Debug.Log("SAMPLE SCENE LOADED");
            Camera mainCamera = Camera.main;
            Debug.Log("MAIN CAMERA" + mainCamera);
            Canvas canvas = GetComponent<Canvas>();
            canvas.worldCamera = mainCamera;
            Debug.Log("World Camera" + canvas.worldCamera);
        }
    }
     private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
}
