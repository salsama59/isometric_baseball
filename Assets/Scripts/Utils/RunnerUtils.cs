using UnityEngine;
using System.Linq;

public class RunnerUtils : MonoBehaviour
{
    public static void AddRunnerAbilitiesToBatter(GameObject player)
    {
        PlayerActionsManager playerActionsManager = GameUtils.FetchPlayerActionsManager();
        PlayerAbilities playerAbilities = PlayerUtils.FetchPlayerAbilitiesScript(player);
        playerAbilities.ReinitAbilities();
        PlayerAbility runPlayerAbility = new PlayerAbility("Run to next base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.RunAction, player);
        PlayerAbility StaySafePlayerAbility = new PlayerAbility("Stay on base", AbilityTypeEnum.BASIC, AbilityCategoryEnum.NORMAL, playerActionsManager.StayOnBaseAction, player);
        playerAbilities.AddAbility(runPlayerAbility);
        playerAbilities.AddAbility(StaySafePlayerAbility);
    }

    public static RunnerBehaviour ConvertBatterToRunner(PlayerStatus batterStatusScript)
    {
        PlayersTurnManager playersTurnManager = GameUtils.FetchPlayersTurnManager();
        GameObject currentBatter = batterStatusScript.gameObject;
        GameObject bat = PlayerUtils.GetPlayerBatGameObject(currentBatter);
        GameManager gameManager = GameUtils.FetchGameManager();
        RunnerBehaviour runnerBehaviour = currentBatter.AddComponent<RunnerBehaviour>();
        gameManager.AttackTeamRunnerList.Add(runnerBehaviour.gameObject);
        gameManager.AttackTeamRunnerListClone.Add(runnerBehaviour.gameObject);
        gameManager.AttackTeamBatterListClone.Remove(currentBatter);
        playersTurnManager.CurrentRunner = runnerBehaviour.gameObject;
        runnerBehaviour.EquipedBat = bat;
        bat.SetActive(false);
        Destroy(currentBatter.GetComponent<BatterBehaviour>());

        batterStatusScript.PlayerFieldPosition = PlayerFieldPositionEnum.RUNNER;
        TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.RUNNER, currentBatter, TeamUtils.GetBaseballPlayerOwner(currentBatter));

        int batterCount = gameManager.AttackTeamBatterListClone.Count;
        if (batterCount > 0)
        {
            GameObject nextBatter = gameManager.AttackTeamBatterListClone.First();
            gameManager.EquipBatToPlayer(nextBatter);
            TeamUtils.AddPlayerTeamMember(PlayerFieldPositionEnum.BATTER, nextBatter, TeamUtils.GetBaseballPlayerOwner(nextBatter));
        }

        string runnerNumber = runnerBehaviour.gameObject.name.Split('_').Last();
        string newRunnerName = NameConstants.RUNNER_NAME + "_" + runnerNumber;
        runnerBehaviour.gameObject.name = newRunnerName;


        playersTurnManager.TurnState = TurnStateEnum.STANDBY;

        return runnerBehaviour;
    }
}
