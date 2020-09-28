using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static GameManager FetchGameManager()
    {
        GameObject gameManagerObject = GameObject.FindGameObjectWithTag(TagsConstants.GAME_MANAGER_TAG);
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
        return gameManager;
    }

    public static PlayersTurnManager FetchPlayersTurnManager()
    {
        GameObject playersTurnManagerObject = GameObject.FindGameObjectWithTag(TagsConstants.PLAYERS_TURN_MANAGER_TAG);
        PlayersTurnManager playersTurnManager = playersTurnManagerObject.GetComponent<PlayersTurnManager>();
        return playersTurnManager;
    }

    public static CommandMenuManager FetchCommandMenuManager()
    {
        GameObject commandMenuManagerObject = GameObject.FindGameObjectWithTag(TagsConstants.COMMAND_MENU_MANAGER_TAG);
        CommandMenuManager commandMenuManager = commandMenuManagerObject.GetComponent<CommandMenuManager>();
        return commandMenuManager;
    }

    public static PlayerActionsManager FetchPlayerActionsManager()
    {
        GameObject playerActionsManagerObject = GameObject.FindGameObjectWithTag(TagsConstants.PLAYER_ACTIONS_MANAGER_TAG);
        PlayerActionsManager playerActionsManager = playerActionsManagerObject.GetComponent<PlayerActionsManager>();
        return playerActionsManager;
    }

    public static TeamsScoreManager FetchTeamsScoreManager()
    {
        GameObject teamsScoreManagerObject = GameObject.FindGameObjectWithTag(TagsConstants.TEAMS_SCORE_MANAGER_TAG);
        TeamsScoreManager teamsScoreManager = teamsScoreManagerObject.GetComponent<TeamsScoreManager>();
        return teamsScoreManager;
    }

    public static DialogBoxManager FetchDialogBoxManager()
    {
        GameObject dialogBoxManagerObject = GameObject.FindGameObjectWithTag(TagsConstants.DIALOG_BOX_MANAGER_TAG);
        DialogBoxManager dialogBoxManager = dialogBoxManagerObject.GetComponent<DialogBoxManager>();
        return dialogBoxManager;
    }

    public static TargetSelectionManager FetchTargetSelectionManager()
    {
        GameObject targetSelectionManagerObject = GameObject.FindGameObjectWithTag(TagsConstants.TARGET_SELECTION_MANAGER_TAG);
        TargetSelectionManager targetSelectionManager = targetSelectionManagerObject.GetComponent<TargetSelectionManager>();
        return targetSelectionManager;
    }
}