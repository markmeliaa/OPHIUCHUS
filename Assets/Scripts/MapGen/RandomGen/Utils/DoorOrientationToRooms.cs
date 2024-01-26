using UnityEngine;

public enum DoorOrientation
{
    INVALID,
    BOTTOM,
    TOP,
    LEFT,
    RIGHT
};

public static class DoorOrientationToRooms
{
    public static DoorOrientation GetOppositeOrientation(DoorOrientation orientation)
    {
        DoorOrientation oppositeOrientation = DoorOrientation.INVALID;
        switch (orientation)
        {
            case DoorOrientation.BOTTOM:
                oppositeOrientation = DoorOrientation.TOP;
                break;

            case DoorOrientation.TOP:
                oppositeOrientation = DoorOrientation.BOTTOM;
                break;

            case DoorOrientation.LEFT:
                oppositeOrientation = DoorOrientation.RIGHT;
                break;

            case DoorOrientation.RIGHT:
                oppositeOrientation = DoorOrientation.LEFT;
                break;

            default:
                break;
        }

        return oppositeOrientation;
    }

    public static GameObject[] GetTemplateRoomsOfOneDirection(DoorOrientation direction)
    {
        CreateDungeonMapManager templates = GetCurrentRoomTemplatesObject();

        GameObject[] rooms = null;
        switch (direction)
        {
            case DoorOrientation.BOTTOM:
                rooms = templates.bottomMinimapRooms;
                break;

            case DoorOrientation.TOP:
                rooms = templates.topMinimapRooms;
                break;

            case DoorOrientation.LEFT:
                rooms = templates.leftMinimapRooms;
                break;

            case DoorOrientation.RIGHT:
                rooms = templates.rightMinimapRooms;
                break;

            default:
                break;
        }

        return rooms;
    }

    public static GameObject GetLimitRoomOfOneDirection(DoorOrientation direction)
    {
        CreateDungeonMapManager templates = GetCurrentRoomTemplatesObject();

        GameObject room = null;
        switch (direction)
        {
            case DoorOrientation.BOTTOM:
                room = templates.bottomMinimapRoom;
                break;

            case DoorOrientation.TOP:
                room = templates.topMinimapRoom;
                break;

            case DoorOrientation.LEFT:
                room = templates.leftMinimapRoom;
                break;

            case DoorOrientation.RIGHT:
                room = templates.rightMinimapRoom;
                break;

            default:
                break;
        }

        return room;
    }

    public static GameObject GetTemplateRoomWithTwoDirections(DoorOrientation orientation1, DoorOrientation orientation2)
    {
        CreateDungeonMapManager templates = GetCurrentRoomTemplatesObject();

        GameObject room = null;
        if (orientation2 < orientation1)
        {
            DoorOrientation aux = orientation2;
            orientation2 = orientation1;
            orientation1 = aux;
        }

        switch (orientation1)
        {
            case DoorOrientation.BOTTOM:
                if (orientation2 == DoorOrientation.TOP)
                {
                    room = templates.topBottomMinimapRoom;
                }
                else if (orientation2 == DoorOrientation.LEFT)
                {
                    room = templates.leftBottomMinimapRoom;
                }
                else if (orientation2 == DoorOrientation.RIGHT)
                {
                    room = templates.rightBottomMinimapRoom;
                }
                break;

            case DoorOrientation.TOP:
                if (orientation2 == DoorOrientation.LEFT)
                {
                    room = templates.topLeftMinimapRoom;
                }
                else if (orientation2 == DoorOrientation.RIGHT)
                {
                    room = templates.topRightMinimapRoom;
                }
                break;

            case DoorOrientation.LEFT:
                room = templates.leftRightMinimapRoom;
                break;

            default:
                break;
        }

        return room;
    }

    static CreateDungeonMapManager GetCurrentRoomTemplatesObject()
    {
        return GameObject.FindGameObjectWithTag("Rooms").GetComponent<CreateDungeonMapManager>();
    }
}
