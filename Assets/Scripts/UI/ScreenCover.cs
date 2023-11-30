using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCover : MonoBehaviour
{
    [SerializeField] private Image[] coverArray;
    private void Awake() {
        for (int i = 0; i < GameManager.numberOfPlayers.Value; i++) {
            coverArray[i].gameObject.SetActive(false);
        }
    }
}
