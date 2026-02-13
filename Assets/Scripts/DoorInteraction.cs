using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 3f;

    private bool isOpen = false;
    private bool playerInside = false;

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = transform.localRotation;
        openRot = Quaternion.Euler(0, openAngle, 0) * closedRot;
    }

    void Update()
    {
        //  BLOKADA GDY UI OTWARTE
        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
            return;

        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        Quaternion target = isOpen ? openRot : closedRot;

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            target,
            Time.deltaTime * speed
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}