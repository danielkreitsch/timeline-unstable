using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DramaGenerator : MonoBehaviour
{

    [SerializeField]
    Game game;
    [SerializeField]
    CameraShaker shaker;

    [SerializeField]
    float shakeInterval;
    [SerializeField]
    int shakesToIncreaseIntensity;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        int shakes = 0;
        while (true) {
            shakes++;
            if (shakes % shakesToIncreaseIntensity == 0) {
                shakeInterval /= 2;
            }
            yield return new WaitForSeconds(shakeInterval);
            shaker.Shake(0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
