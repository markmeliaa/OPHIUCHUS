using UnityEngine;

[CreateAssetMenu(fileName = "DialogueLine", menuName = "DialogueItems/DialogueLine")]
public class DialogueLines : ScriptableObject
{
    [SerializeField] private DialogueBox characterSprite;
    [TextArea] public string dialogueText;
}
