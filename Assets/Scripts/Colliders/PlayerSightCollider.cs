using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSightCollider : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (ColliderUtils.HasBallCollided(collision))
        {
            GameObject player = this.gameObject.transform.parent.parent.parent.gameObject;
            GameObject ballGameObject = collision.gameObject;
            BallController ballControlerScript = BallUtils.FetchBallControllerScript(ballGameObject);
            PlayerStatus currentPlayerStatus = PlayerUtils.FetchPlayerStatusScript(player);
            GenericPlayerBehaviour genericPlayerBehaviourScript = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(player, currentPlayerStatus);

            if (ballControlerScript.IsMoving && ballControlerScript.IsHit && !ballControlerScript.IsPitched)
            {

                if (PlayerUtils.HasPitcherPosition(player) && !ballControlerScript.IsTargetedByFielder)
                {
                    ballControlerScript.IsTargetedByPitcher = true;
                    ((PitcherBehaviour)genericPlayerBehaviourScript).CalculatePitcherTriggerInterraction(ballGameObject, genericPlayerBehaviourScript, currentPlayerStatus);
                }

                if (PlayerUtils.HasFielderPosition(player) && !ballControlerScript.IsTargetedByFielder && !ballControlerScript.IsTargetedByPitcher)
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
        if (ColliderUtils.HasBallCollided(collision))
        {
            GameObject ballGameObject = collision.gameObject;
            BallController ballControlerScript = BallUtils.FetchBallControllerScript(ballGameObject);

            if (ballControlerScript.IsMoving && ballControlerScript.IsHit && !ballControlerScript.IsPitched)
            {
                GameObject player = this.gameObject.transform.parent.parent.parent.gameObject;
                if (PlayerUtils.HasPitcherPosition(player))
                {
                    ballControlerScript.IsTargetedByPitcher = false;
                    PitcherBehaviour pitcherBehaviour = PlayerUtils.FetchPitcherBehaviourScript(player);
                    pitcherBehaviour.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetPitcherBaseTilePosition());
                    pitcherBehaviour.HasSpottedBall = false;
                }
            }
        }
    }
}
