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
    public string objectName { get; set; }
    public ObjectTypes type { get; set; }
    public int level { get; set; }
    public GameObject gameText { get; set; }
    public bool used { get; set; }

    public ItemObject (string _objectName, ObjectTypes _type, int _level)
    {
        objectName = _objectName;
        type = _type;
        level = _level;
        gameText = null;
        used = false;
    }
}
