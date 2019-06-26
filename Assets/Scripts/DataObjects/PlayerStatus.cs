using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus
{
    private PlayerFieldPositionEnum playerFieldPosition;
    private string playerName;
    private bool isAllowedToMove = false;

    public PlayerFieldPositionEnum PlayerFieldPosition { get => playerFieldPosition; set => playerFieldPosition = value; }
    public string PlayerName { get => playerName; set => playerName = value; }
    public bool IsAllowedToMove { get => isAllowedToMove; set => isAllowedToMove = value; }
}
