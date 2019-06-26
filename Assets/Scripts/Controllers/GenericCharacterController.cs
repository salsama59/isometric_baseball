using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericCharacterController : MonoBehaviour
{

    private float moveSpeed = 0.2f;
    private float gridSize = 1f;
    private bool allowDiagonals = false;
    private bool correctDiagonalSpeed = false;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;
    private float factor;
    private IsometricCharacterRenderer isoRenderer;
    private Nullable<Vector3> target;
    private BaseEnum currentBase;
    private BaseEnum nextBase;
    private bool isPrepared = false;
    private GenericPlayerBehaviour playerBehaviour = null;
    private PlayerStatus playerStatusInformations = new PlayerStatus();

    public void Start()
    {
        Target = null;
        IsoRenderer = GetComponentInChildren<IsometricCharacterRenderer>();
    }

    public void Update()
    {
        if (!isMoving)
        {
            if(Target.HasValue && Target.Value != this.transform.position)
            {
                StartCoroutine(Move(transform));            
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
                targetPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetSecondBaseTilePosition());
                this.CurrentBase = BaseEnum.PRIME_BASE;
                this.NextBase = BaseEnum.SECOND_BASE;
                break;
            case PlayerFieldPositionEnum.PITCHER:
                targetPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetMiddleBaseTilePosition());
                this.CurrentBase = BaseEnum.MIDDLE_BASE;
                this.NextBase = BaseEnum.MIDDLE_BASE;
                break;
            case PlayerFieldPositionEnum.RUNNER:
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

    private IEnumerator Move(Transform transform)
    {
        isMoving = true;
        this.IsPrepared = true;
        startPosition = transform.position;
        t = 0;

        endPosition = Target.Value;

        if (allowDiagonals && correctDiagonalSpeed)
        {
            factor = 0.7071f;
        }
        else
        {
            factor = 1f;
        }

        while (t < 1f)
        {
            t += Time.deltaTime * (moveSpeed / gridSize) * factor;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            Vector2 direction = new Vector2();

            if(startPosition.x < endPosition.x)
            {
                direction.x = 1;
            }
            else
            {
                direction.x = -1;
            }

            if (startPosition.y < endPosition.y)
            {
                direction.y = 1;
            }
            else
            {
                direction.y = -1;
            }

            IsoRenderer.SetDirection(direction);
            yield return null;
        }

        isMoving = false;
        yield return 0;
    }

    public BaseEnum CurrentBase { get => currentBase; set => currentBase = value; }
    public BaseEnum NextBase { get => nextBase; set => nextBase = value; }
    public Nullable<Vector3> Target { get => target; set => target = value; }
    public bool IsPrepared { get => isPrepared; set => isPrepared = value; }
    public GenericPlayerBehaviour PlayerBehaviour { get => playerBehaviour; set => playerBehaviour = value; }
    public PlayerStatus PlayerStatusInformations { get => playerStatusInformations; set => playerStatusInformations = value; }
    public IsometricCharacterRenderer IsoRenderer { get => isoRenderer; set => isoRenderer = value; }
}
