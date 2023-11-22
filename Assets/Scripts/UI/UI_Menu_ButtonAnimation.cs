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

    public Selectable buttonSelectable;

    // Al hacer hover sobre un botón, animar y cambiar su sprite
    void Start(){
         buttonSelectable = GetComponent<Selectable>();
    }
    void Update(){
        if (buttonSelectable != null && buttonSelectable.IsInteractable())
        {
            Debug.Log("Button is selected!");
            // Your logic for when the button is selected goes here
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(AnimationHover);
        GetComponent<Animator>().Play(AnimationHover);
        transform.GetComponent<Image>().sprite = spriteHover;
    }

    // Al hacer hover sobre un botón, animar y cambiar su sprite
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log(AnimationHoverExit);
        GetComponent<Animator>().Play(AnimationHoverExit);
        transform.GetComponent<Image>().sprite = sprite;
    }

}
