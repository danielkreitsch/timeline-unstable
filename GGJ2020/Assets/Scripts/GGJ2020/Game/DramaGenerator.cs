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
    ColorAdjustments colorAdjustments;
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
    [SerializeField]
    float brightnessIncrease;
    [SerializeField]
    float cooldownSpeed;

    float maxAbberation;
    float maxVignette;
    bool gameOver = false;

    void StartShake() {
        StartCoroutine(ShakeRepeated());
    }

    // Start is called before the first frame update
    IEnumerator ShakeRepeated()
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
        game.onGameLost.AddListener(LoseEffect);
        game.onGameWon.AddListener(WinEffect);
        game.onCountdownStart.AddListener(StartShake);

        postProcessingVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
        postProcessingVolume.profile.TryGet<ChromaticAberration>(out abberation);
        maxAbberation = abberation.intensity.value;
        postProcessingVolume.profile.TryGet<Vignette>(out vignette);
        maxVignette = vignette.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) {
            float eval = Mathf.Clamp(game.Timer / maxSkyboxTime, 0, maxSkyboxTime);
            skybox.material.SetFloat("Vector1_248BB884", skyboxOffset.Evaluate(eval));
            float abber = abberationIntensity.Evaluate(Mathf.Clamp((game.Timer / maxAbberationTime) * maxAbberation, 0, maxAbberation));
            abberation.intensity.Override(abber);
            vignette.intensity.Override(Mathf.Clamp((game.Timer / maxVignetteTime) * maxVignette, 0, maxVignette));
            vignette.color.Override(Random.ColorHSV());
        }
    }

    void WinEffect() {
        gameOver = true;
        shakeInterval = 1000;
        StartCoroutine(CooldownEffects());
    }

    IEnumerator CooldownEffects() {
        float baseSkyboxspeed = skybox.material.GetFloat("Vector1_248BB884");
        float abberationIntensity = abberation.intensity.value;
        float vignetteIntensity = vignette.intensity.value;
        float timer = cooldownSpeed;
        while (timer > 0.1f) {
            timer -= Time.deltaTime;
            float percent = timer / cooldownSpeed;
            skybox.material.SetFloat("Vector1_248BB884", baseSkyboxspeed*percent);
            abberation.intensity.Override(abberationIntensity*percent);
            vignette.intensity.Override(vignetteIntensity*percent);
            yield return null;
        }
    }

    void LoseEffect() {
        gameOver = true;
        StartCoroutine(IncreaseBrightness());
    }

    IEnumerator IncreaseBrightness() {

        float timer = 0;
        while (timer < 3) {
            timer += Time.deltaTime;
            float exposure = colorAdjustments.postExposure.value;
            colorAdjustments.postExposure.Override(exposure + brightnessIncrease * Time.deltaTime);
            yield return null;
        }
    }
}
