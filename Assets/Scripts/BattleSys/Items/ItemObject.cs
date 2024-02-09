using UnityEngine;

public enum ObjectTypes 
{ 
    NONE, 
    HEALTH, 
    ATTACK, 
    DEFENSE,
    SPEED
}

public class ItemObject
{
    public string ObjectName { get; private set; }
    public ObjectTypes Type { get; private set; }
    public bool Consumed { get; set; }

    // TODO: What are these two for??
    public int Level { get; set; }
    public GameObject GameText { get; set; }

    public ItemObject (string objectName, ObjectTypes type, int level)
    {
        ObjectName = objectName;
        Type = type;
        Consumed = false;

        Level = level;
        GameText = null;
    }
}
