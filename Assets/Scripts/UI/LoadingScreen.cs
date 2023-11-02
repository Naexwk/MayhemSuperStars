using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text loadingText;
    int dotNumber = 3;
    string displayString;

    private void OnEnable() {
        StartCoroutine(ChangeLoadingString());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    IEnumerator ChangeLoadingString () {
        displayString = "Loading";

        for (int i = 0; i < dotNumber; i++) {
            displayString = displayString + ".";
        }

        dotNumber++;

        if (dotNumber > 3) {
            dotNumber = 1;
        }

        loadingText.text = displayString;

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(ChangeLoadingString());
    }

    // Update is called once per frame
    void Update()
    {   
        
    }
}
