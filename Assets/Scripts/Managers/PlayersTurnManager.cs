using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayersTurnManager : MonoBehaviour
{

    private TurnStateEnum turnState;
    public static bool IsCommandPhase;
    private CommandMenuManager commandMenuManager;
    private CameraController cameraController;
    private PlayerFieldPositionEnum currentFielderTypeTurn;
    private TargetSelectionManager targetSelectionManager;
    private GameManager gameManager;
    private GameObject currentRunner;
    private bool isRunnersTurnsDone;
    private int currentIndex;
    private bool isSkipNextRunnerTurnEnabled;
    private Dictionary<string, TurnAvailabilityEnum> playerTurnAvailability;

    private void Start()
    {
        TargetSelectionManager = GameUtils.FetchTargetSelectionManager();
        CameraController = CameraUtils.FetchCameraController();
        CommandMenuManager = GameUtils.FetchCommandMenuManager();
        GameManager = GameUtils.FetchGameManager();
        PlayerTurnAvailability = new Dictionary<string, TurnAvailabilityEnum>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CommandMenuManager.isMenuDisplayEnabled && !TargetSelectionManager.IsActivated)
        {

            PlayerAbilities playerAbilitiesScript = null;

            switch (this.TurnState)
            {
                case TurnStateEnum.STANDBY:
                    break;
                case TurnStateEnum.PITCHER_TURN:
                    GameObject pitcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.PITCHER, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.PITCHER));
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(pitcher);
                    break;
                case TurnStateEnum.BATTER_TURN:
                    GameObject batter = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.BATTER, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.BATTER));
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(batter);
                    break;
                case TurnStateEnum.RUNNER_TURN:
                    GameObject runner = this.GetNextRunner();
                    if(runner != null)
                    {
                        playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(runner);
                    }
                    break;
                case TurnStateEnum.CATCHER_TURN:
                    GameObject catcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.CATCHER, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.CATCHER));
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(catcher);
                    break;
                case TurnStateEnum.FIELDER_TURN:
                    GameObject fielder = TeamUtils.GetPlayerTeamMember(CurrentFielderTypeTurn, TeamUtils.GetPlayerIdFromPlayerFieldPosition(CurrentFielderTypeTurn));
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(fielder);
                    break;
                default:
                    break;
            }

            if(playerAbilitiesScript != null)
            {
                CameraController.FocusOnPlayer(playerAbilitiesScript.gameObject.transform);
                CommandMenuManager.GenerateCommandMenu(playerAbilitiesScript);
            }
            
        }
    }

    private GameObject GetNextRunner()
    {
        if(PlayerTurnAvailability.Count != 0 && !PlayerTurnAvailability.ContainsValue(TurnAvailabilityEnum.READY))
        {
            GameManager gameManager = GameUtils.FetchGameManager();
            gameManager.IsStateCheckAllowed = true;
            TurnState = TurnStateEnum.STANDBY;
            IsCommandPhase = false;
            return null;
        }

        List<GameObject> availableAndNotStayingRunnerList = GameManager.AttackTeamRunnerList
               .Where(runner => {
                   RunnerBehaviour runnerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(runner);
                   return this.IsplayerAvailable(runner.name) && !runnerBehaviour.IsStaying;
                   })
               .ToList();

        int runnerCount = availableAndNotStayingRunnerList.Count;
        if (CurrentRunner == null)
        {
            CurrentRunner = availableAndNotStayingRunnerList.First();
            CurrentIndex = 0;
            if (runnerCount == 1)
            {
                IsRunnersTurnsDone = true;
            }
        }
        else
        {
            int runnerIndex = availableAndNotStayingRunnerList.IndexOf(CurrentRunner);

            if (runnerIndex == runnerCount - 1)
            {
                CurrentRunner = availableAndNotStayingRunnerList.First();
                CurrentIndex = 0;
                IsRunnersTurnsDone = true;
            }
            else if (runnerIndex == -1)
            {
                //Index can't be found
                CurrentRunner = availableAndNotStayingRunnerList.First();
                CurrentIndex = 0;
            }
            else
            {
                int nextIndex = runnerIndex + 1;
                CurrentRunner = availableAndNotStayingRunnerList[nextIndex];
                CurrentIndex = nextIndex;
            }

            if (IsSkipNextRunnerTurnEnabled)
            {
                this.SkipNextRunner(runnerCount, availableAndNotStayingRunnerList);
            }
        }

        return CurrentRunner;
        
    }

    private void SkipNextRunner(int runnerCount, List<GameObject> availableRunnerList)
    {
        
            Debug.Log("Skipping " + CurrentRunner.name + " Turn");
            if (CurrentIndex == runnerCount - 1)
            {
                CurrentRunner = availableRunnerList[0];
                CurrentIndex = 0;
            }
            else
            {
                CurrentRunner = availableRunnerList[CurrentIndex + 1];
                CurrentIndex += 1;
            }

            IsSkipNextRunnerTurnEnabled = false;
            Debug.Log("The turn will be " + CurrentRunner.name + "'s");
    }

    public GameObject GetNextRunnerTakingAction()
    {
        List<GameObject> availableRunnerList = GameManager.AttackTeamRunnerList
                .Where(attackTeamRunner => {
                    RunnerBehaviour runnerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(attackTeamRunner);
                    return this.IsplayerAvailable(attackTeamRunner.name) && !runnerBehaviour.IsStaying;
                })
               .ToList();

        int runnerCount = availableRunnerList.Count;

        if(runnerCount == 0)
        {
            return null;
        }

        GameObject runner;

        if (CurrentRunner == null)
        {
            runner = availableRunnerList.First();
        }
        else
        {
            runner = CurrentRunner;
        }

        int runnerIndex = availableRunnerList.IndexOf(runner);

        if (runnerIndex == runnerCount - 1)
        {
            runner = availableRunnerList.First();
        }
        else
        {
            runner = availableRunnerList[runnerIndex + 1];
        }

        return runner;
    }

    public void UpdatePlayerTurnAvailability(string playerName, TurnAvailabilityEnum turnAvailabilityEnum)
    {
        if (!this.PlayerTurnAvailability.ContainsKey(playerName))
        {
            this.PlayerTurnAvailability.Add(playerName, turnAvailabilityEnum);
        }
        else
        {
            this.PlayerTurnAvailability[playerName] = turnAvailabilityEnum;
        }
    }

    public bool IsplayerAvailable(string playerName)
    {
        return this.PlayerTurnAvailability.ContainsKey(playerName) 
            && this.PlayerTurnAvailability[playerName].Equals(TurnAvailabilityEnum.READY);
    }

    public void MakeAllPlayerAvailable()
    {
        this.PlayerTurnAvailability = this.PlayerTurnAvailability
            .ToDictionary(entry => entry.Key
            , entry => TurnAvailabilityEnum.READY);
    }

    public CommandMenuManager CommandMenuManager { get => commandMenuManager; set => commandMenuManager = value; }
    public CameraController CameraController { get => cameraController; set => cameraController = value; }
    public PlayerFieldPositionEnum CurrentFielderTypeTurn { get => currentFielderTypeTurn; set => currentFielderTypeTurn = value; }
    public TurnStateEnum TurnState { get => turnState; set => turnState = value; }
    public TargetSelectionManager TargetSelectionManager { get => targetSelectionManager; set => targetSelectionManager = value; }
    public GameManager GameManager { get => gameManager; set => gameManager = value; }
    public GameObject CurrentRunner { get => currentRunner; set => currentRunner = value; }
    public bool IsRunnersTurnsDone { get => isRunnersTurnsDone; set => isRunnersTurnsDone = value; }
    public int CurrentIndex { get => currentIndex; set => currentIndex = value; }
    public bool IsSkipNextRunnerTurnEnabled { get => isSkipNextRunnerTurnEnabled; set => isSkipNextRunnerTurnEnabled = value; }
    public Dictionary<string, TurnAvailabilityEnum> PlayerTurnAvailability { get => playerTurnAvailability; set => playerTurnAvailability = value; }
}
