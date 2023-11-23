using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageChanger : MonoBehaviour
{
    [SerializeField] private Sprite[] imageArray;

    public void ChangeImage(int index){
        GetComponent<Image>().sprite = imageArray[index];
    }   
}
