using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleOnHover : MonoBehaviour
{
    [SerializeField] private Image spr; 
    private float duration = 0.009f;
    private bool canPerformScaleUp = true; 
    private bool canPerformScaleDown = false; 

    public void ScaleUp(){
        if(canPerformScaleUp){
            StartCoroutine(ScaleUpOverTime(duration));
        }
        
    }
    public IEnumerator ScaleUpOverTime(float duration)
    {
        canPerformScaleUp = false;
        Vector3 originalScale = spr.transform.localScale;
        Vector3 targetScale = originalScale * (1 + 0.25f);

        float currentTime = 0.0f;

        while (currentTime < duration)
        {
            spr.transform.localScale = Vector3.Lerp(originalScale, targetScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }

        // Ensure that the scale is exactly the target scale when the duration is complete
        spr.transform.localScale = targetScale;
        canPerformScaleDown = true; 
        
    }
    public void ScaleDown(){
        if(canPerformScaleDown){
             StartCoroutine(ScaleDownOverTime(duration));
        }
    }
    public IEnumerator ScaleDownOverTime(float duration)
    {
        canPerformScaleDown = false; 
        Vector3 originalScale = spr.transform.localScale;
        Vector3 targetScale = originalScale * 0.8f;

        float currentTime = 0.0f;

        while (currentTime < duration)
        {
            spr.transform.localScale = Vector3.Lerp(originalScale, targetScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }

        // Ensure that the scale is exactly the target scale when the duration is complete
        spr.transform.localScale = targetScale;
        canPerformScaleUp = true; 
    }
}