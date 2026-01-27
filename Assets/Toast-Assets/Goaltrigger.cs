using UnityEngine;
using TMPro;

public class Goaltrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private GameObject targetCube;

    private bool goalReached = false;

    public bool IsGoalReached()
    {
        return goalReached;
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Check if the colliding object is the target cube
        if (collision.gameObject == targetCube)
        {
            goalReached = true;
            if (goalText != null)
            {
                goalText.text = "6";
            }
        }
    }
}
