using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class PurchasePhaseNames : MonoBehaviour
{
    [SerializeField] private string[] sponsorNames;
    [SerializeField] private string[] propNames;

    [SerializeField] private bool IsSponsor;
    [SerializeField] private int index;

    private OptionsSelector optionsSelector;

    void OnEnable()
    {
        optionsSelector = GameObject.FindWithTag("OptionsSelector").GetComponent<OptionsSelector>();
        
        if (IsSponsor) {
            gameObject.GetComponent<TMP_Text>().text = sponsorNames[optionsSelector.sponsorOptions[index]-1];

        } else {
            gameObject.GetComponent<TMP_Text>().text = propNames[optionsSelector.propOptions[index][0]];
        }
        
    }
}