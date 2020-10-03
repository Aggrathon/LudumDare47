using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; protected set; }

    [SerializeField] CommandStreamCharacter ghostPrefab;
    [SerializeField] Image timerBottom;
    [SerializeField] Image timerTop;
    public float resetTime = 12f;
    public int maxGhosts = 1;

    private List<CommandStreamCharacter> ghosts;
    private CommandStreamCharacter activePlayer;
    private float startTime;
    private int oldestGhost = 0;

    private void Awake()
    {
        ghosts = new List<CommandStreamCharacter>();
        instance = this;
        startTime = Time.time;
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
            ghosts.Add(character);
    }

    // Update is called once per frame
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
        }
        startTime = Time.time;
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
