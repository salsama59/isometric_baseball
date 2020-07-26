﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerActionsManager : MonoBehaviour
{

    private GameObject ballGameObject;
    private GameObject pitcherGameObject;
    private BallController ballControllerScript;
    private TargetSelectionManager targetSelectionManager;

    private void Start()
    {
        TargetSelectionManager = GameUtils.FetchTargetSelectionManager();
    }

    public void PitchBallAction(GameObject actionUser)
    {
        //PITCHER TURN
        //literaly throw the ball!!!
        ballGameObject.SetActive(true);
        BallControllerScript.BallHeight = BallHeightEnum.LOW;
        BallControllerScript.IsPitched = true;
        BallControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
    }

    public void CatchBallAction(GameObject actionUser)
    {
        //CATCHER TURN
        GameObject catcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.CATCHER, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.CATCHER));
        PlayerStatus catcherStatusScript = PlayerUtils.FetchPlayerStatusScript(catcher);
        CatcherBehaviour genericCatcherBehaviourScript = ((CatcherBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(catcher, catcherStatusScript));
        genericCatcherBehaviourScript.CalculateCatcherColliderInterraction(PitcherGameObject, ballGameObject, ballControllerScript);
    }

    public void HitBallAction(GameObject actionUser)
    {
        //BATTER TURN
        GameObject batter = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.BATTER, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.BATTER));
        BatterBehaviour batterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(batter);

        batterBehaviourScript.IsReadyToSwing = true;
        batterBehaviourScript.IsSwingHasFinished = false;
        
        if (PlayerUtils.IsCurrentPlayerPosition(batter, PlayerFieldPositionEnum.BATTER))
        {
            PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(batter);
            batterBehaviourScript.CalculateBatterColliderInterraction(PitcherGameObject, BallControllerScript, playerStatusScript);
        }
    }

    public void RunAction(GameObject actionUser)
    {
        //RUNNER TURN
        RunnerBehaviour genericRunnerBehaviourScript;
        BaseEnum baseReachedEnum = this.GetCurrentBaseReached(out genericRunnerBehaviourScript);
        genericRunnerBehaviourScript.CalculateRunnerColliderInterraction(baseReachedEnum, false);
    }

    public void PassBallAction(GameObject actionUser, GameObject actionTarget)
    {
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        playersTurnManager.TurnState = TurnStateEnum.STANDBY;
        PlayerStatus actionUserStatus =  PlayerUtils.FetchPlayerStatusScript(actionUser);
        GenericPlayerBehaviour actionUserGenericBehaviour = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(actionUser, actionUserStatus);
        BallControllerScript.gameObject.transform.SetParent(null);
        ballGameObject.SetActive(true);
        BallControllerScript.BallHeight = BallHeightEnum.LOW;
        BallControllerScript.IsPassed = true;
        BallControllerScript.IsHit = false;
        BallControllerScript.Target = actionTarget.transform.position;
        BallControllerScript.IsHeld = false;
        BallControllerScript.CurrentPasser = actionUser;
        actionUserGenericBehaviour.IsHoldingBall = false;
    }

    public void GenericPassAction(GameObject actionUser)
    {

        List<GameObject> fielders = PlayerUtils.GetFieldersOnField()
            .Where(fielder => !fielder.Equals(actionUser))
            .OrderBy(fielder => Vector3.Distance(actionUser.transform.position, fielder.transform.position))
            .ToList();
        TargetSelectionManager.EnableSelection(fielders.First().transform.position, fielders, PassBallAction, actionUser);
    }

    public void StayOnBaseAction(GameObject actionUser)
    {
        //RUNNER TURN
        RunnerBehaviour runnerBehaviourScript;
        BaseEnum baseReachedEnum = this.GetCurrentBaseReached(out runnerBehaviourScript);
        runnerBehaviourScript.CalculateRunnerColliderInterraction(baseReachedEnum, true);
    }


    public void GenericTagOutAimAction(GameObject actionUser)
    {
        List<GameObject> runners = PlayerUtils.GetRunnersOnField()
           .OrderBy(runner => Vector3.Distance(actionUser.transform.position, runner.transform.position))
           .ToList();

        if(runners.Count > 1)
        {
            TargetSelectionManager.EnableSelection(runners.First().transform.position, runners, TagOutAimAction, actionUser);
        }
        else
        {
            GameObject nearestRunner = runners.First();
            this.TagOutAimAction(actionUser, nearestRunner);
        }
        
    }

    public void TagOutAimAction(GameObject actionUser, GameObject actionTarget)
    {
        PlayerStatus actionUserStatusScript = PlayerUtils.FetchPlayerStatusScript(actionUser);
        GenericPlayerBehaviour genericPlayerBehaviourScript = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(actionUser, actionUserStatusScript);
        genericPlayerBehaviourScript.TargetPlayerToTagOut = actionTarget;
    }

    public static void InterceptBall(GameObject ballGameObject, BallController ballControllerScript, GenericPlayerBehaviour genericPlayerBehaviourScript)
    {
        ballControllerScript.BallHeight = BallHeightEnum.NONE;
        ballGameObject.transform.SetParent(genericPlayerBehaviourScript.gameObject.transform);
        ballGameObject.SetActive(false);
        ballControllerScript.CurrentHolder = genericPlayerBehaviourScript.gameObject;
        ballControllerScript.IsHeld = true;
        ballControllerScript.IsHit = false;
        ballControllerScript.IsPassed = false;
        ballControllerScript.Target = null;
        ballControllerScript.IsMoving = false;
        ballControllerScript.IsTargetedByPitcher = false;
        genericPlayerBehaviourScript.IsHoldingBall = true;
        genericPlayerBehaviourScript.HasSpottedBall = false;
        genericPlayerBehaviourScript.Target = null;
    }

    private BaseEnum GetCurrentBaseReached(out RunnerBehaviour genericRunnerBehaviourScript)
    {
        Debug.Log("Runner choice!!!!");
        GameObject runner = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.RUNNER, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.RUNNER));
        PlayerStatus runnerStatusScript = PlayerUtils.FetchPlayerStatusScript(runner);
        genericRunnerBehaviourScript = ((RunnerBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(runner, runnerStatusScript));
        return genericRunnerBehaviourScript.NextBase;
    }

    public GameObject BallGameObject { get => ballGameObject; set => ballGameObject = value; }
    public BallController BallControllerScript { get => ballControllerScript; set => ballControllerScript = value; }
    public GameObject PitcherGameObject { get => pitcherGameObject; set => pitcherGameObject = value; }
    public TargetSelectionManager TargetSelectionManager { get => targetSelectionManager; set => targetSelectionManager = value; }
}
