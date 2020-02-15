using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersTurnManager : MonoBehaviour
{

    public TurnStateEnum turnState;
    private GameObject ball;
    public static bool IsCommandPhase;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {

            BallController ballControllerScript = BallUtils.FetchBallControllerScript(ball);
            GameObject pitcherGameObject = ballControllerScript.CurrentPitcher;

            switch (this.turnState)
            {
                case TurnStateEnum.STANDBY:
                    break;
                case TurnStateEnum.PITCHER_TURN:
                    Debug.Log("Activate pitcher action");
                    //PITCHER TURN
                    //literaly throw the ball!!!
                    ball.SetActive(true);
                    ballControllerScript.IsThrown = true;
                    break;
                case TurnStateEnum.BATTER_TURN:
                    GameObject batter = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.BATTER, PlayerEnum.PLAYER_1);
                    BatterBehaviour batterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(batter);
                    Debug.Log("Activate batter action");

                    batterBehaviourScript.IsReadyToSwing = true;
                    batterBehaviourScript.IsSwingHasFinished = false;
                    //BATTER TURN
                    if (PlayerUtils.IsCurrentPlayerPosition(batter, PlayerFieldPositionEnum.BATTER))
                    {
                        PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(batter);
                        batterBehaviourScript.CalculateBatterColliderInterraction(pitcherGameObject, ballControllerScript, playerStatusScript);
                    }
                    break;
                case TurnStateEnum.RUNNER_TURN:
                    //RUNNER TURN

                    Debug.Log("Runner choice!!!!");
                    GameObject runner = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.RUNNER, PlayerEnum.PLAYER_1);
                    PlayerStatus runnerStatusScript = PlayerUtils.FetchPlayerStatusScript(runner);
                    GenericPlayerBehaviour genericRunnerBehaviourScript = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(runner, runnerStatusScript);
                    RunnerBehaviour runnerBehaviour = ((RunnerBehaviour)genericRunnerBehaviourScript);
                    BaseEnum baseEnum = runnerBehaviour.NextBase;
                    runnerBehaviour.CalculateRunnerColliderInterraction(baseEnum);
                    break;
                case TurnStateEnum.CATCHER_TURN:
                    //CATCHER TURN
                    GameObject catcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.CATCHER, PlayerEnum.PLAYER_1);
                    PlayerStatus catcherStatusScript = PlayerUtils.FetchPlayerStatusScript(catcher);
                    GenericPlayerBehaviour genericCatcherBehaviourScript = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(catcher, catcherStatusScript);
                    ((CatcherBehaviour)genericCatcherBehaviourScript).CalculateCatcherColliderInterraction(pitcherGameObject, ball, ballControllerScript);
                    break;
                default:
                    break;
            }

            PlayersTurnManager.IsCommandPhase = false;

        }
    }


    public GameObject Ball { get => ball; set => ball = value; }
}
