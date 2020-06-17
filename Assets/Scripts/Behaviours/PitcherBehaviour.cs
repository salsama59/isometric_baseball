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
        //FIXME Else is temporary (need to be sure the ball is caught without touching the ground)
        if(ballControllerScript.BallHeight == BallHeightEnum.HIGH || ballControllerScript.BallHeight == BallHeightEnum.LOW)
        {
            GameObject runnerToGetOut = PlayerUtils.GetRunnersOnField().First();
            GameManager gameManager = GameUtils.FetchGameManager();
            DialogBoxManager dialogBoxManagerScript = GameUtils.FetchDialogBoxManager();
            dialogBoxManagerScript.SetDialogTextBox("TAG OUT !!!!!!!");
            dialogBoxManagerScript.ToggleDialogTextBox();
            //FIXME Migrate the wait and reinit to render generic by itterrationg on all player of the field and réinit all
            //StartCoroutine(gameManager.WaitAndReinit(dialogBoxManagerScript, PlayerUtils.FetchPlayerStatusScript(runnerToGetOut), PlayerUtils.FetchPlayerStatusScript(this.gameObject), FieldBall));
        }
        
        ballControllerScript.BallHeight = BallHeightEnum.NONE;
        ballGameObject.transform.SetParent(this.gameObject.transform);
        ballGameObject.SetActive(false);
        ballControllerScript.CurrentHolder = this.gameObject;
        ballControllerScript.IsHeld = true;
        genericPlayerBehaviourScript.IsHoldingBall = true;
        genericPlayerBehaviourScript.HasSpottedBall = false;

        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(this.gameObject);
        playerAbilities.PlayerAbilityList.Clear();
        PlayerAbility passPlayerAbility = new PlayerAbility("Pass to fielder", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.PassAction);
        playerAbilities.AddAbility(passPlayerAbility);

        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        playersTurnManager.turnState = TurnStateEnum.PITCHER_TURN;
        PlayersTurnManager.IsCommandPhase = true;
        
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
}
