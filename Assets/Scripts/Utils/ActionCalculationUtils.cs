using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCalculationUtils
{
    public static float MIN_PERCENTAGE_VALUE = 0f;
    public static float MAX_PERCENTAGE_VALUE = 100f;

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

        float result = pitcherStatus.PitchEfficiency + (pitcherStatus.PitchingPower * powerEfectiveness) - opponentDefensiveCharacteristic;

        return AdjustResult(result);
    }

    public static float CalculateCatchSuccessRate(GameObject catcherGameObject, GameObject opponentGameObject)
    {
        PlayerStatus catcherStatus = PlayerUtils.FetchPlayerStatusScript(catcherGameObject);
        PlayerStatus opponentStatus = PlayerUtils.FetchPlayerStatusScript(opponentGameObject);

        float opponentOffensiveCharacteristic = 0f;
        float powerEfectiveness = 0f;

        switch (opponentStatus.PlayerFieldPosition)
        {
            case PlayerFieldPositionEnum.PITCHER:
                opponentOffensiveCharacteristic = opponentStatus.PitchEfficiency;
                powerEfectiveness = 2f;
                break;
        }

        float result = catcherStatus.CatchEfficiency - (opponentStatus.PitchingPower * powerEfectiveness) - opponentOffensiveCharacteristic;
        
        return AdjustResult(result);
    }

    private static float AdjustResult(float result)
    {
        return Mathf.Clamp(result, MIN_PERCENTAGE_VALUE, MAX_PERCENTAGE_VALUE);
    }

    public static bool HasActionSucceeded(float sucessRate)
    {
        float randomValue = Random.value;
        Debug.Log("randomValue = " + randomValue);
        Debug.Log("Action has succeeded ?? => " + (randomValue <= (sucessRate / 100)));
        return randomValue <= (sucessRate / 100);
    }
}
