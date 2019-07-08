using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCalculationUtils
{
    public static float CalculatePitchSuccessRate(GameObject pitcherGameObject, GameObject opponentGameObject)
    {
        PlayerStatus pitcherStatus = PlayerUtils.FetchPlayerStatusScript(pitcherGameObject);
        PlayerStatus opponentStatus = PlayerUtils.FetchPlayerStatusScript(opponentGameObject);

        float opponentDefensiveCharacteristic = 0f;
        float powerEfectiveness = 0f;

        switch (opponentStatus.PlayerFieldPosition)
        {
            case PlayerFieldPositionEnum.BATTER:
                opponentDefensiveCharacteristic = opponentStatus.BattingEfficiency;
                powerEfectiveness = 5f;
                break;
            case PlayerFieldPositionEnum.CATCHER:
                opponentDefensiveCharacteristic = opponentStatus.CatchEfficiency;
                powerEfectiveness = 2f;
                break;
        }

        return pitcherStatus.PitchEfficiency + (pitcherStatus.PitchingPower * powerEfectiveness) - opponentDefensiveCharacteristic;
    }

    public static bool HasActionSucceeded(float sucessRate)
    {
        return Random.value <= (sucessRate / 100);
    }
}
