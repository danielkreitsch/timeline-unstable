using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuBGM : MonoBehaviour
{
    [SerializeField]
    AudioSource source;

    // Start is called before the first frame update
    private void Start() {
        if (FindObjectsOfType<MenuBGM>().Length > 1) {
            Destroy(gameObject);
        } else {

            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        switch (scene.name) {
            case "main_menu":
                StartPlay();
                break;
            case "credits":
                break;
            case "game":
                StopPlay();
                break;
        }
    }

    void StartPlay() {
        if (!source.isPlaying) {
            source.Play();
        }
    }

    void StopPlay() {
        if (source.isPlaying) {
            FadeOut(source, 1);
        }
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
