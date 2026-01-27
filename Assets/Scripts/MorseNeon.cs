using System.Collections;
using UnityEngine;

public class MorseNeon : MonoBehaviour
{
    public Light neonLight;

    private float dot = 0.3f;
    private float dash = 0.8f;
    private float pause = 0.3f;

    private bool isRunning = false;

    // Wird vom Button aufgerufen
    public void ActivateMorse()
    {
        if (!isRunning)
        {
            StartCoroutine(MorseRoutine());
        }
    }

    IEnumerator MorseRoutine()
    {
        isRunning = true;

        // • • • • —
        yield return Blink(dot);
        yield return Blink(dot);
        yield return Blink(dot);
        yield return Blink(dot);
        yield return Blink(dash);

        // am Ende sicher ausschalten
        neonLight.enabled = false;

        isRunning = false;
    }

    IEnumerator Blink(float duration)
    {
        neonLight.enabled = true;
        yield return new WaitForSeconds(duration);

        neonLight.enabled = false;
        yield return new WaitForSeconds(pause);
    }
}
