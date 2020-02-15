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
}
