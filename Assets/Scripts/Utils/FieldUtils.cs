using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldUtils
{
    public static float GRID_SIZE = 1f;

    public static Vector2Int GetPitcherBaseTilePosition()
    {
        return new Vector2Int(0, 0);
    }

    public static Vector2Int GetHomeBaseTilePosition()
    {
        Tilemap tileMap = FetchTileMap();
        int xTilePosition = (tileMap.cellBounds.xMin + 1) / 2;
        int yTilePosition = (tileMap.cellBounds.yMin + 1) / 2;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetFirstBaseTilePosition()
    {
        Tilemap tileMap = FetchTileMap();
        int xTilePosition = tileMap.cellBounds.xMax * 4 / 9;
        int yTilePosition = tileMap.cellBounds.yMin * 4 / 9;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetSecondBaseTilePosition()
    {
        Tilemap tileMap = FetchTileMap();
        int xTilePosition = tileMap.cellBounds.xMax * 4 / 9;
        int yTilePosition = tileMap.cellBounds.yMax * 1 / 2;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetThirdBaseTilePosition()
    {
        Tilemap tileMap = FetchTileMap();
        int xTilePosition = (tileMap.cellBounds.xMin + 1) / 2;
        int yTilePosition = tileMap.cellBounds.yMax * 1 / 2;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetCatcherZonePosition()
    {
        Tilemap tileMap = FetchTileMap();
        int xTilePosition = (tileMap.cellBounds.xMin - 3) / 2;
        int yTilePosition = (tileMap.cellBounds.yMin - 2) / 2;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetCathcherOutBallZonePosition()
    {
        Tilemap tileMap = FetchTileMap();
        int xTilePosition = tileMap.cellBounds.xMin;
        int yTilePosition = tileMap.cellBounds.yMin;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector3 GetTileCenterPositionInGameWorld(Vector2Int tilePositionInGrid)
    {
        Tilemap fieldTileMap = FetchTileMap();
        Vector3Int localPlace = new Vector3Int(tilePositionInGrid.x, tilePositionInGrid.y, (int)fieldTileMap.transform.position.y);
        return fieldTileMap.GetCellCenterWorld(localPlace);
    }

    public static Vector2Int GetGameObjectTilePositionOnField(GameObject gameObject)
    {
        Tilemap tileMap = FetchTileMap();
        Grid tileMapGrid = tileMap.layoutGrid;
        Vector3 gameObjectWorldPosition = gameObject.transform.position;
        Vector3Int cellPosition = tileMapGrid.WorldToCell(gameObjectWorldPosition);

        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

    public static Tilemap FetchTileMap()
    {
        GameManager gameManager = GameUtils.FetchGameManager();
        return gameManager.fieldTileMap;
    }
}
