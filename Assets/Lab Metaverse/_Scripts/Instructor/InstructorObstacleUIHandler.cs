using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructorObstacleUIHandler : MonoBehaviour
{
    [SerializeField] private List<Image> CrackWindowPanels;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayCrackPanel(int i)
    {
        CrackWindowPanels[i].enabled = true;
    }

    public void HideCrackPanel(int i)
    {
        CrackWindowPanels[i].enabled = false;
    }
}
