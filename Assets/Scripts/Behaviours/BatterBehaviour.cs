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
    private int foulOutcomeCount = 0;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        IsSwingHasFinished = true;
        IsoRenderer.LastDirection = 6;
        IsoRenderer.PreferredDirection = 6;
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
            this.EquipedBat.GetComponent<CapsuleCollider2D>().enabled = false;
            ballControllerScript.IsPitched = false;
            ballControllerScript.IsHit = true;
            GameObject homeBase = GameUtils.FetchBaseGameObject(TagsConstants.HOME_BASE_TAG);
            List<Vector2Int> ballPositionList = ActionCalculationUtils.CalculateBallFallPositionList(this.gameObject, 0, (int)MathUtils.HALF_CIRCLE_ANGLE_IN_DEGREE, 10, true, FieldUtils.GetGameObjectTilePositionOnField(homeBase));
            int ballPositionIndex = Random.Range(0, ballPositionList.Count - 1);
            Vector2Int ballTilePosition = ballPositionList[ballPositionIndex];
            ballControllerScript.StopCoroutine(ballControllerScript.MovementCoroutine);
            ballControllerScript.gameObject.transform.position = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(ballTilePosition);
            Vector2 direction = MathUtils.CalculateDirection(ballControllerScript.gameObject.transform.position, ballControllerScript.Target.Value);
            float ballDirectionAngle = MathUtils.CalculateDirectionAngle(direction);
            Vector3 firstBasePosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFirstBaseTilePosition());
            Vector3 homeBasePosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
            float homeBaseToFirstBaseDistance = Vector3.Distance(homeBasePosition, firstBasePosition);
            Vector3 rigthSideFictionalPosition = new Vector3(firstBasePosition.x, homeBasePosition.y, 0);
            float homeBaseToRigthSideFictionalDistance = Vector3.Distance(homeBasePosition, rigthSideFictionalPosition);
            float foulZoneAngle = Mathf.Acos(homeBaseToRigthSideFictionalDistance / homeBaseToFirstBaseDistance);

            //if ball not sent in foul zone
            if (ballDirectionAngle < MathUtils.HALF_CIRCLE_ANGLE_IN_DEGREE - foulZoneAngle * Mathf.Rad2Deg && ballDirectionAngle > foulZoneAngle * Mathf.Rad2Deg)
            {
                RunnerBehaviour runnerBehaviour = this.ConvertBatterToRunner(playerStatusScript);
                this.AddRunnerAbilitiesToBatter(this.gameObject);
                playerStatusScript.IsAllowedToMove = true;
                runnerBehaviour.EnableMovement = true;
            }

            playersTurnManager.IsRunnersTurnsDone = false;

            StartCoroutine(this.WaitToEnableBatCollider());
        }
        else
        {

            float ballOutcomeRate = ActionCalculationUtils.CalculateBallOutcomeProbability(ballControllerScript.CurrentPitcher);
            bool isBallOutcome = ActionCalculationUtils.HasActionSucceeded(ballOutcomeRate);
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
            TextManager textManagerScript = GameUtils.FetchTextManager();
            textManagerScript.CreateText(this.transform.position, outcomeMessage, Color.black, 1f, true);
        }
    }


    private IEnumerator WaitToEnableBatCollider()
    {
        yield return new WaitForSeconds(2f);
        this.EquipedBat.GetComponent<CapsuleCollider2D>().enabled = true;
    }

    public void AddRunnerAbilitiesToBatter(GameObject player)
    {
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(player);
        playerAbilities.ReinitAbilities();
        PlayerAbility runPlayerAbility = new PlayerAbility("Run to next base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.RunAction, player);
        PlayerAbility StaySafePlayerAbility = new PlayerAbility("Stay on base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.StayOnBaseAction, player);
        playerAbilities.AddAbility(runPlayerAbility);
        playerAbilities.AddAbility(StaySafePlayerAbility);
    }

    public RunnerBehaviour ConvertBatterToRunner(PlayerStatus batterStatusScript)
    {
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        GameObject currentBatter = batterStatusScript.gameObject;
        GameObject bat = PlayerUtils.GetPlayerBatGameObject(currentBatter);
        GameManager gameManager = GameUtils.FetchGameManager();
        RunnerBehaviour runnerBehaviour = currentBatter.AddComponent<RunnerBehaviour>();
        gameManager.AttackTeamRunnerList.Add(runnerBehaviour.gameObject);
        gameManager.AttackTeamRunnerListClone.Add(runnerBehaviour.gameObject);
        gameManager.AttackTeamBatterListClone.Remove(currentBatter);
        playersTurnManager.CurrentRunner = runnerBehaviour.gameObject;
        runnerBehaviour.EquipedBat = bat;
        bat.SetActive(false);
        Destroy(currentBatter.GetComponent<BatterBehaviour>());

        batterStatusScript.PlayerFieldPosition = PlayerFieldPositionEnum.RUNNER;
        TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.RUNNER, currentBatter, TeamUtils.GetBaseballPlayerOwner(currentBatter));
        
        int batterCount = gameManager.AttackTeamBatterListClone.Count;
        if (batterCount > 0)
        {
            GameObject nextBatter = gameManager.AttackTeamBatterListClone.First();
            gameManager.EquipBatToPlayer(nextBatter);
            TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.BATTER, nextBatter, TeamUtils.GetBaseballPlayerOwner(nextBatter));
        }

        string runnerNumber = runnerBehaviour.gameObject.name.Split('_').Last();
        string newRunnerName = NameConstants.RUNNER_NAME + "_" + runnerNumber;
        runnerBehaviour.gameObject.name = newRunnerName;

        
        playersTurnManager.TurnState = TurnStateEnum.STANDBY;
        
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
    public int FoulOutcomeCount { get => foulOutcomeCount; set => foulOutcomeCount = value; }
}
