using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitcherBehaviour : GenericPlayerBehaviour
{
    public override void Start()
    {
        base.Start();
        IsoRenderer.SetDirection(Vector2.zero);
    }

    public override void Awake()
    {
        base.Awake();
    }
}
