using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterBehaviour : GenericPlayerBehaviour
{

    private Quaternion targetRotation = Quaternion.Euler(0, 0, 45f);
    private bool isReadyToSwing = false;
    private bool isSwingHasFinished = false;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        IsSwingHasFinished = true;
        IsoRenderer.LastDirection = 6;
        IsoRenderer.SetDirection(Vector2.zero);
    }

    private void Update()
    {
        if (IsReadyToSwing)
        {
            StartCoroutine(RotatePlayer(this.gameObject));
            if (this.transform.rotation.eulerAngles.z >= targetRotation.eulerAngles.z)
            {
                IsReadyToSwing = false;
                IsSwingHasFinished = true;
                //this.transform.rotation = Quaternion.identity;
            }
        }
    }

    public void CalculateBatterColliderInterraction(GameObject pitcherGameObject, BallController ballControllerScript, PlayerStatus playerStatusScript)
    {
        float pitchSuccesRate = ActionCalculationUtils.CalculatePitchSuccessRate(pitcherGameObject, this.gameObject);
        Debug.Log("pitchSuccesRate = " + pitchSuccesRate);
        StartCoroutine(this.WaitForSwing(pitchSuccesRate, ballControllerScript, playerStatusScript));
        Debug.Log("After the coroutine to WaitForSwing");
    }

    private void DoBattingAction(BallController ballControllerScript, PlayerStatus playerStatusScript, float pitchSuccesRate)
    {
        if (!ActionCalculationUtils.HasActionSucceeded(pitchSuccesRate))
        {
            Debug.Log("Pitch has not succeed");
            Debug.Log("Batter has hit the ball");
            ballControllerScript.IsBeingHitten = true;
            List<Vector2Int> ballPositionList = ActionCalculationUtils.CalculateBallFallPositionList(this.gameObject, 0, 180, 10, true);
            int ballPositionIndex = Random.Range(0, ballPositionList.Count - 1);
            Vector2Int ballTilePosition = ballPositionList[ballPositionIndex];
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(ballTilePosition);
            GameObject bat = PlayerUtils.GetPlayerBatGameObject(this.gameObject);
            bat.SetActive(false);
            Destroy(this.gameObject.GetComponent<BatterBehaviour>());
            
            playerStatusScript.PlayerFieldPosition = PlayerFieldPositionEnum.RUNNER;
            TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.RUNNER, this.gameObject, PlayerEnum.PLAYER_1);
            PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
            playersTurnManager.turnState = TurnStateEnum.STANDBY;
            RunnerBehaviour runnerBehaviour = this.gameObject.AddComponent<RunnerBehaviour>();
            runnerBehaviour.EquipedBat = bat;
            PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
            PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(this.gameObject);
            playerAbilities.PlayerAbilityList.Clear();
            PlayerAbility runPlayerAbility = new PlayerAbility("Run to next base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.RunAction);
            PlayerAbility StaySafePlayerAbility = new PlayerAbility("Stay on base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.StayAction);
            playerAbilities.AddAbility(runPlayerAbility);
            playerAbilities.AddAbility(StaySafePlayerAbility);
            playerStatusScript.IsAllowedToMove = true;
            runnerBehaviour.EnableMovement = true;
        }
        else
        {
            Debug.Log("Pitch has succeeded");
            Debug.Log("Batter has not hit the ball");
            Debug.Log("Go to the catcher");
            ballControllerScript.IsThrown = true;
            ballControllerScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetCatcherZonePosition());
        }
    }

    private IEnumerator RotatePlayer(GameObject player)
    {
        player.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 130f * Time.deltaTime);
        yield return null;
    }

    private IEnumerator WaitForSwing(float pitchSuccesRate, BallController ballControllerScript, PlayerStatus playerStatusScript)
    {
        yield return new WaitUntil(() => IsSwingHasFinished == true);
        Debug.Log("During the coroutine to WaitForSwing");
        this.DoBattingAction(ballControllerScript, playerStatusScript, pitchSuccesRate);
    }

    public bool IsReadyToSwing { get => isReadyToSwing; set => isReadyToSwing = value; }
    public bool IsSwingHasFinished { get => isSwingHasFinished; set => isSwingHasFinished = value; }
}
