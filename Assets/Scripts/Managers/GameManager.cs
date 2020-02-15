using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private static int columnMaximum = 10;
    private static int columnMinimum = -10;
    private static int rowMinimum = -10;
    private static int rowMaximum = 10;
    public Tilemap fieldTileMap = null;
    public Tilemap baseTileMap = null;
    public List<Tile> tilePool;
    public GameObject playerModel = null;
    public GameObject ballModel = null;
    public GameObject batModel = null;
    public GameObject baseModel = null;

    private Dictionary<string, TileTypeEnum> horizontalyPaintedDirtDictionary = new Dictionary<string, TileTypeEnum>();
    private Dictionary<string, TileTypeEnum> verticalyPaintedDirtDictionary = new Dictionary<string, TileTypeEnum>();

    void Start()
    {
        this.BuildGameField();
        GameObject ball = Instantiate(ballModel, this.transform.position, this.transform.rotation);
        ball.SetActive(false);
        this.SetPlayersCharacteristics(PlayerEnum.PLAYER_1, ball);

        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        playersTurnManager.Ball = ball;
        PlayersTurnManager.IsCommandPhase = true;
        playersTurnManager.turnState = TurnStateEnum.PITCHER_TURN;

    }

    private void SetPlayersCharacteristics(PlayerEnum playerId, GameObject ball)
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
                    player.AddComponent<BatterBehaviour>();
                    GameObject bat = Instantiate(batModel, FieldUtils.GetBatCorrectPosition(entry.Value), Quaternion.Euler(0f, 0f, -70f));
                    bat.transform.parent = player.transform;
                    break;
                case PlayerFieldPositionEnum.PITCHER:
                    //ball.SetActive(true);
                    playerStatus.PitchEfficiency = 50f;
                    playerStatus.PitchingPower = 2;
                    playerStatus.PitchingEffect = 10f;
                    BallController ballControllerScript = BallUtils.FetchBallControllerScript(ball);
                    ballControllerScript.CurrentPitcher = player;
                    player.AddComponent<PitcherBehaviour>();
                    break;
                case PlayerFieldPositionEnum.RUNNER:
                    playerStatus.Speed = 2f;
                    player.AddComponent<RunnerBehaviour>();
                    break;
                case PlayerFieldPositionEnum.CATCHER:
                    playerStatus.CatchEfficiency = 100f;
                    player.AddComponent<CatcherBehaviour>();
                    break;
                case PlayerFieldPositionEnum.FIRST_BASEMAN:
                    playerStatus.CatchEfficiency = 80f;
                    playerStatus.Speed = 2f;
                    UpdatePlayerColliderSettings(player);
                    player.AddComponent<FielderBehaviour>();
                    break;
                case PlayerFieldPositionEnum.THIRD_BASEMAN:
                    playerStatus.CatchEfficiency = 80f;
                    playerStatus.Speed = 2f;
                    UpdatePlayerColliderSettings(player);
                    player.AddComponent<FielderBehaviour>();
                    break;
                case PlayerFieldPositionEnum.SECOND_BASEMAN:
                    playerStatus.CatchEfficiency = 80f;
                    playerStatus.Speed = 2f;
                    UpdatePlayerColliderSettings(player);
                    player.AddComponent<FielderBehaviour>();
                    break;
                case PlayerFieldPositionEnum.SHORT_STOP:
                    playerStatus.CatchEfficiency = 80f;
                    playerStatus.Speed = 2f;
                    UpdatePlayerColliderSettings(player);
                    player.AddComponent<FielderBehaviour>();
                    break;
                case PlayerFieldPositionEnum.LEFT_FIELDER:
                    playerStatus.CatchEfficiency = 80f;
                    playerStatus.Speed = 2f;
                    UpdatePlayerColliderSettings(player);
                    player.AddComponent<FielderBehaviour>();
                    break;
                case PlayerFieldPositionEnum.RIGHT_FIELDER:
                    playerStatus.CatchEfficiency = 80f;
                    playerStatus.Speed = 2f;
                    UpdatePlayerColliderSettings(player);
                    player.AddComponent<FielderBehaviour>();
                    break;
                case PlayerFieldPositionEnum.CENTER_FIELDER:
                    playerStatus.CatchEfficiency = 80f;
                    playerStatus.Speed = 2f;
                    UpdatePlayerColliderSettings(player);
                    player.AddComponent<FielderBehaviour>();
                    break;
            }

            player.name = playerStatus.PlayerFieldPosition.ToString();

            GenericPlayerBehaviour genericPlayerBehaviourScript = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(player, playerStatus);
            if (genericPlayerBehaviourScript != null)
            {
                genericPlayerBehaviourScript.FieldBall = ball;
            }
            

            TeamUtils.AddPlayerTeamMember(entry.Key, player, playerId);
        }
    }

    private void UpdatePlayerColliderSettings(GameObject player)
    {
        BoxCollider2D boxCollider = player.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector2(2, 15);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void BuildGameField()
    {
        this.CalculateHorizontalyPaintedDirtPositions(HorizontalyPaintedDirtDictionary, FieldUtils.GetHomeBaseTilePosition(), FieldUtils.GetFirstBaseTilePosition(), TileTypeEnum.HORIZONTAL_PAINTED_DIRT);
        this.CalculateVerticalyPaintedDirtPositions(VerticalyPaintedDirtDictionary, FieldUtils.GetFirstBaseTilePosition(), FieldUtils.GetSecondBaseTilePosition(), TileTypeEnum.VERTICAL_PAINTED_DIRT);
        this.CalculateHorizontalyPaintedDirtPositions(HorizontalyPaintedDirtDictionary, FieldUtils.GetSecondBaseTilePosition(), FieldUtils.GetThirdBaseTilePosition(), TileTypeEnum.HORIZONTAL_PAINTED_DIRT);
        this.CalculateVerticalyPaintedDirtPositions(VerticalyPaintedDirtDictionary, FieldUtils.GetThirdBaseTilePosition(), FieldUtils.GetHomeBaseTilePosition(), TileTypeEnum.VERTICAL_PAINTED_DIRT);

        for (int collumn = ColumnMinimum; collumn < ColumnMaximum + 1; collumn++)
        {
            for (int row = RowMinimum; row < RowMaximum + 1; row++)
            {
                Vector3Int localPlace = new Vector3Int(collumn, row, (int)fieldTileMap.transform.position.y);
               
                    TileInformation tileInformation = this.GetFieldTileInformation(localPlace, fieldTileMap, collumn, row);
                    int tilePoolIndex = tileInformation.TileIndex;
                    fieldTileMap.SetTile(localPlace, tilePool[tilePoolIndex]);
            }
        }

        for (int collumn = ColumnMinimum; collumn < ColumnMaximum + 1; collumn++)
        {
            for (int row = RowMinimum; row < RowMaximum + 1; row++)
            {
                Vector3Int localPlace = new Vector3Int(collumn, row, (int)baseTileMap.transform.position.y);
                TileInformation tileInformation = this.GetBaseTileInformation(localPlace, baseTileMap, collumn, row);
                if(tileInformation != null)
                {
                    Vector3 basePosition = FieldUtils.GetTileCenterPositionInGameWorld(new Vector2Int(localPlace.x, localPlace.y));
                    GameObject baseGameObject = Instantiate(baseModel, basePosition, Quaternion.identity);
                    baseGameObject.name = tileInformation.TileName;
                    baseGameObject.tag = this.GetBaseTagFromName(tileInformation.TileName);
                }
                baseTileMap.SetTile(localPlace, null);
            }
        }
    }

    private string GetBaseTagFromName(string baseName)
    {
        string tagValue = TagsConstants.HOME_BASE_TAG;

        if (baseName.Equals(NameConstants.HOME_BASE_NAME))
        {
            tagValue = TagsConstants.HOME_BASE_TAG;
        }
        else if (baseName.Equals(NameConstants.FIRST_BASE_NAME))
        {
            tagValue = TagsConstants.FIRST_BASE_TAG;
        }
        else if (baseName.Equals(NameConstants.SECOND_BASE_NAME))
        {
            tagValue = TagsConstants.SECOND_BASE_TAG;
        }
        else if (baseName.Equals(NameConstants.THIRD_BASE_NAME))
        {
            tagValue = TagsConstants.THIRD_BASE_TAG;
        }
        else if (baseName.Equals(NameConstants.PITCHER_BASE_NAME))
        {
            tagValue = TagsConstants.PITCHER_BASE_TAG;
        }

        return tagValue;
    }

    private TileInformation GetFieldTileInformation(Vector3Int localPlace, Tilemap tilemap, int collumn, int line)
    {
        //float camHeight = this.GetCamHeight()/2;
        //float camWidth = this.GetCamWidth()/2;
        TileInformation tileInformation = new TileInformation();
        
        string dictionarykey = this.BuildKey(collumn, line);
        tileInformation.TileIndex = (int)TileTypeEnum.GRASS;
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
        Vector2Int pitcherBaseTilePosition = FieldUtils.GetPitcherBaseTilePosition();
        Vector2Int homeBaseTilePosition = FieldUtils.GetHomeBaseTilePosition();
        Vector2Int firstBaseTilePosition = FieldUtils.GetFirstBaseTilePosition();
        Vector2Int secondBaseTilePosition = FieldUtils.GetSecondBaseTilePosition();
        Vector2Int thirdBaseTilePosition = FieldUtils.GetThirdBaseTilePosition();

        bool isPitcherBasePosition = FieldUtils.CompareTilePosition(pitcherBaseTilePosition, currentTileCoordinates);
        bool isHomeBasePosition = FieldUtils.CompareTilePosition(homeBaseTilePosition, currentTileCoordinates);
        bool isFirstBasePosition = FieldUtils.CompareTilePosition(firstBaseTilePosition, currentTileCoordinates);
        bool isSecondBasePosition = FieldUtils.CompareTilePosition(secondBaseTilePosition, currentTileCoordinates);
        bool isThirdBasePosition = FieldUtils.CompareTilePosition(thirdBaseTilePosition, currentTileCoordinates);

        if (isPitcherBasePosition)
        {
            tileInformation.TileName = NameConstants.PITCHER_BASE_NAME;
        }
        else if (isHomeBasePosition)
        {
            tileInformation.TileName = NameConstants.HOME_BASE_NAME;
        }
        else if (isFirstBasePosition)
        {
            tileInformation.TileName = NameConstants.FIRST_BASE_NAME;
        }
        else if (isSecondBasePosition)
        {
            tileInformation.TileName = NameConstants.SECOND_BASE_NAME;
        }
        else if (isThirdBasePosition)
        {
            tileInformation.TileName = NameConstants.THIRD_BASE_NAME;
        }

        if (isPitcherBasePosition || isHomeBasePosition || isFirstBasePosition || isSecondBasePosition || isThirdBasePosition)
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

        if (slopeDenominator != 0)
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

        if (slopeDenominator != 0)
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
    public static int ColumnMaximum { get => columnMaximum; set => columnMaximum = value; }
    public static int ColumnMinimum { get => columnMinimum; set => columnMinimum = value; }
    public static int RowMinimum { get => rowMinimum; set => rowMinimum = value; }
    public static int RowMaximum { get => rowMaximum; set => rowMaximum = value; }
}
