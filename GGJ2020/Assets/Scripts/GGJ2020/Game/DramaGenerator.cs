using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class DramaGenerator : MonoBehaviour
{

    [SerializeField]
    Game game;
    [SerializeField]
    CameraShaker shaker;
    [SerializeField]
    Volume postProcessingVolume;
    [SerializeField]
    Renderer skybox;

    ChromaticAberration abberation;

    Vignette vignette;

    [SerializeField]
    float shakeInterval;
    [SerializeField, Range(1, 20)]
    int shakesToIncreaseIntensity = 10;
    [SerializeField]
    float maxAbberationTime;
    [SerializeField]
    AnimationCurve abberationIntensity;
    [SerializeField]
    float maxVignetteTime;
    [SerializeField]
    AnimationCurve skyboxOffset;
    [SerializeField]
    float maxSkyboxTime;

    float maxAbberation;
    float maxVignette;


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

    private void Awake() {
        postProcessingVolume.profile.TryGet<ChromaticAberration>(out abberation);
        maxAbberation = abberation.intensity.value;
        postProcessingVolume.profile.TryGet<Vignette>(out vignette);
        maxVignette = vignette.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {
        float eval = Mathf.Clamp(game.Timer / maxSkyboxTime, 0, maxSkyboxTime);
        skybox.material.SetFloat("Vector1_248BB884", skyboxOffset.Evaluate(eval));
        float abber = abberationIntensity.Evaluate(Mathf.Clamp((game.Timer / maxAbberationTime) * maxAbberation, 0, maxAbberation));
        abberation.intensity.Override(abber);
        vignette.intensity.Override(Mathf.Clamp((game.Timer / maxVignetteTime)*maxVignette, 0, maxVignette));
        vignette.color.Override(Random.ColorHSV());
    }
}
