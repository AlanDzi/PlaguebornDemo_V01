using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 9f;
    public float jumpForce = 12f;
    public float mouseSensitivity = 2f;
    public float gravityMultiplier = 2.5f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float stamina = 100f;
    public float staminaRegen = 15f;
    public float sprintCost = 20f;
    public float jumpCost = 15f;

    [Header("Sprint Limits")]
    public float minToStartSprint = 15f;
    public float minToKeepSprint = 5f;

    [Header("Ground Check")]
    public float groundDistance = 0.6f;
    public LayerMask groundMask;

    [Header("Camera")]
    public Transform cameraPivot;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;
    public float stepInterval = 0.5f;

    Rigidbody rb;
    Collider col;

    bool isGrounded;
    bool isSprinting;
    bool wantsToSprint;

    float xRotation;

    float horizontal;
    float vertical;

    float stepTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        footstepSource = GetComponent<AudioSource>();

        rb.freezeRotation = true;

        stamina = maxStamina;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Auto-setup kamery
        if (cameraPivot == null)
        {
            Camera cam = GetComponentInChildren<Camera>();

            if (cam != null)
                cameraPivot = cam.transform.parent;
        }
    }

    void Update()
    {
        // BLOKADA GRACZA GDY UI OTWARTE
        if (UIManager.Instance != null &&
            UIManager.Instance.IsAnyUIOpen)
            return;

        HandleInput();
        HandleMouseLook();
        HandleStamina();
        HandleJump();
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        CheckGround();
        HandleMovement();
    }

    // ================= INPUT =================

    void HandleInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        wantsToSprint = Input.GetKey(KeyCode.LeftShift);
    }

    // ================= CAMERA =================

    void HandleMouseLook()
    {
        if (UIManager.Instance != null &&
            UIManager.Instance.IsAnyUIOpen)
            return;

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        float mouseX =
            Input.GetAxis("Mouse X") *
            mouseSensitivity * 100f * Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            mouseSensitivity * 100f * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraPivot.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    // ================= GROUND =================

    void CheckGround()
    {
        Vector3 origin = col.bounds.center;
        origin.y = col.bounds.min.y + 0.05f;

        isGrounded = Physics.Raycast(
            origin,
            Vector3.down,
            groundDistance,
            groundMask
        );
    }

    // ================= MOVEMENT =================

    void HandleMovement()
    {
        Vector3 moveDir =
            transform.right * horizontal +
            transform.forward * vertical;

        moveDir.Normalize();

        float speed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 targetVelocity =
            new Vector3(
                moveDir.x * speed,
                rb.linearVelocity.y,
                moveDir.z * speed
            );

        rb.linearVelocity = targetVelocity;
    }

    // ================= STAMINA =================

    void HandleStamina()
    {
        if (isSprinting)
        {
            stamina -= sprintCost * Time.deltaTime;

            if (stamina <= minToKeepSprint)
                isSprinting = false;
        }
        else
        {
            stamina += staminaRegen * Time.deltaTime;
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        if (wantsToSprint &&
            isGrounded &&
            stamina >= minToStartSprint &&
            !isSprinting)
        {
            isSprinting = true;
        }

        if (!wantsToSprint || !isGrounded)
        {
            isSprinting = false;
        }
    }

    // ================= JUMP =================

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") &&
            isGrounded &&
            stamina >= jumpCost)
        {
            stamina -= jumpCost;

            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );
        }

        if (rb.linearVelocity.y < 0 && !isGrounded)
        {
            rb.AddForce(
                Vector3.down * gravityMultiplier,
                ForceMode.Acceleration
            );
        }
    }

    // ================= FOOTSTEPS =================

    void HandleFootsteps()
    {
        if (!isGrounded) return;

        if (horizontal == 0 && vertical == 0) return;

        float currentInterval = isSprinting
            ? stepInterval * 0.6f
            : stepInterval;

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            if (footstepSource != null && footstepClip != null)
            {
                footstepSource.PlayOneShot(footstepClip);
            }

            stepTimer = currentInterval;
        }
    }
}
