using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBackgroundColor : MonoBehaviour
{
    [SerializeField] private Image background;
    Color[] colorArray = {Color.red, Color.blue, Color.yellow, Color.green};

    public void ChangeColor(int playerNumber){
        background.color = colorArray[playerNumber];
        Color helperColor = background.color;
        background.color = new Color(helperColor.r,helperColor.g,helperColor.b, 0.1f);
    }
}
