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
        BallController ballControlerScript = BallUtils.FetchBallControllerScript(FieldBall);
        if (FieldBall.activeInHierarchy && !HasSpottedBall)
        {
            GetAngleToLookAt();
        }
        else if (HasSpottedBall && FieldBall.activeInHierarchy && !IsHoldingBall && ballControlerScript.IsTargetedByFielder)
        {
            Target = FieldBall.transform.position;
        }
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
            PlayerActionsManager.InterceptBall(ballGameObject, ballControllerScript, genericPlayerBehaviourScript);
            PlayerStatus fielderStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
            playersTurnManager.TurnState = TurnStateEnum.FIELDER_TURN;
            playersTurnManager.CurrentFielderTypeTurn = fielderStatus.PlayerFieldPosition;
            PlayersTurnManager.IsCommandPhase = true;
        }

    }

    public void TagOutRunner(GameObject targetToTagOut)
    {
        GameManager gameManager = GameUtils.FetchGameManager();
        GameObject newBatter = gameManager.AttackTeamBatterList.First();
        PlayerStatus fielderPlayerStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
        PlayerStatus newBatterStatus = PlayerUtils.FetchPlayerStatusScript(newBatter);

        GameData.isPaused = true;
        DialogBoxManager dialogBoxManagerScript =  GameUtils.FetchDialogBoxManager();

        dialogBoxManagerScript.SetDialogTextBox("TAG OUT !!!!!!!");
        dialogBoxManagerScript.ToggleDialogTextBox();

        gameManager.AttackTeamRunnerList.Remove(targetToTagOut);
        targetToTagOut.SetActive(false);
        newBatter.SetActive(true);
        RunnerBehaviour tagOutRunnerBehaviourScript = PlayerUtils.FetchRunnerBehaviourScript(targetToTagOut);
        BatterBehaviour newbatterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(newBatter);
        newbatterBehaviourScript.EquipedBat = tagOutRunnerBehaviourScript.EquipedBat;
        tagOutRunnerBehaviourScript.EquipedBat = null;
        StartCoroutine(gameManager.WaitAndReinit(dialogBoxManagerScript, newBatterStatus, fielderPlayerStatus, FieldBall));
    }

    public void CalculateFielderTriggerInterraction(GenericPlayerBehaviour genericPlayerBehaviourScript)
    {
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        playerActionsManager.AimForTheBall(genericPlayerBehaviourScript);
    }
}
