using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCalculationUtils
{
    public static float MIN_PERCENTAGE_VALUE = 0f;
    public static float MAX_PERCENTAGE_VALUE = 100f;
    public static int ISOMETRIC_ANGLE_FIX = 45;
    public static List<int> criticalFactorList = new List<int>()
    {
        2,3,4,5
    };

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

    public static List<Vector2Int> CalculateBallFallPositionList(GameObject playerInvolved, int angleMinRange, int angleMaxRange, int angleStep, bool Iscritical)
    {
        List<Vector2Int> ballPositionList = new List<Vector2Int>();
        int x = 0;
        int y = 0;
        int criticalFactor = 1;
        PlayerStatus playerInvolvedStatus = PlayerUtils.FetchPlayerStatusScript(playerInvolved);
        Vector2Int playerInvolvedPosition = FieldUtils.GetGameObjectTilePositionOnField(playerInvolved);
        int property = playerInvolvedStatus.BattingPower;

        /*
        parametric circle equation : 
        x = R * cos(theta) + a
        y = R * sin(theta) + b*/

        /*
         pi => 180
         ? => x
         */
        
        if (Iscritical)
        {
            criticalFactor = CalculateCriticalFactor();
        }

        for (int angle = angleMinRange; angle < angleMaxRange - angleStep; angle = angle + angleStep)
        {
            float theta = ConvertDegreeAngleToRadianAngle(GetFixedIsometricAngle(angle));
            x = (int)(property * criticalFactor * Mathf.Cos(theta) + playerInvolvedPosition.x);
            y = (int)(property * criticalFactor * Mathf.Sin(theta) + playerInvolvedPosition.y);

            ballPositionList.Add(new Vector2Int(x, y));
        }

        return ballPositionList;
    }

    public static bool HasActionSucceeded(float sucessRate)
    {
        float randomValue = Random.value;
        Debug.Log("randomValue = " + randomValue);
        Debug.Log("Action has succeeded ?? => " + (randomValue <= (sucessRate / 100)));
        return randomValue <= (sucessRate / 100);
    }

    private static int CalculateCriticalFactor()
    {
        return criticalFactorList[Random.Range(0, criticalFactorList.Count - 1)];  
    }

    private static float ConvertDegreeAngleToRadianAngle(int angleToConvert)
    {
        return angleToConvert * Mathf.Deg2Rad;
    }

    private static int GetFixedIsometricAngle(int angle)
    {
        return angle - ISOMETRIC_ANGLE_FIX;
    }

    private static float AdjustResult(float result)
    {
        return Mathf.Clamp(result, MIN_PERCENTAGE_VALUE, MAX_PERCENTAGE_VALUE);
    }
}
