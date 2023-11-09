using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInputs _inputReader = default;

    [SerializeField] private float _jumpVelocity;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private LayerMask _layerMask;

    private Vector2 _frameVelocity;

    private Vector2 _lastDir;
    private bool _isMoving;

    private Rigidbody2D _rb;
    private float _time;

    [Header("Jump Settings")]
    private bool _jumpAvailable;
    private bool _bufferedJumpAvailable;
    private bool _coyoteJumpAvailable;
    private bool _endedJumpEarly;
    private float _timeJumpPressed;
    private float _frameLeftGround;
    [SerializeField] private float _fallAccel;
    [SerializeField] private float _jumpBufferWindow;
    [SerializeField] private float _coyoteBufferWindow;
    [SerializeField] private float _terminalVelocity;
    [SerializeField] private float _jumpEarlyModifier;
    [SerializeField] private float _longJumpDuration;
    private bool _jumpKeyPressed;
    private bool _leftGround;

    private float _timeJumpHeld = 0;

    private bool HasBufferedJump => _bufferedJumpAvailable && _time < _timeJumpPressed + _jumpBufferWindow;
    private bool HasCoyoteJump => _coyoteJumpAvailable && !IsGrounded() && _time < _frameLeftGround + _coyoteBufferWindow;


    private void OnEnable()
    {
        _inputReader.JumpEvent += HandleJump;
        _inputReader.JumpCancelled += Gravity;
        _inputReader.MoveEvent += GetDir;
        _inputReader.MoveCancelled += StopMove;
    }

    private void OnDisable()
    {
        _inputReader.JumpEvent -= HandleJump;
        _inputReader.JumpCancelled -= Gravity;
        _inputReader.MoveEvent -= GetDir;
        _inputReader.MoveCancelled -= StopMove;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();

        Debug.DrawRay(transform.position, Vector2.down * (transform.localScale.y / 2 + 0.1f), Color.red);
    }

    private void GatherInput()
    {
        if (!_jumpAvailable && _inputReader.gameInput.Player.Jump.phase == InputActionPhase.Performed)
        {
            _jumpAvailable = true;
            _timeJumpPressed = _time;
        }
        //if (_inputReader.gameInput.Player.Jump.phase == InputActionPhase.Performed)
        //{
        //    _timeJumpHeld += Time.fixedDeltaTime;
        //}
    }

    private void FixedUpdate()
    {
        IsGrounded();
        if (_isMoving)
        {
            _rb.position += _moveSpeed * Vector2.right * _lastDir * Time.fixedDeltaTime;
        }
        ApplyMovement();
        Gravity();
    }

    private void ApplyMovement() => _rb.velocity = _frameVelocity;

    private void GetDir(Vector2 lastDir)
    {
        _lastDir = lastDir;
        _isMoving = true;
    }

    private void StopMove()
    {
        _isMoving = false;
    }

    private void HandleJump()
    {
        _jumpKeyPressed = true;
        if (!_endedJumpEarly && !IsGrounded() && _timeJumpHeld < _longJumpDuration && _jumpKeyPressed && _rb.velocity.y > 0) _endedJumpEarly = true;

        if (!_jumpAvailable && !HasBufferedJump) return;

        if (IsGrounded() || HasCoyoteJump) AddJumpForce();

        _jumpAvailable = false;

    }

    private void AddJumpForce()
    {
        _coyoteJumpAvailable = false;
        _endedJumpEarly = false;
        _timeJumpPressed = 0;
        _bufferedJumpAvailable = false;
        _jumpAvailable = false;
        _frameVelocity.y = _jumpVelocity;
    }

    private void Gravity()
    {
        if (IsGrounded() && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = 0f;
        }
        else
        {
            var deltaVelocity = _fallAccel;
            if (_endedJumpEarly && _frameVelocity.y > 0) deltaVelocity *= _jumpEarlyModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_terminalVelocity, deltaVelocity * Time.fixedDeltaTime);
        }
        _jumpKeyPressed = false;
    }

    private bool IsGrounded()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.1f, _layerMask))
        {
            _coyoteJumpAvailable = true;
            _bufferedJumpAvailable = true;
            _endedJumpEarly = false;
            _jumpAvailable = true;
            _timeJumpHeld = 0;
            _leftGround = false;
            return true;
        }
        else
        {
            if (!_leftGround)
                _frameLeftGround = _time; _leftGround = true;
            return false;
        }
    }
}
