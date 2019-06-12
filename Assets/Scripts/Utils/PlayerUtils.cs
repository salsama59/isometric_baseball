using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUtils : MonoBehaviour
{
    public static BatterBehaviour FetchBatterBehaviourScript(GameObject player)
    {
        return player.GetComponent<BatterBehaviour>();
    }

    public static RunnerBehaviour FetchRunnerBehaviourScript(GameObject player)
    {
        return player.GetComponent<RunnerBehaviour>();
    }

    public static GenericCharacterController FetchCharacterControllerScript(GameObject player)
    {
        return player.GetComponent<GenericCharacterController>();
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
}
