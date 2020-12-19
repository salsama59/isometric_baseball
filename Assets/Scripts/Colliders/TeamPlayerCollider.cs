using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeamPlayerCollider : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
        GenericPlayerBehaviour genericPlayerBehaviourScript = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(this.gameObject, playerStatusScript);

        if (ColliderUtils.HasBallCollided(collision.collider))
        {
            GameObject ballGameObject = collision.collider.gameObject;
            BallController ballControllerScript = BallUtils.FetchBallControllerScript(ballGameObject);

            if (PlayerUtils.HasCatcherPosition(this.gameObject) && ballControllerScript.CurrentPasser != this.gameObject)
            {
                CatcherBehaviour catcherBehaviour = (CatcherBehaviour)genericPlayerBehaviourScript;
                if (catcherBehaviour.CatcherMode == ModeConstants.CATCHER_FIELDER_MODE)
                {
                    PlayerActionsManager.InterceptBall(ballGameObject, ballControllerScript, genericPlayerBehaviourScript);
                    catcherBehaviour.CatcherMode = ModeConstants.CATCHER_NORMAL_MODE;
                }

                this.gameObject.transform.position = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCatcherZonePosition());
                catcherBehaviour.IsoRenderer.ReinitializeAnimator();

                PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                playersTurnManager.TurnState = TurnStateEnum.CATCHER_TURN;
                PlayersTurnManager.IsCommandPhase = true;
            }
            else if(PlayerUtils.HasFielderPosition(this.gameObject) && !ballControllerScript.IsPitched && ballControllerScript.CurrentPasser != this.gameObject)
            {
                ((FielderBehaviour)genericPlayerBehaviourScript).CalculateFielderColliderInterraction(ballGameObject, ballControllerScript, genericPlayerBehaviourScript);
            }
            else if (PlayerUtils.HasPitcherPosition(this.gameObject) && !ballControllerScript.IsPitched && !ballControllerScript.IsPassed && ballControllerScript.CurrentPasser != this.gameObject)
            {
                ((PitcherBehaviour)genericPlayerBehaviourScript).CalculatePitcherColliderInterraction(ballGameObject, ballControllerScript, genericPlayerBehaviourScript);
            }
        }
        else if (ColliderUtils.HasPlayerCollided(collision))
        {
            if (PlayerUtils.HasFielderPosition(this.gameObject) && genericPlayerBehaviourScript.IsHoldingBall && PlayerUtils.HasRunnerPosition(collision.gameObject))
            {
                PlayerStatus runnerToTagOutStatus = PlayerUtils.FetchPlayerStatusScript(collision.transform.gameObject);
                RunnerBehaviour runnerBehaviourScript = ((RunnerBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(collision.transform.gameObject, runnerToTagOutStatus));

                if (!runnerBehaviourScript.IsSafe)
                {
                    ((FielderBehaviour)genericPlayerBehaviourScript).TagOutRunner(collision.transform.gameObject);
                }
                else
                {
                    ((FielderBehaviour)genericPlayerBehaviourScript).ReplanAction();
                }
            }
        }
        else
        {
            if (PlayerUtils.HasRunnerPosition(this.gameObject))
            {
                
                if (!ColliderUtils.IsBaseTile(collision.gameObject.name))
                {
                    return;
                }

                if (genericPlayerBehaviourScript == null)
                {
                    return;
                }

                if (!genericPlayerBehaviourScript.IsPrepared)
                {
                    return;
                }

                RunnerBehaviour runnerBehaviour = ((RunnerBehaviour)genericPlayerBehaviourScript);
                BaseEnum baseReached = runnerBehaviour.NextBase;

                
                if (baseReached == BaseEnum.HOME_BASE && runnerBehaviour.HasPassedThroughThreeFirstBases())
                {
                    //win a point automaticaly without issuing commands if arrived at home base after a complete turn
                    runnerBehaviour.CalculateRunnerColliderInterraction(FieldUtils.GetTileEnumFromName(collision.gameObject.name), true);
                }
                else if(baseReached == BaseEnum.FIRST_BASE && runnerBehaviour.IsInWalkState)
                {
                    //Walk done after 4 ball from pitcher
                    runnerBehaviour.CalculateRunnerColliderInterraction(FieldUtils.GetTileEnumFromName(collision.gameObject.name));
                    PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                    playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
                    PlayersTurnManager.IsCommandPhase = true;
                }
                else if(baseReached == BaseEnum.HOME_BASE)
                {
                    //automaticaly run to next base, no need for command input
                    runnerBehaviour.CalculateRunnerColliderInterraction(FieldUtils.GetTileEnumFromName(collision.gameObject.name));
                    runnerBehaviour.GoToNextBase(baseReached, true);
                }
                else
                {
                    //Runner next turn
                    runnerBehaviour.CalculateRunnerColliderInterraction(FieldUtils.GetTileEnumFromName(collision.gameObject.name), true);
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (PlayerUtils.HasRunnerPosition(this.gameObject) && ColliderUtils.IsBaseTile(collision.gameObject.name))
        {
            PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            RunnerBehaviour runnerBehaviourScript = ((RunnerBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(this.gameObject, playerStatusScript));
            runnerBehaviourScript.IsSafe = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (PlayerUtils.HasRunnerPosition(this.gameObject) && ColliderUtils.IsBaseTile(collision.gameObject.name))
        {
            PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            RunnerBehaviour runnerBehaviourScript = ((RunnerBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(this.gameObject, playerStatusScript));
            runnerBehaviourScript.ToggleRunnerSafeStatus();
        }
    }
}
