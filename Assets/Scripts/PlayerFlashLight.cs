using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashLight : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.Light2D flashLight;

    // Start is called before the first frame update
    void Start()
    {
        flashLight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        StartCoroutine(FlickerFlashLight());
    }

    // Flickers the flash light after random intervals
    IEnumerator FlickerFlashLight()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(15f, 30f));
            flashLight.intensity = 0f;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            flashLight.intensity = 1f;
        }
    }
}
