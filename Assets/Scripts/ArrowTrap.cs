using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{

    public List<Projectile> projectiles;

    AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void Fire()
    {
        if (projectiles.Count > 0)
        {
            projectiles[projectiles.Count - 1].Fire();
            projectiles.RemoveAt(projectiles.Count - 1);
            if (audio)
                audio.Play();
        }
    }
}
