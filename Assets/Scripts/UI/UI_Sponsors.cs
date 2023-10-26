using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sponsors : MonoBehaviour
{
    public Sprite empty,sausageHeart, fosfofosfo, cheese, testosterone, coderSmell, rugileoPepsi, vampire;
    
    Image sponsorImage;

    void Awake()
    {
        sponsorImage = GetComponent<Image>();
    }

    public void SetSponsorImg(SponsorStatus status)
    {
        switch (status)
        {
            case SponsorStatus.EmptySponsor:
                sponsorImage.sprite = empty;
                break;
            case SponsorStatus.SausageHeart:
                sponsorImage.sprite = sausageHeart;
                break;
            case SponsorStatus.Fosfofosfo:
                sponsorImage.sprite = fosfofosfo;
                break;
            case SponsorStatus.Cheese:
                sponsorImage.sprite = cheese;
                break; 
            case SponsorStatus.Testosterone:
                sponsorImage.sprite = testosterone;
                break;
            case SponsorStatus.RugileoPepsi:
                sponsorImage.sprite = rugileoPepsi;
                break; 
            case SponsorStatus.CodersSmell:
                sponsorImage.sprite = coderSmell;
                break;
            case SponsorStatus.Vampire:
                sponsorImage.sprite = vampire;
                break; 
        }
    }
}

public enum SponsorStatus
{
    EmptySponsor = 0,
    SausageHeart = 1,
    Fosfofosfo = 2,
    Cheese = 3,
    Testosterone = 4,
    CodersSmell= 5,
    RugileoPepsi = 6,
    Vampire=7
}

