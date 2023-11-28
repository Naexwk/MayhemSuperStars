using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class PurchasePhaseDesc : MonoBehaviour
{
    [SerializeField] private string[] sponsorDesc;
    [SerializeField] private string[] propDesc;

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
            gameObject.GetComponent<TMP_Text>().text = sponsorDesc[optionsSelector.sponsorOptions[index]-1];

        } else {
            gameObject.GetComponent<TMP_Text>().text = propDesc[optionsSelector.propOptions[index][0]];
        }
        
    }
}
