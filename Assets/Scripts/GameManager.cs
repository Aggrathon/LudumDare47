using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; protected set; }

    [SerializeField] CommandStreamCharacter ghostPrefab;

    private List<CommandStreamCharacter> ghosts;
    private CommandStreamCharacter activePlayer;

    private void Awake()
    {
        ghosts = new List<CommandStreamCharacter>();
        instance = this;
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
            var ghost = Instantiate<CommandStreamCharacter>(ghostPrefab, activePlayer.transform.position, activePlayer.transform.rotation);
            ghost.SetStream(stream);
            ghosts.Add(ghost);
        }
    }
}
