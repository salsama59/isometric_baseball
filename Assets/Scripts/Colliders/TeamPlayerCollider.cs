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
            GameObject ballGameObject = collision.collider.gameObject;
            BallController ballControllerScript = BallUtils.FetchBallControllerScript(ballGameObject);
            GameObject pitcherGameObject = ballControllerScript.Pitcher;

            if (PlayerUtils.IsCorrespondingPlayerPosition(this.gameObject, PlayerFieldPositionEnum.BATTER))
            {
                
                float pitchSuccesRate = ActionCalculationUtils.CalculatePitchSuccessRate(pitcherGameObject, this.gameObject);
                Debug.Log("pitchSuccesRate = " + pitchSuccesRate);
                
                if (!ActionCalculationUtils.HasActionSucceeded(pitchSuccesRate))
                {
                    ballControllerScript.IsBeingHitten = true;
                    ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetSecondBaseTilePosition());
                    playerCharacterController.PlayerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(this.gameObject);
                    PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
                    playerStatusScript.IsAllowedToMove = true;
                    playerStatusScript.PlayerFieldPosition = PlayerFieldPositionEnum.RUNNER;
                }
                else
                {
                    ballControllerScript.IsThrown = true;
                    ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCatcherZonePosition());
                }
                
            }
            else if (PlayerUtils.IsCorrespondingPlayerPosition(this.gameObject, PlayerFieldPositionEnum.CATCHER))
            {
                float catchSuccesRate = ActionCalculationUtils.CalculateCatchSuccessRate(this.gameObject, pitcherGameObject);
                Debug.Log("catchSuccesRate = " + catchSuccesRate);
                if (!ActionCalculationUtils.HasActionSucceeded(catchSuccesRate))
                {
                    ballControllerScript.IsThrown = true;
                    ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCathcherOutBallZonePosition());
                }
                else
                {
                    Destroy(ballGameObject);
                }
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
