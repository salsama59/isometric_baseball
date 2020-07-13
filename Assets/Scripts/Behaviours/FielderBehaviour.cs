using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FielderBehaviour : GenericPlayerBehaviour
{

    public override void Start()
    {
        base.Start();
        IsoRenderer.LastDirection = 4;
        IsoRenderer.SetDirection(Vector2.zero);
    }

    public override void Awake()
    {
        base.Awake();
    }

    public void Update()
    {
        BallController ballControlerScript = BallUtils.FetchBallControllerScript(FieldBall);
        if (TargetPlayerToTagOut != null && PlayerUtils.HasFielderPosition(this.gameObject))
        {
            Target = TargetPlayerToTagOut.transform.position;
        }
        else if (HasSpottedBall && FieldBall.activeInHierarchy && !IsHoldingBall && ballControlerScript.IsTargetedByFielder)
        {
            Target = FieldBall.transform.position;
        }

        if (Target.HasValue && Target.Value != this.transform.position)
        {
            MovePlayer();
            this.IsPrepared = true;
        }
        else
        {
            PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            if (playerStatus.IsAllowedToMove)
            {
                this.InitiateFielderAction();
            }
        }
    }

    private void InitiateFielderAction()
    {
        Nullable<Vector3> targetPosition = new Nullable<Vector3>();
        BallController ballControlerScript = BallUtils.FetchBallControllerScript(FieldBall);
        if (FieldBall.activeInHierarchy && !HasSpottedBall)
        {
            GetAngleToLookAt();
        }
        else if (!FieldBall.activeInHierarchy && IsHoldingBall)
        {
            //Find the nearest runner on field
            GameObject nearestRunner = TeamUtils.GetNearestRunerFromFielder(this.gameObject);
            TargetPlayerToTagOut = nearestRunner;
            targetPosition = TargetPlayerToTagOut.transform.position;
        }
        else if (HasSpottedBall && FieldBall.activeInHierarchy && !IsHoldingBall && ballControlerScript.IsTargetedByFielder)
        {
            targetPosition = FieldBall.transform.position;
        }

        Target = targetPosition;
    }

    private void GetAngleToLookAt()
    {
        Vector3 dir = FieldBall.transform.position - this.transform.position;
        dir.Normalize();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    public void CalculateFielderColliderInterraction(GameObject ballGameObject, BallController ballControllerScript, GenericPlayerBehaviour genericPlayerBehaviourScript)
    {

        float passSuccessRate;

        if (ballControllerScript.IsPassed)
        {
            passSuccessRate = ActionCalculationUtils.CalculatePassSuccessRate(this.gameObject, ballControllerScript.CurrentHolder, ballControllerScript.BallHeight);
        }
        else
        {
            passSuccessRate = 100f;
        }

        if (ActionCalculationUtils.HasActionSucceeded(passSuccessRate))
        {
            ballControllerScript.BallHeight = BallHeightEnum.NONE;
            ballGameObject.transform.SetParent(this.gameObject.transform);
            ballGameObject.SetActive(false);
            ballControllerScript.CurrentHolder = this.gameObject;
            ballControllerScript.IsHeld = true;
            genericPlayerBehaviourScript.IsHoldingBall = true;
            genericPlayerBehaviourScript.HasSpottedBall = false;
        }

    }

    public void TagOutRunner(GameObject targetToTagOut)
    {
        GameManager gameManager = GameUtils.FetchGameManager();
        PlayerStatus fielderPlayerStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
        PlayerStatus runnerPlayerStatus = PlayerUtils.FetchPlayerStatusScript(gameManager.AttackTeamBatterList.First());

        GameData.isPaused = true;
        DialogBoxManager dialogBoxManagerScript =  GameUtils.FetchDialogBoxManager();

        dialogBoxManagerScript.SetDialogTextBox("TAG OUT !!!!!!!");
        dialogBoxManagerScript.ToggleDialogTextBox();

        gameManager.AttackTeamRunnerList.Remove(targetToTagOut);
        targetToTagOut.SetActive(false);
        gameManager.AttackTeamBatterList.First().SetActive(true);
        RunnerBehaviour runnerBehaviourScript = PlayerUtils.FetchRunnerBehaviourScript(targetToTagOut);
        BatterBehaviour batterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(gameManager.AttackTeamBatterList.First());
        batterBehaviourScript.EquipedBat = runnerBehaviourScript.EquipedBat;
        runnerBehaviourScript.EquipedBat = null;
        StartCoroutine(gameManager.WaitAndReinit(dialogBoxManagerScript, runnerPlayerStatus, fielderPlayerStatus, FieldBall));

        
    }

    public void CalculateFielderTriggerInterraction(GameObject ballGameObject, GenericPlayerBehaviour genericPlayerBehaviourScript, PlayerStatus playerStatus)
    {
        BallController ballControlerScript = BallUtils.FetchBallControllerScript(ballGameObject);
        ballControlerScript.IsTargetedByFielder = true;
        genericPlayerBehaviourScript.HasSpottedBall = true;
        playerStatus.IsAllowedToMove = true;
        genericPlayerBehaviourScript.Target = ballGameObject.transform.position;
        this.transform.rotation = Quaternion.identity;
    }
}
