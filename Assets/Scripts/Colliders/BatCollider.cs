﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.HasBallCollided(collision))
        {
            PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
            playersTurnManager.TurnState = TurnStateEnum.BATTER_TURN;
            PlayersTurnManager.IsCommandPhase = true;
        }
    }

    private bool HasBallCollided(Collider2D collider2D)
    {
        return collider2D.transform.gameObject.CompareTag(TagsConstants.BALL_TAG);
    }
}
