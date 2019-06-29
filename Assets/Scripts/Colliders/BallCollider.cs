using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidedGameObject = collision.collider.gameObject;

        if (collidedGameObject.CompareTag(TagsConstants.BASEBALL_PLAYER_TAG))
        {
            BallController ballControllerScript = this.gameObject.GetComponent<BallController>();
            ballControllerScript.IsThrown = true;
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetSecondBaseTilePosition());
        }
    }
}
