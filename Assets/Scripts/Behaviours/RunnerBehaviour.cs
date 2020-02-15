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

    public override void Start()
    {
        base.Start();
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
        /*else
        {
            PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            if (playerStatus.IsAllowedToMove)
            {
                this.InitiateRunnerAction(playerStatus);
            }
        }*/
    }

    private void InitiateRunnerAction(PlayerStatus playerStatus)
    {
        Nullable<Vector3> targetPosition = new Nullable<Vector3>();
        targetPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFirstBaseTilePosition());
        this.CurrentBase = BaseEnum.NONE;
        this.NextBase = BaseEnum.HOME_BASE;
        Target = targetPosition;
    }

    public void CalculateRunnerColliderInterraction(BaseEnum baseReached)
    {

        this.CurrentBase = baseReached;

        PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(this.gameObject);

        switch (baseReached)
        {
            case BaseEnum.HOME_BASE:

                if (baseReached == BaseEnum.HOME_BASE && !this.HasPassedThroughThreeFirstBases())
                {
                    Debug.Log("Get on HOME BASE FIRST TIME !!");
                    this.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFirstBaseTilePosition());
                    this.NextBase = BaseEnum.FIRST_BASE;
                    this.HasReachedFirstBase = true;
                    EnableMovement = true;
                }
                else
                {
                    Debug.Log("Get on HOME BASE to mark a point");
                    Debug.Log("WIN ONE POINT !!!");
                    this.Target = null;
                    this.NextBase = this.CurrentBase;
                    playerStatusScript.IsAllowedToMove = false;
                    this.HasReachedHomeBase = true;
                }
                
                break;
            case BaseEnum.FIRST_BASE:
                Debug.Log("Get on FIRST BASE");
                this.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetSecondBaseTilePosition());
                this.NextBase = BaseEnum.SECOND_BASE;
                this.HasReachedFirstBase = true;
                EnableMovement = true;
                break;
            case BaseEnum.SECOND_BASE:
                Debug.Log("Get on SECOND BASE");
                this.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetThirdBaseTilePosition());
                this.NextBase = BaseEnum.THIRD_BASE;
                this.HasReachedSecondBase = true;
                EnableMovement = true;
                break;
            case BaseEnum.THIRD_BASE:
                Debug.Log("Get on THIRD BASE");
                this.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
                this.NextBase = BaseEnum.HOME_BASE;
                this.HasReachedThirdBase = true;
                EnableMovement = true;
                break;
            default:
                Debug.Log("DO NOT KNOW WHAT HAPPEN");
                break;
        }

        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        playersTurnManager.turnState = TurnStateEnum.STANDBY;
        PlayersTurnManager.IsCommandPhase = false;
    }

    public bool HasPassedThroughThreeFirstBases()
    {
        return this.HasReachedFirstBase && this.HasReachedSecondBase && this.HasReachedThirdBase;
    }

    public bool HasReachedFirstBase { get => hasReachedFirstBase; set => hasReachedFirstBase = value; }
    public bool HasReachedSecondBase { get => hasReachedSecondBase; set => hasReachedSecondBase = value; }
    public bool HasReachedThirdBase { get => hasReachedThirdBase; set => hasReachedThirdBase = value; }
    public bool HasReachedHomeBase { get => hasReachedHomeBase; set => hasReachedHomeBase = value; }
    public bool EnableMovement { get => enableMovement; set => enableMovement = value; }
}
