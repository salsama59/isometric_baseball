using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

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

    public void AimForTheBall(GenericPlayerBehaviour playerBehaviour)
    {
        GameObject player = playerBehaviour.gameObject;
        PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(player);
        BallController ballControlerScript = BallUtils.FetchBallControllerScript(BallGameObject);
        ballControlerScript.IsTargetedByFielder = true;
        playerBehaviour.HasSpottedBall = true;
        playerStatus.IsAllowedToMove = true;
        playerBehaviour.Target = ballGameObject.transform.position;
        player.transform.rotation = Quaternion.identity;
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
        GameObject catcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.CATCHER, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.CATCHER));
        PlayerStatus catcherStatusScript = PlayerUtils.FetchPlayerStatusScript(catcher);
        CatcherBehaviour genericCatcherBehaviourScript = ((CatcherBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(catcher, catcherStatusScript));
        genericCatcherBehaviourScript.CalculateCatcherColliderInterraction(PitcherGameObject, ballGameObject, ballControllerScript);
    }

    public void HitBallAction(GameObject actionUser)
    {
        GameObject batter = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.BATTER, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.BATTER));
        BatterBehaviour batterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(batter);

        batterBehaviourScript.IsReadyToSwing = true;
        batterBehaviourScript.IsSwingHasFinished = false;
        
        if (PlayerUtils.HasBatterPosition(batter))
        {
            batterBehaviourScript.CalculateBatterColliderInterraction(PitcherGameObject, BallControllerScript);
        }
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

        Action<GameObject, GameObject> finalActionsToPerform = PassBallAction;

        //Add an additionnal action for the pass. => go back to initial placement for the catcher.
        if (PlayerUtils.HasCatcherPosition(actionUser))
        {
            CatcherBehaviour catcherBehaviour = PlayerUtils.FetchCatcherBehaviourScript(actionUser);
            finalActionsToPerform += catcherBehaviour.ReturnToInitialPosition;
        }

        TargetSelectionManager.EnableSelection(fielders.First().transform.position, fielders, finalActionsToPerform, actionUser);
    }

    public void RunAction(GameObject actionUser)
    {
        //RUNNER TURN
        RunnerBehaviour runnerBehaviourScript = PlayerUtils.FetchRunnerBehaviourScript(actionUser);
        runnerBehaviourScript.GoToNextBase(runnerBehaviourScript.CurrentBase);
    }

    public void StayOnBaseAction(GameObject actionUser)
    {
        //RUNNER TURN
        RunnerBehaviour runnerBehaviourScript = PlayerUtils.FetchRunnerBehaviourScript(actionUser);
        runnerBehaviourScript.StayOnCurrentBase();
    }


    public void GenericTagOutAimAction(GameObject actionUser)
    {
        List<GameObject> runners = PlayerUtils.GetRunnersOnField().Where(runner => {
            RunnerBehaviour runnerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(runner);
            return !runnerBehaviour.IsSafe && !runnerBehaviour.IsStaying;
        }).OrderBy(runner => Vector3.Distance(actionUser.transform.position, runner.transform.position))
        .ToList();

        if (runners.Count > 1)
        {
            TargetSelectionManager.EnableSelection(runners.First().transform.position, runners, TagOutAimAction, actionUser);
            PlayersTurnManager.IsCommandPhase = true;
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
        PlayersTurnManager.IsCommandPhase = false;
    }

    public static void InterceptBall(GameObject ballGameObject, BallController ballControllerScript, GenericPlayerBehaviour genericPlayerBehaviourScript)
    {
        ballControllerScript.BallHeight = BallHeightEnum.NONE;
        ballGameObject.transform.position = genericPlayerBehaviourScript.gameObject.transform.position;
        ballGameObject.transform.rotation = Quaternion.identity;
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

    public GameObject BallGameObject { get => ballGameObject; set => ballGameObject = value; }
    public BallController BallControllerScript { get => ballControllerScript; set => ballControllerScript = value; }
    public GameObject PitcherGameObject { get => pitcherGameObject; set => pitcherGameObject = value; }
    public TargetSelectionManager TargetSelectionManager { get => targetSelectionManager; set => targetSelectionManager = value; }
}
