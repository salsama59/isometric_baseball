using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeamPlayerCollider : MonoBehaviour
{


    private bool IsBaseTile(string tileName)
    {
        return tileName == NameConstants.PRIME_BASE_NAME 
            || tileName == NameConstants.SECOND_BASE_NAME 
            || tileName == NameConstants.THIRD_BASE_NAME 
            || tileName == NameConstants.FOURTH_BASE_NAME;
    }

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

        GameObject playerGameObject = this.gameObject;

        GenericCharacterController genericCharacterController = playerGameObject.GetComponent<GenericCharacterController>();

        BaseEnum nextBase = genericCharacterController.NextBase;

        if (!genericCharacterController.IsPrepared)
        {
            return;
        }

        genericCharacterController.CurrentBase = nextBase;

        switch (nextBase)
        {
            case BaseEnum.PRIME_BASE:
                Debug.Log("Get on FIRST BASE");
                Debug.Log("WIN ONE POINT !!!");
                break;
            case BaseEnum.SECOND_BASE:
                Debug.Log("Get on SECOND BASE");
                genericCharacterController.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetThirdBaseTilePosition());
                genericCharacterController.NextBase = BaseEnum.THIRD_BASE;
                break;
            case BaseEnum.THIRD_BASE:
                Debug.Log("Get on THIRD BASE");
                genericCharacterController.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFourthBaseTilePosition());
                genericCharacterController.NextBase = BaseEnum.FOURTH_BASE;
                break;
            case BaseEnum.FOURTH_BASE:
                Debug.Log("Get on FOURTH BASE");
                genericCharacterController.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetPrimeBaseTilePosition());
                genericCharacterController.NextBase = BaseEnum.PRIME_BASE;
                break;
            default:
                Debug.Log("DO NOT KNOW WHAT HAPPEN");
                break;
        }
    }
}
