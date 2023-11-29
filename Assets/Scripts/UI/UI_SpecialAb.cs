using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpecialAb : MonoBehaviour
{
    public Sprite cheesemanSA, cheesemanDisable, sargeSA, sargeDisable, sleekSA, sleekDisable;
    Image specialAbImage;

    void Awake()
    {
        specialAbImage = GetComponent<Image>();
    }

    public void SetSpecialAbImage(string characterCode, float cooldown){

        if (cooldown >= 1f)
        {
            if(characterCode == "cheeseman"){
                specialAbImage.sprite = cheesemanSA;
            }
            else if(characterCode == "sarge"){
                specialAbImage.sprite = sargeSA;
            }
            else if(characterCode == "sleek"){
                specialAbImage.sprite = sleekSA;
            }
        }
        else
        {
            if(characterCode == "cheeseman"){
                specialAbImage.sprite = cheesemanDisable;
            }
            else if(characterCode == "sarge"){
                specialAbImage.sprite = sargeDisable;
            }
            else if(characterCode == "sleek"){
                specialAbImage.sprite = sleekDisable;
            }
        }
    }
}

