using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class TeamsScoreManager : MonoBehaviour
{

    public GameObject scoreDisplayModel;
    private Dictionary<int, int> teamsScore;
    private GameObject scoreGameObject;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //this.ToggleScoreUiDisplay();
        TeamsScore = new Dictionary<int, int>();
        this.InitializeTeamsScore();
        gameManager = GameUtils.FetchGameManager();
    }

    private void Awake()
    {
        if(ScoreGameObject == null)
        {
            ScoreGameObject = Instantiate(scoreDisplayModel, this.transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateTeamsScore();
        this.UpdateInning();
    }

    public void UpdateInning()
    {
        GameObject inningCountField =  GameObject.FindGameObjectWithTag(TagsConstants.INNING_COUNT_TAG);
        GameObject inningPhaseField = GameObject.FindGameObjectWithTag(TagsConstants.INNING_PHASE_TAG);
        TextMeshProUGUI inningCountText = inningCountField.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI inningPhaseText = inningPhaseField.GetComponentInChildren<TextMeshProUGUI>();
        inningCountText.text = gameManager.InningCount.ToString();
        inningPhaseText.text = gameManager.CurrentInningPhase.ToString();
    }

    public void ToggleScoreUiDisplay()
    {
        ScoreGameObject.SetActive(!ScoreGameObject.activeInHierarchy);
    }

    public void UpdateTeamScore(TeamIdEnum teamIdEnum)
    {
        int teamId = (int)teamIdEnum;
        string teamsInformationPanelTag = TagsConstants.TEAMS_INFORMATIONS_PANEL_TAG;
        string scoreTextTag;

        switch (teamIdEnum)
        {
            case TeamIdEnum.TEAM_1:
                scoreTextTag = TagsConstants.TEAM_1_SCORE_TAG;
                break;
            case TeamIdEnum.TEAM_2:
                scoreTextTag = TagsConstants.TEAM_2_SCORE_TAG;
                break;
            default:
                scoreTextTag = "";
                break;
        }

        Canvas scoreCanvas = ScoreGameObject.GetComponentInChildren<Canvas>();
        GameObject informationsPanelGameObject = scoreCanvas.gameObject.GetComponentsInChildren<RectTransform>().Where(panelGameObjectRectTransform => panelGameObjectRectTransform.gameObject.CompareTag(teamsInformationPanelTag)).First().gameObject;
        TextMeshProUGUI teamSCoreText = informationsPanelGameObject.GetComponentsInChildren<TextMeshProUGUI>().Where(teamScoreText => teamScoreText.gameObject.CompareTag(scoreTextTag)).First();
        teamSCoreText.text = TeamsScore[teamId].ToString();
    }

    public void UpdateTeamName(TeamIdEnum teamIdEnum, string teamName)
    {
        int teamId = (int)teamIdEnum;
        string teamsInformationsPanelTag = TagsConstants.TEAMS_INFORMATIONS_PANEL_TAG;
        string nameTextTag;

        switch (teamIdEnum)
        {
            case TeamIdEnum.TEAM_1:
                nameTextTag = TagsConstants.TEAM_1_NAME_TAG;
                break;
            case TeamIdEnum.TEAM_2:
                nameTextTag = TagsConstants.TEAM_2_NAME_TAG;
                break;
            default:
                nameTextTag = "";
                break;
        }

        Canvas scoreCanvas = ScoreGameObject.GetComponentInChildren<Canvas>();
        GameObject informationsPanelGameObject = scoreCanvas.gameObject.GetComponentsInChildren<RectTransform>().Where(panelGameObjectRectTransform => panelGameObjectRectTransform.CompareTag(teamsInformationsPanelTag)).First().gameObject;
        TextMeshProUGUI teamNameText = informationsPanelGameObject.GetComponentsInChildren<TextMeshProUGUI>().Where(teamNameTextComponent => teamNameTextComponent.gameObject.CompareTag(nameTextTag)).First();
        teamNameText.text = teamName;
    }

    public void IncrementTeamScore(TeamIdEnum teamIdEnum)
    {
        int teamId = (int)teamIdEnum;
        int newScore = TeamsScore[teamId] + 1;
        TeamsScore[teamId] = newScore;
    }

    public TeamIdEnum CalculateCurrentWinnerTeam()
    {
        TeamIdEnum result;

        int team1score = TeamsScore[(int)TeamIdEnum.TEAM_1];
        int team2score = TeamsScore[(int)TeamIdEnum.TEAM_2];

        if (team1score > team2score)
        {
            result = TeamIdEnum.TEAM_1;
        }
        else if(team2score > team1score)
        {
            result = TeamIdEnum.TEAM_2;
        }
        else
        {
            result = TeamIdEnum.NO_TEAM;
        }

        return result;
    }

    private void UpdateTeamsScore()
    {
        TeamIdEnum teamIdEnum = TeamIdEnum.TEAM_1;

        for (int i = 0; i < 2; i++)
        {
            if (i == (int)TeamIdEnum.TEAM_1)
            {
                teamIdEnum = TeamIdEnum.TEAM_1;
            }
            else if(i == (int)TeamIdEnum.TEAM_2)
            {
                teamIdEnum = TeamIdEnum.TEAM_2;
            }

            this.UpdateTeamScore(teamIdEnum);
        }

    }

    private void InitializeTeamsScore()
    {
        for (int i = 0; i < GameData.playerNumber; i++)
        {
            TeamsScore.Add(i, 0);
        }
    }

    public Dictionary<int, int> TeamsScore { get => teamsScore; set => teamsScore = value; }
    public GameObject ScoreGameObject { get => scoreGameObject; set => scoreGameObject = value; }
}