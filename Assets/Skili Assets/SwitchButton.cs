using UnityEngine;

public class SwitchButton : MonoBehaviour
{
    public int switchNumber;
    public PowerPuzzle puzzle;

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
                    puzzle.PressSwitch(switchNumber);
                }
            }
        }
    }
}
