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
            bool isInWalkState = false;

            ballGameObject.SetActive(false);
            ballControllerScript.CurrentHolder = this.gameObject;

            if (currentBatterBehaviour.StrikeOutcomeCount == 3)
            {
                currentBatter.SetActive(false);
                gameManager.AttackTeamBatterList.Remove(currentBatter);
                this.SetUpNewBatter(gameManager, bat);
                currentBatterBehaviour.StrikeOutcomeCount = 0;
            }
            else if(currentBatterBehaviour.BallOutcomeCount == 4)
            {
                isInWalkState = true;
                PlayerStatus currentBatterStatus = PlayerUtils.FetchPlayerStatusScript(currentBatter);
                RunnerBehaviour newRunnerBehaviour = currentBatterBehaviour.ConvertBatterToRunner(currentBatterStatus);

                PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
                PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(currentBatter);
                playerAbilities.PlayerAbilityList.Clear();
                PlayerAbility runPlayerAbility = new PlayerAbility("Run to next base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.RunAction, currentBatter);
                PlayerAbility StaySafePlayerAbility = new PlayerAbility("Stay on base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.StayOnBaseAction, currentBatter);
                playerAbilities.AddAbility(runPlayerAbility);
                playerAbilities.AddAbility(StaySafePlayerAbility);

                gameManager.AttackTeamRunnerList.Add(newRunnerBehaviour.gameObject);
                gameManager.AttackTeamBatterList.Remove(currentBatter);
                newRunnerBehaviour.IsInWalkState = isInWalkState;
                currentBatterStatus.IsAllowedToMove = true;
                newRunnerBehaviour.EnableMovement = true;
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

            if (!isInWalkState)
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
