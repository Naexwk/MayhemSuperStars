using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class CharacterSelectManuManager : NetworkBehaviour 
{
    private GameManager gm;
    public GameObject startGameUI;
    void Start(){
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    void Update()
    {
        if(gm.readyPlay.Value == GameManager.numberOfPlayers.Value){
            if(IsServer){
                startGameUI.SetActive(true);
            }
        }else{
            if(IsServer){
                startGameUI.SetActive(false);
            }
        }
    }
}
