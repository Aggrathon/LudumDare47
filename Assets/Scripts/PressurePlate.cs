using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    public SpriteRenderer visual;

    private int pressers;

    private void OnTriggerEnter2D(Collider2D other)
    {
        pressers++;
        if (pressers == 1)
        {
            onPressed.Invoke();
            var scale = visual.transform.localScale;
            scale.y = 0.1f;
            visual.transform.localScale = scale;
            visual.color = Color.gray;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        pressers--;
        if (pressers == 0)
        {
            onReleased.Invoke();
            var scale = visual.transform.localScale;
            scale.y = 0.2f;
            visual.transform.localScale = scale;
            visual.color = Color.white;
        }
    }
}
