﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersTurnManager : MonoBehaviour
{

    public TurnStateEnum turnState;
    public static bool IsCommandPhase;
    private CommandMenuManager commandMenuManager;

    private void Start()
    {
        CommandMenuManager = GameUtils.FetchCommandMenuManager();
    }

    // Update is called once per frame
    void Update()
    {
        if (CommandMenuManager.isMenuDisplayEnabled)
        {

            PlayerAbilities playerAbilitiesScript = null;

            switch (this.turnState)
            {
                case TurnStateEnum.STANDBY:
                    break;
                case TurnStateEnum.PITCHER_TURN:
                    GameObject pitcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.PITCHER, PlayerEnum.PLAYER_1);
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(pitcher);
                    break;
                case TurnStateEnum.BATTER_TURN:
                    GameObject batter = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.BATTER, PlayerEnum.PLAYER_1);
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(batter);
                    break;
                case TurnStateEnum.RUNNER_TURN:
                    GameObject runner = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.RUNNER, PlayerEnum.PLAYER_1);
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(runner);
                    break;
                case TurnStateEnum.CATCHER_TURN:
                    GameObject catcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.CATCHER, PlayerEnum.PLAYER_1);
                    playerAbilitiesScript = PlayerUtils.FetchPlayerAbilitiesScript(catcher);
                    break;
                default:
                    break;
            }

            if(playerAbilitiesScript != null)
            {
                CommandMenuManager.GenerateCommandMenu(playerAbilitiesScript);
            }
            
        }
    }

    public CommandMenuManager CommandMenuManager { get => commandMenuManager; set => commandMenuManager = value; }
}
