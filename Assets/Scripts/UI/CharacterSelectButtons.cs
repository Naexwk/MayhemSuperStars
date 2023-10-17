using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

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

    // Mostrar estadisticas de personaje UI
    public TMP_Text characterName;
    public TMP_Text habilityDescription;
    public Image abilityImage;
    public Sprite abilitysSprites;

    // Gameobject para mostrar personajes multiplayer
    [SerializeField] private GameObject tvs;

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
    public void CallChangeCharacter() {
        myPlayer.GetComponent<PlayerController>().ChangeCharacter(characterCode);
        tvs.GetComponent<UI_CS_ShowMultiplayers>().ChangeImagePlayer();
    }

    // Cambiar estadisticas de personaje UI
    private void StatsSwapper() {
        if(characterCode == "cheeseman") {
            characterName.text = "cheeseman";
            habilityDescription.text = "Selected Cheeseman";
            abilityImage.sprite = abilitysSprites;
        } else if (characterCode == "sarge") {
            characterName.text = "sarge";
            habilityDescription.text = "Selected Sarge";
            abilityImage.sprite = abilitysSprites;
        }
    }
}
