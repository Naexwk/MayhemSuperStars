using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class UI_Menu_ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite sprite;
    public Sprite spriteHover;
    public string AnimationHover;
    public string AnimationHoverExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Animator>().Play(AnimationHover);
        transform.GetComponent<Image>().sprite = spriteHover;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Animator>().Play(AnimationHoverExit);
        transform.GetComponent<Image>().sprite = sprite;
    }

}
