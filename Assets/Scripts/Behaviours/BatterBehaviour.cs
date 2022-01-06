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

    public void CalculateBatterColliderInterraction(GameObject pitcherGameObject, BallController ballControllerScript)
    {
        float pitchSuccesRate = ActionCalculationUtils.CalculatePitchSuccessRate(pitcherGameObject, this.gameObject);
        StartCoroutine(this.WaitForSwing(pitchSuccesRate, ballControllerScript));
    }

    private void DoBattingAction(BallController ballControllerScript, float pitchSuccesRate)
    {
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
            //Do the calculation in advance to keep the original result before managing the foul state of the ball
            ballControllerScript.BallHitDirection = MathUtils.CalculateDirection(ballControllerScript.gameObject.transform.position, ballControllerScript.Target.Value);
            StartCoroutine(this.ManageFoulStateActions(ballControllerScript));
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


    private IEnumerator ManageFoulStateActions(BallController ballControllerScript)
    {
        yield return new WaitForSeconds(0.6f);
        GameObject ball = ballControllerScript.gameObject;
        GameManager gameManager = GameUtils.FetchGameManager();
        float ballDirectionAngle = MathUtils.CalculateDirectionAngle(ballControllerScript.BallHitDirection);
        Vector3 firstBasePosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFirstBaseTilePosition());
        Vector3 homeBasePosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
        float homeBaseToFirstBaseDistance = Vector3.Distance(homeBasePosition, firstBasePosition);
        Vector3 rigthSideFictionalPosition = new Vector3(firstBasePosition.x, homeBasePosition.y, 0);
        float homeBaseToRigthSideFictionalDistance = Vector3.Distance(homeBasePosition, rigthSideFictionalPosition);
        float foulZoneAngle = Mathf.Acos(homeBaseToRigthSideFictionalDistance / homeBaseToFirstBaseDistance);

        //if ball not sent in foul zone
        if (ballDirectionAngle < MathUtils.HALF_CIRCLE_ANGLE_IN_DEGREE - foulZoneAngle * Mathf.Rad2Deg && ballDirectionAngle > foulZoneAngle * Mathf.Rad2Deg)
        {
            ballControllerScript.IsInFoulState = false;
            GameObject currentBatter = gameManager.AttackTeamBatterListClone.First();
            PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(currentBatter);
            RunnerBehaviour runnerBehaviour = RunnerUtils.ConvertBatterToRunner(playerStatusScript);
            RunnerUtils.AddRunnerAbilitiesToBatter(currentBatter);
            playerStatusScript.IsAllowedToMove = true;
            runnerBehaviour.EnableMovement = true;
            PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
            playersTurnManager.IsRunnersTurnsDone = false;
        }
        else
        {
            ballControllerScript.IsInFoulState = true;
            Vector3 textPosition = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
            TextManager textManagerScript = GameUtils.FetchTextManager();
            textManagerScript.CreateText(textPosition, "FOUL!!", Color.black, 1f, true);
            PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
            GameObject pitcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.PITCHER, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.PITCHER));

            GameObject currentBatter = gameManager.AttackTeamBatterListClone.First();
            BatterBehaviour currentBatterBehaviour = PlayerUtils.FetchBatterBehaviourScript(currentBatter);
            GameObject bat = currentBatterBehaviour.EquipedBat;
            currentBatterBehaviour.FoulOutcomeCount += 1;
            currentBatter.transform.rotation = Quaternion.identity;
            bat.transform.position = FieldUtils.GetBatCorrectPosition(currentBatter.transform.position);
            bat.transform.rotation = Quaternion.Euler(0f, 0f, -70f);
            gameManager.ReinitPitcher(pitcher);
            gameManager.ReturnBallToPitcher(ballControllerScript.gameObject);
            gameManager.ReinitRunners(gameManager.AttackTeamRunnerList);
            playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
            PlayersTurnManager.IsCommandPhase = true;
        }
    }


    private IEnumerator WaitToEnableBatCollider()
    {
        yield return new WaitForSeconds(2f);
        this.EquipedBat.GetComponent<CapsuleCollider2D>().enabled = true;
    }

    private IEnumerator RotatePlayer(GameObject player)
    {
        player.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 130f * Time.deltaTime);
        yield return null;
    }

    private IEnumerator WaitForSwing(float pitchSuccesRate, BallController ballControllerScript)
    {
        yield return new WaitUntil(() => IsSwingHasFinished == true);
        this.DoBattingAction(ballControllerScript, pitchSuccesRate);
    }

    public bool IsReadyToSwing { get => isReadyToSwing; set => isReadyToSwing = value; }
    public bool IsSwingHasFinished { get => isSwingHasFinished; set => isSwingHasFinished = value; }
    public int StrikeOutcomeCount { get => strikeOutcomeCount; set => strikeOutcomeCount = value; }
    public int BallOutcomeCount { get => ballOutcomeCount; set => ballOutcomeCount = value; }
    public int FoulOutcomeCount { get => foulOutcomeCount; set => foulOutcomeCount = value; }
}
