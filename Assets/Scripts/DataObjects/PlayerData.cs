using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField]
    private int playerId;
    [SerializeField]
    private int playerTeamId;
    [SerializeField]
    private int playerStatusId;
    [SerializeField]
    private int playerAbilityId;
    [SerializeField]
    private string playerFirstName;
    [SerializeField]
    private string playerLastName;
    [SerializeField]
    private string playerUniformNumber;
    [SerializeField]
    private string playerFieldPosition;
    [SerializeField]
    private string defaultPlayerFieldPosition;
    [SerializeField]
    private PlayerStatusData playerStatus;

    public PlayerData(int playerId, int playerTeamId, int playerStatusId, int playerAbilityId, string playerFirstName, string playerLastName, string playerUniformNumber, string playerFieldPosition, string defaultPlayerFieldPosition, PlayerStatusData playerStatus)
    {
        PlayerId = playerId;
        PlayerTeamId = playerTeamId;
        PlayerStatusId = playerStatusId;
        PlayerAbilityId = playerAbilityId;
        PlayerFirstName = playerFirstName;
        PlayerLastName = playerLastName;
        PlayerUniformNumber = playerUniformNumber;
        PlayerFieldPosition = playerFieldPosition;
        DefaultPlayerFieldPosition = defaultPlayerFieldPosition;
        PlayerStatus = playerStatus;
    }

    public int PlayerId { get => playerId; set => playerId = value; }
    public int PlayerTeamId { get => playerTeamId; set => playerTeamId = value; }
    public int PlayerStatusId { get => playerStatusId; set => playerStatusId = value; }
    public int PlayerAbilityId { get => playerAbilityId; set => playerAbilityId = value; }
    public string PlayerFirstName { get => playerFirstName; set => playerFirstName = value; }
    public string PlayerLastName { get => playerLastName; set => playerLastName = value; }
    public string PlayerUniformNumber { get => playerUniformNumber; set => playerUniformNumber = value; }
    public string PlayerFieldPosition { get => playerFieldPosition; set => playerFieldPosition = value; }
    public string DefaultPlayerFieldPosition { get => defaultPlayerFieldPosition; set => defaultPlayerFieldPosition = value; }
    public PlayerStatusData PlayerStatus { get => playerStatus; set => playerStatus = value; }
}
