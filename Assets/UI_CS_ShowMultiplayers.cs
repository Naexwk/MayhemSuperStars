using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Collections;
using UnityEngine.UI;

public class UI_CS_ShowMultiplayers : NetworkBehaviour
{
    // Lista de jugadores
    private GameObject[] players;

    // Jugador de este equipo
    private GameObject myPlayer;

    // Mostrar Sprite de personaje UI
    public Sprite[] characterSprites;

    // Imagen que contiene al jugador
    public Image[] characterImages;
    
    void Awake()
    {
        StartCoroutine(SearchMyPlayer());
    }

    // Búsqueda de jugador local
    IEnumerator SearchMyPlayer() {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>().IsOwner) {
                myPlayer = player;
            }
        }
        yield return new WaitForSeconds(1);
        if (myPlayer == null) {
            StartCoroutine(SearchMyPlayer());
        }
    }

    public void ChangeImagePlayer() {
        //changeOtherPlayerImages();
        changeImageServerRpc(myPlayer.GetComponent<PlayerController>().playerNumber);
    }

    public void changeOtherPlayerImages(){
        StartCoroutine(timerChangeOtherPlayersImage());
    }

    IEnumerator timerChangeOtherPlayersImage() {
        int i = 0;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            PlayerController script = player.GetComponent<PlayerController>();
            if (!player.GetComponent<NetworkObject>().IsOwner) {
                Debug.Log("Called changeimage");
                ChangeImage(i, script.characterCode.Value.ToString());
                i++;
            }
        }
        yield return new WaitForSeconds(1);
    }

    // Llamar a cambiar el animador de un jugador en los clientes
    [ServerRpc(RequireOwnership = false)]
    public void changeImageServerRpc(ulong _playerNumber){
        changeImageClientRpc(_playerNumber);
    }

    // Cambiar el animador de un jugador en todos los clientes
    [ClientRpc]
    public void changeImageClientRpc(ulong _playerNumber){
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        if (myPlayer.GetComponent<PlayerController>().playerNumber != _playerNumber) {
            Debug.Log("found a mf");
            changeOtherPlayerImages();
        }
        /*
        foreach (GameObject player in players) {
            PlayerController script = player.GetComponent<PlayerController>();
            if (!(script.playerNumber == _playerNumber)) {
                // Se ejecuta en todos los clientes menos el de la señal
                Debug.Log("Executing here");
                changeOtherPlayerImages();
            }
        }*/
        
    }

    // Cambiar la imagen correspondiente al "televisor" y su respectivo personaje UI
    private void ChangeImage(int image, string characterCode) {
        if (characterCode == "cheeseman") {
            characterImages[image].sprite = characterSprites[0];
        } else if (characterCode == "sarge") {
            characterImages[image].sprite = characterSprites[1];
        }
    }

}
