using UnityEngine;
using System;

using Unity.Cinemachine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private ScriptableStats _stats;
    [SerializeField] private CinemachineCamera vCam;
    public Animator animLink;

    [Header("Debug")]
    public bool disableMove;
    public bool facingRight;
    public bool catchedPl;

    private FrameInput _frameInput;

    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;

    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    [SerializeField] private GameObject DialogBox;

    #region Interface

    public Vector2 FrameDirection => new Vector2(_frameInput.Horizontal, _frameVelocity.y);
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    #endregion

    private float _time;

    private bool startDisMove;
    private void Awake()
    {
        Instance = this;
        startDisMove = true;

        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();

        if (startDisMove)
        {
            disableMove = true;
        }
    }

    private void GatherInput()
    {
        if (!disableMove && Input.GetKeyDown(KeyCode.Space))
        {
            _jumpToConsume = true;
            _frameInput.JumpHeld = true;
            _timeJumpWasPressed = _time;
        }

        float inputHorizontal = Input.GetAxis("Horizontal");
        _frameInput = new FrameInput
        {
            JumpHeld = !disableMove ? Input.GetKey(KeyCode.Space) : false,
            Horizontal = !disableMove ? inputHorizontal != 0 ? inputHorizontal : Mathf.Lerp(_frameInput.Horizontal, inputHorizontal, Time.fixedDeltaTime * 10f) : 0,
        };

        if (_stats.SnapInput) _frameInput.Horizontal = Mathf.Abs(_frameInput.Horizontal) < _stats.HorizontalDeadZoneThreshold ? 0 : _frameInput.Horizontal;
    }

    private void FixedUpdate()
    {
        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();

        ApplyMovement();

        if (!disableMove) DistanceCam();
    }

    private void DistanceCam()
    {
        if (_frameVelocity.y == _stats.GroundingForce)
        {
            vCam.Lens.OrthographicSize = Mathf.MoveTowards(vCam.Lens.OrthographicSize, 4.4f, Time.deltaTime * 0.5f);
        }
        else if (_frameVelocity.y < _stats.GroundingForce || _frameVelocity.y > _stats.GroundingForce)
        {
            vCam.Lens.OrthographicSize = Mathf.MoveTowards(vCam.Lens.OrthographicSize, Mathf.Abs(_frameVelocity.y / 30f) + 4.4f, Time.deltaTime * 0.5f);
        }
    }

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    [SerializeField] private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        Vector2 playerScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size * playerScale, _col.direction, 0, Vector2.down, _stats.GrounderDistance, _stats.GroundLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size * playerScale, _col.direction, 0, Vector2.up, _stats.GrounderDistance, _stats.GroundLayer);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));

            if(startDisMove)
            {
                startDisMove = false;
                disableMove = false;

                DialogBox.SetActive(true);
                disableMove = true;
            }
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    public bool GetGround(float distanceCheck)
    {
        // Ground and Ceiling
        Vector2 playerScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        Vector2 size = _col.size * playerScale;
        size.x *= 12f;
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, size, _col.direction, distanceCheck, Vector2.down, distanceCheck, _stats.GroundLayer);

        // Landed on the Ground
        if (groundHit)
        {
            return true;
        }
        // Left the Ground
        else if (!groundHit)
        {
            return false;
        }
        return false;
    }

    #endregion

    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote)
        {
            ExecuteJump();
        }

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        float jumpPower = _stats.JumpPower;

        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = jumpPower;
        Jumped?.Invoke();
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        if (_frameInput.Horizontal == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            float speed = _stats.MaxSpeed;
            if (!_grounded) speed *= _stats.airMultiplier;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Horizontal * speed, _stats.Acceleration * Time.fixedDeltaTime);
        }

        if (_frameInput.Horizontal > 0 && !facingRight)
        {
            Flip();
        }
        else if (_frameInput.Horizontal < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;
}

public struct FrameInput
{
    public bool JumpHeld;
    public float Horizontal;
}

public interface IPlayerController
{
    public Vector2 FrameDirection { get; }

    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
}