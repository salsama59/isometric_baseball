﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PitcherBehaviour : GenericPlayerBehaviour
{
    private BallController ballControlerScript;

    public override void Start()
    {
        base.Start();
        IsoRenderer.LastDirection = 4;
        IsoRenderer.SetDirection(Vector2.zero);
        IsoRenderer.PreferredDirection = 4;
        ballControlerScript = BallUtils.FetchBallControllerScript(FieldBall);
    }

    public override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {

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

    public void CalculatePitcherColliderInterraction(GameObject ballGameObject, BallController ballControllerScript, GenericPlayerBehaviour genericPlayerBehaviourScript)
    {
        int runnersOnFieldCount = -1;
        List<GameObject> runners = PlayerUtils.GetRunnersOnField();
        runnersOnFieldCount = runners.Count;

        if(runnersOnFieldCount < 1)
        {
            return;
        }

        //Choose the runner who just hit the ball
        GameObject runnerToGetOut = runners.Last();

        bool hasIntercepted = false;
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();

        if (ballControllerScript.BallHeight == BallHeightEnum.HIGH || ballControllerScript.BallHeight == BallHeightEnum.LOW)
        {
            
            GameManager gameManager = GameUtils.FetchGameManager();
            DialogBoxManager dialogBoxManagerScript = GameUtils.FetchDialogBoxManager();
            dialogBoxManagerScript.DisplayDialogAndTextForGivenAmountOfTime(1f, false, "TAG OUT !!!!!!!");

            ballControllerScript.Target = null;
            
            
            PlayerActionsManager.InterceptBall(ballGameObject, ballControllerScript, genericPlayerBehaviourScript);
            hasIntercepted = true;

            gameManager.AttackTeamRunnerList.Remove(runnerToGetOut);
            runnerToGetOut.SetActive(false);
            playersTurnManager.UpdatePlayerTurnQueue(runnerToGetOut);
            gameManager.AttackTeamBatterListClone.First().SetActive(true);
            RunnerBehaviour runnerBehaviourScript = PlayerUtils.FetchRunnerBehaviourScript(runnerToGetOut);
            BatterBehaviour batterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(gameManager.AttackTeamBatterListClone.First());
            batterBehaviourScript.EquipedBat = runnerBehaviourScript.EquipedBat;
            runnerBehaviourScript.EquipedBat = null;

            if (runnersOnFieldCount == 1)
            {
                GameData.isPaused = true;
                StartCoroutine(gameManager.WaitAndReinit(dialogBoxManagerScript, PlayerUtils.FetchPlayerStatusScript(gameManager.AttackTeamBatterListClone.First()), FieldBall));
                return;
            }
            else
            {
                GameObject bat = batterBehaviourScript.EquipedBat;
                bat.transform.SetParent(null);
                bat.transform.position = FieldUtils.GetBatCorrectPosition(batterBehaviourScript.transform.position);
                bat.transform.rotation = Quaternion.Euler(0f, 0f, -70f);
                bat.transform.SetParent(gameManager.AttackTeamBatterListClone.First().transform);
                batterBehaviourScript.EquipedBat.SetActive(true);
                TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.BATTER, batterBehaviourScript.gameObject, TeamUtils.GetBaseballPlayerOwner(batterBehaviourScript.gameObject));
            }
            
        }
        
        if(runnersOnFieldCount >= 1)
        {
            if (!hasIntercepted)
            {
                PlayerActionsManager.InterceptBall(ballGameObject, ballControllerScript, genericPlayerBehaviourScript);
            }
            
            PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
            PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(this.gameObject);
            playerAbilities.ReinitAbilities();
            PlayerAbility passPlayerAbility = new PlayerAbility("Pass to fielder", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.GenericPassAction, this.gameObject, true);
            playerAbilities.AddAbility(passPlayerAbility);
            playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
            PlayersTurnManager.IsCommandPhase = true;
        }

    }

    public void CalculatePitcherTriggerInterraction(GameObject ballGameObject, GenericPlayerBehaviour genericPlayerBehaviourScript, PlayerStatus playerStatus)
    {
        BallController ballControlerScript = BallUtils.FetchBallControllerScript(ballGameObject);
        ballControlerScript.IsTargetedByPitcher = true;
        genericPlayerBehaviourScript.HasSpottedBall = true;
        playerStatus.IsAllowedToMove = true;
        genericPlayerBehaviourScript.Target = ballGameObject.transform.position;
        this.transform.rotation = Quaternion.identity;
    }
}
