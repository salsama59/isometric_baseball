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

        if (this.HasBallCollided(collision.collider))
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
        else if (this.HasPlayerCollided(collision))
        {
            if (PlayerUtils.HasFielderPosition(this.gameObject) && genericPlayerBehaviourScript.IsHoldingBall && PlayerUtils.IsCurrentPlayerPosition(collision.gameObject, PlayerFieldPositionEnum.RUNNER))
            {
                PlayerStatus runnerToTagOutStatus = PlayerUtils.FetchPlayerStatusScript(collision.transform.gameObject);
                RunnerBehaviour runnerBehaviourScript = ((RunnerBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(collision.transform.gameObject, runnerToTagOutStatus));

                if (!runnerBehaviourScript.IsSafe)
                {
                    ((FielderBehaviour)genericPlayerBehaviourScript).TagOutRunner(collision.transform.gameObject);
                }
            }
        }
        else
        {
            if (PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.RUNNER))
            {
                
                if (!IsBaseTile(collision.gameObject.name))
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
                    PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                    playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
                    PlayersTurnManager.IsCommandPhase = true;
                }
                else if(baseReached == BaseEnum.HOME_BASE)
                {
                    //automaticaly run to next base, no need for command input
                    runnerBehaviour.CalculateRunnerColliderInterraction(FieldUtils.GetTileEnumFromName(collision.gameObject.name), false);
                }
                else
                {
                    //Runner next turn
                    PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                    playersTurnManager.TurnState = TurnStateEnum.RUNNER_TURN;
                    PlayersTurnManager.IsCommandPhase = true;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.RUNNER) && IsBaseTile(collision.gameObject.name))
        {
            PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            RunnerBehaviour runnerBehaviourScript = ((RunnerBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(this.gameObject, playerStatusScript));
            runnerBehaviourScript.IsSafe = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.RUNNER) && IsBaseTile(collision.gameObject.name))
        {
            PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            RunnerBehaviour runnerBehaviourScript = ((RunnerBehaviour)PlayerUtils.FetchCorrespondingPlayerBehaviourScript(this.gameObject, playerStatusScript));
            runnerBehaviourScript.ToggleRunnerSafeStatus();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.HasBallCollided(collision))
        {
            GameObject ballGameObject = collision.gameObject;
            BallController ballControlerScript =  BallUtils.FetchBallControllerScript(ballGameObject);
            PlayerStatus currentPlayerStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            GenericPlayerBehaviour genericPlayerBehaviourScript = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(this.gameObject, currentPlayerStatus);

            if (ballControlerScript.IsMoving && ballControlerScript.IsHit && !ballControlerScript.IsPitched)
            {

                if (PlayerUtils.HasPitcherPosition(this.gameObject) && !ballControlerScript.IsTargetedByFielder)
                {
                    ballControlerScript.IsTargetedByPitcher = true;
                    ((PitcherBehaviour)genericPlayerBehaviourScript).CalculateFielderTriggerInterraction(ballGameObject, genericPlayerBehaviourScript, currentPlayerStatus);
                }

                if (PlayerUtils.HasFielderPosition(this.gameObject) && !ballControlerScript.IsTargetedByFielder && !ballControlerScript.IsTargetedByPitcher)
                {
                    GameObject nearestFielder = TeamUtils.GetNearestFielderFromGameObject(ballGameObject);
                    PlayerStatus nearestFielderStatus = PlayerUtils.FetchPlayerStatusScript(nearestFielder);

                    if (nearestFielderStatus.PlayerFieldPosition == currentPlayerStatus.PlayerFieldPosition)
                    {
                        ((FielderBehaviour)genericPlayerBehaviourScript).CalculateFielderTriggerInterraction(genericPlayerBehaviourScript);
                    }
                }
            }
                
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (this.HasBallCollided(collision))
        {
            GameObject ballGameObject = collision.gameObject;
            BallController ballControlerScript = BallUtils.FetchBallControllerScript(ballGameObject);

            if (!ballControlerScript.IsMoving && !ballControlerScript.IsHit && !ballControlerScript.IsPitched)
            {
                if (PlayerUtils.HasPitcherPosition(this.gameObject))
                {
                    ballControlerScript.IsTargetedByPitcher = false;
                }
            }
        }
    }

    private bool IsBaseTile(string tileName)
    {
        return tileName == NameConstants.HOME_BASE_NAME
            || tileName == NameConstants.FIRST_BASE_NAME
            || tileName == NameConstants.SECOND_BASE_NAME
            || tileName == NameConstants.THIRD_BASE_NAME;
    }

    private bool HasBallCollided(Collider2D collider2D)
    {
        return collider2D.transform.gameObject.CompareTag(TagsConstants.BALL_TAG);
    }

    private bool HasPlayerCollided(Collision2D collision2D)
    {
        return collision2D.collider.transform.gameObject.CompareTag(TagsConstants.BASEBALL_PLAYER_TAG);
    }
}
