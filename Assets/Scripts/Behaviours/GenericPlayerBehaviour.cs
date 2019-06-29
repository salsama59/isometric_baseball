using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericPlayerBehaviour : MonoBehaviour
{
    public virtual void Awake()
    {
        this.enabled = false;
    }

    public abstract void CalculateNextAction();
}
