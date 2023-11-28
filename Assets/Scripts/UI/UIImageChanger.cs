using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageChanger : MonoBehaviour
{
    [SerializeField] private Sprite[] imageArray;
    [SerializeField] private Image[] selectionImages;
    [SerializeField] private Sprite[] activeImages;
    [SerializeField] private Sprite[] inactiveImages;

    public void ChangeImage(int index){
        GetComponent<Image>().sprite = imageArray[index];

        for (int i = 0; i < selectionImages.Length; i++) {
            if (i == index) {
                selectionImages[i].sprite = activeImages[i];
            } else {
                selectionImages[i].sprite = inactiveImages[i];
            }
        }
    }
}
