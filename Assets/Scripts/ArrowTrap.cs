using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{

    public List<Projectile> projectiles;

    public void Fire()
    {
        if (projectiles.Count > 0)
        {
            projectiles[projectiles.Count - 1].Fire();
            projectiles.RemoveAt(projectiles.Count - 1);
        }
    }
}