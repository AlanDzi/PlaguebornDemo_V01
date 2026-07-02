using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    [TextArea]
    public string text;

    public string GetPromptText()
    {
        return "E - Read";
    }

    public void Interact()
    {
        UIManager.Instance.ShowNote(text);
    }
}