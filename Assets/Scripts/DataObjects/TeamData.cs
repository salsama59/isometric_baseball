using System;
using UnityEngine;

[Serializable]
public class TeamData
{
    [SerializeField]
    private int teamId;
    [SerializeField]
    private string teamFullName;
    [SerializeField]
    private string teamShortName;
    [SerializeField]
    private PlayerData[] players;

    public TeamData(int teamId, string teamFullName, string teamShortName, PlayerData[] players)
    {
        TeamId = teamId;
        TeamFullName = teamFullName;
        TeamShortName = teamShortName;
        Players = players;
    }

    public string TeamShortName { get => teamShortName; set => teamShortName = value; }
    public string TeamFullName { get => teamFullName; set => teamFullName = value; }
    public int TeamId { get => teamId; set => teamId = value; }
    public PlayerData[] Players { get => players; set => players = value; }
}
