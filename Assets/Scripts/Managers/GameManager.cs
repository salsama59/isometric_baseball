using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Video;

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
    public GameObject foulZoneModel = null;
    private Dictionary<string, TileTypeEnum> horizontalyPaintedDirtDictionary = new Dictionary<string, TileTypeEnum>();
    private Dictionary<string, TileTypeEnum> verticalyPaintedDirtDictionary = new Dictionary<string, TileTypeEnum>();
    private List<GameObject> attackTeamBatterListClone = new List<GameObject>();
    private List<GameObject> attackTeamRunnerList = new List<GameObject>();
    private int batterOutCount = 0;
    private GameObject fieldBall;
    private bool isStateCheckAllowed;
    private List<GameObject> attackTeamBatterList = new List<GameObject>();
    private GameObject bat = null;
    private InningPhaseEnum currentInningPhase;
    

    void Start()
    {
        this.BuildGameField();
        this.BuildFoulZones();

        GameObject ball = Instantiate(ballModel, this.transform.position, this.transform.rotation);
        ball.SetActive(false);
        FieldBall = ball;
        CameraController cameraController = CameraUtils.FetchCameraController();
        cameraController.BallGameObject = ball;

        PlayerEnum playerEnum;
        TeamIdEnum teamIdEnum;
        TeamSideEnum teamSideEnum;
        CurrentInningPhase = InningPhaseEnum.FIRST_HALF;

        for (int i = 0; i < GameData.playerNumber; i++)
        {
            
            if(i == (int)PlayerEnum.PLAYER_1)
            {
                playerEnum = PlayerEnum.PLAYER_1;
                teamIdEnum = TeamIdEnum.TEAM_1;
                teamSideEnum = TeamSideEnum.ATTACK;
            }
            else
            {
                playerEnum = PlayerEnum.PLAYER_2;
                teamIdEnum = TeamIdEnum.TEAM_2;
                teamSideEnum = TeamSideEnum.DEFENSE;
            }

            GameData.teamIdEnumMap.Add(playerEnum, teamIdEnum);
            GameData.teamSideEnumMap.Add(teamIdEnum, teamSideEnum);

            this.SetPlayersCharacteristics(playerEnum, ball);
        }

        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        playerActionsManager.BallGameObject = ball;
        playerActionsManager.BallControllerScript = BallUtils.FetchBallControllerScript(ball);
        playerActionsManager.PitcherGameObject = playerActionsManager.BallControllerScript.CurrentPitcher;

        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        PlayersTurnManager.IsCommandPhase = true;
        playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;

        TeamsScoreManager teamsScoreManager = GameUtils.FetchTeamsScoreManager();
        teamsScoreManager.UpdateTeamName(TeamIdEnum.TEAM_1, "FC.TOUTOU");
        teamsScoreManager.UpdateTeamName(TeamIdEnum.TEAM_2, "AS.DETERMINE");

    }

    void Update()
    {
        if (IsStateCheckAllowed)
        {
            this.CheckRunnersState();
            IsStateCheckAllowed = false;
        }

    }

    public void ProcessNextInningHalf()
    {
        this.SwappTeamSide();
        this.UpdateAllPlayersLocation();

        if(CurrentInningPhase == InningPhaseEnum.FIRST_HALF)
        {
            CurrentInningPhase = InningPhaseEnum.SECOND_HALF;
        }
        else if(CurrentInningPhase == InningPhaseEnum.SECOND_HALF)
        {
            CurrentInningPhase = InningPhaseEnum.FIRST_HALF;
        }
    }

    private void UpdateAllPlayersLocation()
    {
        foreach (KeyValuePair<PlayerFieldPositionEnum, Vector3> playerTeamMenberPositionLocationEntry in TeamUtils.playerTeamMenberPositionLocation)
        {
            PlayerFieldPositionEnum playerPosition = playerTeamMenberPositionLocationEntry.Key;
            Vector3 playerLocation = playerTeamMenberPositionLocationEntry.Value;
            GameObject player = TeamUtils.GetPlayerTeamMember(playerPosition, TeamUtils.GetPlayerIdFromPlayerFieldPosition(playerPosition));
            player.transform.position = playerLocation;
        }
    }

    public void SwappTeamSide()
    {
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();

        Dictionary<TeamIdEnum, TeamSideEnum> newTeamSideDictionary = new Dictionary<TeamIdEnum, TeamSideEnum>();
        foreach (KeyValuePair<TeamIdEnum, TeamSideEnum> teamSideKeyValuePair in GameData.teamSideEnumMap)
        {
            TeamSideEnum currentSide = GameData.teamSideEnumMap[teamSideKeyValuePair.Key];
            TeamSideEnum swappedSide = TeamSideEnum.NONE;

            if (currentSide == TeamSideEnum.DEFENSE)
            {
                swappedSide = TeamSideEnum.ATTACK;
            }
            else if(currentSide == TeamSideEnum.ATTACK)
            {
                swappedSide = TeamSideEnum.DEFENSE;
            }

            newTeamSideDictionary.Add(teamSideKeyValuePair.Key, swappedSide);
        }

        GameData.teamSideEnumMap = newTeamSideDictionary;

        PlayerEnum playerEnum;
        Dictionary<int, GameObject> newPlayerAttackTeamDictionary = new Dictionary<int, GameObject>();
        Dictionary<int, GameObject> newPlayerDefenseTeamDictionary = new Dictionary<int, GameObject>();
        Dictionary<PlayerEnum, Dictionary<int, GameObject>> generalPlayerDictionary = new Dictionary<PlayerEnum, Dictionary<int, GameObject>>();

        for (int i = 0; i < GameData.playerNumber; i++)
        {

            if (i == (int)PlayerEnum.PLAYER_1)
            {
                playerEnum = PlayerEnum.PLAYER_1;
            }
            else
            {
                playerEnum = PlayerEnum.PLAYER_2;
            }

            TeamSideEnum side = GameData.teamSideEnumMap[GameData.teamIdEnumMap[playerEnum]];

            if (side == TeamSideEnum.ATTACK)
            {
                AttackTeamBatterListClone.Clear();
                int batterNameIndex = 0;
                foreach (KeyValuePair<int, GameObject> teamKeyValuePair in TeamUtils.GetPlayerTeam(playerEnum))
                {
                   
                    GameObject attackPlayer = teamKeyValuePair.Value;
                    PlayerStatus status = PlayerUtils.FetchPlayerStatusScript(attackPlayer);
                    status.PlayerFieldPosition = PlayerFieldPositionEnum.BATTER;
                    PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(attackPlayer);

                    if (PlayerUtils.HasCatcherPosition(attackPlayer))
                    {
                        Destroy(attackPlayer.GetComponent<CatcherBehaviour>());
                    }
                    else if (PlayerUtils.HasFielderPosition(attackPlayer))
                    {
                        Destroy(attackPlayer.GetComponent<FielderBehaviour>());
                    }
                    else if (PlayerUtils.HasPitcherPosition(attackPlayer))
                    {
                        Destroy(attackPlayer.GetComponent<PitcherBehaviour>());
                    }

                    status.PlayerFieldPosition = PlayerFieldPositionEnum.BATTER;
                    BoxCollider2D boxCollider = attackPlayer.transform
                        .GetChild(0)
                        .GetChild(0)
                        .GetComponentInChildren<BoxCollider2D>();
                    boxCollider.offset = Vector2.zero;
                    this.CalculatePlayerStats(FieldBall, attackPlayer, status, playerAbilities, playerActionsManager);
                    attackPlayer.transform.position = TeamUtils.playerTeamMenberPositionLocation[status.PlayerFieldPosition];
                    AttackTeamBatterListClone.Add(attackPlayer);
                    attackPlayer.name += "_" + batterNameIndex;
                    attackPlayer.SetActive(false);
                    batterNameIndex++;
                }

                GameObject firstBatter = AttackTeamBatterListClone.First();
                firstBatter.SetActive(true);
                this.EquipBatToPlayer(firstBatter, Bat);
                newPlayerAttackTeamDictionary.Add((int)PlayerFieldPositionEnum.BATTER, firstBatter);
                generalPlayerDictionary.Add(playerEnum, newPlayerAttackTeamDictionary);
            }
            else if (side == TeamSideEnum.DEFENSE)
            {
                TeamUtils.fielderList.Clear();
                foreach (GameObject defensePlayer in AttackTeamBatterList)
                {
                    Destroy(defensePlayer.GetComponent<BatterBehaviour>());
                    PlayerStatus status = PlayerUtils.FetchPlayerStatusScript(defensePlayer);
                    status.PlayerFieldPosition = status.DefaultPlayerFieldPosition;
                    PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(defensePlayer);
                    this.CalculatePlayerStats(FieldBall, defensePlayer, status, playerAbilities, playerActionsManager);
                    this.UpdatePlayerGenericBehaviour(FieldBall, defensePlayer, status);
                    newPlayerDefenseTeamDictionary.Add((int)status.DefaultPlayerFieldPosition, defensePlayer);
                    defensePlayer.SetActive(true);
                    if (PlayerUtils.HasFielderPosition(defensePlayer))
                    {
                        TeamUtils.fielderList.Add(defensePlayer);
                    }

                    if (PlayerUtils.HasPitcherPosition(defensePlayer))
                    {
                        this.ReinitPitcher(defensePlayer);
                        this.ReturnBallToPitcher(fieldBall);
                    }
                }
                generalPlayerDictionary.Add(playerEnum, newPlayerDefenseTeamDictionary);
            }   
        }

        //Update team map
        foreach (KeyValuePair<PlayerEnum, Dictionary<int, GameObject>> generalTeamKeyValuePair in generalPlayerDictionary)
        {
            TeamUtils.SetPlayerTeam(generalTeamKeyValuePair.Key, generalTeamKeyValuePair.Value);
        }

        //Swapp the position list
        List<PlayerFieldPositionEnum> player1PositionList = GameData.playerFieldPositionEnumListMap[PlayerEnum.PLAYER_1];
        List<PlayerFieldPositionEnum> player2PositionList = GameData.playerFieldPositionEnumListMap[PlayerEnum.PLAYER_2];
        GameData.playerFieldPositionEnumListMap[PlayerEnum.PLAYER_1] = player2PositionList;
        GameData.playerFieldPositionEnumListMap[PlayerEnum.PLAYER_2] = player1PositionList;

        //Clone the original list of player
        AttackTeamBatterList = new List<GameObject>(AttackTeamBatterListClone);

        AttackTeamRunnerList.Clear();

        //Update fielders informations
        this.ReinitFielders(TeamUtils.fielderList);

        //Reinit turn
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        PlayersTurnManager.IsCommandPhase = true;
        playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
        playersTurnManager.MakeAllPlayerAvailable();

        //Remove pause state
        GameData.isPaused = false;
    }

    private void BuildFoulZones()
    {
        
        Vector3 firstBasePosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFirstBaseTilePosition());
        Vector3 homeBasePosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
        Vector3 thirdBasePosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetThirdBaseTilePosition());

        //Left foul zone creation and positioning
        GameObject leftSideFoulzone = Instantiate(foulZoneModel, this.transform.position, this.transform.rotation);
        leftSideFoulzone.name = NameConstants.LEFT_SIDE_FOUL_ZONE_NAME;
        leftSideFoulzone.tag = TagsConstants.LEFT_SIDE_FOUL_ZONE_TAG;
        float homeBaseToThirdBaseDistance = Vector3.Distance(homeBasePosition, thirdBasePosition);
        Vector3 leftSideFictionalPosition = new Vector3(thirdBasePosition.x, homeBasePosition.y, 0);
        float homeBaseToLeftSideFictionalDistance = Vector3.Distance(homeBasePosition, leftSideFictionalPosition);
        float leftSidefoulZoneAngle = Mathf.Acos(homeBaseToLeftSideFictionalDistance / homeBaseToThirdBaseDistance);
        leftSideFoulzone.transform.rotation = Quaternion.Euler(0, 0, leftSidefoulZoneAngle * -1 * Mathf.Rad2Deg);
        float leftSideZonePositioningAndEffectSize = (float)rowMinimum / 4f + (float)rowMinimum / 2f + (float)FieldUtils.GRID_SIZE;
        leftSideFoulzone.transform.position = new Vector3(leftSideZonePositioningAndEffectSize, (float)columnMinimum / 2f - (float)FieldUtils.GRID_SIZE / 2f, 0);
        BoxCollider2D leftSideFoulZoneCollider = leftSideFoulzone.GetComponent<BoxCollider2D>();
        leftSideFoulZoneCollider.size = new Vector2(homeBaseToThirdBaseDistance, Mathf.Abs(leftSideZonePositioningAndEffectSize));

        //Rigth foul zone creation and positioning
        GameObject rightSideFoulzone = Instantiate(foulZoneModel, this.transform.position, this.transform.rotation);
        rightSideFoulzone.name = NameConstants.RIGTH_SIDE_FOUL_ZONE_NAME;
        rightSideFoulzone.tag = TagsConstants.RIGTH_SIDE_FOUL_ZONE_TAG;
        float homeBaseToFirstBaseDistance = Vector3.Distance(homeBasePosition, firstBasePosition);
        Vector3 rigthSideFictionalPosition = new Vector3(firstBasePosition.x, homeBasePosition.y, 0);
        float homeBaseToRigthSideFictionalDistance = Vector3.Distance(homeBasePosition, rigthSideFictionalPosition);
        float rigthSidefoulZoneAngle = Mathf.Acos(homeBaseToRigthSideFictionalDistance / homeBaseToFirstBaseDistance);
        rightSideFoulzone.transform.rotation = Quaternion.Euler(0, 0, rigthSidefoulZoneAngle * Mathf.Rad2Deg);
        float rigthSideZonePositioningAndEffectSize = (float)rowMaximum / 4f + (float)rowMaximum / 2f - (float)FieldUtils.GRID_SIZE;
        rightSideFoulzone.transform.position = new Vector3(rigthSideZonePositioningAndEffectSize, (float)columnMinimum / 2f - (float)FieldUtils.GRID_SIZE/2f, 0);
        BoxCollider2D rightSideFoulZoneCollider = rightSideFoulzone.GetComponent<BoxCollider2D>();
        rightSideFoulZoneCollider.size = new Vector2(homeBaseToFirstBaseDistance, rigthSideZonePositioningAndEffectSize);
    }



    private void CheckRunnersState()
    {
        if(this.AttackTeamRunnerList.Count > 0)
        {
           

           bool isRunnersAllSafeAndStaying = this.AttackTeamRunnerList.TrueForAll(runner => {
                RunnerBehaviour runnerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(runner);
                return runnerBehaviour.IsSafe && runnerBehaviour.IsStaying;
            });

            if (isRunnersAllSafeAndStaying)
            {
                BallController ballControllerScript = BallUtils.FetchBallControllerScript(FieldBall);
                //Return ball to pitcher and place back players.
                this.ReinitPitcher(ballControllerScript.CurrentPitcher);
                this.ReturnBallToPitcher(ballControllerScript.gameObject);
                this.ReinitFielders(TeamUtils.fielderList);
                this.ReinitRunners(this.AttackTeamRunnerList);

                GameObject newBatter = this.AttackTeamBatterListClone.First();
                GameObject lastRunner = this.AttackTeamRunnerList.Last();
                newBatter.SetActive(true);

                BatterBehaviour newbatterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(newBatter);
                RunnerBehaviour runnerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(lastRunner);

                newbatterBehaviourScript.EquipedBat = runnerBehaviour.EquipedBat;
                this.EquipBatToPlayer(newBatter, newbatterBehaviourScript.EquipedBat);

                PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
                PlayersTurnManager.IsCommandPhase = true;
                playersTurnManager.MakeAllPlayerAvailable();
            }
            
        }
        
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

        GameData.playerFieldPositionEnumListMap.Add(playerId, eligibilityList);

        TeamSideEnum side = GameData.teamSideEnumMap[GameData.teamIdEnumMap[playerId]];

        //according to the side if DEFENSE do like down there else if attack populate batterlist and place the first element on the field

        if(side == TeamSideEnum.ATTACK)
        {
            KeyValuePair<PlayerFieldPositionEnum, Vector3> batterKeyValuePair = new KeyValuePair<PlayerFieldPositionEnum, Vector3>(PlayerFieldPositionEnum.BATTER, TeamUtils.playerTeamMenberPositionLocation[PlayerFieldPositionEnum.BATTER]);
            for (int i = 0; i < TeamUtils.teamSize; i++)
            {
                GameObject batter = this.ProcessPlayer(playerId, ball, batterKeyValuePair, side);
                PlayerStatus status = PlayerUtils.FetchPlayerStatusScript(batter);
                //TODO : Stop using this map as soon as possible!!!
                status.DefaultPlayerFieldPosition = TeamUtils.genericPositionAllocationMap[i];
                batter.name += "_" + i;
                AttackTeamBatterList.Add(batter);
            }

            //Clone the original list of player
            AttackTeamBatterListClone = new List<GameObject>(AttackTeamBatterList);
            GameObject firstBatter = AttackTeamBatterListClone.First();
            BatterBehaviour firstBatterBehaviour = PlayerUtils.FetchBatterBehaviourScript(firstBatter);
            GameObject bat = Instantiate(batModel, FieldUtils.GetBatCorrectPosition(batterKeyValuePair.Value), Quaternion.Euler(0f, 0f, -70f));
            Bat = bat;
            bat.transform.parent = firstBatter.transform;
            firstBatter.SetActive(true);
            firstBatterBehaviour.EquipedBat = bat;
            TeamUtils.AddPlayerTeamMember(batterKeyValuePair.Key, firstBatter, playerId);
        }
        else if (side == TeamSideEnum.DEFENSE)
        {
            foreach (KeyValuePair<PlayerFieldPositionEnum, Vector3> entry in TeamUtils.playerTeamMenberPositionLocation)
            {
                if (eligibilityList.Contains(entry.Key))
                {
                    GameObject defensePlayer = this.ProcessPlayer(playerId, ball, entry, side);
                    PlayerStatus status = PlayerUtils.FetchPlayerStatusScript(defensePlayer);
                    status.DefaultPlayerFieldPosition = entry.Key;
                }
            }
        }
        
    }

    private GameObject ProcessPlayer(PlayerEnum playerId, GameObject ball, KeyValuePair<PlayerFieldPositionEnum, Vector3> entry, TeamSideEnum teamSide)
    {
        GameObject player = Instantiate(playerModel, entry.Value, Quaternion.identity);
        PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(player);
        playerStatus.PlayerFieldPosition = entry.Key;
        playerStatus.IsAllowedToMove = PlayerUtils.IsPlayerAllowedToMove(player);
        playerStatus.PlayerOwner = playerId;

        PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(player);

        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        this.CalculatePlayerStats(ball, player, playerStatus, playerAbilities, playerActionsManager);
        this.UpdatePlayerGenericBehaviour(ball, player, playerStatus);

        if (PlayerUtils.HasFielderPosition(player))
        {
            TeamUtils.fielderList.Add(player);
        }

        if (teamSide == TeamSideEnum.DEFENSE)
        {
            TeamUtils.AddPlayerTeamMember(entry.Key, player, playerId);
        }

        return player;
    }

    private void UpdatePlayerGenericBehaviour(GameObject ball, GameObject player, PlayerStatus playerStatus)
    {
        GenericPlayerBehaviour genericPlayerBehaviourScript = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(player, playerStatus);
        if (genericPlayerBehaviourScript != null)
        {
            genericPlayerBehaviourScript.FieldBall = ball;
        }
    }

    private void CalculatePlayerStats(GameObject ball, GameObject player, PlayerStatus playerStatus, PlayerAbilities playerAbilities, PlayerActionsManager playerActionsManager)
    {
        playerAbilities.PlayerAbilityList.Clear();

        switch (playerStatus.PlayerFieldPosition)
        {
            case PlayerFieldPositionEnum.BATTER:
                playerStatus.BattingEfficiency = 10f;
                playerStatus.BattingPower = 5;
                player.AddComponent<BatterBehaviour>();
                PlayerAbility hitBallPlayerAbility = new PlayerAbility("Hit ball", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.HitBallAction, player);
                playerAbilities.AddAbility(hitBallPlayerAbility);
                player.SetActive(false);
                break;
            case PlayerFieldPositionEnum.PITCHER:
                playerStatus.PitchEfficiency = 50f;
                playerStatus.PitchingPower = 2;
                playerStatus.PitchingEffect = 10f;
                BallController ballControllerScript = BallUtils.FetchBallControllerScript(ball);
                ballControllerScript.CurrentPitcher = player;
                player.AddComponent<PitcherBehaviour>();
                PlayerAbility throwBallPlayerAbility = new PlayerAbility("Throw", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.PitchBallAction, player);
                PlayerAbility gyroBallSpecialPlayerAbility = new PlayerAbility("Gyro ball", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.NORMAL, playerActionsManager.PitchBallAction, player);
                PlayerAbility fireBallSpecialPlayerAbility = new PlayerAbility("Fire ball", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.NORMAL, playerActionsManager.PitchBallAction, player);
                PlayerAbility menuBackAction = new PlayerAbility("Back", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.UI, null, null);
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
                PlayerAbility runPlayerAbility = new PlayerAbility("Run to next base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.RunAction, player);
                PlayerAbility staySafePlayerAbility = new PlayerAbility("Stay on base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.StayOnBaseAction, player);
                playerAbilities.AddAbility(runPlayerAbility);
                playerAbilities.AddAbility(staySafePlayerAbility);
                break;
            case PlayerFieldPositionEnum.CATCHER:
                playerStatus.CatchEfficiency = 100f;
                player.AddComponent<CatcherBehaviour>();
                PlayerAbility catchPlayerAbility = new PlayerAbility("Catch ball", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.CatchBallAction, player);
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

        if (PlayerUtils.HasFielderPosition(player))
        {
            PlayerAbility passPlayerAbility = new PlayerAbility("Pass to fielder", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.GenericPassAction, player, true);
            PlayerAbility tagOutPlayerAbility = new PlayerAbility("Go for tagout", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.GenericTagOutAimAction, player, false);
            playerAbilities.AddAbility(passPlayerAbility);
            playerAbilities.AddAbility(tagOutPlayerAbility);
        }

        player.name = playerStatus.PlayerFieldPosition.ToString();
    }

    private void UpdatePlayerColliderSettings(GameObject player)
    {

        BoxCollider2D boxCollider = player.transform.GetChild(0)
            .GetChild(0)
            .GetComponentInChildren<BoxCollider2D>();

        boxCollider.isTrigger = true;
        boxCollider.size = new Vector2(2, 15);
        if (PlayerUtils.HasPitcherPosition(player))
        {
            boxCollider.offset = new Vector2(0, -1.5f);
            boxCollider.size = new Vector2(2, 3);
        }

    }

    public IEnumerator WaitAndReinit(DialogBoxManager dialogBoxManagerScript, PlayerStatus newBatterStatus, GameObject fieldBall)
    {
        yield return new WaitForSeconds(2f);
        if (dialogBoxManagerScript.DialogTextBoxGameObject.activeSelf)
        {
            dialogBoxManagerScript.ToggleDialogTextBox();
        }

        BallController ballControllerScript = BallUtils.FetchBallControllerScript(fieldBall);

        //Update Pitcher informations
        this.ReinitPitcher(ballControllerScript.CurrentPitcher);

        //Update ball informations
        this.ReturnBallToPitcher(fieldBall);

        //Update taged out runner and new batter informations
        BatterBehaviour batterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(newBatterStatus.gameObject);
        GameObject bat = batterBehaviourScript.EquipedBat;
        this.EquipBatToPlayer(newBatterStatus.gameObject, bat);
        TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.BATTER, batterBehaviourScript.gameObject, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.BATTER));

        //Update fielders informations
        this.ReinitFielders(TeamUtils.fielderList);

        //Update runners informations
        this.ReinitRunners(this.attackTeamRunnerList);

        //Reinit turn
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        PlayersTurnManager.IsCommandPhase = true;
        playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
        playersTurnManager.MakeAllPlayerAvailable();

        //Remove pause state
        GameData.isPaused = false;
    }

    public void ReinitPitcher(GameObject pitcher)
    {
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        PitcherBehaviour pitcherBehaviourScript = PlayerUtils.FetchPitcherBehaviourScript(pitcher);
        PlayerAbilities pitcherPlayerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(pitcher);
        PlayerStatus pitcherPlayerStatus = PlayerUtils.FetchPlayerStatusScript(pitcher);
        PlayerAbility throwBallPlayerAbility = new PlayerAbility("Throw", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.PitchBallAction, pitcher);
        PlayerAbility gyroBallSpecialPlayerAbility = new PlayerAbility("Gyro ball", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.NORMAL, playerActionsManager.PitchBallAction, pitcher);
        PlayerAbility fireBallSpecialPlayerAbility = new PlayerAbility("Fire ball", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.NORMAL, playerActionsManager.PitchBallAction, pitcher);
        PlayerAbility menuBackAction = new PlayerAbility("Back", AbilityTypeEnum.SPECIAL, AbilityCategoryEnum.UI, null, null);
        pitcherPlayerAbilities.PlayerAbilityList.Clear();
        pitcherPlayerAbilities.AddAbility(throwBallPlayerAbility);
        pitcherPlayerAbilities.AddAbility(gyroBallSpecialPlayerAbility);
        pitcherPlayerAbilities.AddAbility(fireBallSpecialPlayerAbility);
        pitcherPlayerAbilities.AddAbility(menuBackAction);
        pitcherPlayerAbilities.HasSpecialAbilities = true;
        pitcherBehaviourScript.Target = null;
        pitcher.transform.position = TeamUtils.playerTeamMenberPositionLocation[pitcherPlayerStatus.PlayerFieldPosition];
        pitcherPlayerStatus.IsAllowedToMove = false;
        pitcherBehaviourScript.HasSpottedBall = false;
        if(pitcherBehaviourScript.IsoRenderer == null)
        {
            pitcherBehaviourScript.IsoRenderer = PlayerUtils.FetchPlayerIsometricRenderer(pitcher);
        }
        pitcherBehaviourScript.IsoRenderer.ReinitializeAnimator();
    }

    public void EquipBatToPlayer(GameObject player, GameObject bat)
    {
        bat.transform.SetParent(null);
        bat.transform.position = FieldUtils.GetBatCorrectPosition(player.transform.position);
        bat.transform.rotation = Quaternion.Euler(0f, 0f, -70f);
        bat.transform.SetParent(player.transform);
        bat.SetActive(true);
        bat.GetComponent<CapsuleCollider2D>().enabled = true;
        BatterBehaviour currentBatterBehaviour = PlayerUtils.FetchBatterBehaviourScript(player);
        currentBatterBehaviour.EquipedBat = bat;
    }

    public void ReturnBallToPitcher(GameObject ball)
    {
        BallController ballControllerScript = BallUtils.FetchBallControllerScript(ball);
        GameObject pitcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.PITCHER, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.PITCHER));
        ballControllerScript.BallHeight = BallHeightEnum.NONE;
        ball.transform.position = pitcher.transform.position;
        ballControllerScript.CurrentHolder = null;
        ballControllerScript.Target = null;
        ballControllerScript.CurrentPasser = null;
        //No parent
        ball.transform.SetParent(null);
        ballControllerScript.IsHeld = false;
        ballControllerScript.IsPitched = false;
        ballControllerScript.IsMoving = false;
        ballControllerScript.IsPassed = false;
        ballControllerScript.IsTargetedByFielder = false;
        ballControllerScript.IsTargetedByPitcher = false;
        ballControllerScript.IsHit = false;
        ballControllerScript.IsInFoulState = false;
        ballControllerScript.StopCoroutine(ballControllerScript.MovementCoroutine);
    }

    public void ReinitFielders(List<GameObject> fielders)
    {
        foreach (GameObject fielder in fielders)
        {
            FielderBehaviour fielderBehaviourScript = PlayerUtils.FetchFielderBehaviourScript(fielder);
            PlayerStatus fielderPlayerStatus = PlayerUtils.FetchPlayerStatusScript(fielder);
            fielderPlayerStatus.IsAllowedToMove = true;
            fielderBehaviourScript.HasSpottedBall = false;
            fielderBehaviourScript.IsPrepared = false;
            fielderBehaviourScript.Target = null;
            fielderBehaviourScript.IsHoldingBall = false;
            fielderBehaviourScript.TargetPlayerToTagOut = null;
            fielderBehaviourScript.IsMoving = false;
            fielderBehaviourScript.gameObject.transform.position = TeamUtils.playerTeamMenberPositionLocation[fielderPlayerStatus.PlayerFieldPosition];
            GameObject fielderSight = fielderBehaviourScript.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
            fielderSight.transform.rotation = Quaternion.identity;
            if (fielderBehaviourScript.IsoRenderer == null)
            {
                fielderBehaviourScript.IsoRenderer = PlayerUtils.FetchPlayerIsometricRenderer(fielder);
            }
            fielderBehaviourScript.IsoRenderer.ReinitializeAnimator();
            fielderBehaviourScript.IsoRenderer.LastDirection = 4;
            fielderBehaviourScript.IsoRenderer.SetDirection(Vector2.zero);
        }

    }

    public void ReinitRunners(List<GameObject> runners)
    {
        foreach (GameObject runner in runners)
        {
            RunnerBehaviour runnerBehaviourScript = PlayerUtils.FetchRunnerBehaviourScript(runner);
            runnerBehaviourScript.IsStaying = false;
        }
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

    public Dictionary<string, TileTypeEnum> HorizontalyPaintedDirtDictionary { get => horizontalyPaintedDirtDictionary; set => horizontalyPaintedDirtDictionary = value; }
    public Dictionary<string, TileTypeEnum> VerticalyPaintedDirtDictionary { get => verticalyPaintedDirtDictionary; set => verticalyPaintedDirtDictionary = value; }
    public static int ColumnMaximum { get => columnMaximum; set => columnMaximum = value; }
    public static int ColumnMinimum { get => columnMinimum; set => columnMinimum = value; }
    public static int RowMinimum { get => rowMinimum; set => rowMinimum = value; }
    public static int RowMaximum { get => rowMaximum; set => rowMaximum = value; }
    public List<GameObject> AttackTeamBatterListClone { get => attackTeamBatterListClone; set => attackTeamBatterListClone = value; }
    public List<GameObject> AttackTeamRunnerList { get => attackTeamRunnerList; set => attackTeamRunnerList = value; }
    public int BatterOutCount { get => batterOutCount; set => batterOutCount = value; }
    public GameObject FieldBall { get => fieldBall; set => fieldBall = value; }
    public bool IsStateCheckAllowed { get => isStateCheckAllowed; set => isStateCheckAllowed = value; }
    public List<GameObject> AttackTeamBatterList { get => attackTeamBatterList; set => attackTeamBatterList = value; }
    public GameObject Bat { get => bat; set => bat = value; }
    public InningPhaseEnum CurrentInningPhase { get => currentInningPhase; set => currentInningPhase = value; }
}
