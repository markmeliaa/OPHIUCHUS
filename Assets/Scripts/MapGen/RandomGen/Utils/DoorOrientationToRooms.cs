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

    public static GameObject[] GetMinimapRoomsWithOneDirection(DoorOrientation direction)
    {
        GameObject[] rooms = null;
        switch (direction)
        {
            case DoorOrientation.BOTTOM:
                rooms = RoomsHolderSingleton.Instance.bottomMinimapRooms;
                break;

            case DoorOrientation.TOP:
                rooms = RoomsHolderSingleton.Instance.topMinimapRooms;
                break;

            case DoorOrientation.LEFT:
                rooms = RoomsHolderSingleton.Instance.leftMinimapRooms;
                break;

            case DoorOrientation.RIGHT:
                rooms = RoomsHolderSingleton.Instance.rightMinimapRooms;
                break;

            default:
                break;
        }

        return rooms;
    }

    public static GameObject GetMinimapRoomWithOneDirection(DoorOrientation direction)
    {
        GameObject room = null;
        switch (direction)
        {
            case DoorOrientation.BOTTOM:
                room = RoomsHolderSingleton.Instance.bottomMinimapRoom;
                break;

            case DoorOrientation.TOP:
                room = RoomsHolderSingleton.Instance.topMinimapRoom;
                break;

            case DoorOrientation.LEFT:
                room = RoomsHolderSingleton.Instance.leftMinimapRoom;
                break;

            case DoorOrientation.RIGHT:
                room = RoomsHolderSingleton.Instance.rightMinimapRoom;
                break;

            default:
                break;
        }

        return room;
    }

    public static GameObject GetMinimapRoomWithTwoDirections(DoorOrientation orientation1, DoorOrientation orientation2)
    {
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
                    room = RoomsHolderSingleton.Instance.topBottomMinimapRoom;
                }
                else if (orientation2 == DoorOrientation.LEFT)
                {
                    room = RoomsHolderSingleton.Instance.leftBottomMinimapRoom;
                }
                else if (orientation2 == DoorOrientation.RIGHT)
                {
                    room = RoomsHolderSingleton.Instance.rightBottomMinimapRoom;
                }
                break;

            case DoorOrientation.TOP:
                if (orientation2 == DoorOrientation.LEFT)
                {
                    room = RoomsHolderSingleton.Instance.topLeftMinimapRoom;
                }
                else if (orientation2 == DoorOrientation.RIGHT)
                {
                    room = RoomsHolderSingleton.Instance.topRightMinimapRoom;
                }
                break;

            case DoorOrientation.LEFT:
                room = RoomsHolderSingleton.Instance.leftRightMinimapRoom;
                break;

            default:
                break;
        }

        return room;
    }
}
