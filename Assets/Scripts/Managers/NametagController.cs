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
    Color[] colors = { Color.red, Color.blue, Color.yellow, Color.green };
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if (scene.name == "SampleScene" && player != null) {
            Camera mainCamera = Camera.main;
            Canvas canvas = GetComponent<Canvas>();
            canvas.worldCamera = mainCamera;
            foreach(GameObject players in player){
                int index = (int)players.GetComponent<PlayerController>().playerNumber;
                nametagname.text = gameManager.networkPlayerNames[index].ToString();
                nametagname.color = colors[index];
                Debug.Log(players.GetComponent<PlayerController>().playerNumber.ToString());
            }
        }
    }
    private void Awake() {
        player = GameObject.FindGameObjectsWithTag("Player");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
}
