using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

// Controlador de botones de Character Select
public class CharacterSelectButtons : MonoBehaviour
{
    // Referencia a este boton
    private Button button;

    // Código de personaje al que se cambiará con este boton
    [SerializeField] private string characterCode;

    // Lista de jugadores
    private GameObject[] players;

    // Jugador de este equipo
    private GameObject myPlayer;

    void Awake()
    {
        button = gameObject.GetComponent<Button>();
        StartCoroutine(SearchMyPlayer());
    }

    // Búsqueda de jugador local
    IEnumerator SearchMyPlayer() {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>().IsOwner) {
                myPlayer = player;
                button.onClick.AddListener(CallChangeCharacter);
            }
        }
        yield return new WaitForSeconds(1);
        if (myPlayer == null) {
            StartCoroutine(SearchMyPlayer());
        }
    }

    // Cambiar personaje del jugador al especificado por characterCode
    public void CallChangeCharacter(){
        myPlayer.GetComponent<PlayerController>().changeCharacter(characterCode);
    }
}
