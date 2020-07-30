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
        GameObject currentBatter = gameManager.AttackTeamBatterList.First();
        BatterBehaviour currentBatterBehaviour = PlayerUtils.FetchBatterBehaviourScript(currentBatter);
        GameObject bat = currentBatterBehaviour.EquipedBat;
        PlayerStatus currentBatterStatus = PlayerUtils.FetchPlayerStatusScript(currentBatter);

        float catchSuccesRate = ActionCalculationUtils.CalculateCatchSuccessRate(this.gameObject, pitcherGameObject);
        if (!ActionCalculationUtils.HasActionSucceeded(catchSuccesRate))
        {
            StopCoroutine(ballControllerScript.MovementCoroutine);
            ballControllerScript.IsPitched = false;
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCathcherOutBallZonePosition());

            if(currentBatterBehaviour.StrikeOutcomeCount == 3)
            {
                RunnerBehaviour runnerBehaviour = currentBatterBehaviour.ConvertBatterToRunner(currentBatterStatus);
                currentBatterBehaviour.AddRunnerAbilitiesToBatter(currentBatter);

                gameManager.AttackTeamRunnerList.Add(currentBatter);
                gameManager.AttackTeamBatterList.Remove(currentBatter);

                //Not realy hit but rather not catch properly!!!!!
                ballControllerScript.IsHit = true;
                currentBatterStatus.IsAllowedToMove = true;
                runnerBehaviour.EnableMovement = true;
                this.SetUpNewBatter(gameManager, bat);

                //Catcher must go look for the ball 
            }
        }
        else
        {
            StopCoroutine(ballControllerScript.MovementCoroutine);
            bool isInWalkState = false;

            ballGameObject.SetActive(false);
            ballControllerScript.CurrentHolder = this.gameObject;

            if (currentBatterBehaviour.StrikeOutcomeCount == 3)
            {
                currentBatter.SetActive(false);
                gameManager.AttackTeamBatterList.Remove(currentBatter);
                this.SetUpNewBatter(gameManager, bat);
                currentBatterBehaviour.StrikeOutcomeCount = 0;
                currentBatterBehaviour.BallOutcomeCount = 0;
                gameManager.BatterOutCount++;
            }
            else if(currentBatterBehaviour.BallOutcomeCount == 4)
            {
                isInWalkState = true;
                
                RunnerBehaviour newRunnerBehaviour = currentBatterBehaviour.ConvertBatterToRunner(currentBatterStatus);
                currentBatterBehaviour.AddRunnerAbilitiesToBatter(currentBatter);

                gameManager.AttackTeamRunnerList.Add(newRunnerBehaviour.gameObject);
                gameManager.AttackTeamBatterList.Remove(currentBatter);
                newRunnerBehaviour.IsInWalkState = isInWalkState;
                currentBatterStatus.IsAllowedToMove = true;
                newRunnerBehaviour.EnableMovement = true;
                currentBatterBehaviour.StrikeOutcomeCount = 0;
                currentBatterBehaviour.BallOutcomeCount = 0;
                this.SetUpNewBatter(gameManager, bat);
            }
            else
            {
                currentBatter.transform.rotation = Quaternion.identity;
                bat.transform.position = FieldUtils.GetBatCorrectPosition(currentBatter.transform.position);
                bat.transform.rotation = Quaternion.Euler(0f, 0f, -70f);
            }

            gameManager.ReturnBallToPitcher(ballControllerScript.gameObject);

            bool isInningHalfEnd = gameManager.BatterOutCount == 3;

            if (isInningHalfEnd)
            {
                //TODO
                //team switch attack/defense
            }

            if (!isInWalkState && !isInningHalfEnd)
            {
                PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
                PlayersTurnManager.IsCommandPhase = true;
            }
        }
    }

    private void SetUpNewBatter(GameManager gameManager, GameObject bat)
    {
        GameObject newBatter = gameManager.AttackTeamBatterList.First();
        TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.BATTER, newBatter, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.BATTER));
        newBatter.SetActive(true);
        gameManager.EquipBatToPlayer(newBatter, bat);
    }
}
