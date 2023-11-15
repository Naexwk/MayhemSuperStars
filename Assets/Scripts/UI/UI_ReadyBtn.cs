using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class UI_ReadyBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Sprites de botones
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite spriteHover;
    [SerializeField] private Sprite spriteActive;

    // Animaciones de botones
    [SerializeField] private string AnimationHover;
    [SerializeField] private string AnimationHoverExit;

    private bool lockIn = false;

    // Al hacer hover sobre un botón, animar y cambiar su sprite
    public void OnPointerEnter(PointerEventData eventData)
    {

        GetComponent<Animator>().Play(AnimationHover);
        if(!lockIn){
            transform.GetComponent<Image>().sprite = spriteHover;
        }else{
            transform.GetComponent<Image>().sprite = spriteActive;
        }
    }

    // Al hacer hover sobre un botón, animar y cambiar su sprite
    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Animator>().Play(AnimationHoverExit);
        if(!lockIn){
            transform.GetComponent<Image>().sprite = sprite;
        }else{
            transform.GetComponent<Image>().sprite = spriteActive;
        }
    }

    public void BtnActive(){
        if(!lockIn){
            lockIn = true;
            transform.GetComponent<Image>().sprite = spriteActive;
            GameObject gm;
            gm = GameObject.FindGameObjectWithTag("GameManager");
            gm.GetComponent<GameManager>().ReadyPlayServerRpc();
        }else{
            lockIn = false;
            transform.GetComponent<Image>().sprite = spriteHover;
            GameObject gm;
            gm = GameObject.FindGameObjectWithTag("GameManager");
            gm.GetComponent<GameManager>().NotReadyPlayServerRpc();
        }
    }
}
