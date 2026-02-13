using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactableLayer;

    private Camera cam;
    private IInteractable current;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // NIE INTERAKTUJ GDY UI OTWARTE
        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
            return;

        CheckInteractable();

        // BLOKADA NA 0.2s PO ZAMKNIÊCIU UI
        if (current != null &&
            Input.GetKeyDown(KeyCode.E) &&
            UIManager.Instance != null &&
            UIManager.Instance.CanInteractNow)
        {
            current.Interact();
        }
    }

    void CheckInteractable()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                current = interactable;

                if (UIManager.Instance != null)
                    UIManager.Instance.ShowInteractionPrompt(
                        true,
                        interactable.GetPromptText()
                    );

                return;
            }
        }

        current = null;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowInteractionPrompt(false);
    }
}
