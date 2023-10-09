using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CharacterSelectButtons : MonoBehaviour
{
    public Button button;
    public string characterCode;
    public GameObject[] players;
    public GameObject myPlayer;
    void Awake()
    {
        //GameManager.changedPlayers.OnValueChanged += StateChange;
        button = gameObject.GetComponent<Button>();
        StartCoroutine(searchMyPlayer());
    }

    IEnumerator searchMyPlayer() {
        players = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log(players.Length);
        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>().IsOwner) {
                myPlayer = player;
                button.onClick.AddListener(callChangeCharacter);
            }
        }

        yield return new WaitForSeconds(1);

        if (myPlayer == null) {
            StartCoroutine(searchMyPlayer());
        }
    }

    public void callChangeCharacter(){
        myPlayer.GetComponent<PlayerController>().changeCharacter(characterCode);
    }

    /*public override void OnDestroy()
    {
        GameManager.changedPlayers.OnValueChanged -= StateChange;
    }*/
}
