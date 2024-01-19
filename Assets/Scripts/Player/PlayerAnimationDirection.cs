using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationDirection : MonoBehaviour
{
    private static readonly string[] idleDirections = { "Static N", "Static NW", "Static W", "Static SW", 
                                                        "Static S", "Static SE", "Static E", "Static NE" };

    private static readonly string[] moveDirections = { "Run N", "Run NW", "Run W", "Run SW", 
                                                        "Run S", "Run SE", "Run E", "Run NE" };

    private Animator playerAnimatorComp;
    private int lastDirection;

    private void Awake()
    {
        playerAnimatorComp = GetComponent<Animator>();
    }

    public void SetDirection(Vector2 direction)
    {
        string[] directionArray = idleDirections;

        if (direction.magnitude >= 0.01f)
        {
            directionArray = moveDirections;
            lastDirection = DirectionToIndex(direction, idleDirections.Length);
        }

        playerAnimatorComp.Play(directionArray[lastDirection]);
    }

    private static int DirectionToIndex(Vector2 dir, int sliceCount)
    {
        Vector2 normDir = dir.normalized;

        // Calculate the degrees that one slice represents (here will be always 8 slices)
        float step = 360f / sliceCount;

        // Calculate how many degrees a half slice is to offset the pie
        float halfStep = step / 2;

        // Get the direction vector for the UP vector
        float angle = Vector2.SignedAngle(Vector2.up, normDir);

        // Add the precalculated offset
        angle += halfStep;

        // If the angle is negative, add 360 to make it positive
        if (angle < 0.0f)
        {
            angle += 360.0f;
        }

        // Calculate how many steps are needed to reach the angle
        float stepCount = angle / step;

        // Round it and return it
        return Mathf.FloorToInt(stepCount);
    }
}
