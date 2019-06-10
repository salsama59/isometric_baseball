using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericCharacterController : MonoBehaviour
{

    private float moveSpeed = 0.2f;
    private float gridSize = 1f;
    private enum Orientation
    {
        Horizontal,
        Vertical
    };
    //private Orientation gridOrientation = Orientation.Vertical;
    private bool allowDiagonals = true;
    private bool correctDiagonalSpeed = true;
    //private Vector2 input;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;
    private float factor;
    private IsometricCharacterRenderer isoRenderer;
    private Vector3 target;
    private BaseEnum currentBase;
    private BaseEnum nextBase;
    private bool isPrepared = false;

    public void Start()
    {
        isoRenderer = GetComponentInChildren<IsometricCharacterRenderer>();
        Vector3 playerPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetPrimeBaseTilePosition());
        playerPosition.Set(playerPosition.x, playerPosition.y, playerPosition.z);
        this.transform.position = playerPosition;

        Vector3 targetPosition = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetSecondBaseTilePosition());
        targetPosition.Set(targetPosition.x, targetPosition.y, targetPosition.z);
        Target = targetPosition;
        this.CurrentBase = BaseEnum.PRIME_BASE;
        this.NextBase = BaseEnum.SECOND_BASE;
    }

    public void Update()
    {
        if (!isMoving)
        {
            if(Target != this.transform.position)
            {
                //input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                /*if (!allowDiagonals)
                {
                    if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                    {
                        input.y = 0;
                    }
                    else
                    {
                        input.x = 0;
                    }
                }*/

                //if (input != Vector2.zero)
                //{
                    StartCoroutine(move(transform));
                //}
            }
        }
    }

    public IEnumerator move(Transform transform)
    {
        isMoving = true;
        this.IsPrepared = true;
        startPosition = transform.position;
        t = 0;

        /*if (gridOrientation == Orientation.Horizontal)
        {
            endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * gridSize,
                startPosition.y, startPosition.z + System.Math.Sign(input.y) * gridSize);
        }
        else
        {
            endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * gridSize,
                startPosition.y + System.Math.Sign(input.y) * gridSize/2, startPosition.z);
        }*/

        endPosition = Target;

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

            isoRenderer.SetDirection(direction);
            yield return null;
        }

        isMoving = false;
        yield return 0;
    }

    public BaseEnum CurrentBase { get => currentBase; set => currentBase = value; }
    public BaseEnum NextBase { get => nextBase; set => nextBase = value; }
    public Vector3 Target { get => target; set => target = value; }
    public bool IsPrepared { get => isPrepared; set => isPrepared = value; }
}
