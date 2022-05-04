using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tent : Interactable
{
    [SerializeField] Transform entrance;

    private void Start()
    {
        EventManager.Instance.onPlayerAwake.AddListener(PlayerAwake);
    }

    public override void Interact()
    {
        EventManager.Instance.onPlayerSleep.Invoke();
        GameManager.Player.SetPlayerState(PlayerState.Deactivated);
    }

    public void PlayerAwake()
    {
        GameManager.Player.transform.position = entrance.position;
        GameManager.Player.SetPlayerState(PlayerState.Idle);
        PlayerSurvival.Instance.Energy.Init(GameManager.Player);
        GridManager.Instance.GenerateMap();
    }

    public override string ToString()
    {
        return "Sleep";
    }
}
