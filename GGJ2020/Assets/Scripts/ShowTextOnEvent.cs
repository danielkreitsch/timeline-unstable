using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowTextOnEvent : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    float displayDuration;
    [SerializeField]
    AnimationCurve flickerProbabilityThreshhold;
    [SerializeField]
    float predelay;


  

    private void Awake() {
        text.gameObject.SetActive(false);
    }

    public void Show() {
        StartCoroutine(Flicker());
    }    

    IEnumerator Flicker() {
        yield return new WaitForSeconds(predelay);
        float timer = 0;
        while (timer < displayDuration) {
            timer += Time.deltaTime;
            float threshhold = flickerProbabilityThreshhold.Evaluate(timer / displayDuration);
            if (Random.Range(0, 1f) > threshhold) {
                text.gameObject.SetActive(false);
            } else {
                text.gameObject.SetActive(true);
            }
            yield return null;
        }
        text.gameObject.SetActive(false);
    }
}
