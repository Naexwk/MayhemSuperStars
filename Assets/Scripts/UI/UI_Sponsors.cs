using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sponsors : MonoBehaviour
{
    public Sprite empty,sausageHeart, fosfofosfo, cheese;
    
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
        }
    }
}

public enum SponsorStatus
{
    EmptySponsor = 0,
    SausageHeart = 1,
    Fosfofosfo = 2,
    Cheese = 3
}

