using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageChanger : MonoBehaviour
{
    [SerializeField] private Sprite[] imageArray;
    [SerializeField] private Sprite[] selectionImages;
    [SerializeField] private Sprite[] activeImages;
    [SerializeField] private Sprite[] inactiveImages;

    public void ChangeImage(int index){
        GetComponent<Image>().sprite = imageArray[index];
    }

    public void ActivateImage(int index){
        for (int i = 0; i < selectionImages.Length; i++) {
            if (i == index) {
                selectionImages[i] = activeImages[i];
            } else {
                selectionImages[i] = inactiveImages[i];
            }
        }
    }
}
