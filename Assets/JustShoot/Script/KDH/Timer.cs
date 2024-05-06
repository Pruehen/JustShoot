using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float remainTime = 120.0f;
    TextMeshProUGUI m_TextMeshProUGUI;
    float resultTime;
    // Start is called before the first frame update
    void Awake()
    {
        m_TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.Instance.onPlay)
        {
            if (remainTime > 0.01f)
                remainTime -= Time.deltaTime;
        }
        

        UpdateTime();
    }
    void UpdateTime()
    {
        m_TextMeshProUGUI.text = null;
        m_TextMeshProUGUI.text += $"Timer\n";
        m_TextMeshProUGUI.text += $"{(int)remainTime / 60 % 60} : {(int)remainTime % 60}";
    }
}
