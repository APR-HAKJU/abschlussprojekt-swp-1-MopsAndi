using UnityEngine;

public class PowerPuzzle : MonoBehaviour
{
    public int[] correctOrder = { 2, 1, 3 };
    private int currentIndex = 0;

    public GameObject powerLight;
    public GameObject zahlenReihenfolge;

    public void PressSwitch(int switchNumber)
    {
        if (switchNumber == correctOrder[currentIndex])
        {
            currentIndex++;

            if (currentIndex >= correctOrder.Length)
            {
                ActivatePower();
            }
        }
        else
        {
            currentIndex = 0;
        }
    }

    void ActivatePower()
    {
        powerLight.SetActive(true);
        zahlenReihenfolge.SetActive(true);
        Debug.Log("STROM AKTIVIERT – ZAHL ANGEZEIGT");
    }
}
