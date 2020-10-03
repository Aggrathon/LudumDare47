using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public bool checkpoint = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var chr = other.GetComponent<CommandStreamCharacter>();
        if (chr != null && chr.activePlayer)
        {
            // TODO: Sound FX
            if (checkpoint)
            {
                if (GameManager.instance != null)
                    GameManager.instance.SetSpawn(transform.position);
            }
            else
                NextLevel();
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }
}
