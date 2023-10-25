//Player movement control

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControlScript : MonoBehaviour {

    //Player scripts
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private PlayerAnimationController _playerAnimationController;
    private PlayerManager _playerManager;

    //Player movement variables
    [SerializeField, ReadOnly] bool isGrounded;
    [Header("Movement")]
    [SerializeField] private float fallVelocity;
    private static float movementSpeed = 8;
    [SerializeField] private float gravityValue;
    [SerializeField] private float turnSpeed;
    [SerializeField] private LayerMask groundMask;
    [Space]
    [Header("Control input detection")]
    [SerializeField] private float deadzone;
    [SerializeField] LayerMask mouseDetection;
    //actions
    InputAction moveInput;
    public Vector2 move;
    InputAction lookInput;
    Vector2 look;
    public bool isDashing = false;

    private void Awake() {
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        _playerManager = GetComponent<PlayerManager>();
        _playerAnimationController = GetComponent<PlayerAnimationController>();
        moveInput = _playerInput.actions["Move"];
        lookInput = _playerInput.actions["Look"];
    }

    private void Start() {
        _playerManager.moveSpeed.BaseValue = movementSpeed;
        _playerManager.moveSpeed.AddModifiers(new(_playerManager.PlayerClass.stats.moveSpeedMultiplier, StatModType.PercentAdd));
    }

    private void OnEnable() {
        _playerInput.actions["Pause"].performed += OnPause;

    }
    private void OnDisable() {
        _playerInput.actions["Pause"].performed -= OnPause;

    }

    private void Update() {
        if (!isDashing) {
            move = moveInput.ReadValue<Vector2>();
            look = lookInput.ReadValue<Vector2>();
        }
        PlayerFall();
        PlayerMove();
        if (!isDashing) {
            PlayerLook();
        }
    }

    void PlayerFall() {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 0.5f, 0), 1, groundMask);
        fallVelocity += gravityValue * Time.deltaTime;
        if (isGrounded) //TouchingGround
        {
            fallVelocity = 0;
        }
    }

    void PlayerMove() {
        _playerAnimationController.SetAnimMoveVector(move);

        Vector3 moveDir = new(move.x, 0, move.y);
        moveDir *= _playerManager.moveSpeed.Value * Time.deltaTime;
        moveDir.y = fallVelocity;

        _characterController.Move(moveDir);
    }

    void PlayerLook() {
        if (Time.timeScale == 0 || !_playerManager.AliveState) { return; }

        switch (_playerInput.currentControlScheme) {

            case ("Gamepad"):
                //transform.rotation = Quaternion.LookRotation(new Vector3(look.x, 0, look.y), Vector3.zero);
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, new Vector3(look.x, 0, look.y), (turnSpeed * Time.deltaTime) * Time.timeScale, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDirection);
                break;

            case ("KeyboardMouse"):
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseDetection)) {
                    Vector3 lookDirection = new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z);

                    transform.rotation = Quaternion.LookRotation(lookDirection);
                }
                break;
        }
    }

    public void TeleportPlayer(Vector3 position) {
        _characterController.enabled = false;
        transform.position = position;
        _characterController.enabled = true;
    }

    private void OnDestroy() {
        _playerInput.actions["Pause"].performed -= OnPause;
    }

    public void OnPause(InputAction.CallbackContext context) {
        GameManager.TogglePause();
    }
}

