using UnityEngine;

public enum objectTypes { health, attack, defense, speed }

public class ItemObject
{
    public string objectName { get; set; }
    public objectTypes type { get; set; }
    public int level { get; set; }
    public GameObject gameText { get; set; }
    public bool used { get; set; }

    public ItemObject (string _objectName, objectTypes _type, int _level)
    {
        objectName = _objectName;
        type = _type;
        level = _level;
        gameText = null;
        used = false;
    }
}
