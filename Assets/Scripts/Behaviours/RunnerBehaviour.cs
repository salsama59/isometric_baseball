using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RunnerBehaviour : GenericPlayerBehaviour
{
    private bool hasReachedFirstBase = false;
    private bool hasReachedSecondBase = false;
    private bool hasReachedThirdBase = false;
    private bool hasReachedHomeBase = false;
    private bool enableMovement = false;
    private bool isSafe = false;
    private bool isInWalkState = false;
    private bool isStaying = false;

    public override void Start()
    {
        base.Start();
        IsoRenderer.PreferredDirection = 4;
        PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
        if (playerStatus.IsAllowedToMove)
        {
            this.InitiateRunnerAction(playerStatus);
        }
    }

    public override void Awake()
    {
        base.Awake();
    }

    public void Update()
    {
        if (EnableMovement)
        {
            if(Target.HasValue && Target.Value != this.transform.position)
            {
                MovePlayer();
                this.IsPrepared = true;
            }
            else
            {
                EnableMovement = false;
            }
            
        }
    }

    private void InitiateRunnerAction(PlayerStatus playerStatus)
    {
        Nullable<Vector3> targetPosition;
        targetPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFirstBaseTilePosition());
        this.CurrentBase = BaseEnum.NONE;
        this.NextBase = BaseEnum.HOME_BASE;
        Target = targetPosition;
    }


    public void GoToNextBase(BaseEnum currentBase, bool isAutomaticCommand = false)
    {
        Debug.Log(this.name + " go to next base");
        this.IsStaying = false;
        
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        playersTurnManager.UpdatePlayerTurnAvailability(this.gameObject.name, TurnAvailabilityEnum.WAITING);

        Vector3 nextPosition = this.GetNextBasePosition(currentBase);

        this.Target = nextPosition;
        this.EnableMovement = true;

        GameManager gameManager = GameUtils.FetchGameManager();

        if (!isAutomaticCommand)
        {
            Debug.Log(this.name + " proceed manually");
            this.CalculateNextAction();
        }
        else if (isAutomaticCommand && gameManager.AttackTeamRunnerList.Count > 1)
        {

            if (this.gameObject.Equals(playersTurnManager.GetNextRunnerTakingAction()))
            {
                playersTurnManager.IsSkipNextRunnerTurnEnabled = true;
            }

            Debug.Log(this.name + " proceed automaticaly");

            playersTurnManager.TurnState = TurnStateEnum.RUNNER_TURN;
            PlayersTurnManager.IsCommandPhase = true;
        }
    }

    private Vector3 GetNextBasePosition(BaseEnum currentBase)
    {
        Vector3 nextPosition = new Vector3();

        switch (currentBase)
        {
            case BaseEnum.HOME_BASE:
                Debug.Log("GO FROM HOME BASE TO FIRST BASE !!");
                nextPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFirstBaseTilePosition());
                break;
            case BaseEnum.FIRST_BASE:
                Debug.Log("GO FROM FIRST BASE TO SECOND BASE !!");
                nextPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetSecondBaseTilePosition());
                break;
            case BaseEnum.SECOND_BASE:
                Debug.Log("GO FROM SECOND BASE TO THIRD BASE !!");
                nextPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetThirdBaseTilePosition());
                break;
            case BaseEnum.THIRD_BASE:
                Debug.Log("GO FROM THIRD BASE TO HOME BASE !!");
                nextPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
                break;
            default:
                Debug.Log("DO NOT KNOW WHAT HAPPEN");
                break;
        }

        return nextPosition;
    }

    public void StayOnCurrentBase()
    {
        Debug.Log(this.name + " wait on current base");
        this.IsStaying = true;
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        playersTurnManager.UpdatePlayerTurnAvailability(this.gameObject.name, TurnAvailabilityEnum.WAITING);
        IsoRenderer.ReinitializeAnimator();

        GameManager gameManager = GameUtils.FetchGameManager();

        bool isRunnersAllSafeAndStaying = gameManager.AttackTeamRunnerList.TrueForAll(runner => {
            RunnerBehaviour runnerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(runner);
            return runnerBehaviour.IsSafe && runnerBehaviour.IsStaying;
        });

        //test if all remaining runner are out and there no more batters
        int runnersCount = gameManager.AttackTeamRunnerList.Count;
        int batterCount = gameManager.AttackTeamBatterListClone.Count;

        if (runnersCount == 0 && batterCount == 0 || batterCount == 0 && runnersCount > 0 && isRunnersAllSafeAndStaying)
        {
            gameManager.IsStateCheckAllowed = false;
            gameManager.ProcessNextInningHalf();
            return;
        }
        else if (isRunnersAllSafeAndStaying)
        {
            playersTurnManager.IsRunnersTurnsDone = true;
        }

        Vector3 nextBasePosition = this.GetNextBasePosition(this.CurrentBase);
        IsoRenderer.LookAtFieldElementAnimation(nextBasePosition);

        this.CalculateNextAction();
    }

    public void CalculateRunnerColliderInterraction(BaseEnum baseReached, bool isNextRunnerTurnPossible = false)
    {
        Debug.Log(this.name + " interracted with " + baseReached.ToString());
        this.CurrentBase = baseReached;

        PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
        this.EnableMovement = false;

        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        TurnAvailabilityEnum turnAvailabilityEnum = TurnAvailabilityEnum.READY;

        switch (baseReached)
        {
            case BaseEnum.HOME_BASE:
                
                if (baseReached == BaseEnum.HOME_BASE && !this.HasPassedThroughThreeFirstBases())
                {
                    Debug.Log("Get on HOME BASE FIRST TIME !!");
                    this.NextBase = BaseEnum.FIRST_BASE;
                    playersTurnManager.TurnState = TurnStateEnum.STANDBY;
                    playersTurnManager.UpdatePlayerTurnAvailability(this.gameObject.name, turnAvailabilityEnum);
                }
                else
                {
                    Debug.Log("Get on HOME BASE to mark a point");
                    Debug.Log("WIN ONE POINT !!!");
                    this.Target = null;
                    this.NextBase = this.CurrentBase;
                    playerStatusScript.IsAllowedToMove = false;
                    this.HasReachedHomeBase = true;

                    PlayerEnum playerEnum = playerStatusScript.PlayerOwner;
                    TeamsScoreManager teamsScoreManagerScript = GameUtils.FetchTeamsScoreManager();
                    teamsScoreManagerScript.IncrementTeamScore(GameData.teamIdEnumMap[playerEnum]);
                    this.IsStaying = true;
                    IsoRenderer.ReinitializeAnimator();
                    GameManager gameManager = GameUtils.FetchGameManager();
                    
                    this.gameObject.SetActive(false);
                    gameManager.AttackTeamRunnerList.Remove(this.gameObject);
                    playersTurnManager.PlayerTurnAvailability.Remove(this.gameObject.name);
                    gameManager.AttackTeamRunnerHomeList.Add(this.gameObject);

                    int batterCount = gameManager.AttackTeamBatterListClone.Count;
                    if (this.EquipedBat != null && batterCount > 0)
                    {
                        GameObject newBatter = gameManager.AttackTeamBatterListClone.First();
                        BatterBehaviour newbatterBehaviourScript = PlayerUtils.FetchBatterBehaviourScript(newBatter);
                        newbatterBehaviourScript.EquipedBat = this.EquipedBat;
                        this.EquipedBat = null;
                        gameManager.EquipBatToPlayer(newBatter);
                    }
                   
                    int runnersCount = gameManager.AttackTeamRunnerList.Count;
                    
                    bool isRunnersAllSafeAndStaying = gameManager.AttackTeamRunnerList.TrueForAll(runner => {
                        RunnerBehaviour runnerBehaviour = PlayerUtils.FetchRunnerBehaviourScript(runner);
                        return runnerBehaviour.IsSafe && runnerBehaviour.IsStaying;
                    });

                    if (runnersCount == 0 && batterCount == 0 || batterCount == 0 && runnersCount > 0 && isRunnersAllSafeAndStaying)
                    {
                        gameManager.IsStateCheckAllowed = false;
                        gameManager.ProcessNextInningHalf();
                    }
                    else
                    {
                        gameManager.IsStateCheckAllowed = true;
                    }
                }
                
                break;
            case BaseEnum.FIRST_BASE:
                Debug.Log("Get on FIRST BASE");

                this.NextBase = BaseEnum.SECOND_BASE;
                this.HasReachedFirstBase = true;
                if (this.IsInWalkState)
                {
                    this.IsInWalkState = false;
                    turnAvailabilityEnum = TurnAvailabilityEnum.WAITING;
                    IsoRenderer.ReinitializeAnimator();
                }
                
                playersTurnManager.UpdatePlayerTurnAvailability(this.gameObject.name, turnAvailabilityEnum);
                break;
            case BaseEnum.SECOND_BASE:
                Debug.Log("Get on SECOND BASE");
                this.NextBase = BaseEnum.THIRD_BASE;
                this.HasReachedSecondBase = true;
                playersTurnManager.UpdatePlayerTurnAvailability(this.gameObject.name, turnAvailabilityEnum);
                break;
            case BaseEnum.THIRD_BASE:
                Debug.Log("Get on THIRD BASE");
                this.NextBase = BaseEnum.HOME_BASE;
                this.HasReachedThirdBase = true;
                playersTurnManager.UpdatePlayerTurnAvailability(this.gameObject.name, turnAvailabilityEnum);
                break;
            default:
                Debug.Log("DO NOT KNOW WHAT HAPPEN");
                break;
        }

        if (isNextRunnerTurnPossible)
        {
            playersTurnManager.TurnState = TurnStateEnum.RUNNER_TURN;
            PlayersTurnManager.IsCommandPhase = true;
        }
    }

    private void CalculateNextAction()
    {
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        if (playersTurnManager.IsRunnersTurnsDone)
        {
            playersTurnManager.TurnState = TurnStateEnum.STANDBY;
            PlayersTurnManager.IsCommandPhase = false;
            GameManager gameManager = GameUtils.FetchGameManager();
            gameManager.IsStateCheckAllowed = true;
            playersTurnManager.IsRunnersTurnsDone = false;
        }
        else
        {
            playersTurnManager.TurnState = TurnStateEnum.RUNNER_TURN;
            PlayersTurnManager.IsCommandPhase = true;
        }
    }

    public bool HasPassedThroughThreeFirstBases()
    {
        return this.HasReachedFirstBase && this.HasReachedSecondBase && this.HasReachedThirdBase;
    }

    public bool HasPassedThroughAllBases()
    {
        return this.HasPassedThroughThreeFirstBases() && this.HasReachedHomeBase;
    }

    public void ToggleRunnerSafeStatus()
    {
        this.IsSafe = !this.IsSafe;
    }

    public bool HasReachedFirstBase { get => hasReachedFirstBase; set => hasReachedFirstBase = value; }
    public bool HasReachedSecondBase { get => hasReachedSecondBase; set => hasReachedSecondBase = value; }
    public bool HasReachedThirdBase { get => hasReachedThirdBase; set => hasReachedThirdBase = value; }
    public bool HasReachedHomeBase { get => hasReachedHomeBase; set => hasReachedHomeBase = value; }
    public bool EnableMovement { get => enableMovement; set => enableMovement = value; }
    public bool IsSafe { get => isSafe; set => isSafe = value; }
    public bool IsInWalkState { get => isInWalkState; set => isInWalkState = value; }
    public bool IsStaying { get => isStaying; set => isStaying = value; }
}
