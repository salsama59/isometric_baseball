using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FielderBehaviour : GenericPlayerBehaviour
{
    public override void Awake()
    {
        base.Awake();
    }

    public void Update()
    {
        if (TargetPlayerToTagOut != null && (PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.FIRST_BASEMAN) || PlayerUtils.IsCurrentPlayerPosition(this.gameObject, PlayerFieldPositionEnum.THIRD_BASEMAN)))
        {
            Target = TargetPlayerToTagOut.transform.position;
        }

        if (Target.HasValue && Target.Value != this.transform.position)
        {
            MovePlayer();
            this.IsPrepared = true;
        }
        else
        {
            PlayerStatus playerStatus = PlayerUtils.FetchPlayerStatusScript(this.gameObject);
            if (playerStatus.IsAllowedToMove)
            {
                this.InitiateFielderAction(playerStatus);
            }
        }
    }

    private void InitiateFielderAction(PlayerStatus playerStatus)
    {
        Nullable<Vector3> targetPosition = new Nullable<Vector3>();

        if (FieldBall.activeInHierarchy && !HasSpottedBall)
        {
            GetAngleToLookAt();
        }
        else if (!FieldBall.activeInHierarchy && IsHoldingBall)
        {
            //TODO find the nearest runner on field
            List<GameObject> runners = PlayerUtils.GetRunnersOnField();
            TargetPlayerToTagOut = runners.First();
            targetPosition = TargetPlayerToTagOut.transform.position;
        }

        Target = targetPosition;
    }

    private void GetAngleToLookAt()
    {
        Vector3 dir = FieldBall.transform.position - this.transform.position;
        dir.Normalize();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    public void CalculateFielderColliderInterraction(GameObject ballGameObject, BallController ballControllerScript, GenericPlayerBehaviour genericPlayerBehaviourScript)
    {
        ballGameObject.transform.SetParent(this.gameObject.transform);
        ballGameObject.SetActive(false);
        ballControllerScript.CurrentHolder = this.gameObject;
        genericPlayerBehaviourScript.IsHoldingBall = true;
        genericPlayerBehaviourScript.HasSpottedBall = false;
    }

    public void CalculateFielderTriggerInterraction(GameObject ballGameObject, GenericPlayerBehaviour genericPlayerBehaviourScript, PlayerStatus playerStatus)
    {
        genericPlayerBehaviourScript.HasSpottedBall = true;
        playerStatus.IsAllowedToMove = true;
        genericPlayerBehaviourScript.Target = ballGameObject.transform.position;
    }
}
