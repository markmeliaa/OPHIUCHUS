using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DialogueBox", menuName = "DialogueItems/DialogueBox")]
public class DialogueBox : ScriptableObject
{
    public string nameSpeaker;
    public Sprite imageSpeaker;

    public List<DialogConversation> characterConversations;
}
