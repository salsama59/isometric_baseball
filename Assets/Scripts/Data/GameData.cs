using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public static int playerNumber = 2;
    public static Dictionary<PlayerEnum, List<PlayerFieldPositionEnum>> playerEligibilityMap = new Dictionary<PlayerEnum, List<PlayerFieldPositionEnum>>();
    public static Dictionary<PlayerEnum, TeamIdEnum> playerEnumTeamMap = new Dictionary<PlayerEnum, TeamIdEnum>();
}