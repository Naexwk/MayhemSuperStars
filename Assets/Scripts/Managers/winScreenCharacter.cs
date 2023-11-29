using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class winScreenCharacter : MonoBehaviour
{
    [SerializeField] private Sprite[] characterImages;
    [SerializeField] private Image targetImage;
    int index;
    GameObject winnerPlayer;

    private void OnEnable() {
        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players)
        {
            if (Convert.ToInt32(player.GetComponent<PlayerController>().playerNumber) == gameManager.networkLeaderboard[0]) {
                winnerPlayer = player;
            }
        }

        players = GameObject.FindGameObjectsWithTag("Dead Player");
        
        foreach (GameObject player in players)
        {
            if (Convert.ToInt32(player.GetComponent<PlayerController>().playerNumber) == gameManager.networkLeaderboard[0]) {
                winnerPlayer = player;
            }
        }

        string eval = winnerPlayer.GetComponent<PlayerController>().characterCode.Value.ToString();
        switch(eval) 
        {
        case "cheeseman":
            index = 0;
            break;
        case "sarge":
            index = 1;
            break;
        case "sleek":
            index = 2;
            break;
        default:
            index = 0;
            break;
        }

        targetImage.sprite = characterImages[index];
    }
}
