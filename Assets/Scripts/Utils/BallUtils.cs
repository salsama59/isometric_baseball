using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallUtils
{
    public static BallController FetchBallControllerScript(GameObject ball)
    {
        return ball.GetComponent<BallController>();
    }

    public static GameObject FetchBallPitcherGameObject(GameObject ball)
    {
        return FetchBallControllerScript(ball).CurrentPitcher;
    }
}
