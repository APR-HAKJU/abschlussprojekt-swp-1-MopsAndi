using UnityEngine;  

public class ActivateMorseOnClick : MonoBehaviour
{
    public MorseNeon morseNeon;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    morseNeon.ActivateMorse();
                }
            }
        }
    }
}
