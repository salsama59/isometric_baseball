using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatterBehaviour : GenericPlayerBehaviour
{

    private Quaternion targetRotation = Quaternion.Euler(0, 0, 45f);
    private bool isReadyToSwing = false;
    private bool isSwingHasFinished = false;
    private int strikeOutcomeCount = 0;
    private int ballOutcomeCount = 0;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        IsSwingHasFinished = true;
        IsoRenderer.LastDirection = 6;
        IsoRenderer.SetDirection(Vector2.zero);
    }

    private void Update()
    {
        if (IsReadyToSwing)
        {
            StartCoroutine(RotatePlayer(this.gameObject));
            if (this.transform.rotation.eulerAngles.z >= targetRotation.eulerAngles.z)
            {
                IsReadyToSwing = false;
                IsSwingHasFinished = true;
            }
        }
    }

    public void CalculateBatterColliderInterraction(GameObject pitcherGameObject, BallController ballControllerScript, PlayerStatus playerStatusScript)
    {
        float pitchSuccesRate = ActionCalculationUtils.CalculatePitchSuccessRate(pitcherGameObject, this.gameObject);
        StartCoroutine(this.WaitForSwing(pitchSuccesRate, ballControllerScript, playerStatusScript));
    }

    private void DoBattingAction(BallController ballControllerScript, PlayerStatus playerStatusScript, float pitchSuccesRate)
    {
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();

        if (!ActionCalculationUtils.HasActionSucceeded(pitchSuccesRate))
        {
            ballControllerScript.IsPitched = false;
            ballControllerScript.IsHit = true;
            List<Vector2Int> ballPositionList = ActionCalculationUtils.CalculateBallFallPositionList(this.gameObject, 0, 180, 10, true);
            int ballPositionIndex = Random.Range(0, ballPositionList.Count - 1);
            Vector2Int ballTilePosition = ballPositionList[ballPositionIndex];
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(ballTilePosition);
            RunnerBehaviour runnerBehaviour = this.ConvertBatterToRunner(playerStatusScript);
            this.AddRunnerAbilitiesToBatter(this.gameObject);

            playerStatusScript.IsAllowedToMove = true;
            runnerBehaviour.EnableMovement = true;
            playersTurnManager.IsRunnersTurnsDone = false;
        }
        else
        {

            float ballOutcomeRate = ActionCalculationUtils.CalculateBallOutcomeProbability(ballControllerScript.CurrentPitcher);
            bool isBallOutcome = ActionCalculationUtils.HasActionSucceeded(ballOutcomeRate);
            DialogBoxManager dialogBoxManagerScript = GameUtils.FetchDialogBoxManager();
            string outcomeMessage;

            if (!isBallOutcome)
            {
                StrikeOutcomeCount++;
                outcomeMessage = "STRIKE!!!";
                
            }
            else
            {
                BallOutcomeCount++;
                outcomeMessage = "BALL!!!";
            }

            ballControllerScript.IsPitched = true;
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCatcherZonePosition());
            dialogBoxManagerScript.DisplayDialogAndTextForGivenAmountOfTime(1f, false, outcomeMessage);
        }
    }

    public void AddRunnerAbilitiesToBatter(GameObject player)
    {
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(player);
        playerAbilities.PlayerAbilityList.Clear();
        PlayerAbility runPlayerAbility = new PlayerAbility("Run to next base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.RunAction, player);
        PlayerAbility StaySafePlayerAbility = new PlayerAbility("Stay on base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.StayOnBaseAction, player);
        playerAbilities.AddAbility(runPlayerAbility);
        playerAbilities.AddAbility(StaySafePlayerAbility);
    }

    public RunnerBehaviour ConvertBatterToRunner(PlayerStatus batterStatusScript)
    {
        GameObject currentBatter = batterStatusScript.gameObject;
        GameObject bat = PlayerUtils.GetPlayerBatGameObject(currentBatter);
        GameManager gameManager = GameUtils.FetchGameManager();
        RunnerBehaviour runnerBehaviour = currentBatter.AddComponent<RunnerBehaviour>();
        gameManager.AttackTeamRunnerList.Add(runnerBehaviour.gameObject);
        gameManager.AttackTeamBatterList.Remove(currentBatter);
        runnerBehaviour.EquipedBat = bat;
        bat.SetActive(false);
        Destroy(currentBatter.GetComponent<BatterBehaviour>());

        batterStatusScript.PlayerFieldPosition = PlayerFieldPositionEnum.RUNNER;
        TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.RUNNER, currentBatter, PlayerEnum.PLAYER_1);
        GameObject nextBatter = gameManager.AttackTeamBatterList.First();
        TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.BATTER, nextBatter, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.BATTER));

        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        playersTurnManager.TurnState = TurnStateEnum.STANDBY;
        string newRunnerName = NameConstants.RUNNER_NAME + "_" + gameManager.AttackTeamRunnerList.IndexOf(runnerBehaviour.gameObject);
        runnerBehaviour.gameObject.name = newRunnerName;

        return runnerBehaviour;
    }

    private IEnumerator RotatePlayer(GameObject player)
    {
        player.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 130f * Time.deltaTime);
        yield return null;
    }

    private IEnumerator WaitForSwing(float pitchSuccesRate, BallController ballControllerScript, PlayerStatus playerStatusScript)
    {
        yield return new WaitUntil(() => IsSwingHasFinished == true);
        this.DoBattingAction(ballControllerScript, playerStatusScript, pitchSuccesRate);
    }

    public bool IsReadyToSwing { get => isReadyToSwing; set => isReadyToSwing = value; }
    public bool IsSwingHasFinished { get => isSwingHasFinished; set => isSwingHasFinished = value; }
    public int StrikeOutcomeCount { get => strikeOutcomeCount; set => strikeOutcomeCount = value; }
    public int BallOutcomeCount { get => ballOutcomeCount; set => ballOutcomeCount = value; }
}
