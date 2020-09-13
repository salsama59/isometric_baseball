using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ColliderUtils : MonoBehaviour
{
    public static bool IsBaseTile(string tileName)
    {
        return tileName == NameConstants.HOME_BASE_NAME
            || tileName == NameConstants.FIRST_BASE_NAME
            || tileName == NameConstants.SECOND_BASE_NAME
            || tileName == NameConstants.THIRD_BASE_NAME;
    }

    public static bool HasBallCollided(Collider2D collider2D)
    {
        return collider2D.transform.gameObject.CompareTag(TagsConstants.BALL_TAG);
    }

    public static bool HasPlayerCollided(Collision2D collision2D)
    {
        return collision2D.collider.transform.gameObject.CompareTag(TagsConstants.BASEBALL_PLAYER_TAG);
    }
}
