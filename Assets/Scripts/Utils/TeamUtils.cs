using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class TeamUtils : MonoBehaviour
{
    public static int teamSize = 9;
    public static Dictionary<PlayerFieldPositionEnum, Vector3> playerTeamMenberPositionLocation = new Dictionary<PlayerFieldPositionEnum, Vector3>() {
        {PlayerFieldPositionEnum.BATTER,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetBatterTilePosition())},
        {PlayerFieldPositionEnum.PITCHER,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetPitcherBaseTilePosition())},
        {PlayerFieldPositionEnum.CATCHER,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCatcherZonePosition())},
        {PlayerFieldPositionEnum.FIRST_BASEMAN,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFirstBaseTilePosition())},
        {PlayerFieldPositionEnum.THIRD_BASEMAN,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetThirdBaseTilePosition())},
        {PlayerFieldPositionEnum.SECOND_BASEMAN,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetSecondBasemanTilePosition())},
        {PlayerFieldPositionEnum.SHORT_STOP,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetShortStopTilePosition())},
        {PlayerFieldPositionEnum.LEFT_FIELDER,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetLeftFielderTilePosition())},
        {PlayerFieldPositionEnum.RIGHT_FIELDER,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetRightFielderTilePosition())},
        {PlayerFieldPositionEnum.CENTER_FIELDER,  FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCenterFielderTilePosition())}
    };
    private static Dictionary<int, GameObject> player1Team = new Dictionary<int, GameObject>();
    private static Dictionary<int, GameObject> player2Team = new Dictionary<int, GameObject>();

    public static List<GameObject> fielderList = new List<GameObject>();

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
        GameObject playerTeamMember;
        int positionId = (int)playerTeamMemberPosition;
        playerTeamMember = GetPlayerTeam(playerId)[positionId];
        return playerTeamMember;
    }


    public static PlayerEnum GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum playerFieldPositionEnum)
    {
        foreach (KeyValuePair<PlayerEnum, List<PlayerFieldPositionEnum>> entry in GameData.playerFieldPositionEnumListMap)
        {
            if(entry.Value.Contains(playerFieldPositionEnum))
            {
                return entry.Key;
            }
        }

        Debug.LogWarning("No match found for player position during search in playerFieldPositionEnumListMap so default will be : " + PlayerEnum.PLAYER_1);
        return PlayerEnum.PLAYER_1;
    }

    public static PlayerEnum GetBaseballPlayerOwner(GameObject baseballPlayer)
    {
        PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(baseballPlayer);
        return playerStatus.PlayerOwner;
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

    public static void SetPlayerTeam(PlayerEnum playerId, Dictionary<int, GameObject> playerTeam)
    {
        switch (playerId)
        {
            case PlayerEnum.PLAYER_1:
                player1Team = playerTeam;
                break;
            case PlayerEnum.PLAYER_2:
                player2Team = playerTeam;
                break;
            default:
                break;
        }
    }

    public static void ClearPlayerTeam(PlayerEnum playerId)
    {
        GetPlayerTeam(playerId).Clear();
    }

    public static GameObject GetNearestFielderFromGameObject(GameObject gameObject)
    {
        Nullable<float> nearestDistance = null;
        GameObject nearestFielder = null;

        foreach (GameObject fielderGameObject in fielderList)
        {
            float distance = Vector3.Distance(gameObject.transform.position, fielderGameObject.transform.position);

            if (!nearestDistance.HasValue)
            {
                nearestDistance = distance;
                nearestFielder = fielderGameObject;
            }

            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestFielder = fielderGameObject;
            }
        }

        return nearestFielder;
    }

    public static GameObject GetNearestRunerFromFielder(GameObject fielder)
    {
        Nullable<float> nearestDistance = null;
        GameObject nearestRunner = null;

        List<GameObject> runnerList = PlayerUtils.GetRunnersOnField();
        foreach (GameObject runnerGameObject in runnerList)
        {
            float distance = Vector3.Distance(fielder.transform.position, runnerGameObject.transform.position);

            if (!nearestDistance.HasValue)
            {
                nearestDistance = distance;
                nearestRunner = runnerGameObject;
            }

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestRunner = runnerGameObject;
            }
        }

        return nearestRunner;
    }
}
