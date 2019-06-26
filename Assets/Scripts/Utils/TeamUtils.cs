using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamUtils : MonoBehaviour
{
    public static int teamSize = 9;
    public static Dictionary<PlayerFieldPositionEnum, Vector3> playerTeamMenberPositionLocation = new Dictionary<PlayerFieldPositionEnum, Vector3>() {
        {PlayerFieldPositionEnum.BATTER,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetPrimeBaseTilePosition())},
        {PlayerFieldPositionEnum.PITCHER,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetMiddleBaseTilePosition())}
    };
    private static Dictionary<int, GameObject> player1Team = new Dictionary<int, GameObject>();
    private static Dictionary<int, GameObject> player2Team = new Dictionary<int, GameObject>();

    public static void AddPlayerTeamMember(PlayerFieldPositionEnum playerTeamMemberPosition, GameObject teamMemberGameObject, PlayerEnum playerId)
    {
        int positionId = (int)playerTeamMemberPosition;

        Dictionary<int, GameObject> playerTeam = GetPlayerTeam(playerId);

        if(playerTeam != null)
        {
            if (playerTeam.ContainsKey(positionId))
            {
                playerTeam[positionId] = teamMemberGameObject;
            }
            else
            {
                playerTeam.Add(positionId, teamMemberGameObject);
            }
        }
        else
        {
            Debug.Log("No team with id = " + playerId + " founded.");
        }
    }

    public static GameObject GetPlayerTeamMember(PlayerFieldPositionEnum playerTeamMemberPosition, PlayerEnum playerId)
    {
        GameObject playerTeamMember = null;
        int positionId = (int)playerTeamMemberPosition;
        playerTeamMember = GetPlayerTeam(playerId)[positionId];
        return playerTeamMember;
    }

    public static Dictionary<int, GameObject> GetPlayerTeam(PlayerEnum playerId)
    {
        Dictionary<int, GameObject> playerTeam = null;

        switch (playerId)
        {
            case PlayerEnum.PLAYER_1:
                playerTeam = player1Team;
                break;
            case PlayerEnum.PLAYER_2:
                playerTeam = player2Team;
                break;
            default:
                break;
        }

        return playerTeam;
    }

    public static void ClearPlayerTeam(PlayerEnum playerId)
    {
        GetPlayerTeam(playerId).Clear();
    }
}
