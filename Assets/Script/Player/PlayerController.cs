using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float _maxMoveSpeed = 3f;
    private float _currentSpeed = 0f;
    private float _targetSpeed = 0f;
    private float _currentDampingVelocity = 0f;
    private float _WalkSpeed = 2.8f;
    private float _SprintSpeed = 5.6f;
    private Vector3 _normalizedForwardDirection = Vector3.zero;
    private Vector3 _normalizedRightDirection = Vector3.zero;
    [SerializeField] private float _accDampTime = 0.3f;
    private bool _isSprinting = false;
    [SerializeField][Range(0f, 1f)] private float _turnSpeed = 0.3f;
    private Vector2 _moveInput;
    private Vector3 _moveDirection = Vector3.zero;
    [SerializeField] private float _jumpHeight = 1.1f;
    [SerializeField][Range(0f, 1f)] private float _gravityScale = 1f;
    private bool _isJumping = false;
    private float _verticalVelocity = 0f;
    private float _fallTime = 0f;
    [SerializeField] private float _fallThresholsTime = 0.2f;
    private bool _isMovable = true;

    [Header("Look Camera")]
    [SerializeField] private CinemachineCamera _playerFollowCinemachineCamera;
    private Animator _playerAnimator;
    private CharacterController _playerCharacterController;

    void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerCharacterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        DialogueManager.OnDialogueStart += ControlPlayerMovementAndCameraFollow;
        DialogueManager.OnDialogueEnd += ControlPlayerMovementAndCameraFollow;
        PauseManager.OnPasuMenuOpen += DisableMovement;
        PauseManager.OnPasuMenuClose += EnableMovement;
        SaveManager.Instance.LoadPlayerTransform(transform);
    }

    void Update()
    {
        if (_isMovable)
        {
            JumpAndGravity();
            Move();
        }
    }
    
    void OnDisable()
    {
        DialogueManager.OnDialogueStart -= ControlPlayerMovementAndCameraFollow;
        DialogueManager.OnDialogueEnd -= ControlPlayerMovementAndCameraFollow;
        PauseManager.OnPasuMenuOpen -= DisableMovement;
        PauseManager.OnPasuMenuClose -= EnableMovement;
    }

    private void EnableMovement() => _isMovable = true;
    private void DisableMovement() => _isMovable = false;

    private void JumpAndGravity()
    {
        if (_isJumping && _playerCharacterController.isGrounded && _verticalVelocity <= 0f)
        {
            _isJumping = false;
        }
        if (!_playerCharacterController.isGrounded)
        {
            _verticalVelocity += Physics.gravity.y * _gravityScale * Time.deltaTime;
            if (_verticalVelocity < 0f)
            {
                _fallTime += Time.deltaTime;
                if (_fallTime > _fallThresholsTime)
                {
                    _playerAnimator.SetBool("isFalling", true);
                }
            }
        }
        else
        {
            _playerAnimator.SetBool("isFalling", false);
            _fallTime = 0f;
        }
        _verticalVelocity = Mathf.Clamp(_verticalVelocity, -10f, 10f);
    }

    private void Move()
    {
        if (_moveInput.magnitude != 0f)
        {
            Vector2 targetSpeed = _moveInput * (Input.GetKey(KeyCode.LeftShift) ? _SprintSpeed : _WalkSpeed);
            _targetSpeed = targetSpeed.magnitude;
        }
        else
        {
            _targetSpeed = 0f;
            _isSprinting = false;
        }
        // 移动加减速阻尼，手柄不同的输入值会导致不同的移动速度
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref _currentDampingVelocity, _accDampTime);
        _playerAnimator.SetFloat("Speed", _currentSpeed);
        // 角色带阻尼转向
        if (_moveInput.magnitude != 0f)
        {
            _moveDirection = (_normalizedForwardDirection * _moveInput.y + _normalizedRightDirection * _moveInput.x).normalized;
            Vector3 targetDirection = _moveDirection;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed);
        }

        // 计算角色相机朝向的移动方向
        Vector3 movement = _moveDirection * _currentSpeed * _maxMoveSpeed * Time.deltaTime;
        // 模拟重力
        movement.y += _verticalVelocity * Time.deltaTime;
        // 应用移动
        _playerCharacterController.Move(movement);
    }

    void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        Vector3 camForward = _playerFollowCinemachineCamera.transform.forward;
        Vector3 camRight = _playerFollowCinemachineCamera.transform.right;
        // 忽略摄像机俯仰角
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        // 合成移动方向
        _normalizedForwardDirection = camForward;
        _normalizedRightDirection = camRight;
    }

    /// <summary>
    /// 有Bug的冲刺输入，先按Shift后按W，会导致角色进入行走状态，不进入冲刺状态
    /// </summary>
    /// <param name="value"></param>
    void OnSprint(InputValue value)
    {
        _isSprinting = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        if (_isJumping) return;
        if (value.isPressed && _playerCharacterController.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * _jumpHeight);
            _isJumping = true;
            _playerAnimator.SetTrigger("Jump");
        }
    }

    private void ControlPlayerMovementAndCameraFollow(bool active)
    {
        // 停止角色移动
        var a = GetComponent<PlayerInput>();
        a.enabled = active;
        // 停止角色相机跟随
        _playerFollowCinemachineCamera.gameObject.SetActive(active);
    }
}