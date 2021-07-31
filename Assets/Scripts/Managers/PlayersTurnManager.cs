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
    private GameObject nextRunner;

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

    public void UpdatePlayerTurnQueue(GameObject player)
    {
       this.playerTurnAvailability.Remove(player.name);
        if (player == this.NextRunner)
        {
            List<GameObject> runnerList = GameManager.AttackTeamRunnerListClone;
            int runnerListCount = runnerList.Count;
            int currentIndex = runnerList.IndexOf(this.CurrentRunner);
            this.NextRunner = this.FullRunnerSearch(currentIndex, runnerListCount, runnerList);
        }
    }

    private GameObject GetNextRunner()
    {
        GameObject foundRunner;

        if(this.NextRunner == null)
        {
            List<GameObject> runnerList = GameManager.AttackTeamRunnerListClone;
            int runnerListCount = runnerList.Count;
            int currentIndex = runnerList.IndexOf(this.CurrentRunner);
            foundRunner = this.FullRunnerSearch(currentIndex, runnerListCount, runnerList);
        }
        else
        {
            int precalculatedRunnerIndex = GameManager.AttackTeamRunnerListClone.IndexOf(this.NextRunner);
            RunnerBehaviour runnerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(this.NextRunner);
            if (this.IsplayerAvailable(this.NextRunner.name) && !runnerBehaviour.IsStaying && this.NextRunner.activeInHierarchy)
            {
                this.CurrentRunner = this.NextRunner;
                

                if (precalculatedRunnerIndex != GameManager.AttackTeamRunnerListClone.Count - 1)
                {
                    this.NextRunner = GameManager.AttackTeamRunnerListClone[precalculatedRunnerIndex + 1];
                }
                else
                {
                    this.NextRunner = GameManager.AttackTeamRunnerListClone.First();
                }
                foundRunner = this.CurrentRunner;
            }
            else
            {
                foundRunner = this.FullRunnerSearch(precalculatedRunnerIndex, GameManager.AttackTeamRunnerListClone.Count, GameManager.AttackTeamRunnerListClone);
            }
        }

        if(foundRunner == null)
        {
            IsRunnersTurnsDone = true;
            gameManager.IsStateCheckAllowed = true;
            TurnState = TurnStateEnum.STANDBY;
            IsCommandPhase = false;
        }

        return foundRunner;
    }

    private GameObject FullRunnerSearch(int currentIndex, int runnerListCount, List<GameObject> runnerList)
    {
        GameObject foundRunner = null;

        int startSearchIndex;

        if (currentIndex == runnerListCount - 1)
        {
            startSearchIndex = 0;
        }
        else
        {
            startSearchIndex = currentIndex + 1;
        }

        for (int i = startSearchIndex; i < runnerListCount; i++)
        {
            foundRunner = this.FilterCurrentSearchRunner(i, runnerList, runnerListCount);
            if (foundRunner != null)
            {
                break;
            }

        }

        //Do reverse search only in some cases
        if (startSearchIndex != 0 && startSearchIndex != runnerListCount - 1 && foundRunner == null)
        {
            for (int i = startSearchIndex - 1; i > 0; i--)
            {
                foundRunner = this.FilterCurrentSearchRunner(i, runnerList, runnerListCount);
                if (foundRunner != null)
                {
                    break;
                }
            }
        }

        return foundRunner;
    }

    private GameObject FilterCurrentSearchRunner(int i, List<GameObject> runnerList, int runnerListCount)
    {
        GameObject currentSearchRunner = runnerList[i];
        RunnerBehaviour runnerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(currentSearchRunner);

        if (this.IsplayerAvailable(currentSearchRunner.name) && !runnerBehaviour.IsStaying && currentSearchRunner.activeInHierarchy)
        {

            if (i != runnerListCount - 1)
            {
                this.NextRunner = runnerList[i + 1];
            }
            else
            {
                this.NextRunner = runnerList.First();
            }

            this.CurrentRunner = currentSearchRunner;
            return this.CurrentRunner;
        }

        return null;
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
           this.playerTurnAvailability.Add(playerName, turnAvailabilityEnum);
        }
        else
        {
           this.playerTurnAvailability[playerName] = turnAvailabilityEnum;
        }
    }

    public bool IsplayerAvailable(string playerName)
    {
        return this.playerTurnAvailability.ContainsKey(playerName) 
            && this.playerTurnAvailability[playerName].Equals(TurnAvailabilityEnum.READY);
    }

    public void MakeAllPlayerAvailable()
    {
       this.playerTurnAvailability =this.playerTurnAvailability
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
    public GameObject NextRunner { get => nextRunner; set => nextRunner = value; }
}
