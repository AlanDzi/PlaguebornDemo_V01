using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;

    [Header("Bobbing")]
    public float bobSpeed = 8f;
    public float bobAmount = 0.05f;
    public float sprintMultiplier = 1.5f;
    public float smooth = 8f;

    private float timer;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;

        if (playerController == null)
            playerController = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
            return;

        if (playerController == null)
            return;

        float move =
            Mathf.Abs(Input.GetAxis("Horizontal")) +
            Mathf.Abs(Input.GetAxis("Vertical"));

        if (move > 0.1f && playerController.stamina > 0)
        {
            float speed = bobSpeed;

            if (playerController.GetComponent<Rigidbody>().linearVelocity.magnitude > playerController.walkSpeed)
                speed *= sprintMultiplier;

            timer += Time.deltaTime * speed;

            float x = Mathf.Cos(timer) * bobAmount;
            float y = Mathf.Sin(timer * 2f) * bobAmount;

            Vector3 target = startPos + new Vector3(x, y, 0);

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                target,
                Time.deltaTime * smooth
            );
        }
        else
        {
            timer = 0;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startPos,
                Time.deltaTime * smooth
            );
        }
    }
}