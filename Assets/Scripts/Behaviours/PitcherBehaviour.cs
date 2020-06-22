using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PitcherBehaviour : GenericPlayerBehaviour
{
    public override void Start()
    {
        base.Start();
        IsoRenderer.SetDirection(Vector2.zero);
    }

    public override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        BallController ballControlerScript = BallUtils.FetchBallControllerScript(FieldBall);

        if (HasSpottedBall && FieldBall.activeInHierarchy && !IsHoldingBall && ballControlerScript.IsTargetedByFielder)
        {
            Target = FieldBall.transform.position;
        }

        if (Target.HasValue && Target.Value != this.transform.position)
        {
            MovePlayer();
            this.IsPrepared = true;
        }
    }

    public void CalculatePitcherColliderInterraction(GameObject ballGameObject, BallController ballControllerScript, GenericPlayerBehaviour genericPlayerBehaviourScript)
    {
        int runnersOnFieldCount = -1;
        List<GameObject> runners = PlayerUtils.GetRunnersOnField();
        runnersOnFieldCount = runners.Count;
        //Choose the runner who just hit the ball
        GameObject runnerToGetOut = runners.First();

        bool hasIntercepted = false;

        if (ballControllerScript.BallHeight == BallHeightEnum.HIGH || ballControllerScript.BallHeight == BallHeightEnum.LOW)
        {
            
            GameManager gameManager = GameUtils.FetchGameManager();
            DialogBoxManager dialogBoxManagerScript = GameUtils.FetchDialogBoxManager();
            dialogBoxManagerScript.SetDialogTextBox("TAG OUT !!!!!!!");
            dialogBoxManagerScript.ToggleDialogTextBox();

            ballControllerScript.Target = null;
            
            GameData.isPaused = true;
            this.InterceptBall(ballGameObject, ballControllerScript, genericPlayerBehaviourScript);
            hasIntercepted = true;
            StartCoroutine(gameManager.WaitAndReinit(dialogBoxManagerScript, PlayerUtils.FetchPlayerStatusScript(runnerToGetOut), null, FieldBall));

            if(runnersOnFieldCount == 1)
            {
                return;
            }
            
        }
        
        if(runnersOnFieldCount >= 1)
        {
            if (!hasIntercepted)
            {
                this.InterceptBall(ballGameObject, ballControllerScript, genericPlayerBehaviourScript);
            }
            
            PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
            PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(this.gameObject);
            playerAbilities.PlayerAbilityList.Clear();
            PlayerAbility passPlayerAbility = new PlayerAbility("Pass to fielder", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.PassAction);
            playerAbilities.AddAbility(passPlayerAbility);

            PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
            playersTurnManager.turnState = TurnStateEnum.PITCHER_TURN;
            PlayersTurnManager.IsCommandPhase = true;
        }

    }

    public void CalculateFielderTriggerInterraction(GameObject ballGameObject, GenericPlayerBehaviour genericPlayerBehaviourScript, PlayerStatus playerStatus)
    {
        BallController ballControlerScript = BallUtils.FetchBallControllerScript(ballGameObject);
        ballControlerScript.IsTargetedByPitcher = true;
        genericPlayerBehaviourScript.HasSpottedBall = true;
        playerStatus.IsAllowedToMove = true;
        genericPlayerBehaviourScript.Target = ballGameObject.transform.position;
        this.transform.rotation = Quaternion.identity;
    }

    private void InterceptBall(GameObject ballGameObject, BallController ballControllerScript, GenericPlayerBehaviour genericPlayerBehaviourScript)
    {
        ballControllerScript.BallHeight = BallHeightEnum.NONE;
        ballGameObject.transform.SetParent(this.gameObject.transform);
        ballGameObject.SetActive(false);
        ballControllerScript.CurrentHolder = this.gameObject;
        ballControllerScript.IsHeld = true;
        ballControllerScript.IsHit = false;
        ballControllerScript.IsPassed = false;
        ballControllerScript.Target = null;
        ballControllerScript.IsMoving = false;
        ballControllerScript.IsTargetedByPitcher = false;
        genericPlayerBehaviourScript.IsHoldingBall = true;
        genericPlayerBehaviourScript.HasSpottedBall = false;
        genericPlayerBehaviourScript.Target = null;
    }
}
