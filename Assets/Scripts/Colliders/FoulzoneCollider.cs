using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoulzoneCollider : MonoBehaviour
{
    private float timeElapsed = 0f;
    private const float TIME_TO_WAIT_IN_FOUL_ZONE = 2f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ColliderUtils.HasBallCollided(collision))
        {
            GameObject ball = collision.gameObject;
            BallController ballControllerScript = BallUtils.FetchBallControllerScript(ball);

            if (ballControllerScript.IsHit)
            {
                ballControllerScript.IsInFoulState = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(ColliderUtils.HasBallCollided(collision) && !GameData.isPaused)
        {
            GameObject ball = collision.gameObject;
            BallController ballControllerScript = BallUtils.FetchBallControllerScript(ball);

            if (ballControllerScript.IsHit)
            {

                timeElapsed += Time.deltaTime;

                if(timeElapsed >= TIME_TO_WAIT_IN_FOUL_ZONE)
                {
                    Debug.Log("the ball is foul");
                    timeElapsed = 0;
                    
                    DialogBoxManager dialogBoxManagerScript = GameUtils.FetchDialogBoxManager();
                    dialogBoxManagerScript.DisplayDialogAndTextForGivenAmountOfTime(1f, false, "FOUL!!");
                    PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
                    GameObject pitcher = TeamUtils.GetPlayerTeamMember(PlayerFieldPositionEnum.PITCHER, TeamUtils.GetPlayerIdFromPlayerFieldPosition(PlayerFieldPositionEnum.PITCHER));
                    GameManager gameManager = GameUtils.FetchGameManager();
                    GameObject currentBatter = gameManager.AttackTeamBatterListClone.First();
                    BatterBehaviour currentBatterBehaviour = PlayerUtils.FetchBatterBehaviourScript(currentBatter);
                    GameObject bat = currentBatterBehaviour.EquipedBat;
                    currentBatterBehaviour.FoulOutcomeCount += 1;
                    currentBatter.transform.rotation = Quaternion.identity;
                    bat.transform.position = FieldUtils.GetBatCorrectPosition(currentBatter.transform.position);
                    bat.transform.rotation = Quaternion.Euler(0f, 0f, -70f);
                    gameManager.ReinitPitcher(pitcher);
                    gameManager.ReturnBallToPitcher(ballControllerScript.gameObject);
                    gameManager.ReinitRunners(gameManager.AttackTeamRunnerList);
                    ballControllerScript.IsInFoulState = false;
                    playersTurnManager.TurnState = TurnStateEnum.PITCHER_TURN;
                    PlayersTurnManager.IsCommandPhase = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (ColliderUtils.HasBallCollided(collision))
        {
            GameObject ball = collision.gameObject;
            BallController ballControllerScript = BallUtils.FetchBallControllerScript(ball);

            if (ballControllerScript.IsHit)
            {
                Debug.Log("the ball not foul any more");
                ballControllerScript.IsInFoulState = false;
                timeElapsed = 0;
            }
        }
    }
}
