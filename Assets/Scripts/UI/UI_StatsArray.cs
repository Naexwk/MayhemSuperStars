using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatsArray : MonoBehaviour
{
    [SerializeField] private Image[] healthArray;
    [SerializeField] private Image[] damageArray;
    [SerializeField] private Image[] speedArray;
    [SerializeField] private Image[] fireRateArray;

    private Color whiteColor = new Color(1f, 1f, 1f, 1f);
    private Color blackColor = new Color(0f, 0f, 0f, 1f);

    private int refHealth, refDamage, refSpeed, refFireRate;

    public void ChangeStats (string characterCode) {
        switch(characterCode) 
        {
        case "cheeseman":
            refHealth = 3;
            refDamage = 3;
            refSpeed = 3;
            refFireRate = 3;
            break;
        case "sarge":
            refHealth = 5;
            refDamage = 4;
            refSpeed = 1;
            refFireRate = 2;
            break;
        case "sleek":
            refHealth = 1;
            refDamage = 3;
            refSpeed = 5;
            refFireRate = 3;
            break;
        default:
            break;
        }

        ChangeImages();
    }

    private void ChangeImages(){
        for (int i = 0; i < healthArray.Length; i++) {
            if (i < refHealth){
                healthArray[i].color = whiteColor;
            } else {
                healthArray[i].color = blackColor;
            }
        }

        for (int i = 0; i < damageArray.Length; i++) {
            if (i < refDamage){
                damageArray[i].color = whiteColor;
            } else {
                damageArray[i].color = blackColor;
            }
        }

        for (int i = 0; i < speedArray.Length; i++) {
            if (i < refSpeed){
                speedArray[i].color = whiteColor;
            } else {
                speedArray[i].color = blackColor;
            }
        }

        for (int i = 0; i < fireRateArray.Length; i++) {
            if (i < refFireRate){
                fireRateArray[i].color = whiteColor;
            } else {
                fireRateArray[i].color = blackColor;
            }
        }
    }
}
