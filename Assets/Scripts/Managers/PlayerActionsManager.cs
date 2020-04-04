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
        BallControllerScript.IsThrown = true;
    }

    public void CatchBallAction()
    {
        //CATCHER TURN
        GameObject catcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.CATCHER, PlayerEnum.PLAYER_1);
        PlayerStatus catcherStatusScript = PlayerUtils.FetchPlayerStatusScript(catcher);
        CatcherBehaviour genericCatcherBehaviourScript = ((CatcherBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(catcher, catcherStatusScript));
        genericCatcherBehaviourScript.CalculateCatcherColliderInterraction(PitcherGameObject, ballGameObject, ballControllerScript);
    }

    public void HitBallAction()
    {
        //BATTER TURN
        GameObject batter = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.BATTER, PlayerEnum.PLAYER_1);
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
        Debug.Log("Runner choice!!!!");
        GameObject runner = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.RUNNER, PlayerEnum.PLAYER_1);
        PlayerStatus runnerStatusScript = PlayerUtils.FetchPlayerStatusScript(runner);
        RunnerBehaviour genericRunnerBehaviourScript = ((RunnerBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(runner, runnerStatusScript));
        BaseEnum baseEnum = genericRunnerBehaviourScript.NextBase;
        genericRunnerBehaviourScript.CalculateRunnerColliderInterraction(baseEnum);
    }

    public GameObject BallGameObject { get => ballGameObject; set => ballGameObject = value; }
    public BallController BallControllerScript { get => ballControllerScript; set => ballControllerScript = value; }
    public GameObject PitcherGameObject { get => pitcherGameObject; set => pitcherGameObject = value; }
}
