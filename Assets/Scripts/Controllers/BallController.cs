using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallController : MonoBehaviour
{
    private bool isPitched;
    private GameObject currentPitcher;
    private bool isHit;
    private bool isHeld;
    private GameObject currentHolder;
    private Animator ballAnimator;
    private bool isTargetedByFielder;
    private BallHeightEnum ballHeight;
    private bool isPassed;
    private GameObject currentPasser;

    private bool allowDiagonals = false;
    private bool correctDiagonalSpeed = false;
    private float t;
    private float factor;
    protected float moveSpeed = 0.2f;
    private float gridSize = 1f;
    private bool isMoving = false;
    private Nullable<Vector3> target;
    private Coroutine movementCoroutine;
    private bool isTargetedByPitcher;

    // Start is called before the first frame update
    public void Start()
    {
        //BallHeight = BallHeightEnum.NONE;
        BallAnimator = this.GetComponent<Animator>();
        Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
        moveSpeed = 0.6f;
    }

    // Update is called once per frame
    public void Update()
    {

        if (Target.HasValue && Target.Value == this.transform.position)
        {
            Target = null;

            if (IsHit)
            {
                IsHit = false;
            }
        }

        if (!IsMoving && !PlayersTurnManager.IsCommandPhase && !GameData.isPaused)
        {
            //Move inside first if statement to avoid graphical bugs
            if (Target.HasValue && Target.Value != this.transform.position)
            {
                BallAnimator.enabled = true;
                MovementCoroutine = StartCoroutine(Move(transform.position, Target.Value, IsHit || IsPassed));
            }
            else
            {
                BallAnimator.enabled = false;
            }
        }
        else if (IsMoving && PlayersTurnManager.IsCommandPhase || GameData.isPaused)
        {
            BallAnimator.enabled = false;
        }

        IsMoving = (IsHit || IsPitched || IsPassed) && Target.HasValue;
    }

    private BallHeightEnum GetBallHeightState(Vector3 ballStartPosition, Vector3 ballEndposition, Vector3 ballCurrentPosition)
    {
        BallHeightEnum resultingState = BallHeightEnum.NONE;

        float ballTotalTravellingDistance = Vector3.Distance(ballStartPosition, ballEndposition);
        float ballCurrentTraveledDistance = Vector3.Distance(ballStartPosition, ballCurrentPosition);

        float distanceThreshold = ballTotalTravellingDistance / 4;

        
        if (ballCurrentTraveledDistance >= 0 && ballCurrentTraveledDistance < distanceThreshold)
        {
            resultingState = BallHeightEnum.LOW;
        }
        else if(ballCurrentTraveledDistance >= distanceThreshold && ballCurrentTraveledDistance < distanceThreshold * 2)
        {
            resultingState = BallHeightEnum.HIGH;
        }
        else if (ballCurrentTraveledDistance >= distanceThreshold * 2 && ballCurrentTraveledDistance < distanceThreshold * 3)
        {
            resultingState = BallHeightEnum.LOW;
        }
        else if (ballCurrentTraveledDistance >= distanceThreshold * 3 && ballCurrentTraveledDistance <= distanceThreshold * 4)
        {
            resultingState = BallHeightEnum.GROUNDED;
        }

        return resultingState;
    }

    private IEnumerator Move(Vector3 startPosition, Vector3 endPosition, bool enableHeightVariation)
    {
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

            if (enableHeightVariation)
            {
                BallHeight = GetBallHeightState(startPosition, endPosition, this.transform.position);
            }
            
            yield return null;
        }

        IsMoving = false;
        yield return 0;
    }

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public Nullable<Vector3> Target { get => target; set => target = value; }
    public bool IsPitched { get => isPitched; set => isPitched = value; }
    public GameObject CurrentPitcher { get => currentPitcher; set => currentPitcher = value; }
    public bool IsHit { get => isHit; set => isHit = value; }
    public bool IsHeld { get => isHeld; set => isHeld = value; }
    public GameObject CurrentHolder { get => currentHolder; set => currentHolder = value; }
    public Animator BallAnimator { get => ballAnimator; set => ballAnimator = value; }
    public bool IsTargetedByFielder { get => isTargetedByFielder; set => isTargetedByFielder = value; }
    public BallHeightEnum BallHeight { get => ballHeight; set => ballHeight = value; }
    public Coroutine MovementCoroutine { get => movementCoroutine; set => movementCoroutine = value; }
    public bool IsTargetedByPitcher { get => isTargetedByPitcher; set => isTargetedByPitcher = value; }
    public bool IsPassed { get => isPassed; set => isPassed = value; }
    public GameObject CurrentPasser { get => currentPasser; set => currentPasser = value; }
}
