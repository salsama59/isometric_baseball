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
        int xTilePosition = GameManager.ColumnMinimum;
        int yTilePosition = GameManager.RowMinimum;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetFirstBaseTilePosition()
    {
        int xTilePosition = GameManager.ColumnMaximum;
        int yTilePosition = GameManager.RowMinimum;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetSecondBaseTilePosition()
    {
        int xTilePosition = GameManager.ColumnMaximum;
        int yTilePosition = GameManager.RowMaximum;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetThirdBaseTilePosition()
    {
        int xTilePosition = GameManager.ColumnMinimum;
        int yTilePosition = GameManager.RowMaximum;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetCatcherZonePosition()
    {
        int xTilePosition = GameManager.ColumnMinimum - (int)GRID_SIZE * 2;
        int yTilePosition = GameManager.RowMinimum - (int)GRID_SIZE * 2;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetBatterTilePosition()
    {
        int xTilePosition = GameManager.ColumnMinimum - (int)GRID_SIZE;
        int yTilePosition = GameManager.RowMinimum;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetSecondBasemanTilePosition()
    {
        int xTilePosition = GameManager.ColumnMaximum + (GameManager.ColumnMaximum/3);
        int yTilePosition = GameManager.RowMaximum - (GameManager.RowMaximum/3);
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetShortStopTilePosition()
    {
        int xTilePosition = GameManager.ColumnMaximum - (GameManager.ColumnMaximum / 3);
        int yTilePosition = GameManager.RowMaximum + (GameManager.RowMaximum / 3);
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetLeftFielderTilePosition()
    {
        int xTilePosition = 0;
        int yTilePosition = GameManager.RowMaximum * 2;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetRightFielderTilePosition()
    {
        int xTilePosition = GameManager.ColumnMaximum * 2;
        int yTilePosition = 0;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetCenterFielderTilePosition()
    {
        int xTilePosition = GameManager.ColumnMaximum * 2;
        int yTilePosition = GameManager.RowMaximum * 2;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector2Int GetCathcherOutBallZonePosition()
    {
        int xTilePosition = GameManager.ColumnMinimum - (int)GRID_SIZE * 3;
        int yTilePosition = GameManager.RowMinimum - (int)GRID_SIZE * 3;
        return new Vector2Int(xTilePosition, yTilePosition);
    }

    public static Vector3 GetTileCenterPositionInGameWorld(Vector2Int tilePositionInGrid)
    {
        Tilemap fieldTileMap = FetchFieldTileMap();
        Vector3Int localPlace = new Vector3Int(tilePositionInGrid.x, tilePositionInGrid.y, (int)fieldTileMap.transform.position.y);
        return fieldTileMap.GetCellCenterWorld(localPlace);
    }

    public static Vector3 GetTilePositionInGameWorld(Vector3Int tilePositionInGrid)
    {
        Tilemap fieldTileMap = FetchFieldTileMap();
        Vector3Int localPlace = new Vector3Int(tilePositionInGrid.x, tilePositionInGrid.y, (int)fieldTileMap.transform.position.y);
        return fieldTileMap.CellToWorld(localPlace);
    }

    public static Vector2Int GetGameObjectTilePositionOnField(GameObject gameObject)
    {
        Tilemap tileMap = FetchFieldTileMap();
        Grid tileMapGrid = tileMap.layoutGrid;
        Vector3 gameObjectWorldPosition = gameObject.transform.position;
        Vector3Int cellPosition = tileMapGrid.WorldToCell(gameObjectWorldPosition);

        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

    public static Vector3 GetGameObjectTileCenterPositionOnField(GameObject gameObject)
    {
        Tilemap tileMap = FetchFieldTileMap();
        Grid tileMapGrid = tileMap.layoutGrid;
        Vector3Int cellPosition = tileMapGrid.WorldToCell(gameObject.transform.position);
        Vector3 localCellCenter = tileMapGrid.GetCellCenterLocal(cellPosition);
        return localCellCenter;
    }

    public static Vector3 GetBatCorrectPosition(Vector3 position)
    {
        return new Vector3(position.x + GRID_SIZE/3, position.y + GRID_SIZE/4, position.z);
    }

    public static Tilemap FetchFieldTileMap()
    {
        GameManager gameManager = GameUtils.FetchGameManager();
        return gameManager.fieldTileMap;
    }

    public static BaseEnum GetTileEnumFromName(string tileName)
    {
        BaseEnum baseEnumValue = BaseEnum.HOME_BASE;

        if (tileName.Equals(NameConstants.HOME_BASE_NAME))
        {
            baseEnumValue = BaseEnum.HOME_BASE;
        }
        else if (tileName.Equals(NameConstants.FIRST_BASE_NAME))
        {
            baseEnumValue = BaseEnum.FIRST_BASE;
        }
        else if (tileName.Equals(NameConstants.SECOND_BASE_NAME))
        {
            baseEnumValue = BaseEnum.SECOND_BASE;
        }
        else if (tileName.Equals(NameConstants.THIRD_BASE_NAME))
        {
            baseEnumValue = BaseEnum.THIRD_BASE;
        }
        else if (tileName.Equals(NameConstants.PITCHER_BASE_NAME))
        {
            baseEnumValue = BaseEnum.PITCHER_BASE;
        }

        return baseEnumValue;
    }

    public static string GetBaseTileNameFromPosition(Vector2Int position)
    {
        string tileName = null;

        if (CompareTilePosition(GetHomeBaseTilePosition(), position))
        {
            tileName = NameConstants.HOME_BASE_NAME;
        }
        else if (CompareTilePosition(GetFirstBaseTilePosition(), position))
        {
            tileName = NameConstants.FIRST_BASE_NAME;
        }
        else if (CompareTilePosition(GetSecondBaseTilePosition(), position))
        {
            tileName = NameConstants.SECOND_BASE_NAME;
        }
        else if (CompareTilePosition(GetThirdBaseTilePosition(), position))
        {
            tileName = NameConstants.THIRD_BASE_NAME;
        }
        else if (CompareTilePosition(GetPitcherBaseTilePosition(), position))
        {
            tileName = NameConstants.PITCHER_BASE_NAME;
        }

        return tileName;
    }

    public static bool CompareTilePosition(Vector2Int firstTilePosition, Vector2Int secondTilePosition)
    {
        bool isXpositionEquals = firstTilePosition.x == secondTilePosition.x;
        bool isYpositionEquals = firstTilePosition.y == secondTilePosition.y;

        return isXpositionEquals && isYpositionEquals;
    }
}
