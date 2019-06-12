using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeamPlayerCollider : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
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

        GenericCharacterController genericCharacterController = PlayerUtils.FetchCharacterControllerScript(this.gameObject);

        if (!genericCharacterController.IsPrepared)
        {
            return;
        }

        genericCharacterController.PlayerBehaviour.CalculateNextAction();
    }

    private bool IsBaseTile(string tileName)
    {
        return tileName == NameConstants.PRIME_BASE_NAME
            || tileName == NameConstants.SECOND_BASE_NAME
            || tileName == NameConstants.THIRD_BASE_NAME
            || tileName == NameConstants.FOURTH_BASE_NAME;
    }
}
