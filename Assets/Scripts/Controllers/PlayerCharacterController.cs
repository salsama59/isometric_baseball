using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : GenericController
{

    private BaseEnum currentBase;
    private BaseEnum nextBase;
    private bool isPrepared = false;
    private GenericPlayerBehaviour playerBehaviour = null;
    private PlayerStatus playerStatusInformations = new PlayerStatus();
    public GameObject ballGameObject;

    public void Start()
    {
        Target = null;
        IsoRenderer = PlayerUtils.FetchPlayerIsometricRenderer(this.gameObject);
        if(PlayerStatusInformations.PlayerFieldPosition == PlayerFieldPositionEnum.PITCHER)
        {
            GameObject ball = Instantiate(ballGameObject, this.transform.position, this.transform.rotation);
            BallController ballControllerScript = ball.GetComponent<BallController>();
            ballControllerScript.Pitcher = this.gameObject;
        }
    }

    public void Update()
    {
        if (!IsMoving)
        {
            if(Target.HasValue && Target.Value != this.transform.position)
            {
                StartCoroutine(Move(transform.position, Target.Value));
                this.IsPrepared = true;
            }
            else
            {
                if (PlayerStatusInformations.IsAllowedToMove)
                {
                    this.CalculateNextAction();
                }
            }
        }
    }

    private void CalculateNextAction()
    {
        PlayerBehaviour = PlayerUtils.FetchCorrespondingPlayerBehaviourScript(this.gameObject, PlayerStatusInformations);
        PlayerBehaviour.enabled = true;
        Nullable<Vector3> targetPosition = new Nullable<Vector3>();

        switch (PlayerStatusInformations.PlayerFieldPosition)
        {
            case PlayerFieldPositionEnum.BATTER:
                this.CurrentBase = BaseEnum.PRIME_BASE;
                this.NextBase = BaseEnum.PRIME_BASE;
                break;
            case PlayerFieldPositionEnum.PITCHER:
                targetPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetMiddleBaseTilePosition());
                this.CurrentBase = BaseEnum.MIDDLE_BASE;
                this.NextBase = BaseEnum.MIDDLE_BASE;
                break;
            case PlayerFieldPositionEnum.RUNNER:
                targetPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetSecondBaseTilePosition());
                this.CurrentBase = BaseEnum.PRIME_BASE;
                this.NextBase = BaseEnum.SECOND_BASE;
                break;
            case PlayerFieldPositionEnum.CATCHER:
                break;
            case PlayerFieldPositionEnum.FIRST_BASEMAN:
                break;
            case PlayerFieldPositionEnum.SECOND_BASEMAN:
                break;
            case PlayerFieldPositionEnum.THIRD_BASEMAN:
                break;
            case PlayerFieldPositionEnum.SHORT_STOP:
                break;
            case PlayerFieldPositionEnum.LEFT_FIELDER:
                break;
            case PlayerFieldPositionEnum.CENTER_FIELDER:
                break;
            case PlayerFieldPositionEnum.RIGHT_FIELDER:
                break;
            default:
                break;
        }

        Target = targetPosition;
    }

    public BaseEnum CurrentBase { get => currentBase; set => currentBase = value; }
    public BaseEnum NextBase { get => nextBase; set => nextBase = value; }
    public bool IsPrepared { get => isPrepared; set => isPrepared = value; }
    public GenericPlayerBehaviour PlayerBehaviour { get => playerBehaviour; set => playerBehaviour = value; }
    public PlayerStatus PlayerStatusInformations { get => playerStatusInformations; set => playerStatusInformations = value; }
    public GameObject BallGameObject { get => ballGameObject; set => ballGameObject = value; }
}
