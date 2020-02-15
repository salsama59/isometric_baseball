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
            //GameObject pitcherGameObject = ballControllerScript.CurrentPitcher;

            if (PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.CATCHER))
            {
                PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                playersTurnManager.turnState = TurnStateEnum.CATCHER_TURN;
                PlayersTurnManager.IsCommandPhase = true;
                //((CatcherBehaviour)genericPlayerBehaviourScript).CalculateCatcherColliderInterraction(pitcherGameObject, ballGameObject, ballControllerScript);
            }
            else if(PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.FIRST_BASEMAN) || PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.THIRD_BASEMAN))
            {
                ((FielderBehaviour)genericPlayerBehaviourScript).CalculateFielderColliderInterraction(ballGameObject, ballControllerScript, genericPlayerBehaviourScript);
            }
        }
        else if (this.HasPlayerCollided(collision))
        {
            return;
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
                    runnerBehaviour.CalculateRunnerColliderInterraction(FieldUtils.GetTileEnumFromName(collision.gameObject.name));
                }
                else
                {
                    PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                    playersTurnManager.turnState = TurnStateEnum.RUNNER_TURN;
                    PlayersTurnManager.IsCommandPhase = true;
                }

                //((RunnerBehaviour)genericPlayerBehaviourScript).CalculateRunnerColliderInterraction(FieldUtils.GetTileEnumFromName(collision.gameObject.name));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.HasBallCollided(collision))
        {
            GameObject ballGameObject = collision.gameObject;
            BallController ballControlerScript =  BallUtils.FetchBallControllerScript(ballGameObject);
            PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            GenericPlayerBehaviour genericPlayerBehaviourScript = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(this.gameObject, playerStatus);
            if (!ballControlerScript.IsMoving && PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.FIRST_BASEMAN) || !ballControlerScript.IsMoving && PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.THIRD_BASEMAN))
            {
                ((FielderBehaviour)genericPlayerBehaviourScript).CalculateFielderTriggerInterraction(ballGameObject, genericPlayerBehaviourScript, playerStatus);
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
