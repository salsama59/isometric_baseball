using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RunnerBehaviour : GenericPlayerBehaviour
{
    public override void Awake()
    {
        base.Awake();
    }

    public void Update()
    {
        if (!IsMoving)
        {
            if (Target.HasValue && Target.Value != this.transform.position)
            {
                StartCoroutine(Move(transform.position, Target.Value));
                this.IsPrepared = true;
            }
            else
            {
                PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
                if (playerStatus.IsAllowedToMove)
                {
                    this.Act(playerStatus);
                }
            }
        }
        else
        {
            if (TargetPlayerToTagOut != null && (PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.FIRST_BASEMAN) || PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.THIRD_BASEMAN)))
            {
                Target = TargetPlayerToTagOut.transform.position;

            }
        }
    }

    private void Act(PlayerStatus playerStatus)
    {
        Nullable<Vector3> targetPosition = new Nullable<Vector3>();
        targetPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetFirstBaseTilePosition());
        this.CurrentBase = BaseEnum.PRIME_BASE;
        this.NextBase = BaseEnum.SECOND_BASE;
        Target = targetPosition;
    }

    public void CalculateRunnerColliderInterraction()
    {
        RunnerBehaviour runnerBehaviourScript = PlayerUtils.FetchRunnerBehaviourScript(this.gameObject);

        BaseEnum nextBase = runnerBehaviourScript.NextBase;
        runnerBehaviourScript.CurrentBase = nextBase;

        PlayerStatus playerStatusScript = PlayerUtils.FetchPlayerStatusScript(this.gameObject);

        switch (nextBase)
        {
            case BaseEnum.PRIME_BASE:
                Debug.Log("Get on FIRST BASE");
                Debug.Log("WIN ONE POINT !!!");
                runnerBehaviourScript.Target = null;
                runnerBehaviourScript.NextBase = runnerBehaviourScript.CurrentBase;
                playerStatusScript.IsAllowedToMove = false;
                break;
            case BaseEnum.SECOND_BASE:
                Debug.Log("Get on SECOND BASE");
                runnerBehaviourScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetSecondBaseTilePosition());
                runnerBehaviourScript.NextBase = BaseEnum.THIRD_BASE;
                break;
            case BaseEnum.THIRD_BASE:
                Debug.Log("Get on THIRD BASE");
                runnerBehaviourScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetThirdBaseTilePosition());
                runnerBehaviourScript.NextBase = BaseEnum.FOURTH_BASE;
                break;
            case BaseEnum.FOURTH_BASE:
                Debug.Log("Get on FOURTH BASE");
                runnerBehaviourScript.Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
                runnerBehaviourScript.NextBase = BaseEnum.PRIME_BASE;
                break;
            default:
                Debug.Log("DO NOT KNOW WHAT HAPPEN");
                break;
        }
    }
}
