using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerAnimationDirection playerAnimationDirection;
    private Rigidbody2D playerRigidBodyComp;
    private AudioSource playerAudioSourceComp;

    private float horInput;
    private float vertInput;
    [HideInInspector] public bool canMove = true;
    [SerializeField] private float playerSpeed = 2.5f;

    private void Awake()
    {
        playerAnimationDirection = GetComponent<PlayerAnimationDirection>();
        playerRigidBodyComp = GetComponent<Rigidbody2D>();
        playerAudioSourceComp = GetComponent<AudioSource>();
        playerAudioSourceComp.mute = true;
    }

    private void Update()
    {
        ManageStepSound();
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }

        MovePlayer();
    }

    private void ManageStepSound()
    {
        bool isPlayerMoving = horInput != 0.0f || vertInput != 0.0f;
        if (!isPlayerMoving || !canMove)
        {
            MuteStepSound();
        }
        else
        {
            UnmuteStepSound();
        }
    }

    private void MuteStepSound()
    {
        playerAudioSourceComp.mute = true;
    }

    private void UnmuteStepSound()
    {
        playerAudioSourceComp.mute = false;
    }

    private void MovePlayer()
    {
        Vector2 currentPos = playerRigidBodyComp.position;

        horInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");

        Vector2 inputVect = new Vector2(horInput, vertInput);

        // Prevent diagonal movement to be faster than cardinal direction movement
        inputVect = Vector2.ClampMagnitude(inputVect, 1.0f);

        Vector2 movement = inputVect * playerSpeed;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;

        playerAnimationDirection.SetDirection(movement);
        playerRigidBodyComp.MovePosition(newPos);
    }

    public void StopPlayer()
    {
        horInput = 0.0f;
        vertInput = 0.0f;
        playerAnimationDirection.SetDirection(new Vector2(horInput, vertInput));

        canMove = false;
    }
}
