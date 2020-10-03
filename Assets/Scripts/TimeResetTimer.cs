using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeResetTimer : MonoBehaviour
{
    public Image bottom;
    public Image top;
    public float resetTime = 12f;

    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        var frac = (Time.time - startTime) / resetTime;
        if (frac >= 1f)
        {
            if (GameManager.instance != null)
                GameManager.instance.ResetTime();
            startTime = Time.time;
            frac = 0.0f;
        }
        top.fillAmount = 1.0f - frac;
        bottom.fillAmount = frac;
    }
}
