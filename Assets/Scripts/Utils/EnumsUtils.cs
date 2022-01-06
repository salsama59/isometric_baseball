using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumsUtils
{

    public static PlayerFieldPositionEnum StringToPlayerFieldPositionEnum(string stringEnum)
    {

        foreach (PlayerFieldPositionEnum playerFieldPositionEnum in Enum.GetValues(typeof(PlayerFieldPositionEnum)))
        {
            if (stringEnum.Equals(Enum.GetName(typeof(PlayerFieldPositionEnum), playerFieldPositionEnum)))
            {
                return playerFieldPositionEnum;
            }
        }

        return PlayerFieldPositionEnum.BATTER;
    }
    
}
