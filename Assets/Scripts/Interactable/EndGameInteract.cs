using UnityEngine;

public class EndGameInteract : MonoBehaviour, IInteractable
{
    public string GetPromptText()
    {
        return "Naciœnij E aby zakoñczyæ grê";
    }

    public void Interact()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ShowEndGame();
    }
}
