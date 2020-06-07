using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionsManager : MonoBehaviour
{

    private GameObject ballGameObject;
    private GameObject pitcherGameObject;
    private BallController ballControllerScript;

    public void ThrowBallAction()
    {
        Debug.Log("Activate pitcher action");
        //PITCHER TURN
        //literaly throw the ball!!!
        ballGameObject.SetActive(true);
        BallControllerScript.BallHeight = BallHeightEnum.LOW;
        BallControllerScript.IsPitched = true;
    }

    public void CatchBallAction()
    {
        //CATCHER TURN
        GameObject catcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.CATCHER, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.CATCHER));
        PlayerStatus catcherStatusScript = PlayerUtils.FetchPlayerStatusScript(catcher);
        CatcherBehaviour genericCatcherBehaviourScript = ((CatcherBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(catcher, catcherStatusScript));
        genericCatcherBehaviourScript.CalculateCatcherColliderInterraction(PitcherGameObject, ballGameObject, ballControllerScript);
    }

    public void HitBallAction()
    {
        //BATTER TURN
        GameObject batter = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.BATTER, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.BATTER));
        BatterBehaviour batterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(batter);
        Debug.Log("Activate batter action");

        batterBehaviourScript.IsReadyToSwing = true;
        batterBehaviourScript.IsSwingHasFinished = false;
        
        if (PlayerUtils.IsCurrentPlayerPosition(batter, PlayerFieldPositionEnum.BATTER))
        {
            PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(batter);
            batterBehaviourScript.CalculateBatterColliderInterraction(PitcherGameObject, BallControllerScript, playerStatusScript);
        }
    }

    public void RunAction()
    {
        //RUNNER TURN
        RunnerBehaviour genericRunnerBehaviourScript;
        BaseEnum baseReachedEnum = this.GetCurrentBaseReached(out genericRunnerBehaviourScript);
        genericRunnerBehaviourScript.CalculateRunnerColliderInterraction(baseReachedEnum, false);
    }

    public void PassAction()
    {
        //PITCHER TURN
        GameObject pitcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.PITCHER, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.PITCHER));
        PitcherBehaviour pitcherBehaviourScript =  PlayerUtils.FetchPitcherBehaviourScript(pitcher);
        GameObject nearestFielder = TeamUtils.GetNearestFielderFromGameObject(this.gameObject);
        ballGameObject.SetActive(true);
        BallControllerScript.BallHeight = BallHeightEnum.LOW;
        BallControllerScript.IsPitched = true;
        BallControllerScript.Target = nearestFielder.transform.position;
        BallControllerScript.EnableMovement = true;
        BallControllerScript.IsHeld = false;
        pitcherBehaviourScript.IsHoldingBall = false;
    }

    public void StayAction()
    {
        //RUNNER TURN
        RunnerBehaviour genericRunnerBehaviourScript;
        BaseEnum baseReachedEnum = this.GetCurrentBaseReached(out genericRunnerBehaviourScript);
        genericRunnerBehaviourScript.CalculateRunnerColliderInterraction(baseReachedEnum, true);
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
}
