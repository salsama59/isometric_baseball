﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

        CameraController cameraController = CameraUtils.FetchCameraController();
        cameraController.BallGameObject = ball;

        PlayerEnum playerEnum;
        TeamIdEnum teamIdEnum;

        for (int i = 0; i < GameData.playerNumber; i++)
        {
            

            if(i == (int)PlayerEnum.PLAYER_1)
            {
                playerEnum = PlayerEnum.PLAYER_1;
                teamIdEnum = TeamIdEnum.TEAM_1;
            }
            else
            {
                playerEnum = PlayerEnum.PLAYER_2;
                teamIdEnum = TeamIdEnum.TEAM_2;
            }

            GameData.playerEnumTeamMap.Add(playerEnum, teamIdEnum);

            this.SetPlayersCharacteristics(playerEnum, ball);
        }

        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        playerActionsManager.BallGameObject = ball;
        playerActionsManager.BallControllerScript = BallUtils.FetchBallControllerScript(ball);
        playerActionsManager.PitcherGameObject = playerActionsManager.BallControllerScript.CurrentPitcher;

        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        PlayersTurnManager.IsCommandPhase = true;
        playersTurnManager.turnState = TurnStateEnum.PITCHER_TURN;

        TeamsScoreManager teamsScoreManager = GameUtils.FetchTeamsScoreManager();
        teamsScoreManager.UpdateTeamName(TeamIdEnum.TEAM_1, "FC.TOUTOU");
        teamsScoreManager.UpdateTeamName(TeamIdEnum.TEAM_2, "AS.DETERMINE");

    }


    private List<PlayerFieldPositionEnum> GetEligiblePlayerFieldPositionList(PlayerEnum playerEnum)
    {

        List<PlayerFieldPositionEnum> playerFieldPositions = new List<PlayerFieldPositionEnum>();

        foreach (PlayerFieldPositionEnum playerFieldPosition in TeamUtils.playerTeamMenberPositionLocation.Keys)
        {
            switch (playerEnum)
            {
                case PlayerEnum.PLAYER_1:
                    if (playerFieldPosition == PlayerFieldPositionEnum.BATTER)
                    {
                        playerFieldPositions.Add(playerFieldPosition);
                    }
                    break;
                case PlayerEnum.PLAYER_2:
                    if (playerFieldPosition != PlayerFieldPositionEnum.BATTER)
                    {
                        playerFieldPositions.Add(playerFieldPosition);
                    }
                    break;
                default:
                    break;
            }

        }

        return playerFieldPositions;
    }

    private void SetPlayersCharacteristics(PlayerEnum playerId, GameObject ball)
    {

        List<PlayerFieldPositionEnum> eligibilityList = this.GetEligiblePlayerFieldPositionList(playerId);

        GameData.playerEligibilityMap.Add(playerId, eligibilityList);

        foreach (KeyValuePair<PlayerFieldPositionEnum, Vector3> entry in TeamUtils.playerTeamMenberPositionLocation)
        {

            if (eligibilityList.Contains(entry.Key))
            {
                GameObject player = Instantiate(playerModel, entry.Value, Quaternion.identity);
                PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(player);
                playerStatus.PlayerFieldPosition = entry.Key;
                playerStatus.IsAllowedToMove = PlayerUtils.IsPlayerAllowedToMove(player);
                playerStatus.PlayerOwner = playerId;

                PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(player);

                PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();

                switch (playerStatus.PlayerFieldPosition)
                {
                    case PlayerFieldPositionEnum.BATTER:
                        playerStatus.BattingEfficiency = 10f;
                        playerStatus.BattingPower = 5;
                        player.AddComponent<BatterBehaviour>();
                        GameObject bat = Instantiate(batModel, FieldUtils.GetBatCorrectPosition(entry.Value), Quaternion.Euler(0f, 0f, -70f));
                        bat.transform.parent = player.transform;
                        PlayerAbility hitBallPlayerAbility = new PlayerAbility("Hit ball", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.HitBallAction);
                        playerAbilities.AddAbility(hitBallPlayerAbility);
                        break;
                    case PlayerFieldPositionEnum.PITCHER:
                        //ball.SetActive(true);
                        playerStatus.PitchEfficiency = 50f;
                        playerStatus.PitchingPower = 2;
                        playerStatus.PitchingEffect = 10f;
                        BallController ballControllerScript = BallUtils.FetchBallControllerScript(ball);
                        ballControllerScript.CurrentPitcher = player;
                        player.AddComponent<PitcherBehaviour>();
                        PlayerAbility throwBallPlayerAbility = new PlayerAbility("Throw", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.ThrowBallAction);
                        PlayerAbility gyroBallSpecialPlayerAbility = new PlayerAbility("Gyro ball", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.NORMAL, playerActionsManager.ThrowBallAction);
                        PlayerAbility fireBallSpecialPlayerAbility = new PlayerAbility("Fire ball", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.NORMAL, playerActionsManager.ThrowBallAction);
                        PlayerAbility menuBackAction = new PlayerAbility("Back", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.UI, null);
                        playerAbilities.AddAbility(throwBallPlayerAbility);
                        playerAbilities.AddAbility(gyroBallSpecialPlayerAbility);
                        playerAbilities.AddAbility(fireBallSpecialPlayerAbility);
                        playerAbilities.AddAbility(menuBackAction);
                        playerAbilities.HasSpecialAbilities = true;
                        this.UpdatePlayerColliderSettings(player);
                        break;
                    case PlayerFieldPositionEnum.RUNNER:
                        playerStatus.Speed = 2f;
                        player.AddComponent<RunnerBehaviour>();
                        PlayerAbility runPlayerAbility = new PlayerAbility("Run to next base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.RunAction);
                        PlayerAbility staySafePlayerAbility = new PlayerAbility("Stay on base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.StayAction);
                        playerAbilities.AddAbility(runPlayerAbility);
                        playerAbilities.AddAbility(staySafePlayerAbility);
                        break;
                    case PlayerFieldPositionEnum.CATCHER:
                        playerStatus.CatchEfficiency = 100f;
                        player.AddComponent<CatcherBehaviour>();
                        PlayerAbility catchPlayerAbility = new PlayerAbility("Catch ball", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.CatchBallAction);
                        playerAbilities.AddAbility(catchPlayerAbility);
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

                if (PlayerUtils.HasFielderPosition(player))
                {
                    TeamUtils.fielderList.Add(player);
                }


                TeamUtils.AddPlayerTeamMember(entry.Key, player, playerId);
            }
            
        }
    }

    private void UpdatePlayerColliderSettings(GameObject player)
    {
        BoxCollider2D boxCollider = player.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector2(2, 15);
        if (PlayerUtils.HasPitcherPosition(player))
        {
            boxCollider.offset = new Vector2(0, -2);
            boxCollider.size = new Vector2(2, 4);
        }

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


    public IEnumerator WaitAndReinit(DialogBoxManager dialogBoxManagerScript, PlayerStatus tagedOutRunnerStatus, PlayerStatus fielderPlayerStatus, GameObject fieldBall)
    {
        yield return new WaitForSeconds(2f);
        if (dialogBoxManagerScript.DialogTextBoxGameObject.activeSelf)
        {
            dialogBoxManagerScript.ToggleDialogTextBox();
        }

        BallController ballControllerScript = BallUtils.FetchBallControllerScript(fieldBall);
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        FielderBehaviour fielderBehaviourScript = PlayerUtils.FetchFielderBehaviourScript(fielderPlayerStatus.gameObject);
        //Update Pitcher informations
        PitcherBehaviour pitcherBehaviourScript = PlayerUtils.FetchPitcherBehaviourScript(ballControllerScript.CurrentPitcher);
        PlayerAbilities pitcherPlayerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(ballControllerScript.CurrentPitcher);
        PlayerStatus pitcherPlayerStatus = PlayerUtils.FetchPlayerStatusScript(ballControllerScript.CurrentPitcher);
        PlayerAbility throwBallPlayerAbility = new PlayerAbility("Throw", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.ThrowBallAction);
        PlayerAbility gyroBallSpecialPlayerAbility = new PlayerAbility("Gyro ball", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.NORMAL, playerActionsManager.ThrowBallAction);
        PlayerAbility fireBallSpecialPlayerAbility = new PlayerAbility("Fire ball", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.NORMAL, playerActionsManager.ThrowBallAction);
        PlayerAbility menuBackAction = new PlayerAbility("Back", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.UI, null);
        pitcherPlayerAbilities.PlayerAbilityList.Clear();
        pitcherPlayerAbilities.AddAbility(throwBallPlayerAbility);
        pitcherPlayerAbilities.AddAbility(gyroBallSpecialPlayerAbility);
        pitcherPlayerAbilities.AddAbility(fireBallSpecialPlayerAbility);
        pitcherPlayerAbilities.AddAbility(menuBackAction);
        pitcherPlayerAbilities.HasSpecialAbilities = true;
        ballControllerScript.CurrentPitcher.transform.position = TeamUtils.playerTeamMenberPositionLocation[pitcherPlayerStatus.PlayerFieldPosition];
        pitcherPlayerStatus.IsAllowedToMove = false;
        pitcherBehaviourScript.Target = null;
        pitcherBehaviourScript.HasSpottedBall = false;

        //Update ball informations
        ballControllerScript.BallHeight = BallHeightEnum.NONE;
        fieldBall.transform.position = ballControllerScript.CurrentPitcher.transform.position;
        ballControllerScript.CurrentHolder = null;
        ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
        //No parent
        fieldBall.transform.SetParent(null);
        ballControllerScript.IsHeld = false;
        ballControllerScript.IsPitched = false;
        ballControllerScript.IsMoving = false;
        ballControllerScript.IsTargetedByFielder = false;

        //Update taged out runner and new batter informations
        tagedOutRunnerStatus.IsAllowedToMove = false;
        tagedOutRunnerStatus.PlayerFieldPosition = PlayerFieldPositionEnum.BATTER;
        RunnerBehaviour runnerBehaviourScript = PlayerUtils.FetchRunnerBehaviourScript(tagedOutRunnerStatus.gameObject);
        GameObject bat = runnerBehaviourScript.EquipedBat;
        Destroy(tagedOutRunnerStatus.gameObject.GetComponent<RunnerBehaviour>());
        tagedOutRunnerStatus.gameObject.AddComponent<BatterBehaviour>();
        tagedOutRunnerStatus.gameObject.transform.position = TeamUtils.playerTeamMenberPositionLocation[PlayerFieldPositionEnum.BATTER];
        BatterBehaviour batterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(tagedOutRunnerStatus.gameObject);
        batterBehaviourScript.Start();
        batterBehaviourScript.EquipedBat = bat;
        batterBehaviourScript.EquipedBat.SetActive(true);
        tagedOutRunnerStatus.gameObject.transform.rotation = Quaternion.identity;
        batterBehaviourScript.IsoRenderer.LastDirection = 6;
        batterBehaviourScript.IsoRenderer.SetDirection(Vector2.zero);
        PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(tagedOutRunnerStatus.gameObject);
        PlayerAbility hitBallPlayerAbility = new PlayerAbility("Hit ball", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.HitBallAction);
        playerAbilities.PlayerAbilityList.Clear();
        playerAbilities.AddAbility(hitBallPlayerAbility);

        //Update fielder informations
        fielderPlayerStatus.IsAllowedToMove = true;
        fielderBehaviourScript.HasSpottedBall = false;
        fielderBehaviourScript.IsPrepared = false;
        fielderBehaviourScript.Target = null;
        fielderBehaviourScript.IsHoldingBall = false;
        fielderBehaviourScript.TargetPlayerToTagOut = null;
        fielderBehaviourScript.IsMoving = false;
        fielderBehaviourScript.gameObject.transform.position = TeamUtils.playerTeamMenberPositionLocation[fielderPlayerStatus.PlayerFieldPosition];
        fielderBehaviourScript.transform.rotation = Quaternion.identity;
        fielderBehaviourScript.IsoRenderer.LastDirection = 4;
        fielderBehaviourScript.IsoRenderer.SetDirection(Vector2.zero);

        //Reinit turn
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        PlayersTurnManager.IsCommandPhase = true;
        playersTurnManager.turnState = TurnStateEnum.PITCHER_TURN;

        //Remove pause state
        GameData.isPaused = false;
    }


    public Dictionary<string, TileTypeEnum> HorizontalyPaintedDirtDictionary { get => horizontalyPaintedDirtDictionary; set => horizontalyPaintedDirtDictionary = value; }
    public Dictionary<string, TileTypeEnum> VerticalyPaintedDirtDictionary { get => verticalyPaintedDirtDictionary; set => verticalyPaintedDirtDictionary = value; }
    public static int ColumnMaximum { get => columnMaximum; set => columnMaximum = value; }
    public static int ColumnMinimum { get => columnMinimum; set => columnMinimum = value; }
    public static int RowMinimum { get => rowMinimum; set => rowMinimum = value; }
    public static int RowMaximum { get => rowMaximum; set => rowMaximum = value; }
}
