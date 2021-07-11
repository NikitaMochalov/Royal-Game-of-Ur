using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleChecker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Toggle>().isOn = GameObject.Find("TutorialController").GetComponent<TutorialController>().tutorialOn;
    }
}
