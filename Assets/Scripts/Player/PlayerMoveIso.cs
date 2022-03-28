using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveIso : MonoBehaviour
{
    public float playerSpeed = 1f;
    PlayerRendIso rendIso;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rendIso = GetComponent<PlayerRendIso>();
    }

    private void FixedUpdate()
    {
        Vector2 currentPos = rb.position;

        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        Vector2 inputVect = new Vector2(horInput, vertInput);

        // Prevent diagonal movement to be faster than cardinal direction movement
        inputVect = Vector2.ClampMagnitude(inputVect, 1);

        Vector2 movement = inputVect * playerSpeed;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;

        rendIso.SetDirection(movement);
        rb.MovePosition(newPos);
    }
}
