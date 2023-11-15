using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NametagController : MonoBehaviour
{
    [SerializeField] private TMP_Text nametagname;
    private GameObject[] player;
    private GameManager gameManager;
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if (scene.name == "SampleScene" && player != null) {
            Camera mainCamera = Camera.main;
            Canvas canvas = GetComponent<Canvas>();
            canvas.worldCamera = mainCamera;
            foreach(GameObject players in player){
                nametagname.text = gameManager.networkPlayerNames[(int)players.GetComponent<PlayerController>().playerNumber].ToString();
                Debug.Log(players.GetComponent<PlayerController>().playerNumber.ToString());
            }
            nametagname.color = new Color(0.5f,1f,0.5f,1f);
        }
    }
    private void Awake() {
        player = GameObject.FindGameObjectsWithTag("Player");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
}
