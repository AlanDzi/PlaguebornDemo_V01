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
        // tymczasowo wy³¹cz blokadê UI

        if (UIManager.Instance == null)
        {
            Debug.LogError("UIManager == NULL");
            return;
        }


        CheckInteractable();

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
            var interactable = hit.collider.GetComponentInParent<IInteractable>();

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
