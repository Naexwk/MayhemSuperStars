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
    
    // Buscar players y escuchar nuevas conexiones
    void Awake()
    {
        StartCoroutine(SearchMyPlayer());
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Ejecutar funciones de escena de juego
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameRoom") {
            if (this != null) {
                StartCoroutine(SearchMyPlayer());
            }
        }
    }

    // Búsqueda de jugadores
    IEnumerator SearchMyPlayer() {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>().IsOwner) {
                myPlayer = player;
            } else {
                player.GetComponent<PlayerController>().characterCode.OnValueChanged += OnCharacterCodeChanged;
            }
        }
        yield return new WaitForSeconds(1);
        if (myPlayer == null) {
            StartCoroutine(SearchMyPlayer());
        } else {
            OnCharacterCodeChanged("","");
        }
    }

    // Al entrar un nuevo cliente
    private void OnClientConnected(ulong clientId)
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            if (player.GetComponent<PlayerController>().playerNumber == clientId) {
                player.GetComponent<PlayerController>().characterCode.OnValueChanged += OnCharacterCodeChanged;
            }
        }
    }

    // Al cambiar el characterCode de alguno de los jugadores, actualizar pantallas
    private void OnCharacterCodeChanged(FixedString64Bytes prev, FixedString64Bytes curr){
        CallChangeImage(myPlayer.GetComponent<PlayerController>().playerNumber);
    }

    // Actualizar pantallas, usando a los demás players
    public void CallChangeImage(ulong _playerNumber){
        int i = 0;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            PlayerController script = player.GetComponent<PlayerController>();
            if (!player.GetComponent<NetworkObject>().IsOwner) {
                ChangeImage(i, script.characterCode.Value.ToString());
                i++;
            }
        }
    }

    // Cambiar imagen según el characterCode
    private void ChangeImage(int image, string characterCode) {
        if (characterImages[image] != null) {
            if (characterCode == "cheeseman") {
                characterImages[image].sprite = characterSprites[0];
            } else if (characterCode == "sarge") {
                characterImages[image].sprite = characterSprites[1];
            }
            characterImages[image].transform.position = new Vector3 (characterImages[image].transform.position.x, 885f, characterImages[image].transform.position.z);
        }
        
    }

}
