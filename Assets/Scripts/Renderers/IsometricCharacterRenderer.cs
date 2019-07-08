﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IsometricCharacterRenderer : MonoBehaviour
{

    public static readonly string[] staticDirections = { "Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE" };
    public static readonly string[] runDirections = {"Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE"};

    private Animator animator;
    private int lastDirection = 4;

    private void Awake()
    {
        //cache the animator component
        Animator = GetComponent<Animator>();
    }

    public void SetDirection(Vector2 direction){

        //use the Run states by default
        string[] directionArray = null;

        //measure the magnitude of the input.
        if (direction.magnitude < .01f)
        {
            //if we are basically standing still, we'll use the Static states
            //we won't be able to calculate a direction if the user isn't pressing one, anyway!
            directionArray = staticDirections;
        }
        else
        {
            //we can calculate which direction we are going in
            //use DirectionToIndex to get the index of the slice from the direction vector
            //save the answer to lastDirection
            directionArray = runDirections;
            lastDirection = AnimationUtils.DirectionToIndex(direction, 8);
        }

        //tell the animator to play the requested state
        Animator.Play(directionArray[lastDirection]);
    }

    public Animator Animator { get => animator; set => animator = value; }
}