using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; protected set; }

    [SerializeField] CommandStreamCharacter ghostPrefab;
    [SerializeField] Image timerBottom;
    [SerializeField] Image timerTop;
    [SerializeField] TextMeshProUGUI timerText;
    public float resetTime = 12f;
    public int maxGhosts = 1;

    private List<CommandStreamCharacter> ghosts;
    private List<Destructable> respawns;
    private CommandStreamCharacter activePlayer;
    private float startTime;
    private int oldestGhost = 0;

    private void Awake()
    {
        ghosts = new List<CommandStreamCharacter>();
        instance = this;
        startTime = Time.time;
        timerText.text = string.Format("{0}/{1}", 1, maxGhosts + 1);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void RegisterCharacter(CommandStreamCharacter character)
    {
        if (character.activePlayer)
            activePlayer = character;
        else
        {
            ghosts.Add(character);
            timerText.text = string.Format("{0}/{1}", ghosts.Count + 1, maxGhosts + 1);
        }
    }

    public void RegisterRespawn(Destructable character)
    {
        respawns.Add(character);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
            ResetTime();
        if (timerTop && timerBottom)
        {
            var frac = (Time.time - startTime) / resetTime;
            if (frac >= 1f)
            {
                ResetTime();
                startTime = Time.time;
                frac = 0.0f;
            }
            timerTop.fillAmount = 1.0f - frac;
            timerBottom.fillAmount = frac;
        }
    }

    public void ResetTime()
    {
        foreach (var ghost in ghosts)
        {
            ghost.Reset();
        }
        if (activePlayer)
        {
            activePlayer.AddActionToStreamNow(CommandStreamCharacter.Action.Disable);
            var stream = activePlayer.GetStream();
            activePlayer.SetStream(new List<CommandStreamCharacter.Event>());
            activePlayer.Reset();
            CommandStreamCharacter ghost;
            if (ghosts.Count >= maxGhosts)
            {
                ghost = ghosts[oldestGhost];
                oldestGhost = (oldestGhost + 1) % maxGhosts;
            }
            else
            {
                ghost = Instantiate(ghostPrefab, activePlayer.transform.position, activePlayer.transform.rotation);
            }
            ghost.SetStream(stream);
            ghost.SetSpawn(activePlayer.GetSpawn());
        }
        startTime = Time.time;
        foreach (var s in respawns)
        {
            s.gameObject.SetActive(true);
        }
    }

    public void SetSpawn(Vector3 pos)
    {
        if (activePlayer)
        {
            activePlayer.SetSpawn(pos);
            startTime = Time.time;
        }
    }
}
