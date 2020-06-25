using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public static int playerNumber = 2;
    public static Dictionary<PlayerEnum, List<PlayerFieldPositionEnum>> playerFieldPositionEnumListMap = new Dictionary<PlayerEnum, List<PlayerFieldPositionEnum>>();
    public static Dictionary<PlayerEnum, TeamIdEnum> teamIdEnumMap = new Dictionary<PlayerEnum, TeamIdEnum>();
    public static Dictionary<TeamIdEnum, TeamSideEnum> teamSideEnumMap = new Dictionary<TeamIdEnum, TeamSideEnum>();
    public static bool isPaused = false;
}