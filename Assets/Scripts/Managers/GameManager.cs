using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap fieldTileMap = null;
    public Tilemap baseTileMap = null;
    private List<Vector3> availablePlaces;
    public List<Tile> tilePool;
    public enum TileType{
        GRASS,
        DIRT,
        HORIZONTAL_PAINTED_DIRT,
        BASE,
        VERTICAL_PAINTED_DIRT
    };

    private Dictionary<string, TileType> horizontalyPaintedDirtDictionary = new Dictionary<string, TileType>();
    private Dictionary<string, TileType> verticalyPaintedDirtDictionary = new Dictionary<string, TileType>();

    void Start()
    {

        this.CalculateHorizontalyPaintedDirtPositions(HorizontalyPaintedDirtDictionary, FieldUtils.GetPrimeBaseTilePosition(), FieldUtils.GetSecondBaseTilePosition(), TileType.HORIZONTAL_PAINTED_DIRT);
        this.CalculateVerticalyPaintedDirtPositions(VerticalyPaintedDirtDictionary, FieldUtils.GetSecondBaseTilePosition(), FieldUtils.GetThirdBaseTilePosition(), TileType.VERTICAL_PAINTED_DIRT);
        this.CalculateHorizontalyPaintedDirtPositions(HorizontalyPaintedDirtDictionary, FieldUtils.GetThirdBaseTilePosition(), FieldUtils.GetFourthBaseTilePosition(), TileType.HORIZONTAL_PAINTED_DIRT);
        this.CalculateVerticalyPaintedDirtPositions(VerticalyPaintedDirtDictionary, FieldUtils.GetFourthBaseTilePosition(), FieldUtils.GetPrimeBaseTilePosition(), TileType.VERTICAL_PAINTED_DIRT);

        availablePlaces = new List<Vector3>();

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
                        if (tilePoolIndex == (int)TileType.BASE)
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
        tileInformation.TileIndex = (int)TileType.GRASS;

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
            tileInformation.TileIndex = (int)TileType.BASE;
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

    private void CalculateHorizontalyPaintedDirtPositions(Dictionary<string, TileType> paintedDirtDictionary, Vector2Int firstBasePosition, Vector2Int secondBasePosition, TileType tileType)
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

    private void CalculateVerticalyPaintedDirtPositions(Dictionary<string, TileType> paintedDirtDictionary, Vector2Int firstBasePosition, Vector2Int secondBasePosition, TileType tileType)
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

    // Update is called once per frame
    void Update()
    {

    }

    private void CalculateUsedTiles()
    {
        for (int n = fieldTileMap.cellBounds.xMin; n < fieldTileMap.cellBounds.xMax; n++)
        {
            for (int p = fieldTileMap.cellBounds.yMin; p < fieldTileMap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)fieldTileMap.transform.position.y));
                Vector3 place = fieldTileMap.CellToWorld(localPlace);
                if (fieldTileMap.HasTile(localPlace))
                {
                    //Tile at "place"
                    availablePlaces.Add(place);
                }
                else
                {
                    //No tile at "place"
                }
            }
        }
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

    public Dictionary<string, TileType> HorizontalyPaintedDirtDictionary { get => horizontalyPaintedDirtDictionary; set => horizontalyPaintedDirtDictionary = value; }
    public Dictionary<string, TileType> VerticalyPaintedDirtDictionary { get => verticalyPaintedDirtDictionary; set => verticalyPaintedDirtDictionary = value; }

}
