using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PurchasePhaseIcons : MonoBehaviour
{
    [SerializeField] private Sprite[] sponsorIcons;
    [SerializeField] private Sprite[] propIcons;

    [SerializeField] private bool IsSponsor;
    [SerializeField] private int index;

    private OptionsSelector optionsSelector;

    void OnEnable()
    {
        if (transform.parent.transform.parent.transform.parent.GetComponent<UIHelper>().optionsSelector != null){
            optionsSelector = transform.parent.transform.parent.transform.parent.GetComponent<UIHelper>().optionsSelector.GetComponent<OptionsSelector>();
        } else {
            optionsSelector = GameObject.FindWithTag("OptionsSelector").GetComponent<OptionsSelector>();
        }
        
        if (IsSponsor) {
            gameObject.GetComponent<Image>().sprite = sponsorIcons[optionsSelector.sponsorOptions[index]-1];

        } else {
            gameObject.GetComponent<Image>().sprite = propIcons[optionsSelector.propOptions[index][0]];
        }
        
    }
}
