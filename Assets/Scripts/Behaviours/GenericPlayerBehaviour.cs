using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericPlayerBehaviour : MonoBehaviour
{

    private BaseEnum currentBase;
    private BaseEnum nextBase;
    private bool isPrepared = false;
    private GenericPlayerBehaviour playerBehaviour = null;
    private GameObject fieldBall;
    private bool hasSpottedBall;
    private GameObject targetPlayerToTagOut;
    private bool isHoldingBall;
    private float moveSpeed = 1.5f;
    private bool isMoving = false;
    private IsometricCharacterRenderer isoRenderer;
    private Nullable<Vector3> target;
    private bool isMoveCanceled;

    public virtual void Start()
    {
        Target = null;
        IsoRenderer = PlayerUtils.FetchPlayerIsometricRenderer(this.gameObject);
    }

    public virtual void Awake()
    {
        this.enabled = true;
    }

    protected IEnumerator MoveObject(Vector3 startPosition, Vector3 endPosition)
    {
        IsMoving = true;
        float interpolationPercentage = 0;

        while (interpolationPercentage < 1f)
        {

            if (IsMoveCanceled)
            {
                break;
            }

            interpolationPercentage += Time.deltaTime * (MoveSpeed / FieldUtils.GRID_SIZE);
            transform.position = Vector3.Lerp(startPosition, endPosition, interpolationPercentage);

            if (IsoRenderer != null)
            {
                Vector2 direction = new Vector2();

                if (startPosition.x < endPosition.x)
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

            }

            yield return null;
        }

        IsMoveCanceled = false;
        IsMoving = false;
        yield return 0;
    }

    protected void MovePlayer()
    {
        if (!PlayersTurnManager.IsCommandPhase)
        {
            IsoRenderer.Animator.enabled = true;
            transform.position = Vector3.MoveTowards(transform.position, Target.Value, this.MoveSpeed * Time.deltaTime);

            if (IsoRenderer != null)
            {
                Vector2 direction = new Vector2();

                if (transform.position.x < Target.Value.x)
                {
                    direction.x = 1;
                }
                else
                {
                    direction.x = -1;
                }

                if (transform.position.y < Target.Value.y)
                {
                    direction.y = 1;
                }
                else
                {
                    direction.y = -1;
                }

                IsoRenderer.SetDirection(direction);
            }
        }
        else
        {
            IsoRenderer.Animator.enabled = false;
        }
    }

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public IsometricCharacterRenderer IsoRenderer { get => isoRenderer; set => isoRenderer = value; }
    public Nullable<Vector3> Target { get => target; set => target = value; }
    public bool IsMoveCanceled { get => isMoveCanceled; set => isMoveCanceled = value; }
    public BaseEnum CurrentBase { get => currentBase; set => currentBase = value; }
    public BaseEnum NextBase { get => nextBase; set => nextBase = value; }
    public bool IsPrepared { get => isPrepared; set => isPrepared = value; }
    public GenericPlayerBehaviour PlayerBehaviour { get => playerBehaviour; set => playerBehaviour = value; }
    public bool HasSpottedBall { get => hasSpottedBall; set => hasSpottedBall = value; }
    public GameObject FieldBall { get => fieldBall; set => fieldBall = value; }
    public GameObject TargetPlayerToTagOut { get => targetPlayerToTagOut; set => targetPlayerToTagOut = value; }
    public bool IsHoldingBall { get => isHoldingBall; set => isHoldingBall = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
}
