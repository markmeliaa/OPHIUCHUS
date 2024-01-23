using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueConversation", menuName = "DialogueItems/DialogueConversation")]

public class DialogConversation : ScriptableObject
{
    public int currentDialogueLine = 0;
    public List<DialogueLines> dialogueLines;
}
