using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatcherBehaviour : GenericPlayerBehaviour
{
    public override void Awake()
    {
        base.Awake();
    }

    public void CalculateCatcherColliderInterraction(GameObject pitcherGameObject, GameObject ballGameObject, BallController ballControllerScript)
    {
        float catchSuccesRate = ActionCalculationUtils.CalculateCatchSuccessRate(this.gameObject, pitcherGameObject);
        Debug.Log("catchSuccesRate = " + catchSuccesRate);
        if (!ActionCalculationUtils.HasActionSucceeded(catchSuccesRate))
        {
            StopCoroutine(ballControllerScript.MovementCoroutine);
            ballControllerScript.IsPitched = false;
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCathcherOutBallZonePosition());
        }
        else
        {
            ballGameObject.SetActive(false);
            ballControllerScript.CurrentHolder = this.gameObject;
        }
    }
}
