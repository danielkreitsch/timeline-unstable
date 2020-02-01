using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    Vector3 startingPosition;

    [SerializeField, Range(0, 1)]
    float intensity = 0.1f;
    [SerializeField, Range(0, 1)]
    float duration = 0;
    Camera cam;


    // Start is called before the first frame update
    private void Awake() {
        startingPosition = transform.position;
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration > 0) {
            cam.transform.position = startingPosition + Random.insideUnitSphere * intensity;
        } else {
            cam.transform.position = startingPosition;
        }
    }

    public void Shake(float duration) {
        this.duration = duration;
    }
}
