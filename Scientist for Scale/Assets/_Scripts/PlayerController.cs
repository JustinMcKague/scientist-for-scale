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
    [SerializeField] private float _pushingSpeed;
    [SerializeField] private LayerMask _jumpLayer;
    [SerializeField] private LayerMask _grabLayer;
    private Animator _anim;

    private Transform _playerTransform;

    private Vector2 _frameVelocity;

    private Vector2 _lastDir;
    private bool _isMoving;
    private bool _isGrabbing;

    private Rigidbody2D _grabbedObject;
    private Prop _propData;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
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


    #region Animation Data
    private int _currentState;

    private static readonly int Idle = Animator.StringToHash("Player_Idle");
    private static readonly int Run = Animator.StringToHash("Player_Run");
    private static readonly int Jump = Animator.StringToHash("Player_JumpStart");
    private static readonly int Apex = Animator.StringToHash("Player_JumpPeak");
    private static readonly int Fall = Animator.StringToHash("Player_JumpFall");
    #endregion

    private void OnEnable()
    {
        _inputReader.JumpEvent += HandleJump;
        _inputReader.JumpCancelled += Gravity;
        _inputReader.MoveEvent += GetDir;
        _inputReader.MoveCancelled += StopMove;
        _inputReader.GrabEvent += GrabActive;
        _inputReader.ReleaseEvent += Release;
    }

    private void OnDisable()
    {
        _inputReader.JumpEvent -= HandleJump;
        _inputReader.JumpCancelled -= Gravity;
        _inputReader.MoveEvent -= GetDir;
        _inputReader.MoveCancelled -= StopMove;
        _inputReader.GrabEvent -= GrabActive;
        _inputReader.ReleaseEvent -= Release;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerTransform = transform;
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var state = GetState();

        _time += Time.deltaTime;
        GatherInput();

        if (state == _currentState) return;
        _anim.CrossFade(state, 0, 0);
        _currentState = state;
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
        Debug.DrawRay(_playerTransform.position, _playerTransform.right * (2 * _playerTransform.localScale.x), Color.red);
        IsGrounded();
        if (_isMoving)
        {
            _rb.position += (_isGrabbing ? _pushingSpeed : _moveSpeed) * Vector2.right * _lastDir * Time.fixedDeltaTime;
        }
        ApplyMovement();
        Gravity();

        if (_rb.velocity.y < 0.2f && _rb.velocity.y > -0.2f && !IsGrounded())
        {
            _anim.CrossFade(Apex, 0, 0);
        }

        if (_propData && _propData.CanBePushed && _isGrabbing && PropInRange() && _isMoving && _propData.IsLightEnough)
        {
            _grabbedObject.position += _pushingSpeed * Vector2.right * _lastDir * Time.fixedDeltaTime;
        }
    }

    private int GetState()
    {
        if (_jumpKeyPressed) return Jump;
        if (IsGrounded()) return _isMoving ? Run : Idle;
        return _rb.velocity.y > 0 ? Jump : Fall;
    }

    private void ApplyMovement() => _rb.velocity = _frameVelocity;

    private void GetDir(Vector2 lastDir)
    {
        _lastDir = lastDir;
        Vector2 facing = new Vector2((int)_lastDir.x, 1);
        if (!_isGrabbing)
        {
            if (_lastDir.x > 0)
            {
                _sr.flipX = false;
            }
            else
                _sr.flipX = true;
        }
        _isMoving = true;
    }

    private void GrabActive()
    {
        if (PropInRange())
        {
            _isGrabbing = true;
        }
        else
        {
            _isGrabbing = false;
        }
    }

    private void Release()
    {
        _isGrabbing = false;
        if (_grabbedObject)
        {
            _grabbedObject.velocity = Vector2.zero;
            _grabbedObject.freezeRotation = true;
        }
        _grabbedObject = null;
    }

    private bool PropInRange()
    {
        RaycastHit2D hit = Physics2D.Raycast(_playerTransform.position, _playerTransform.right * (_sr.flipX ? -1 : 1), (1f * _playerTransform.localScale.x), _grabLayer);
        if (hit)
        {
            _grabbedObject = hit.collider.GetComponent<Rigidbody2D>();
            _propData = _grabbedObject.GetComponent<Prop>();
            return true;
        }
        else 
            return false;
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

        if (IsGrounded() && !_isGrabbing || HasCoyoteJump) AddJumpForce();

        _jumpAvailable = false;

    }

    private void AddJumpForce()
    {
        _anim.CrossFade(Jump, 0, 0);
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
            _frameVelocity.y = -1.5f;
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
        if (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.4f, _jumpLayer))
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

    private void OnBecameInvisible()
    {
        CameraManager.Instance.HasCameraPointData = false;
        CameraManager.Instance.CurrentCameraPoint = null;
        CameraManager.Instance.UpdateCameraSize();
    }
}
