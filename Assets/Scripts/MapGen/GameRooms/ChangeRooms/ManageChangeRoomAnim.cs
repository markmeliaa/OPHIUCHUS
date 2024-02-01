using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageChangeRoomAnim : MonoBehaviour
{
    private GameObject player;
    private PlayerMovement playerMovement;
    private PlayerAnimationDirection playerAnimationDirection;

    [HideInInspector] public bool isPlayerTeleporting;

    private DungeonMapManager dungeonMapManager;

    private Vector3 directionToGoWhileTeleporting;
    private Vector2 directionToLookWhileTeleporting;

    private readonly float teleportingXMovement = 0.045f;
    private readonly float teleportingYMovement = 0.024f;

    private float exitAnimTime = 0.75f;
    private float enterAnimTime = 0.75f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimationDirection = player.GetComponent<PlayerAnimationDirection>();

        dungeonMapManager = GameObject.FindGameObjectWithTag("DungeonMngr").GetComponent<DungeonMapManager>();
    }

    private void FixedUpdate()
    {
        if (!isPlayerTeleporting)
        {
            return;
        }

        PerformTeleportMovement();
    }

    public void SetUpRoomTeleport(TravelDirection travelDirection)
    {
        switch (travelDirection)
        {
            case TravelDirection.NORTH:
                directionToGoWhileTeleporting = new Vector2(-teleportingXMovement, teleportingYMovement);
                directionToLookWhileTeleporting = new Vector2(-1, 1);
                break;

            case TravelDirection.SOUTH:
                directionToGoWhileTeleporting = new Vector2(teleportingXMovement, -teleportingYMovement);
                directionToLookWhileTeleporting = new Vector2(1, -1);
                break;

            case TravelDirection.EAST:
                directionToGoWhileTeleporting = new Vector2(teleportingXMovement, teleportingYMovement);
                directionToLookWhileTeleporting = new Vector2(1, 1);
                break;

            case TravelDirection.WEST:
                directionToGoWhileTeleporting = new Vector2(-teleportingXMovement, -teleportingYMovement);
                directionToLookWhileTeleporting = new Vector2(-1, -1);
                break;

            default:
                directionToGoWhileTeleporting = Vector2.zero;
                directionToLookWhileTeleporting = Vector2.zero;
                break;
        }
    }

    public void PerformTeleportMovement()
    {
        if (exitAnimTime >= 0.0f)
        {
            ManageExitRoomTeleportAnimation();
            exitAnimTime -= Time.deltaTime;
        }
        else if (enterAnimTime >= 0.0f)
        {
            ManageEnterRoomTeleportAnimation();
            enterAnimTime -= Time.deltaTime;
        }
        else
        {
            RestoreDefaultTeleportAnimationValues();
            isPlayerTeleporting = false;
            playerMovement.canMove = true;
        }
    }

    void ManageExitRoomTeleportAnimation()
    {
        dungeonMapManager.changeRoomAnim.SetBool("ChangeRoom", true);
        player.GetComponent<Collider2D>().isTrigger = true;
        player.transform.position += directionToGoWhileTeleporting;
        playerAnimationDirection.SetDirection(directionToLookWhileTeleporting);
    }

    void ManageEnterRoomTeleportAnimation()
    {
        dungeonMapManager.changeRoomAnim.SetBool("ChangeRoom", false);
        player.transform.position += directionToGoWhileTeleporting;
        playerAnimationDirection.SetDirection(directionToLookWhileTeleporting);
    }

    void RestoreDefaultTeleportAnimationValues()
    {
        playerAnimationDirection.SetDirection(new Vector2(0, 0));
        player.GetComponent<Collider2D>().isTrigger = false;
        exitAnimTime = 0.75f;
        enterAnimTime = 0.75f;
    }
}
