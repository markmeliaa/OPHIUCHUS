using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveIso : MonoBehaviour
{
    public float playerSpeed = 1f;
    [HideInInspector] public PlayerRendIso rendIso;

    Rigidbody2D rb;
    private RoomTemplates templates;

    private AudioSource audioSource;

    [HideInInspector] public float horInput = 0;
    [HideInInspector] public float vertInput = 0;

    [HideInInspector] public bool moving = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rendIso = GetComponent<PlayerRendIso>();
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = true;
    }

    private void Update()
    {
        if (horInput != 0 || vertInput != 0 && !audioSource.isPlaying)
            audioSource.mute = false;

        else if (horInput == 0 && vertInput == 0 && audioSource.isPlaying)
            audioSource.mute = true;

        if (!templates.mapFormed || templates.changingRoom)
            moving = false;

        else if (templates.mapFormed && !templates.changingRoom)
            moving = true;
    }

    private void FixedUpdate()
    {
        if (!moving) 
            return;

        Vector2 currentPos = rb.position;

        horInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");

        Vector2 inputVect = new Vector2(horInput, vertInput);

        // Prevent diagonal movement to be faster than cardinal direction movement
        inputVect = Vector2.ClampMagnitude(inputVect, 1);

        Vector2 movement = inputVect * playerSpeed;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;

        rendIso.SetDirection(movement);
        rb.MovePosition(newPos);
    }
}
