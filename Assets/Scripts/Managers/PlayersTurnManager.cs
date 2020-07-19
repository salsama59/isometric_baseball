using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersTurnManager : MonoBehaviour
{

    private TurnStateEnum turnState;
    public static bool IsCommandPhase;
    private CommandMenuManager commandMenuManager;
    private CameraController cameraController;
    private PlayerFieldPositionEnum currentFielderTypeTurn;
    private TargetSelectionManager targetSelectionManager;

    private void Start()
    {
        TargetSelectionManager = GameUtils.FetchTargetSelectionManager();
        CameraController = CameraUtils.FetchCameraController();
        CommandMenuManager = GameUtils.FetchCommandMenuManager();
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
                    GameObject pitcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.PITCHER, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.PITCHER));
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(pitcher);
                    break;
                case TurnStateEnum.BATTER_TURN:
                    GameObject batter = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.BATTER, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.BATTER));
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(batter);
                    break;
                case TurnStateEnum.RUNNER_TURN:
                    GameObject runner = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.RUNNER, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.RUNNER));
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(runner);
                    break;
                case TurnStateEnum.CATCHER_TURN:
                    GameObject catcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.CATCHER, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(PlayerFieldPositionEnum.CATCHER));
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(catcher);
                    break;
                case TurnStateEnum.FIELDER_TURN:
                    GameObject fielder = TeamUtils.GetPlayerTeamMember(currentFielderTypeTurn, TeamUtils.GetPlayerEnumEligibleToPlayerPositionEnum(currentFielderTypeTurn));
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

    public CommandMenuManager CommandMenuManager { get => commandMenuManager; set => commandMenuManager = value; }
    public CameraController CameraController { get => cameraController; set => cameraController = value; }
    public PlayerFieldPositionEnum CurrentFielderTypeTurn { get => currentFielderTypeTurn; set => currentFielderTypeTurn = value; }
    public TurnStateEnum TurnState { get => turnState; set => turnState = value; }
    public TargetSelectionManager TargetSelectionManager { get => targetSelectionManager; set => targetSelectionManager = value; }
}
