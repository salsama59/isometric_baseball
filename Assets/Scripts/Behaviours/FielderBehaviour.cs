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
        ballControllerScript.BallHeight = BallHeightEnum.NONE;
        ballGameObject.transform.SetParent(this.gameObject.transform);
        ballGameObject.SetActive(false);
        ballControllerScript.CurrentHolder = this.gameObject;
        ballControllerScript.IsHeld = true;
        genericPlayerBehaviourScript.IsHoldingBall = true;
        genericPlayerBehaviourScript.HasSpottedBall = false;
    }

    public void TagOutRunner(GameObject targetToTagOut)
    {
        PlayerStatus fielderPlayerStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
        PlayerStatus runnerPlayerStatus = PlayerUtils.FetchPlayerStatusScript(targetToTagOut);

        GameData.isPaused = true;
        DialogBoxManager dialogBoxManagerScript =  GameUtils.FetchDialogBoxManager();

        dialogBoxManagerScript.SetDialogTextBox("TAG OUT !!!!!!!");
        dialogBoxManagerScript.ToggleDialogTextBox();

        StartCoroutine(WaitAndReinit(dialogBoxManagerScript, runnerPlayerStatus, fielderPlayerStatus));

    }

    IEnumerator WaitAndReinit(DialogBoxManager dialogBoxManagerScript, PlayerStatus tagedOutRunnerStatus, PlayerStatus fielderPlayerStatus)
    {
        yield return new WaitForSeconds(2f);
        if (dialogBoxManagerScript.DialogTextBoxGameObject.activeSelf)
        {
            dialogBoxManagerScript.ToggleDialogTextBox();
        }

        BallController ballControllerScript = BallUtils.FetchBallControllerScript(FieldBall);
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();

        //Update Pitcher informations
        PitcherBehaviour pitcherBehaviourScript = PlayerUtils.FetchPitcherBehaviourScript(ballControllerScript.CurrentPitcher);
        PlayerAbilities pitcherPlayerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(ballControllerScript.CurrentPitcher);
        PlayerStatus pitcherPlayerStatus = PlayerUtils.FetchPlayerStatusScript(ballControllerScript.CurrentPitcher);
        PlayerAbility throwBallPlayerAbility = new PlayerAbility("Throw", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.ThrowBallAction);
        PlayerAbility gyroBallSpecialPlayerAbility = new PlayerAbility("Gyro ball", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.NORMAL, playerActionsManager.ThrowBallAction);
        PlayerAbility fireBallSpecialPlayerAbility = new PlayerAbility("Fire ball", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.NORMAL, playerActionsManager.ThrowBallAction);
        PlayerAbility menuBackAction = new PlayerAbility("Back", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.UI, null);
        pitcherPlayerAbilities.PlayerAbilityList.Clear();
        pitcherPlayerAbilities.AddAbility(throwBallPlayerAbility);
        pitcherPlayerAbilities.AddAbility(gyroBallSpecialPlayerAbility);
        pitcherPlayerAbilities.AddAbility(fireBallSpecialPlayerAbility);
        pitcherPlayerAbilities.AddAbility(menuBackAction);
        pitcherPlayerAbilities.HasSpecialAbilities = true;
        ballControllerScript.CurrentPitcher.transform.position = TeamUtils.playerTeamMenberPositionLocation[pitcherPlayerStatus.PlayerFieldPosition];
        pitcherPlayerStatus.IsAllowedToMove = false;
        pitcherBehaviourScript.Target = null;
        pitcherBehaviourScript.HasSpottedBall = false;

        //Update ball informations
        ballControllerScript.BallHeight = BallHeightEnum.NONE;
        FieldBall.transform.position = ballControllerScript.CurrentPitcher.transform.position;
        ballControllerScript.CurrentHolder = null;
        ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
        //No parent
        FieldBall.transform.SetParent(null);
        ballControllerScript.IsHeld = false;
        ballControllerScript.IsPitched = false;
        ballControllerScript.IsMoving = false;
        ballControllerScript.IsTargetedByFielder = false;
        ballControllerScript.EnableMovement = true;

        //Update taged out runner and new batter informations
        tagedOutRunnerStatus.IsAllowedToMove = false;
        tagedOutRunnerStatus.PlayerFieldPosition = PlayerFieldPositionEnum.BATTER;
        RunnerBehaviour runnerBehaviourScript =  PlayerUtils.FetchRunnerBehaviourScript(tagedOutRunnerStatus.gameObject);
        GameObject bat = runnerBehaviourScript.EquipedBat;
        Destroy(tagedOutRunnerStatus.gameObject.GetComponent<RunnerBehaviour>());
        tagedOutRunnerStatus.gameObject.AddComponent<BatterBehaviour>();
        tagedOutRunnerStatus.gameObject.transform.position = TeamUtils.playerTeamMenberPositionLocation[PlayerFieldPositionEnum.BATTER];
        BatterBehaviour batterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(tagedOutRunnerStatus.gameObject);
        batterBehaviourScript.Start();
        batterBehaviourScript.EquipedBat = bat;
        batterBehaviourScript.EquipedBat.SetActive(true);
        tagedOutRunnerStatus.gameObject.transform.rotation = Quaternion.identity;
        batterBehaviourScript.IsoRenderer.LastDirection = 6;
        batterBehaviourScript.IsoRenderer.SetDirection(Vector2.zero);
        PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(tagedOutRunnerStatus.gameObject);
        PlayerAbility hitBallPlayerAbility = new PlayerAbility("Hit ball", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.HitBallAction);
        playerAbilities.PlayerAbilityList.Clear();
        playerAbilities.AddAbility(hitBallPlayerAbility);

        //Update fielder informations
        fielderPlayerStatus.IsAllowedToMove = true;
        this.HasSpottedBall = false;
        this.IsPrepared = false;
        this.Target = null;
        this.IsHoldingBall = false;
        this.TargetPlayerToTagOut = null;
        this.IsMoving = false;
        this.gameObject.transform.position = TeamUtils.playerTeamMenberPositionLocation[fielderPlayerStatus.PlayerFieldPosition];
        this.transform.rotation = Quaternion.identity;
        IsoRenderer.LastDirection = 4;
        IsoRenderer.SetDirection(Vector2.zero);

        //Reinit turn
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        PlayersTurnManager.IsCommandPhase = true;
        playersTurnManager.turnState = TurnStateEnum.PITCHER_TURN;

        //Remove pause state
        GameData.isPaused = false;
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
