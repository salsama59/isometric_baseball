using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatcherBehaviour : GenericPlayerBehaviour
{
    private string catcherMode = ModeConstants.CATCHER_NORMAL_MODE;

    public override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        BallController ballControlerScript = BallUtils.FetchBallControllerScript(FieldBall);

        if (HasSpottedBall && FieldBall.activeInHierarchy && !IsHoldingBall && ballControlerScript.IsTargetedByFielder)
        {
            Target = FieldBall.transform.position;
        }

        if (Target.HasValue && Target.Value != this.transform.position)
        {
            MovePlayer();
            this.IsPrepared = true;
        }
    }

    public void CalculateCatcherColliderInterraction(GameObject pitcherGameObject, GameObject ballGameObject, BallController ballControllerScript)
    {
        GameManager gameManager = GameUtils.FetchGameManager();
        GameObject currentBatter = gameManager.AttackTeamBatterListClone.First();
        BatterBehaviour currentBatterBehaviour = PlayerUtils.FetchBatterBehaviourScript(currentBatter);
        GameObject bat = currentBatterBehaviour.EquipedBat;
        PlayerStatus currentBatterStatus = PlayerUtils.FetchPlayerStatusScript(currentBatter);
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        GameObject pitcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.PITCHER, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.PITCHER));

        float catchSuccesRate = ActionCalculationUtils.CalculateCatchSuccessRate(this.gameObject, pitcherGameObject);
        if (!ActionCalculationUtils.HasActionSucceeded(catchSuccesRate))
        {
            ballControllerScript.Target = null;
            ballControllerScript.IsPitched = false;
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCathcherOutBallZonePosition());

            if(currentBatterBehaviour.StrikeOutcomeCount == 3)
            {
                RunnerBehaviour runnerBehaviour = currentBatterBehaviour.ConvertBatterToRunner(currentBatterStatus);
                currentBatterBehaviour.AddRunnerAbilitiesToBatter(currentBatter);

                //Not realy hit but rather not catch properly!!!!!
                ballControllerScript.IsHit = true;
                currentBatterStatus.IsAllowedToMove = true;
                runnerBehaviour.EnableMovement = true;
                this.SetUpNewBatter(gameManager, bat);

                StartCoroutine(this.WaitForAction(4f));
            }
            else
            {
                gameManager.ReinitPitcher(pitcher);
                gameManager.ReturnBallToPitcher(ballControllerScript.gameObject);
                gameManager.ReinitRunners(gameManager.AttackTeamRunnerList);
                currentBatter.transform.rotation = Quaternion.identity;
                bat.transform.position = FieldUtils.GetBatCorrectPosition(currentBatter.transform.position);
                bat.transform.rotation = Quaternion.Euler(0f, 0f, -70f);
                playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
                PlayersTurnManager.IsCommandPhase = true;
            }
        }
        else
        {
            bool isInWalkState = false;

            ballGameObject.SetActive(false);
            ballControllerScript.CurrentHolder = this.gameObject;

            if (currentBatterBehaviour.StrikeOutcomeCount == 3)
            {
                currentBatter.SetActive(false);
                gameManager.AttackTeamBatterListClone.Remove(currentBatter);
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

           
            gameManager.ReinitPitcher(pitcher);
            gameManager.ReturnBallToPitcher(ballControllerScript.gameObject);
            gameManager.ReinitRunners(gameManager.AttackTeamRunnerList);

            bool isInningHalfEnd = gameManager.BatterOutCount == 3;

            if (isInningHalfEnd)
            {
                //Team switch attack/defense
                gameManager.BatterOutCount = 0;
                gameManager.ProcessNextInningHalf();
            }

            if (!isInWalkState && !isInningHalfEnd)
            {
                playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
                PlayersTurnManager.IsCommandPhase = true;
            }
        }
    }

    private void SetUpNewBatter(GameManager gameManager, GameObject bat)
    {
        GameObject newBatter = gameManager.AttackTeamBatterListClone.First();
        TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.BATTER, newBatter, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.BATTER));
        newBatter.SetActive(true);
        gameManager.EquipBatToPlayer(newBatter, bat);
    }

    public void AddFielderAbilitiesToCatcher(GameObject player)
    {
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(player);
        playerAbilities.PlayerAbilityList.Clear();
        PlayerAbility passPlayerAbility = new PlayerAbility("Pass to fielder", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.GenericPassAction, player, true);
        playerAbilities.AddAbility(passPlayerAbility);
    }

    private IEnumerator WaitForAction(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        //Catcher must go look for the ball 
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        CatcherBehaviour catcherBehaviour = PlayerUtils.FetchCatcherBehaviourScript(this.gameObject);
        playerActionsManager.AimForTheBall(catcherBehaviour);

        //switch the catcher mode (mode normal, mode fielder)
        this.CatcherMode = ModeConstants.CATCHER_FIELDER_MODE;

        //Add the relevant abilities
        this.AddFielderAbilitiesToCatcher(this.gameObject);
        
    }

    public void ReturnToInitialPosition(GameObject actionUser = null, GameObject targetPlayer = null)
    {
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(actionUser);
        playerAbilities.PlayerAbilityList.Clear();
        PlayerAbility catchPlayerAbility = new PlayerAbility("Catch ball", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.CatchBallAction, actionUser);
        playerAbilities.AddAbility(catchPlayerAbility);
        PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(actionUser);
        playerStatus.IsAllowedToMove = true;
        this.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCatcherZonePosition());
    }

    public string CatcherMode { get => catcherMode; set => catcherMode = value; }

}
