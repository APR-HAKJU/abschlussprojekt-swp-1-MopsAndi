using UnityEngine;
using TMPro;

public class CrateClick : MonoBehaviour
{
    public GameObject numberGO; 

    void Update()
    {
        // Linksklick
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.Log("Linksklick erkannt");

            if (Physics.Raycast(ray, out hit))
            {
                //Compare by tag
                if (hit.transform.CompareTag("Crate"))
                {
                    numberGO.SetActive(true);
                    Debug.Log("Kiste angeklickt!");
                }
            }
        }
    }
}

