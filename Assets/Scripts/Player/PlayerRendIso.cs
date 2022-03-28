using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRendIso : MonoBehaviour
{
    public static readonly string[] idleDirections = { "Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE" };
    public static readonly string[] moveDirections = { "Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE" };

    Animator animator;
    int lastDirection;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetDirection(Vector2 direction)
    {
        // Use the Move states by default
        string[] directionArray = null;

        // Measure the mag. of the input
        if (direction.magnitude < .01f)
        {
            // If the player is still, use static states
            directionArray = idleDirections;
        }

        else
        {
            // Calculate the direction that the player is moving towards
            // Save last direction
            directionArray = moveDirections;
            lastDirection = DirectionToIndex(direction, 8);
        }

        animator.Play(directionArray[lastDirection]);
    }

    // Converts a direction into an index of the array moveDirections
    // Directions are stored in counter-clockwise direction
    public static int DirectionToIndex(Vector2 dir, int sliceCount)
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
        if (angle < 0)
            angle += 360;

        // Calculate how many steps areneeded to reach the angle
        float stepCount = angle / step;

        // Round it and return it
        return Mathf.FloorToInt(stepCount);
    }
}
