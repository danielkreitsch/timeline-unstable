using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomClipOnAwake : MonoBehaviour
{
    [SerializeField]
    AudioClip[] clips;

    private void Awake() {
        int random = Random.Range(0, clips.Length);
        GetComponent<AudioSource>().PlayOneShot(clips[random]);
    }
}
