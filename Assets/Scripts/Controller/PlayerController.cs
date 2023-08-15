using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib;
using UnityEngine;
using UnityEngine.Events;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// Right now it only contains movement and jumping, but it should be pretty easy to expand... I may even do it myself
    /// if there's enough interest. You can play and compete for best times here: https://tarodev.itch.io/
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/GqeHHnhHpz
    /// </summary>
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        // Public for external hooks
        public  Vector3    Velocity         { get; private set; }
        public  FrameInput Input            { get; private set; }
        public  bool       JumpingThisFrame { get; private set; }
        public  bool       LandingThisFrame { get; private set; }
        public  Vector3    RawMovement      { get; private set; }
        public  bool       Grounded         => _colDown;
        public  Vector2    Move             { get; set; }
        private bool       _jumpPrev;
        private bool       _jump;
        private bool       _climbLeft;
        private bool       _climbRight;
        private bool       _dash;
        
        public bool Dash
        {
            set { _dash = value; }
        }
        
        public bool Jump
        {
            set { _jump = value; }
        }

        private Vector3 _lastPosition;
        private float   _currentHorizontalSpeed, _currentVerticalSpeed;

        // This is horrible, but for some reason colliders are not fully established when update starts...
        private bool _active;
        void Awake() => Invoke(nameof(Activate), 0.5f);
        void Activate() => _active = true;

        public bool ClimbLeft
        {
            get => _climbLeft;
            set => _climbLeft = value;
        }

        public bool ClimbRight
        {
            get => _climbRight;
            set => _climbRight = value;
        }

        [Header("COLLISION")] [SerializeField] private Bounds    _characterBounds;
        [SerializeField]                       private LayerMask _groundLayer;
        [SerializeField]                       private int       _detectorCount      = 3;
        [SerializeField]                       private float     _detectionRayLength = 0.1f;
        [SerializeField] [Range(0.1f, 0.3f)]   private float     _rayBuffer          = 0.1f; // Prevents side detectors hitting the ground
        private                                        RayRange  _raysUp, _raysRight, _raysDown, _raysLeft;
        private                                        bool      _colUp,  _colRight,  _colDown,  _colLeft;
        private                                        float     _timeLeftGrounded;
        [Header("WALKING")] [SerializeField] private   float     _acceleration   = 90;
        [SerializeField]                     private   float     _moveClamp      = 13;
        [SerializeField]                     private   float     _deAcceleration = 60f;
        [SerializeField]                     private   float     _apexBonus      = 2;
        [Header("GRAVITY")] [SerializeField] private   float     _fallClamp      = -40f;
        [SerializeField]                     private   float     _minFallSpeed   = 80f;
        [SerializeField]                     private   float     _maxFallSpeed   = 120f;
        private                                        float     _fallSpeed;
        [Header("JUMPING")]
        [SerializeField] private   float     _jumpHeight                  = 30;
        [SerializeField]                     private   float     _jumpApexThreshold           = 10f;
        [SerializeField]                     private   float     _coyoteTimeThreshold         = 0.1f;
        [SerializeField]                     private   float     _jumpBuffer                  = 0.1f;
        [SerializeField]                     private   float     _jumpEndEarlyGravityModifier = 3;
        private                                        bool      _coyoteUsable;
        private                                        bool      _endedJumpEarly = true;
        private                                        float     _apexPoint; // Becomes 1 at the apex of a jump
        private                                        float     _lastJumpPressed;
        private                                        bool      CanUseCoyote    => _coyoteUsable && !_colDown && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
        private                                        bool      HasBufferedJump => _colDown && _lastJumpPressed + _jumpBuffer > Time.time;
        
        [Header("DASH")]
        [SerializeField]
        private float _dashSpeed = 10;
        [SerializeField]
        private float _dashDuration = 0.7f;
        private float _dashTime;
        public float  _dashBounce;
        private Vector2 _dashDir;
        private bool _dashPrev;
        
        [Header("Physics")]
        public Vector2 _velocity;
        public float   _damping;

        [Header("MOVE")] [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
        private int _freeColliderIterations = 10;

        [Header("CALLBACKS")]
        public UnityEvent<bool> _onDashState;
        public UnityEvent<Collider2D> _onDashHit;
        
        // =======================================================================
        private void Update()
        {
            if (!_active) 
                return;

            // Calculate velocity
            Velocity      = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;
            
            _gatherInput();
            _runCollisionChecks();
            
            if (_dashPrev != Input.Dash)
            {
                _onDashState.Invoke(Input.Dash);
                _dashPrev = Input.Dash;
            }

            if (Input.Dash)
            {
                _moveCharacter(_dashDir * _dashSpeed, out var isBlocked, out var hit);
                if (isBlocked)
                {
                    _dashTime = 0f;
                    _onDashHit.Invoke(hit);
                }
                
                _velocity = Vector2.zero;
                if (isBlocked)
                {
                    _velocity += _dashDir.ToDirection() switch
                    {
                        Direction.Left  => new Vector2(1, 1).normalized * _dashBounce,
                        Direction.Right => new Vector2(-1, 1).normalized * _dashBounce,
                        Direction.Down  => new Vector2(0, 1) * _dashBounce,

                        Direction.Left | Direction.Up   => new Vector2(1, 1).normalized * _dashBounce,
                        Direction.Left | Direction.Down => new Vector2(1, 1).normalized * _dashBounce,

                        Direction.Right | Direction.Up   => new Vector2(-1, 1).normalized * _dashBounce,
                        Direction.Right | Direction.Down => new Vector2(-1, 1).normalized * _dashBounce,

                        Direction.Up     => new Vector2(0, 1) * _dashBounce,
                        Direction.None   => Vector2.zero,
                        Direction.Center => Vector2.zero,
                        _                => throw new ArgumentOutOfRangeException()
                    };
                }
            }
            else
            {
                _calculateWalk();     // Horizontal movement
                _calculateJumpApex(); // Affects fall speed, so calculate before gravity
                _calculateGravity();  // Vertical movement
                _calculateJump();     // Possibly overrides vertical
                
                if (float.IsNaN(_currentHorizontalSpeed))
                    _currentHorizontalSpeed = 0f;
                if (float.IsNaN(_currentVerticalSpeed))
                    _currentVerticalSpeed = 0f;
                
                _moveCharacter(new Vector2(_currentHorizontalSpeed, _currentVerticalSpeed) + _velocity, out _, out _);     // Actually perform the axis movement}
                _velocity = Vector2.MoveTowards(_velocity, Vector2.zero, _damping * Time.deltaTime);
            }
        }

        private void _gatherInput()
        {
            if (_dash)
            {
                _dash     = false;
                if ((Time.time - _dashTime) > _dashDuration)
                {
                    // disallow dash when dashing
                    _dashTime = Time.time;
                    _dashDir  = Move.ToDirectionBox().ToVector2();
                }
            }
            
            Input = new FrameInput { JumpDown = _jumpPrev == false && _jump == true, JumpUp = _jumpPrev == true && _jump == false, X = Move.x, Dash = (Time.time - _dashTime) <= _dashDuration};
            
            _jumpPrev = _jump;
            if (Input.JumpDown)
            {
                _lastJumpPressed = Time.time;
            }
        }

        private void _runCollisionChecks()
        {
            // Generate ray ranges. 
            _calculateRayRanged();

            // Ground
            LandingThisFrame = false;
            var groundedCheck = RunDetection(_raysDown);
            if (_colDown && !groundedCheck) 
            {
                _timeLeftGrounded = Time.time; // Only trigger when first leaving}
            }
            else 
            if (!_colDown && groundedCheck)
            {
                _coyoteUsable    = true; // Only trigger when first touching
                LandingThisFrame = true;
            }

            _colDown = groundedCheck;

            // The rest
            _colUp    = RunDetection(_raysUp);
            _colLeft  = RunDetection(_raysLeft);
            _colRight = RunDetection(_raysRight);

            // -----------------------------------------------------------------------
            bool RunDetection(RayRange range)
            {
                return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _groundLayer));
            }
        }

        private void _calculateRayRanged()
        {
            // This is crying out for some kind of refactor. 
            var b = new Bounds(transform.position + _characterBounds.center, _characterBounds.size);
            _raysDown  = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
            _raysUp    = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
            _raysLeft  = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
            _raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
        }

        private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
        {
            for (var i = 0; i < _detectorCount; i++)
            {
                var t = (float)i / (_detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
        }

        private void OnDrawGizmos()
        {
            // Bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

            // Rays
            if (!Application.isPlaying)
            {
                _calculateRayRanged();
                Gizmos.color = Color.blue;
                foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft })
                {
                    foreach (var point in EvaluateRayPositions(range))
                    {
                        Gizmos.DrawRay(point, range.Dir * _detectionRayLength);
                    }
                }
            }

            if (!Application.isPlaying) return;

            // Draw the future position. Handy for visualizing gravity
            Gizmos.color = Color.red;
            var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
            Gizmos.DrawWireCube(transform.position + _characterBounds.center + move, _characterBounds.size);
        }

        private void _calculateDash()
        {
        }
        
        private void _calculateWalk()
        {
            if (Input.X != 0)
            {
                // Set horizontal move speed
                _currentHorizontalSpeed += Input.X * _acceleration * Time.deltaTime;

                // clamped by max frame movement
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);

                // Apply bonus at the apex of a jump
                var apexBonus = Mathf.Sign(Input.X) * _apexBonus * _apexPoint;
                _currentHorizontalSpeed += apexBonus * Time.deltaTime;
            }
            else
            {
                // No input. Let's slow the character down
                _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
            }

            if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft)
            {
                // Don't walk through walls
                _currentHorizontalSpeed = 0;
            }
        }

        private void _calculateGravity()
        {
            if (_colDown)
            {
                // Move out of the ground
                if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
            }
            else
            {
                // Add downward force while ascending if we ended the jump early
                var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;

                // Fall
                _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

                // Clamp
                if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
            }
        }

        private void _calculateJumpApex()
        {
            if (!_colDown)
            {
                // Gets stronger the closer to the top of the jump
                _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
            }
            else
            {
                _apexPoint = 0;
            }
        }

        private void _calculateJump()
        {
            // Jump if: grounded or within coyote threshold || sufficient jump buffer
            if (Input.JumpDown && CanUseCoyote || HasBufferedJump)
            {
                _currentVerticalSpeed = _jumpHeight;
                _endedJumpEarly       = false;
                _coyoteUsable         = false;
                _timeLeftGrounded     = float.MinValue;
                JumpingThisFrame      = true;
            }
            else
            {
                JumpingThisFrame = false;
            }

            // End the jump early if button released
            if (!_colDown && Input.JumpUp && !_endedJumpEarly && Velocity.y > 0)
            {
                // _currentVerticalSpeed = 0;
                _endedJumpEarly = true;
            }

            if (_colUp)
            {
                if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
            }
        }

        private void _moveCharacter(Vector2 speed, out bool blocked, out Collider2D hit)
        {
            var pos = transform.position + _characterBounds.center;
            RawMovement = speed.To3DXY();
            var move          = RawMovement * Time.deltaTime;
            var furthestPoint = pos + move;
            blocked = false;

            // check furthest movement. If nothing hit, move and don't do extra checks
            hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
            if (!hit)
            {
                transform.position += move;
                return;
            }

            // otherwise increment away from current pos; see what closest position we can move to
            var positionToMoveTo = transform.position;
            for (var i = 1; i < _freeColliderIterations; i++)
            {
                // increment to check all but furthestPoint - we did that already
                var t        = (float)i / _freeColliderIterations;
                var posToTry = Vector2.Lerp(pos, furthestPoint, t);
                hit = Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer);
                if (hit)
                {
                    transform.position = positionToMoveTo;

                    // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                    if (i == 1)
                    {
                        if (_currentVerticalSpeed < 0) 
                            _currentVerticalSpeed = 0;
                        var dir = transform.position - hit.transform.position;
                        transform.position += dir.normalized * move.magnitude;
                    }
                    blocked = true;
                    return;
                }

                positionToMoveTo = posToTry;
            }
        }
    }
}