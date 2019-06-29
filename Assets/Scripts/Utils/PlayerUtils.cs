using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUtils : MonoBehaviour
{
    public static PlayerStatus FetchPlayerStatusScript(GameObject player)
    {
        return FetchPlayerCharacterControllerScript(player).PlayerStatusInformations;
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

    public static PlayerCharacterController FetchPlayerCharacterControllerScript(GameObject player)
    {
        return player.GetComponent<PlayerCharacterController>();
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
                break;
            case PlayerFieldPositionEnum.RUNNER:
                genericPlayerBehaviour = FetchRunnerBehaviourScript(player);
                break;
            case PlayerFieldPositionEnum.CATCHER:
                break;
            case PlayerFieldPositionEnum.FIRST_BASEMAN:
                break;
            case PlayerFieldPositionEnum.SECOND_BASEMAN:
                break;
            case PlayerFieldPositionEnum.THIRD_BASEMAN:
                break;
            case PlayerFieldPositionEnum.SHORT_STOP:
                break;
            case PlayerFieldPositionEnum.LEFT_FIELDER:
                break;
            case PlayerFieldPositionEnum.CENTER_FIELDER:
                break;
            case PlayerFieldPositionEnum.RIGHT_FIELDER:
                break;
            default:
                genericPlayerBehaviour = FetchBatterBehaviourScript(player);
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

    public static bool IsCorrespondingPlayerPosition(GameObject player, PlayerFieldPositionEnum playerFieldPositionToTest)
    {
        PlayerStatus currentPlayerStatusScript = FetchPlayerStatusScript(player);
        return currentPlayerStatusScript.PlayerFieldPosition.Equals(playerFieldPositionToTest);
    }

}
