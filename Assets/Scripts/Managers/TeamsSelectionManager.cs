using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamsSelectionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject selectionScreenModel = null;
    [SerializeField]
    private GameObject teamButtonModel = null;

    [SerializeField]
    private GameObject playerOneSelectionHighlightModel = null;
    [SerializeField]
    private GameObject playerTwoSelectionHighlightModel = null;

    private GameObject playerOneSelectionHighlight = null;
    private GameObject playerTwoSelectionHighlight = null;

    private GameObject selectionScreenGameObject = null;

    private TeamSelectionElement[,] teamSelectionElementMap;

    private static int ROW_INDEX = 0;
    private static int COLUMN_INDEX = 1;

    private TeamSelectionCoordinates playerOneCoordinates;
    private TeamSelectionCoordinates playerTwoCoordinates;



    // Start is called before the first frame update
    void Start()
    {
        TeamData[] teamDatas = this.LoadTeamDatas();

        this.selectionScreenGameObject = Instantiate(selectionScreenModel, this.transform.position, this.transform.rotation);

        GameObject selectionScreenCanvas = selectionScreenGameObject.GetComponentInChildren<Canvas>().gameObject;
        GameObject selectionScreenPanel = selectionScreenCanvas.GetComponentInChildren<Image>().gameObject;

        
        int maximumCollumnCount = selectionScreenPanel.GetComponent<GridLayoutGroup>().constraintCount;
        int maximumRowCount = Mathf.CeilToInt(teamDatas.Length / maximumCollumnCount);

        teamSelectionElementMap = new TeamSelectionElement[maximumRowCount, maximumCollumnCount];

        int row = 0;
        int collumn = 0;
        foreach (TeamData teamData in teamDatas)
        {
            TeamSelectionElement teamSelectionElement = new TeamSelectionElement();
            teamSelectionElement.Team = teamData;
            
            GameObject teamButton = Instantiate(teamButtonModel, selectionScreenPanel.transform);
            TextMeshProUGUI textMeshProGui = teamButton.GetComponentInChildren<TextMeshProUGUI>();
            textMeshProGui.text = teamData.TeamShortName;

            teamSelectionElement.UserInterfaceElement = teamButton;
            teamSelectionElementMap[row, collumn] = teamSelectionElement;

            if (row < teamSelectionElementMap.GetLength(ROW_INDEX) - 1)
            {
                row++;
            }

            if(collumn == teamSelectionElementMap.GetLength(COLUMN_INDEX) - 1)
            {
                collumn = 0;
            } else
            {
                collumn++;
            }
        }

        playerOneCoordinates = new TeamSelectionCoordinates(0, 0);
        playerTwoCoordinates = new TeamSelectionCoordinates(maximumRowCount - 1, maximumCollumnCount - 1);
        this.playerOneSelectionHighlight = Instantiate(playerOneSelectionHighlightModel, teamSelectionElementMap[playerOneCoordinates.Row, playerOneCoordinates.Collumn].UserInterfaceElement.transform);
        this.playerTwoSelectionHighlight = Instantiate(playerTwoSelectionHighlightModel, teamSelectionElementMap[playerTwoCoordinates.Row, playerTwoCoordinates.Collumn].UserInterfaceElement.transform);
    }

    // Update is called once per frame
    void Update()                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
    {
        if (IsAllPlayersChoosedTheirTeams())
        {
            SceneManager.LoadScene(SceneConstants.MATCH_SCENE_NAME);
        }
    }

    public void MoveHighligtht(PlayerEnum playerEnum, SelectionMovementEnum selectionMovementEnum)
    {
        if(playerEnum == PlayerEnum.PLAYER_1)
        {
            this.MovePlayerHighligth(this.playerOneSelectionHighlight, playerOneCoordinates, selectionMovementEnum);
        } else
        {
            this.MovePlayerHighligth(this.playerTwoSelectionHighlight, playerTwoCoordinates, selectionMovementEnum);
        }
    }


    private void MovePlayerHighligth(GameObject selectionHighligthObject, TeamSelectionCoordinates teamSelectionCoordinates, SelectionMovementEnum selectionMovementEnum)
    {

        TeamSelectionCoordinates calculatedCoordinates = this.CalculateNewCoordinates(teamSelectionCoordinates, selectionMovementEnum);
        int row = calculatedCoordinates.Row;
        int collumn = calculatedCoordinates.Collumn;

        TeamSelectionElement teamSelectionElement = teamSelectionElementMap[row, collumn];
        selectionHighligthObject.transform.SetParent(teamSelectionElement.UserInterfaceElement.transform);
        selectionHighligthObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

        teamSelectionCoordinates.Collumn = collumn;
        teamSelectionCoordinates.Row = row;
    }

    private TeamSelectionCoordinates CalculateNewCoordinates(TeamSelectionCoordinates teamSelectionCoordinates, SelectionMovementEnum selectionMovementEnum)
    {
        TeamSelectionCoordinates newCoordinates = new TeamSelectionCoordinates(teamSelectionCoordinates.Row, teamSelectionCoordinates.Collumn);

        int row = teamSelectionCoordinates.Row;
        int collumn = teamSelectionCoordinates.Collumn;

        switch (selectionMovementEnum)
        {
            case SelectionMovementEnum.LEFT:
                if (collumn == 0)
                {
                    collumn = teamSelectionElementMap.GetLength(COLUMN_INDEX) - 1;
                }
                else
                {
                    collumn--;
                }
                break;
            case SelectionMovementEnum.RIGTH:
                if (collumn == teamSelectionElementMap.GetLength(COLUMN_INDEX) - 1)
                {
                    collumn = 0;
                }
                else
                {
                    collumn++;
                }
                break;
            case SelectionMovementEnum.UP:
                if (row == 0)
                {
                    row = teamSelectionElementMap.GetLength(ROW_INDEX) - 1;
                }
                else
                {
                    row--;
                }
                break;
            case SelectionMovementEnum.DOWN:
                if (row == teamSelectionElementMap.GetLength(ROW_INDEX) - 1)
                {
                    row = 0;
                }
                else
                {
                    row++;
                }
                break;
        }

        newCoordinates.Collumn = collumn;
        newCoordinates.Row = row;

        return newCoordinates;
    }


    private TeamData[] LoadTeamDatas()
    {
        string jsonTeamDatasPath = Application.dataPath + "/GameDatas/model-exporter.json";
        string jsonString = File.ReadAllText(jsonTeamDatasPath);
        return JsonUtils.FromJson<TeamData>("{\n\t\"items\":\t" + jsonString + "\n}");
    }

    public void ChooseTeam(PlayerEnum playerEnum)
    {
        int row;
        int column;

        switch (playerEnum)
        {
            case PlayerEnum.PLAYER_1:
                row = playerOneCoordinates.Row;
                column = playerOneCoordinates.Collumn;
                GameData.playerOneTeamChoice = teamSelectionElementMap[row, column].Team;
                break;
            case PlayerEnum.PLAYER_2:
                row = playerTwoCoordinates.Row;
                column = playerTwoCoordinates.Collumn;
                GameData.playerTwoTeamChoice = teamSelectionElementMap[row, column].Team;
                break;
        }
    }

    public void UnChooseTeam(PlayerEnum playerEnum)
    {
        switch (playerEnum)
        {
            case PlayerEnum.PLAYER_1:
                GameData.playerOneTeamChoice = null;
                break;
            case PlayerEnum.PLAYER_2:
                GameData.playerTwoTeamChoice = null;
                break;
        }
    }

    private bool IsAllPlayersChoosedTheirTeams()
    {
        bool isPlayerOneChoosedHisTeam = GameData.playerOneTeamChoice != null;
        bool isPlayerTwoChoosedHisTeam = GameData.playerTwoTeamChoice != null;
        return isPlayerOneChoosedHisTeam && isPlayerTwoChoosedHisTeam;
    }
}
