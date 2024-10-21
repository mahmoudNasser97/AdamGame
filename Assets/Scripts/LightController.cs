using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private Light lightSource;

    public float minIntensity = 0f;
    public float maxIntensity = 2f;
    public float intensityChangeSpeed = 1f;
    void Start()
    {
        lightSource = GetComponent<Light>();
    }
    void Update()
    {
        StartCoroutine(LightChanger());
    }
    public void SetIntensity(float intensity)
    {
        lightSource.intensity = Mathf.Clamp(intensity, minIntensity, maxIntensity);
    }
    IEnumerator LightChanger()
    {
        yield return new WaitForSeconds(50f);
        ChangeGameMode();
    }
    private void ChangeGameMode()
    {
        if (lightSource.intensity < maxIntensity)
            lightSource.intensity += 0.01f;
    }
}
