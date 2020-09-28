using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static readonly float FULL_CIRCLE_ANGLE_IN_DEGREE = 360f;
    public static readonly float HALF_CIRCLE_ANGLE_IN_DEGREE = FULL_CIRCLE_ANGLE_IN_DEGREE/2f;

    public static Vector2 CalculateDirection(Vector3 origin, Vector3 destination)
    {
        Vector3 distance = destination - origin;
        Vector2 direction = distance.normalized;
        return direction;
    }

    public static float CalculateDirectionAngle(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
}
