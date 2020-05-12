using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public static int playerNumber = 0;
    public static Dictionary<PlayerEnum, List<PlayerFieldPositionEnum>> playerEligibilityMap = new Dictionary<PlayerEnum, List<PlayerFieldPositionEnum>>();
}
