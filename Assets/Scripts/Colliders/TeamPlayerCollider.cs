using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeamPlayerCollider : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {

        PlayerCharacterController playerCharacterController = PlayerUtils.FetchPlayerCharacterControllerScript(this.gameObject);

        if (HasBallCollided(collision))
        {
            if (PlayerUtils.IsCorrespondingPlayerPosition(this.gameObject, PlayerFieldPositionEnum.BATTER))
            {
                playerCharacterController.PlayerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(this.gameObject);
                PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
                playerStatusScript.IsAllowedToMove = true;
                playerStatusScript.PlayerFieldPosition = PlayerFieldPositionEnum.RUNNER;
            }
        }
        else
        {
            ContactPoint2D contactPoint2D = collision.GetContact(0);
            Tilemap currentTileMap = contactPoint2D.collider.GetComponent<Tilemap>();
            Grid tileMapGrid = currentTileMap.layoutGrid;
            Vector3 collisionContactPosition = contactPoint2D.collider.transform.position;
            Vector3Int cellPosition = tileMapGrid.WorldToCell(collisionContactPosition);

            TileBase currentCollidedTile = currentTileMap.GetTile(cellPosition);

            if (!IsBaseTile(currentCollidedTile.name))
            {
                return;
            }

            if (!playerCharacterController.IsPrepared)
            {
                return;
            }

            playerCharacterController.PlayerBehaviour.CalculateNextAction();
        }
        
    }

    private bool IsBaseTile(string tileName)
    {
        return tileName == NameConstants.PRIME_BASE_NAME
            || tileName == NameConstants.SECOND_BASE_NAME
            || tileName == NameConstants.THIRD_BASE_NAME
            || tileName == NameConstants.FOURTH_BASE_NAME;
    }

    private bool HasBallCollided(Collision2D collision2D)
    {
        return collision2D.collider.transform.gameObject.CompareTag(TagsConstants.BALL_TAG);
    }

    private bool HasPlayerCollided(Collision2D collision2D)
    {
        return collision2D.collider.transform.gameObject.CompareTag(TagsConstants.BASEBALL_PLAYER_TAG);
    }
}
