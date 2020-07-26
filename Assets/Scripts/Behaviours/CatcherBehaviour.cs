using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatcherBehaviour : GenericPlayerBehaviour
{
    public override void Awake()
    {
        base.Awake();
    }

    public void CalculateCatcherColliderInterraction(GameObject pitcherGameObject, GameObject ballGameObject, BallController ballControllerScript)
    {
        GameManager gameManager = GameUtils.FetchGameManager();
        float catchSuccesRate = ActionCalculationUtils.CalculateCatchSuccessRate(this.gameObject, pitcherGameObject);
        if (!ActionCalculationUtils.HasActionSucceeded(catchSuccesRate))
        {
            StopCoroutine(ballControllerScript.MovementCoroutine);
            ballControllerScript.IsPitched = false;
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCathcherOutBallZonePosition());
        }
        else
        {
            StopCoroutine(ballControllerScript.MovementCoroutine);
            GameObject currentBatter = gameManager.AttackTeamBatterList.First();
            BatterBehaviour currentBatterBehaviour = PlayerUtils.FetchBatterBehaviourScript(currentBatter);
            GameObject bat = currentBatterBehaviour.EquipedBat;

            ballGameObject.SetActive(false);
            ballControllerScript.CurrentHolder = this.gameObject;

            if (currentBatterBehaviour.StrikeOutcomeCount == 3)
            {
                gameManager.AttackTeamBatterList.Remove(currentBatter);
                currentBatter.SetActive(false);
                GameObject newBatter = gameManager.AttackTeamBatterList.First();
                TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.BATTER, newBatter, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.BATTER));
                newBatter.SetActive(true);
                gameManager.EquipBatToPlayer(newBatter, bat);
                currentBatterBehaviour.StrikeOutcomeCount = 0;
                currentBatterBehaviour.BallOutcomeCount = 0;
            }
            else
            {
                currentBatter.transform.rotation = Quaternion.identity;
                bat.transform.position = FieldUtils.GetBatCorrectPosition(currentBatter.transform.position);
                bat.transform.rotation = Quaternion.Euler(0f, 0f, -70f);
            }

            gameManager.ReturnBallToPitcher(ballControllerScript.gameObject);
            
            PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
            playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
            PlayersTurnManager.IsCommandPhase = true;
        }
    }
}
