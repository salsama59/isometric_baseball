using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterBehaviour : GenericPlayerBehaviour
{
    public override void Awake()
    {
        base.Awake();
    }

    public void CalculateBatterColliderInterraction(GameObject pitcherGameObject, BallController ballControllerScript, PlayerStatus playerStatusScript)
    {

        float pitchSuccesRate = ActionCalculationUtils.CalculatePitchSuccessRate(pitcherGameObject, this.gameObject);
        Debug.Log("pitchSuccesRate = " + pitchSuccesRate);

        if (!ActionCalculationUtils.HasActionSucceeded(pitchSuccesRate))
        {
            ballControllerScript.IsBeingHitten = true;
            List<Vector2Int> ballPositionList = ActionCalculationUtils.CalculateBallFallPositionList(this.gameObject, 0, 180, 10, true);
            int ballPositionIndex = Random.Range(0, ballPositionList.Count - 1);
            Vector2Int ballTilePosition = ballPositionList[ballPositionIndex];
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(ballTilePosition);

            Destroy(this.gameObject.GetComponent<BatterBehaviour>());
            this.gameObject.AddComponent<RunnerBehaviour>();
            playerStatusScript.IsAllowedToMove = true;
            playerStatusScript.PlayerFieldPosition = PlayerFieldPositionEnum.RUNNER;
        }
        else
        {
            ballControllerScript.IsThrown = true;
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCatcherZonePosition());
        }

    }
}
