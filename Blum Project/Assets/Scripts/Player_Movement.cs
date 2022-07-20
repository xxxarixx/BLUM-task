using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private Player_References data;

    [Header("Movement")]
    [SerializeField] private float speedOnGround = 500f;
    [SerializeField] private float speedInAir = 250f;
    private Vector2 _lastVelocityBeforeStop;
    private enum _MoveAxis
    {
        Horizontal,
        Verical
    }
    public float speed_current { get; private set; }

    [Header("Jumping")]
    public float justpressJumpVelocity = 5f;
    public AnimationCurve jumpVelocity;
    private bool _jumpReachedEnd = false;

    [Header("Grounded")]
    [SerializeField] private float grounded_DistancePlayerGround;
    [SerializeField] private LayerMask groundLayerMask;
    private float _jumpPressProgress = 0f;
    private bool _isPerformingJump = false;
    private float _maxJumpProgressTime;

    [Header("Debug")]
    [SerializeField] private bool grounded;
    [SerializeField]private Vector3 debug_rbVelocity;
    #region Unity Functions
    private void FixedUpdate()
    {
        _FixedUpdate_Debug();
        grounded = Physics2D.Raycast(data.flip_Pivolt.position, Vector2.down, grounded_DistancePlayerGround, groundLayerMask);
        _Movement_horizontal();
        _Jump_Action();
    }
    private void OnEnable()
    {
        _VeriableSetup();
        data.input.OnJumpCancelled += _Input_OnJumpCancelled;
        data.input.OnJumpJustPressed += _Input_OnJumpJustPressed;
    }

    private void OnDisable()
    {
        data.input.OnJumpCancelled -= _Input_OnJumpCancelled;
        data.input.OnJumpJustPressed -= _Input_OnJumpJustPressed;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = (grounded)?Color.green : Color.red;
        Gizmos.DrawLine(data.flip_Pivolt.position, data.flip_Pivolt.position + Vector3.down * grounded_DistancePlayerGround);
    }
    #endregion


    #region Private Functions
    private void _Input_OnJumpJustPressed()
    {
        _isPerformingJump = (grounded) ? true : false;
        if (_isPerformingJump)
        {
            data.PlayAnimation(Player_References.animations.jump);
        }
    }
    private void _Input_OnJumpCancelled()
    {
        _Jump_Reset();
    }
    private void _VeriableSetup()
    {
        _maxJumpProgressTime = jumpVelocity.keys[jumpVelocity.length - 1].time;
        speed_current = speedOnGround;
    }
    private void _Jump_Perform()
    {
        _jumpPressProgress += Time.deltaTime;
        _Move(jumpVelocity.Evaluate(_jumpPressProgress), _MoveAxis.Verical);
        _jumpReachedEnd = false;
        if(_jumpPressProgress >= _maxJumpProgressTime)
        {
            _jumpReachedEnd = true;
            _Jump_Reset();
        }
    }
    private void _Jump_Reset()
    {
        //reset player velocity Y if player doesnt reached end jump curve velocity and pressed up jump key
        if (!_jumpReachedEnd)
        {
            Debug.Log("AppliedReachedEndVelocity");
            data.rb.velocity = new Vector2(data.rb.velocity.x, justpressJumpVelocity);
            _jumpReachedEnd = true;
        }
        _isPerformingJump = false;
        _jumpPressProgress = 0f;
    }
    private void _Jump_Action()
    {
        if (data.input.jumpPressed && _isPerformingJump) _Jump_Perform();
        if (data.rb.velocity.y < 0 && !grounded && _jumpReachedEnd) data.PlayAnimation(Player_References.animations.falling);
    }
    private void _Movement_horizontal()
    {
        _Move(speed_current, _MoveAxis.Horizontal);
        _FlipBasedOnVelocity();
        _lastVelocityBeforeStop = (data.rb.velocity != Vector2.zero) ? data.rb.velocity : _lastVelocityBeforeStop;
        if (grounded && !_isPerformingJump)
        {
            if(Mathf.Abs(data.rb.velocity.x) > 0f)
            {
                data.PlayAnimation(Player_References.animations.walk);
            }
            else
            {
                data.PlayAnimation(Player_References.animations.idle);
            }
        }

        if (!grounded)
        {
            _ChangeSpeed(speedInAir);
        }
        else
        {
            _ChangeSpeed(speedOnGround);
        }
    }
    private void _ChangeSpeed(float _newSpeed)
    {
        speed_current = _newSpeed;
    }
    private void _Move(float _speed, _MoveAxis _axis)
    {
        //axis that player should move
        Vector2 axis = (_axis == _MoveAxis.Horizontal)? new Vector2(1,0) : new Vector2(0,1);
        //invert axis to get velocity that shouldnt be changed by this movement
        Vector2 invertedAxis = (_axis == _MoveAxis.Horizontal) ? new Vector2(0, 1) : new Vector2(1, 0);
        data.rb.velocity = axis * data.input.moveInput * _speed * Time.fixedDeltaTime + invertedAxis * data.rb.velocity;
    }
    private void _FixedUpdate_Debug()
    {
        debug_rbVelocity = data.rb.velocity;
    }
    private void _FlipBasedOnVelocity()
    {
        var moveDir = (data.rb.velocity.x == 0)? _lastVelocityBeforeStop.x : data.rb.velocity.x;
        data.flip_Pivolt.localScale = new Vector3((moveDir > 0f) ? Mathf.Abs(data.flip_Pivolt.localScale.x) : -Mathf.Abs(data.flip_Pivolt.localScale.x), data.flip_Pivolt.localScale.y, data.flip_Pivolt.localScale.z);
    }
    #endregion

    #region Public Functions

    #endregion
}
