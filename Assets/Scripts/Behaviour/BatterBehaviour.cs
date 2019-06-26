using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterBehaviour : GenericPlayerBehaviour
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void CalculateNextAction()
    {
        GenericCharacterController genericCharacterController = PlayerUtils.FetchCharacterControllerScript(this.gameObject);

        BaseEnum nextBase = genericCharacterController.NextBase;
        genericCharacterController.CurrentBase = nextBase;

        switch (nextBase)
        {
            case BaseEnum.PRIME_BASE:
                Debug.Log("Get on FIRST BASE");
                Debug.Log("WIN ONE POINT !!!");
                genericCharacterController.Target = null;
                genericCharacterController.NextBase = genericCharacterController.CurrentBase;
                genericCharacterController.PlayerStatusInformations.IsAllowedToMove = false;
                break;
            case BaseEnum.SECOND_BASE:
                Debug.Log("Get on SECOND BASE");
                genericCharacterController.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetThirdBaseTilePosition());
                genericCharacterController.NextBase = BaseEnum.THIRD_BASE;
                break;
            case BaseEnum.THIRD_BASE:
                Debug.Log("Get on THIRD BASE");
                genericCharacterController.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFourthBaseTilePosition());
                genericCharacterController.NextBase = BaseEnum.FOURTH_BASE;
                break;
            case BaseEnum.FOURTH_BASE:
                Debug.Log("Get on FOURTH BASE");
                genericCharacterController.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetPrimeBaseTilePosition());
                genericCharacterController.NextBase = BaseEnum.PRIME_BASE;
                break;
            default:
                Debug.Log("DO NOT KNOW WHAT HAPPEN");
                break;
        }
    }
}
