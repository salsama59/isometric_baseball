using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : GenericController
{
    private bool isThrown;
    private GameObject pitcher;
    private bool isBeingHitten;

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
            if ((IsThrown && Target.HasValue && Target.Value != this.transform.position) || IsBeingHitten)
            {
                StartCoroutine(Move(transform.position, Target.Value));
                IsThrown = false;
                IsBeingHitten = false;
            }
        }
    }

    public bool IsThrown { get => isThrown; set => isThrown = value; }
    public GameObject Pitcher { get => pitcher; set => pitcher = value; }
    public bool IsBeingHitten { get => isBeingHitten; set => isBeingHitten = value; }
}
