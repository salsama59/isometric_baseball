﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.HasBallCollided(collision))
        {
            Debug.Log("Ball has collide with the bat collider");
            //GameObject ballGameObject = collision.gameObject;
            //BallController ballControllerScript = BallUtils.FetchBallControllerScript(ballGameObject);
            //GameObject pitcherGameObject = ballControllerScript.CurrentPitcher;

            GameObject batter = this.gameObject.transform.parent.gameObject;
            BatterBehaviour batterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(batter);

            PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
            playersTurnManager.turnState = TurnStateEnum.BATTER_TURN;
            PlayersTurnManager.IsCommandPhase = true;

            //PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(batter);

            //batterBehaviourScript.IsReadyToSwing = true;
            //batterBehaviourScript.IsSwingHasFinished = false;

            /*if (PlayerUtils.IsCurrentPlayerPosition(batter, PlayerFieldPositionEnum.BATTER))
            {
                batterBehaviourScript.CalculateBatterColliderInterraction(pitcherGameObject, ballControllerScript, playerStatusScript);
            }*/
        }
    }

    private bool HasBallCollided(Collider2D collider2D)
    {
        return collider2D.transform.gameObject.CompareTag(TagsConstants.BALL_TAG);
    }
}
