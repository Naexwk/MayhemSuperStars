using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Controlador de despliegue de código de sala en la sala de juego
public class RoomCode : MonoBehaviour
{
    // Referencia al campo de texto donde desplegar
    [SerializeField] private TMP_Text roomCodeText;

    // Referencia al controlador de sesión de red
    [SerializeField] private LanBehaviour relayManager;

    // Actualizar código de sala de juego
    void Start()
    {
        relayManager = GameObject.FindWithTag("RelayManager").GetComponent<LanBehaviour>();
        roomCodeText.text = "Room Code: " + relayManager.hostJoinCode;
    }
}
