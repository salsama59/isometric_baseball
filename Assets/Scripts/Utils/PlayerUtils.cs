using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerUtils : MonoBehaviour
{

    public static PlayerAbilities FetchPlayerAbilitiesScript(GameObject player)
    {
        return player.GetComponent<PlayerAbilities>();
    }

    public static PlayerStatus FetchPlayerStatusScript(GameObject player)
    {
        return player.GetComponent<PlayerStatus>();
    }

    public static IsometricCharacterRenderer FetchPlayerIsometricRenderer(GameObject player)
    {
        return player.GetComponentInChildren<IsometricCharacterRenderer>();
    }

    public static BatterBehaviour FetchBatterBehaviourScript(GameObject player)
    {
        return player.GetComponent<BatterBehaviour>();
    }

    public static RunnerBehaviour FetchRunnerBehaviourScript(GameObject player)
    {
        return player.GetComponent<RunnerBehaviour>();
    }

    public static FielderBehaviour FetchFielderBehaviourScript(GameObject player)
    {
        return player.GetComponent<FielderBehaviour>();
    }

    public static CatcherBehaviour FetchCatcherBehaviourScript(GameObject player)
    {
        return player.GetComponent<CatcherBehaviour>();
    }

    public static PitcherBehaviour FetchPitcherBehaviourScript(GameObject player)
    {
        return player.GetComponent<PitcherBehaviour>();
    }

    public static GenericPlayerBehaviour FetchCorrespondingPlayerBehaviourScript(GameObject player, PlayerStatus playerStatus)
    {
        GenericPlayerBehaviour genericPlayerBehaviour = null;

        switch (playerStatus.PlayerFieldPosition)
        {
            case PlayerFieldPositionEnum.BATTER:
                genericPlayerBehaviour = FetchBatterBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.PITCHER:
                genericPlayerBehaviour = FetchPitcherBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.RUNNER:
                genericPlayerBehaviour = FetchRunnerBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.CATCHER:
                genericPlayerBehaviour = FetchCatcherBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.FIRST_BASEMAN:
                genericPlayerBehaviour = FetchFielderBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.SECOND_BASEMAN:
                genericPlayerBehaviour = FetchFielderBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.THIRD_BASEMAN:
                genericPlayerBehaviour = FetchFielderBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.SHORT_STOP:
                genericPlayerBehaviour = FetchFielderBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.LEFT_FIELDER:
                genericPlayerBehaviour = FetchFielderBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.CENTER_FIELDER:
                genericPlayerBehaviour = FetchFielderBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.RIGHT_FIELDER:
                genericPlayerBehaviour = FetchFielderBehaviourScript(player);
                break;
            default:
                break;
        }

        return genericPlayerBehaviour;
    }

    public static bool IsPlayerAllowedToMove(GameObject player)
    {
        PlayerStatus playerStatus = FetchPlayerStatusScript(player);

        bool isMoveAllowed = false;

        switch (playerStatus.PlayerFieldPosition)
        {
            case PlayerFieldPositionEnum.BATTER:
                isMoveAllowed = false;
                break;
            case PlayerFieldPositionEnum.PITCHER:
                isMoveAllowed = false;
                break;
            case PlayerFieldPositionEnum.RUNNER:
                isMoveAllowed = true;
                break;
            case PlayerFieldPositionEnum.CATCHER:
                isMoveAllowed = false;
                break;
            case PlayerFieldPositionEnum.FIRST_BASEMAN:
                isMoveAllowed = true;
                break;
            case PlayerFieldPositionEnum.SECOND_BASEMAN:
                isMoveAllowed = true;
                break;
            case PlayerFieldPositionEnum.THIRD_BASEMAN:
                isMoveAllowed = true;
                break;
            case PlayerFieldPositionEnum.SHORT_STOP:
                isMoveAllowed = true;
                break;
            case PlayerFieldPositionEnum.LEFT_FIELDER:
                isMoveAllowed = true;
                break;
            case PlayerFieldPositionEnum.CENTER_FIELDER:
                isMoveAllowed = true;
                break;
            case PlayerFieldPositionEnum.RIGHT_FIELDER:
                isMoveAllowed = true;
                break;
            default:
                isMoveAllowed = false;
                break;
        }

        return isMoveAllowed;
    }

    public static bool IsCurrentPlayerPosition(GameObject player, PlayerFieldPositionEnum playerFieldPositionToTest)
    {
        PlayerStatus currentPlayerStatusScript = FetchPlayerStatusScript(player);
        return currentPlayerStatusScript.PlayerFieldPosition.Equals(playerFieldPositionToTest);
    }


    public static bool HasFielderPosition(GameObject player)
    {
        PlayerStatus currentPlayerStatusScript = FetchPlayerStatusScript(player);
        PlayerFieldPositionEnum playerPosition = currentPlayerStatusScript.PlayerFieldPosition;

        return playerPosition == PlayerFieldPositionEnum.CENTER_FIELDER
            || playerPosition == PlayerFieldPositionEnum.LEFT_FIELDER
            || playerPosition == PlayerFieldPositionEnum.RIGHT_FIELDER
            || playerPosition == PlayerFieldPositionEnum.SECOND_BASEMAN
            || playerPosition == PlayerFieldPositionEnum.SHORT_STOP
            || playerPosition == PlayerFieldPositionEnum.THIRD_BASEMAN
            || playerPosition == PlayerFieldPositionEnum.FIRST_BASEMAN;
    }

    public static List<GameObject> GetRunnersOnField()
    {
        return GameObject.FindGameObjectsWithTag(TagsConstants.BASEBALL_PLAYER_TAG)
            .Where(baseBallPlayer => IsCurrentPlayerPosition(baseBallPlayer, PlayerFieldPositionEnum.RUNNER))
            .ToList();
    }

    public static GameObject GetPlayerBatGameObject(GameObject player)
    {
        return GameObject.FindGameObjectsWithTag(TagsConstants.BASEBALL_BAT_TAG)
            .Where(bat => bat.transform.parent.gameObject == player)
            .First();
    }

}
