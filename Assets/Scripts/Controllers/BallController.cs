using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : GenericController
{
    private bool isThrown;
    private GameObject currentPitcher;
    private bool isBeingHitten;
    private bool isHeld;
    private GameObject currentHolder;
    private Animator ballAnimator;

    // Start is called before the first frame update
    public void Start()
    {
        BallAnimator = this.GetComponent<Animator>();
        Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetHomeBaseTilePosition());
        moveSpeed = 0.6f;
    }

    // Update is called once per frame
    public void Update()
    {
        if (!IsMoving && !PlayersTurnManager.IsCommandPhase && !GameData.isPaused)
        {
            BallAnimator.enabled = true;

            if ((IsThrown && Target.HasValue && Target.Value != this.transform.position) || IsBeingHitten)
            {
                StartCoroutine(Move(transform.position, Target.Value));
                IsThrown = false;
                IsBeingHitten = false;
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
    }

    public bool IsThrown { get => isThrown; set => isThrown = value; }
    public GameObject CurrentPitcher { get => currentPitcher; set => currentPitcher = value; }
    public bool IsBeingHitten { get => isBeingHitten; set => isBeingHitten = value; }
    public bool IsHeld { get => isHeld; set => isHeld = value; }
    public GameObject CurrentHolder { get => currentHolder; set => currentHolder = value; }
    public Animator BallAnimator { get => ballAnimator; set => ballAnimator = value; }
}
