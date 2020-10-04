using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public GameObject deathFX;
    public Vector3 fxOffset;

    private void Start()
    {
        if (GameManager.instance)
            GameManager.instance.RegisterRespawn(this);
    }

    public void Die()
    {
        if (deathFX)
            Instantiate(deathFX, transform.position + fxOffset, transform.rotation);
        gameObject.SetActive(false);
    }
}
