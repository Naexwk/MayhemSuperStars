using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

// Controlador de animaciones de UI del menú principal
public class UI_Menu_ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Sprites de botones
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite spriteHover;

    // Animaciones de botones
    [SerializeField] private string AnimationHover;
    [SerializeField] private string AnimationHoverExit;

    // Al hacer hover sobre un botón, animar y cambiar su sprite
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Animator>().Play(AnimationHover);
        transform.GetComponent<Image>().sprite = spriteHover;
    }

    // Al hacer hover sobre un botón, animar y cambiar su sprite
    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Animator>().Play(AnimationHoverExit);
        transform.GetComponent<Image>().sprite = sprite;
    }

}
