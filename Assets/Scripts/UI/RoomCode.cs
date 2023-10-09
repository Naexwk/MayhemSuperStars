using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class RoomCode : MonoBehaviour
{
    public TMP_Text roomCodeText;
    public LanBehaviour relayManager;
    // Start is called before the first frame update
    void Start()
    {
        relayManager = GameObject.FindWithTag("RelayManager").GetComponent<LanBehaviour>();
        roomCodeText.text = "Room Code: " + relayManager.hostJoinCode;
    }
}
