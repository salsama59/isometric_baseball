using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericController : MonoBehaviour
{
    private bool allowDiagonals = false;
    private bool correctDiagonalSpeed = false;
    private float t;
    private float factor;
    protected float moveSpeed = 0.2f;
    private float gridSize = 1f;
    private bool isMoving = false;
    private IsometricCharacterRenderer isoRenderer;
    private Nullable<Vector3> target;
    private bool isMoveCanceled;

    protected IEnumerator Move(Vector3 startPosition, Vector3 endPosition)
    {
        IsMoving = true;
        t = 0;

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

            yield return new WaitUntil(() => !PlayersTurnManager.IsCommandPhase && !GameData.isPaused);
            
            t += Time.deltaTime * (moveSpeed / gridSize) * factor;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            if(IsoRenderer != null)
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

        IsMoving = false;
        yield return 0;
    }

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public IsometricCharacterRenderer IsoRenderer { get => isoRenderer; set => isoRenderer = value; }
    public Nullable<Vector3> Target { get => target; set => target = value; }
    public bool IsMoveCanceled { get => isMoveCanceled; set => isMoveCanceled = value; }
}
