﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap fieldTileMap = null;
    public Tilemap baseTileMap = null;
    public List<Tile> tilePool;
    public GameObject playerModel = null;

    private Dictionary<string, TileTypeEnum> horizontalyPaintedDirtDictionary = new Dictionary<string, TileTypeEnum>();
    private Dictionary<string, TileTypeEnum> verticalyPaintedDirtDictionary = new Dictionary<string, TileTypeEnum>();

    void Start()
    {
        this.BuildGameField();
        this.SetPlayersLocation(PlayerEnum.PLAYER_1);
    }

    private void SetPlayersLocation(PlayerEnum playerId)
    {
        foreach (KeyValuePair<PlayerFieldPositionEnum, Vector3> entry in TeamUtils.playerTeamMenberPositionLocation)
        {
            GameObject player = Instantiate(playerModel, entry.Value, Quaternion.identity);
            PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(player);
            playerStatus.PlayerFieldPosition = entry.Key;
            playerStatus.IsAllowedToMove = PlayerUtils.IsPlayerAllowedToMove(player);

            switch (playerStatus.PlayerFieldPosition)
            {
                case PlayerFieldPositionEnum.BATTER:
                    playerStatus.BattingEfficiency = 10f;
                    playerStatus.BattingPower = 5;
                    break;
                case PlayerFieldPositionEnum.PITCHER:
                    playerStatus.PitchEfficiency = 50f;
                    playerStatus.PitchingPower = 2;
                    playerStatus.PitchingEffect = 10f;
                    break;
                case PlayerFieldPositionEnum.RUNNER:
                    playerStatus.Speed = 2f;
                    break;
                case PlayerFieldPositionEnum.CATCHER:
                    playerStatus.CatchEfficiency = 100f;
                    break;
            }
            TeamUtils.AddPlayerTeamMember(entry.Key, player, playerId);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void BuildGameField()
    {
        this.CalculateHorizontalyPaintedDirtPositions(HorizontalyPaintedDirtDictionary, FieldUtils.GetPrimeBaseTilePosition(), FieldUtils.GetSecondBaseTilePosition(), TileTypeEnum.HORIZONTAL_PAINTED_DIRT);
        this.CalculateVerticalyPaintedDirtPositions(VerticalyPaintedDirtDictionary, FieldUtils.GetSecondBaseTilePosition(), FieldUtils.GetThirdBaseTilePosition(), TileTypeEnum.VERTICAL_PAINTED_DIRT);
        this.CalculateHorizontalyPaintedDirtPositions(HorizontalyPaintedDirtDictionary, FieldUtils.GetThirdBaseTilePosition(), FieldUtils.GetFourthBaseTilePosition(), TileTypeEnum.HORIZONTAL_PAINTED_DIRT);
        this.CalculateVerticalyPaintedDirtPositions(VerticalyPaintedDirtDictionary, FieldUtils.GetFourthBaseTilePosition(), FieldUtils.GetPrimeBaseTilePosition(), TileTypeEnum.VERTICAL_PAINTED_DIRT);

        for (int collumn = fieldTileMap.cellBounds.xMin; collumn < fieldTileMap.cellBounds.xMax; collumn++)
        {
            for (int line = fieldTileMap.cellBounds.yMin; line < fieldTileMap.cellBounds.yMax; line++)
            {
                Vector3Int localPlace = new Vector3Int(collumn, line, (int)fieldTileMap.transform.position.y);
                if (fieldTileMap.HasTile(localPlace))
                {
                    TileInformation tileInformation = this.GetFieldTileInformation(localPlace, fieldTileMap, collumn, line);
                    int tilePoolIndex = tileInformation.TileIndex;
                    fieldTileMap.SetTile(localPlace, tilePool[tilePoolIndex]);
                    TileBase tileBase = fieldTileMap.GetTile(localPlace);
                    tileBase.name = tileInformation.TileName;
                }
                else
                {
                    fieldTileMap.SetTile(localPlace, null);
                }
            }
        }

        for (int collumn = baseTileMap.cellBounds.xMin; collumn < baseTileMap.cellBounds.xMax; collumn++)
        {
            for (int line = baseTileMap.cellBounds.yMin; line < baseTileMap.cellBounds.yMax; line++)
            {
                Vector3Int localPlace = new Vector3Int(collumn, line, (int)baseTileMap.transform.position.y);
                if (baseTileMap.HasTile(localPlace))
                {
                    TileInformation tileInformation = this.GetBaseTileInformation(localPlace, baseTileMap, collumn, line);

                    if (tileInformation != null)
                    {
                        int tilePoolIndex = tileInformation.TileIndex;
                        baseTileMap.SetTile(localPlace, tilePool[tilePoolIndex]);
                        TileBase tileBase = baseTileMap.GetTile(localPlace);
                        tileBase.name = tileInformation.TileName;
                        if (tilePoolIndex == (int)TileTypeEnum.BASE)
                        {
                            baseTileMap.SetColliderType(localPlace, Tile.ColliderType.Sprite);
                        }
                    }
                    else
                    {
                        baseTileMap.SetTile(localPlace, null);
                    }
                }
            }
        }
    }

    private TileInformation GetFieldTileInformation(Vector3Int localPlace, Tilemap tilemap, int collumn, int line)
    {
        //float camHeight = this.GetCamHeight()/2;
        //float camWidth = this.GetCamWidth()/2;
        TileInformation tileInformation = new TileInformation();
        tileInformation.TileIndex = (int)TileTypeEnum.GRASS;

        string dictionarykey = this.BuildKey(collumn, line);

        if (HorizontalyPaintedDirtDictionary.ContainsKey(dictionarykey))
        {
            tileInformation.TileIndex = (int)HorizontalyPaintedDirtDictionary[dictionarykey];
            return tileInformation;
        }

        if (VerticalyPaintedDirtDictionary.ContainsKey(dictionarykey))
        {
            tileInformation.TileIndex = (int)VerticalyPaintedDirtDictionary[dictionarykey];
            return tileInformation;
        }

        //Camera cam = Camera.main;

        //Vector3 camPosition = cam.transform.position;

        //Vector3 place = tileMap.CellToWorld(localPlace);



        /*if (place.x < camPosition.x + camWidth && place.x > camPosition.x - camWidth)
        {
            if(place.y < camPosition.y + camHeight && place.y > camPosition.y - camHeight)
            {
                return (int)TileType.ROAD;
            }
        }*/

        /*if (collumn == tilemap.cellBounds.xMin || collumn == tilemap.cellBounds.xMax - 1 || line == tilemap.cellBounds.yMin || line == tilemap.cellBounds.yMax - 1)
        {
            return (int)TileType.ROAD;
        }
        else
        {
            return (int)TileType.GRASS;
        }*/

        return tileInformation;
    }

    private TileInformation GetBaseTileInformation(Vector3Int localPlace, Tilemap tilemap, int collumn, int line)
    {

        TileInformation tileInformation = new TileInformation();
        Vector2Int currentTileCoordinates = new Vector2Int(localPlace.x, localPlace.y);
        Vector2Int middleBaseTilePosition = FieldUtils.GetMiddleBaseTilePosition();
        Vector2Int primeBaseTilePosition = FieldUtils.GetPrimeBaseTilePosition();
        Vector2Int secondBaseTilePosition = FieldUtils.GetSecondBaseTilePosition();
        Vector2Int thirdBaseTilePosition = FieldUtils.GetThirdBaseTilePosition();
        Vector2Int fourthBaseTilePosition = FieldUtils.GetFourthBaseTilePosition();

        bool isMiddleBasePosition = this.CompareTilePosition(middleBaseTilePosition, currentTileCoordinates);
        bool isPrimeBasePosition = this.CompareTilePosition(primeBaseTilePosition, currentTileCoordinates);
        bool isSecondBasePosition = this.CompareTilePosition(secondBaseTilePosition, currentTileCoordinates);
        bool isThirdBasePosition = this.CompareTilePosition(thirdBaseTilePosition, currentTileCoordinates);
        bool isfourthBasePosition = this.CompareTilePosition(fourthBaseTilePosition, currentTileCoordinates);

        if (isMiddleBasePosition)
        {
            tileInformation.TileName = "";
        }
        else if (isPrimeBasePosition)
        {
            tileInformation.TileName = NameConstants.PRIME_BASE_NAME;
        }
        else if (isSecondBasePosition)
        {
            tileInformation.TileName = NameConstants.SECOND_BASE_NAME;
        }
        else if (isThirdBasePosition)
        {
            tileInformation.TileName = NameConstants.THIRD_BASE_NAME;
        }
        else if (isfourthBasePosition)
        {
            tileInformation.TileName = NameConstants.FOURTH_BASE_NAME;
        }

        if (isMiddleBasePosition || isPrimeBasePosition || isSecondBasePosition || isThirdBasePosition || isfourthBasePosition)
        {
            tileInformation.TileIndex = (int)TileTypeEnum.BASE;
            return tileInformation;
        }

        return null;
    }

    private int GetLineCorespondingToCollum(Vector2Int firstBasePosition, Vector2Int secondBasePosition, int collumn)
    {
        int slopeNumerator = (firstBasePosition.y - secondBasePosition.y);
        float slopeDenominator = (float)(firstBasePosition.x - secondBasePosition.x);

        float slope = 0;

        if (slopeDenominator != 0 || slopeDenominator != 0)
        {
            slope = slopeNumerator / slopeDenominator;
        }

        float origin = firstBasePosition.y - slope * firstBasePosition.x;

        if (slope == 0)
        {
            return Mathf.RoundToInt(origin);
        }
        else
        {
            return Mathf.RoundToInt(slope * collumn + origin);
        }
    }

    private int GetCollumnCorespondingToLine(Vector2Int firstBasePosition, Vector2Int secondBasePosition, int line)
    {
        int slopeNumerator = (firstBasePosition.y - secondBasePosition.y);
        float slopeDenominator = (float)(firstBasePosition.x - secondBasePosition.x);

        float slope = 0;

        if (slopeDenominator != 0 || slopeDenominator != 0)
        {
            slope = slopeNumerator / slopeDenominator;
        }

        float origin = firstBasePosition.y - slope * firstBasePosition.x;

        if(slope == 0)
        {
            return Mathf.RoundToInt(origin);
        }
        else
        {
            return Mathf.RoundToInt((line - origin) / slope);
        }
    }

    private void CalculateHorizontalyPaintedDirtPositions(Dictionary<string, TileTypeEnum> paintedDirtDictionary, Vector2Int firstBasePosition, Vector2Int secondBasePosition, TileTypeEnum tileType)
    {
        if(firstBasePosition.x < secondBasePosition.x)
        {
            for (int collumn = firstBasePosition.x; collumn < secondBasePosition.x; collumn++)
            {
                int line = this.GetLineCorespondingToCollum(firstBasePosition, secondBasePosition, collumn);
                string key = this.BuildKey(collumn, line);
                if (!paintedDirtDictionary.ContainsKey(key))
                {
                    paintedDirtDictionary.Add(key, tileType);
                }
            }
        }
        else
        {
            for (int collumn = firstBasePosition.x; collumn > secondBasePosition.x; collumn--)
            {
                int line = this.GetLineCorespondingToCollum(firstBasePosition, secondBasePosition, collumn);
                string key = this.BuildKey(collumn, line);
                if (!paintedDirtDictionary.ContainsKey(key))
                {
                    paintedDirtDictionary.Add(key, tileType);
                }
            }
        }
        
    }

    private void CalculateVerticalyPaintedDirtPositions(Dictionary<string, TileTypeEnum> paintedDirtDictionary, Vector2Int firstBasePosition, Vector2Int secondBasePosition, TileTypeEnum tileType)
    {
        if(firstBasePosition.y < secondBasePosition.y)
        {
            for (int line = firstBasePosition.y; line < secondBasePosition.y; line++)
            {
                int collumn = this.GetCollumnCorespondingToLine(firstBasePosition, secondBasePosition, line);
                string key = this.BuildKey(collumn, line);
                if (!paintedDirtDictionary.ContainsKey(key))
                {
                    paintedDirtDictionary.Add(key, tileType);
                }
            }
        }
        else
        {
            for (int line = firstBasePosition.y; line > secondBasePosition.y; line--)
            {
                int collumn = this.GetCollumnCorespondingToLine(firstBasePosition, secondBasePosition, line);
                string key = this.BuildKey(collumn, line);

                if (!paintedDirtDictionary.ContainsKey(key))
                {
                    paintedDirtDictionary.Add(key, tileType);
                }
            }
        }
        
    }

    private string BuildKey(int collumn, int line)
    {
        return "x:" + collumn + "y:" + line;
    }

    private bool CompareTilePosition(Vector2Int firstTilePosition, Vector2Int secondTilePosition)
    {
        bool isXpositionEquals = firstTilePosition.x == secondTilePosition.x;
        bool isYpositionEquals = firstTilePosition.y == secondTilePosition.y;

        return isXpositionEquals && isYpositionEquals;
    }

    private float GetCamHeight()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        return height;
    }

    private float GetCamWidth()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        return width;
    }

    public Dictionary<string, TileTypeEnum> HorizontalyPaintedDirtDictionary { get => horizontalyPaintedDirtDictionary; set => horizontalyPaintedDirtDictionary = value; }
    public Dictionary<string, TileTypeEnum> VerticalyPaintedDirtDictionary { get => verticalyPaintedDirtDictionary; set => verticalyPaintedDirtDictionary = value; }

}
