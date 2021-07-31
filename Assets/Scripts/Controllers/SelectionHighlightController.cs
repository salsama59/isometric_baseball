using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionHighlightController: MonoBehaviour
{

    private TeamsSelectionManager teamsSelectionManager;
    private bool isTeamChoosed = false;

    [SerializeField]
    private PlayerEnum playerEnum;
    // Start is called before the first frame update
    void Start()
    {
        teamsSelectionManager = GameUtils.FetchTeamsSelectionManager();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerEnum == PlayerEnum.PLAYER_1)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && !this.isTeamChoosed)
            {
                teamsSelectionManager.MoveHighligtht(playerEnum, SelectionMovementEnum.LEFT);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) && !this.isTeamChoosed)
            {
                teamsSelectionManager.MoveHighligtht(playerEnum, SelectionMovementEnum.RIGTH);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && !this.isTeamChoosed)
            {
                teamsSelectionManager.MoveHighligtht(playerEnum, SelectionMovementEnum.UP);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && !this.isTeamChoosed)
            {
                teamsSelectionManager.MoveHighligtht(playerEnum, SelectionMovementEnum.DOWN);
            }

            if (Input.GetKeyDown(KeyCode.Return) && !this.isTeamChoosed)
            {
                teamsSelectionManager.ChooseTeam(playerEnum);
                this.isTeamChoosed = true;
            }

            if (Input.GetKeyDown(KeyCode.AltGr))
            {
                teamsSelectionManager.UnChooseTeam(playerEnum);
                this.isTeamChoosed = false;
            }

        } else if(playerEnum == PlayerEnum.PLAYER_2)
        {
            if (Input.GetKeyDown(KeyCode.Q) && !this.isTeamChoosed)
            {
                teamsSelectionManager.MoveHighligtht(playerEnum, SelectionMovementEnum.LEFT);
            }

            if (Input.GetKeyDown(KeyCode.D) && !this.isTeamChoosed)
            {
                teamsSelectionManager.MoveHighligtht(playerEnum, SelectionMovementEnum.RIGTH);
            }

            if (Input.GetKeyDown(KeyCode.Z) && !this.isTeamChoosed)
            {
                teamsSelectionManager.MoveHighligtht(playerEnum, SelectionMovementEnum.UP);
            }

            if (Input.GetKeyDown(KeyCode.S) && !this.isTeamChoosed)
            {
                teamsSelectionManager.MoveHighligtht(playerEnum, SelectionMovementEnum.DOWN);
            }

            if (Input.GetKeyDown(KeyCode.Space) && !this.isTeamChoosed)
            {
                teamsSelectionManager.ChooseTeam(playerEnum);
                this.isTeamChoosed = true;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                teamsSelectionManager.UnChooseTeam(playerEnum);
                this.isTeamChoosed = false;
            }
        }
    }
}
