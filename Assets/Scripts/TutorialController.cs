using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public bool tutorialOn;

    void Start()
    {
        tutorialOn = true;
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void DontShowTutorial()
    {
        tutorialOn = !tutorialOn;
    }
}
