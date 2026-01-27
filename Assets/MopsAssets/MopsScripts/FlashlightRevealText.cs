using UnityEngine;
using TMPro;

public class FlashlightRevealText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light flashlight;
    [SerializeField] private TMP_Text text;

    [Header("Reveal Settings")]
    [SerializeField] private float maxDistance = 6f;
    [SerializeField] private float angleTolerance = 25f;

    void Start()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        SetAlpha(0f); // Text unsichtbar
    }

    void Update()
    {
        if (flashlight == null || !flashlight.enabled)
        {
            SetAlpha(0f);
            return;
        }

        Vector3 toText = text.transform.position - flashlight.transform.position;
        float distance = toText.magnitude;

        if (distance > maxDistance)
        {
            SetAlpha(0f);
            return;
        }

        float angle = Vector3.Angle(flashlight.transform.forward, toText);

        if (angle <= angleTolerance)
        {
            SetAlpha(1f); // sichtbar
        }
        else
        {
            SetAlpha(0f);
        }
    }

    void SetAlpha(float value)
    {
        Color c = text.color;
        c.a = value;
        text.color = c;
    }
}
