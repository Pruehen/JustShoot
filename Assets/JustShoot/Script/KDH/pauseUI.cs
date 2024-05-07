using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseUI : MonoBehaviour
{
    public GameObject panel;

    bool isPause = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                panel.SetActive(false);
                Time.timeScale = 1.0f;
                isPause = false;
            }
            else
            {
                panel.SetActive(true);
                Time.timeScale = 0.0f;
                isPause = true;
            }
        }
    }

    void OnResumeClicked()
    {
        panel.SetActive(false);
    }
}
