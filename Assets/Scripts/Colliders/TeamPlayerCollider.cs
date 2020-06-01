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

            if (PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.CATCHER))
            {
                PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                playersTurnManager.turnState = TurnStateEnum.CATCHER_TURN;
                PlayersTurnManager.IsCommandPhase = true;
            }
            else if(PlayerUtils.HasFielderPosition(this.gameObject))
            {
                ((FielderBehaviour)genericPlayerBehaviourScript).CalculateFielderColliderInterraction(ballGameObject, ballControllerScript, genericPlayerBehaviourScript);
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

                //win a point automaticaly without issuing commands if arrived at home base after a complete turn
                if (baseReached == BaseEnum.HOME_BASE && runnerBehaviour.HasPassedThroughThreeFirstBases())
                {
                    runnerBehaviour.CalculateRunnerColliderInterraction(FieldUtils.GetTileEnumFromName(collision.gameObject.name), true);
                }
                else
                {
                    PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                    playersTurnManager.turnState = TurnStateEnum.RUNNER_TURN;
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
            runnerBehaviourScript.ToggleRunnerSafeStatus();
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
            PlayerStatus currentFielderStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            GenericPlayerBehaviour genericPlayerBehaviourScript = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(this.gameObject, currentFielderStatus);
            if (!ballControlerScript.IsMoving && !ballControlerScript.IsHit && !ballControlerScript.IsPitched && !ballControlerScript.IsTargetedByFielder && PlayerUtils.HasFielderPosition(this.gameObject))
            {
                GameObject nearestFielder = TeamUtils.GetNearestFielderFromBall(ballGameObject);
                PlayerStatus nearestFielderStatus = PlayerUtils.FetchPlayerStatusScript(nearestFielder);

                if (nearestFielderStatus.PlayerFieldPosition == currentFielderStatus.PlayerFieldPosition)
                {
                    ((FielderBehaviour)genericPlayerBehaviourScript).CalculateFielderTriggerInterraction(ballGameObject, genericPlayerBehaviourScript, currentFielderStatus);
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
