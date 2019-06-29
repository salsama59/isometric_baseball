using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : GenericController
{
    private bool isThrown;

    // Start is called before the first frame update
    public void Start()
    {
        IsThrown = true;
        Target = FieldUtils.GetTileCenterPositionInGameWorld(FieldUtils.GetPrimeBaseTilePosition());
    }

    // Update is called once per frame
    public void Update()
    {
        if (!IsMoving)
        {
            if (IsThrown && Target.HasValue && Target.Value != this.transform.position)
            {
                StartCoroutine(Move(transform.position, Target.Value));
                IsThrown = false;
            }
        }
    }

    public bool IsThrown { get => isThrown; set => isThrown = value; }
}
