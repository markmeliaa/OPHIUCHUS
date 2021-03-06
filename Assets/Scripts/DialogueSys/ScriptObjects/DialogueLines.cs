using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueLine", menuName = "DialogueItems/DialogueLine")]
public class DialogueLines : ScriptableObject
{
    public DialogueBox characterSprite;
    [TextArea] public string dialogueText;
}
