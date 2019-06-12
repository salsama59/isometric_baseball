using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus
{
    private PlayerFieldPositionEnum playerFieldPosition;
    private string playerName;

    public PlayerFieldPositionEnum PlayerFieldPosition { get => playerFieldPosition; set => playerFieldPosition = value; }
    public string PlayerName { get => playerName; set => playerName = value; }
}
