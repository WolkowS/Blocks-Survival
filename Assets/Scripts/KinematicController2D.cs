#define DEBUG_CC2D_RAYS
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CoreLib;


namespace Game
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class KinematicController2D : MonoBehaviour
    {
        private static RaycastHit2D[] s_RaycastResult = new RaycastHit2D[1];
        private static ContactFilter2D s_ContactFilter2D = new ContactFilter2D() { useDepth = true, useLayerMask = true, useNormalAngle = false, useTriggers = false, useOutsideNormalAngle = false, useOutsideDepth = false };

        public event Action<RaycastHit2D> onControllerCollidedEvent;
        public event Action<Collider2D>   onTriggerEnterEvent;
        public event Action<Collider2D>   onTriggerStayEvent;
        public event Action<Collider2D>   onTriggerExitEvent;

        [SerializeField]
        [Range(0.001f, 0.3f)]
        private float _skinWidth = 0.02f;

        /// <summary>
        /// defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
        /// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit)
        /// </summary>
        public float skinWidth
        {
            get => _skinWidth;
            set
            {
                _skinWidth = value;
                RecalculateDistanceBetweenRays();
            }
        }


        /// <summary>
        /// mask with all layers that the player should interact with
        /// </summary>
        public LayerMask platformMask = 0;

        /// <summary>
        /// mask with all layers that trigger events should fire when intersected
        /// </summary>
        public LayerMask triggerMask = 0;

        [Range(0, 20)]
        public  int _edgeCheck;

        [Range(2, 20)]
        public int totalHorizontalRays = 8;

        [Range(2, 20)]
        public int totalVerticalRays = 4;


        /// <summary>
        /// this is used to calculate the downward ray that is cast to check for slopes. We use the somewhat arbitrary value 75 degrees
        /// to calculate the length of the ray that checks for slopes.
        /// </summary>
        private float _slopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);

        public BoxCollider2D BoxCollider { get; private set; }
        public Rigidbody2D   RigidBody2D { get; private set; }

        [HideInInspector] [NonSerialized]
        public CharacterCollisionState2D collisionState = new CharacterCollisionState2D();

        public Vector3 Velocity;

        private const float k_SkinWidthFloatFudgeFactor = 0.001f;


        /// <summary>
        /// holder for our raycast origin corners (TR, TL, BR, BL)
        /// </summary>
        private CharacterRaycastOrigins _raycastOrigins;

        /// <summary>
        /// stores our raycast hit during movement
        /// </summary>
        private RaycastHit2D _raycastHit;

        /// <summary>
        /// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
        /// horizontally and vertically so that we can send the events after all collision state is set
        /// </summary>
        private List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>(2);

        // horizontal/vertical movement data
        private float _verticalDistanceBetweenRays;
        private float _horizontalDistanceBetweenRays;

        // we use this flag to mark the case where we are travelling up a slope and we modified our delta.y to allow the climb to occur.
        // the reason is so that if we reach the end of the slope we can make an adjustment to stay grounded
        private bool  _isGoingUpSlope = false;
        
        // =======================================================================
        struct CharacterRaycastOrigins
        {
            public Vector3 topLeft;
            public Vector3 bottomRight;
            public Vector3 bottomLeft;
        }

        public class CharacterCollisionState2D
        {
            public bool  right;
            public bool  left;
            public bool  above;
            public bool  below;
            public bool  edge;
            public bool  becameGroundedThisFrame;
            public bool  wasGroundedLastFrame;
            public bool  movingDownSlope;
            public float slopeAngle;

            public bool hasCollision()
            {
                return below || right || left || above;
            }


            public void reset()
            {
                right      = left = above = below = becameGroundedThisFrame = movingDownSlope = edge = false;
                slopeAngle = 0f;
            }


            public override string ToString()
            {
                return $"[CharacterCollisionState2D] r: {right}, l: {left}, a: {above}, b: {below}, " +
                       $"movingDownSlope: {movingDownSlope}, angle: {slopeAngle}, wasGroundedLastFrame: {wasGroundedLastFrame}, becameGroundedThisFrame: {becameGroundedThisFrame}";
            }
        }


        // =======================================================================
        private void Awake()
        {
            // cache some components
            BoxCollider = GetComponent<BoxCollider2D>();
            RigidBody2D = GetComponent<Rigidbody2D>();

            // here, we trigger our properties that have setters with bodies
            skinWidth = _skinWidth;

            // we want to set our CC2D to ignore all collision layers except what is in our triggerMask
            /*for (var i = 0; i < 32; i++)
            {
                // see if our triggerMask contains this layer and if not ignore it
                if ((triggerMask.value & 1 << i) == 0)
                    Physics2D.IgnoreLayerCollision(gameObject.layer, i);
            }*/
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
             if (onTriggerEnterEvent != null)
                onTriggerEnterEvent(col);
        }


        public void OnTriggerStay2D(Collider2D col)
        {
            if (onTriggerStayEvent != null)
                onTriggerStayEvent(col);
        }


        public void OnTriggerExit2D(Collider2D col)
        {
            if (onTriggerExitEvent != null)
                onTriggerExitEvent(col);
        }


        [System.Diagnostics.Conditional("DEBUG_CC2D_RAYS")]
        void DrawRay(Vector3 start, Vector3 dir, Color color)
        {
            Debug.DrawRay(start, dir, color);
        }


        /// <summary>
        /// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
        /// stop when run into.
        /// </summary>
        /// <param name="offset">Delta movement.</param>
        public void Move(Vector2 offset)
        {
            // save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
            collisionState.wasGroundedLastFrame = collisionState.below;

            // clear our state
            collisionState.reset();
            _raycastHitsThisFrame.Clear();
            _isGoingUpSlope = false;
            primeRaycastOrigins();
            RigidBody2D.simulated = false;

            // now we check movement in the horizontal dir
            if (offset.x != 0f)
                moveHorizontally(ref offset);

            // next, check movement in the vertical dir
            if (offset.y != 0f)
                moveVertically(ref offset);

            //RigidBody2D.MovePosition(RigidBody2D.position + deltaMovement.To2DXY());
            transform.Translate(offset, Space.World);

            // only calculate velocity if we have a non-zero deltaTime
            if (Time.deltaTime > 0f)
                Velocity = offset / Time.deltaTime;

            // set our becameGrounded state based on the previous and current collision state
            if (!collisionState.wasGroundedLastFrame && collisionState.below)
                collisionState.becameGroundedThisFrame = true;

            // if we are going up a slope we artificially set a y velocity so we need to zero it out here
            if (_isGoingUpSlope)
                Velocity.y = 0;

            // send off the collision events if we have a listener
            if (onControllerCollidedEvent != null)
            {
                for (var i = 0; i < _raycastHitsThisFrame.Count; i++)
                    onControllerCollidedEvent(_raycastHitsThisFrame[i]);
            }

            RigidBody2D.simulated = true;
        }
        
        /// <summary>
        /// this should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
        /// It is also used in the skinWidth setter in case it is changed at runtime.
        /// </summary>
        public void RecalculateDistanceBetweenRays()
        {
            // figure out the distance between our rays in both directions
            // horizontal
            var colliderUseableHeight = BoxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2f * _skinWidth);
            _verticalDistanceBetweenRays = colliderUseableHeight / (totalHorizontalRays - 1);

            // vertical
            var colliderUseableWidth = BoxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2f * _skinWidth);
            _horizontalDistanceBetweenRays = colliderUseableWidth / (totalVerticalRays - 1);
        }


        /// <summary>
        /// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
        /// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
        /// </summary>
        /// <param name="futurePosition">Future position.</param>
        /// <param name="deltaMovement">Delta movement.</param>
        private void primeRaycastOrigins()
        {
            // our raycasts need to be fired from the bounds inset by the skinWidth
            var modifiedBounds = BoxCollider.bounds;
            modifiedBounds.Expand(-2f * _skinWidth);

            _raycastOrigins.topLeft     = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);
            _raycastOrigins.bottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);
            _raycastOrigins.bottomLeft  = modifiedBounds.min;
        }


        /// <summary>
        /// we have to use a bit of trickery in this one. The rays must be cast from a small distance inside of our
        /// collider (skinWidth) to avoid zero distance rays which will get the wrong normal. Because of this small offset
        /// we have to increase the ray distance skinWidth then remember to remove skinWidth from deltaMovement before
        /// actually moving the player
        /// </summary>
        private void moveHorizontally(ref Vector2 deltaMovement)
        {
            var isGoingRight     = deltaMovement.x > 0;
            var rayDistance      = Mathf.Abs(deltaMovement.x) + _skinWidth;
            var rayDirection     = isGoingRight ? Vector2.right : Vector2.left;
            var initialRayOrigin = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

            var stepOverRay = new Vector2(initialRayOrigin.x, initialRayOrigin.y);

            DrawRay(stepOverRay, rayDirection * rayDistance, Color.yellow);
            for (var i = 0; i < totalHorizontalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays);


                DrawRay(ray, rayDirection * rayDistance, Color.red);

                // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
                // walk up sloped oneWayPlatforms
                if (i == 0 && collisionState.wasGroundedLastFrame)
                    _raycastHit = _raycast(ray, rayDirection, rayDistance, platformMask);
                else
                    _raycastHit = _raycast(ray, rayDirection, rayDistance, platformMask);

                if (_raycastHit)
                {
                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.x = _raycastHit.point.x - ray.x;
                    rayDistance     = Mathf.Abs(deltaMovement.x);

                    // remember to remove the skinWidth from our deltaMovement
                    if (isGoingRight)
                    {
                        deltaMovement.x      -= _skinWidth;
                        collisionState.right =  true;
                    }
                    else
                    {
                        deltaMovement.x     += _skinWidth;
                        collisionState.left =  true;
                    }

                    _raycastHitsThisFrame.Add(_raycastHit);

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    if (rayDistance < _skinWidth + k_SkinWidthFloatFudgeFactor)
                        break;
                }
            }
        }

        private void moveVertically(ref Vector2 deltaMovement)
        {
            var isGoingUp        = deltaMovement.y > 0;
            var rayDistance      = Mathf.Abs(deltaMovement.y) + _skinWidth;
            var rayDirection     = isGoingUp ? Vector2.up : Vector2.down;
            var initialRayOrigin = isGoingUp ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

            // apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
            initialRayOrigin.x += deltaMovement.x;

            // if we are moving up, we should ignore the layers in oneWayPlatformMask
            var mask         = platformMask;
            var groundedRays = 0;

            for (var i = 0; i < totalVerticalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y);

                DrawRay(ray, rayDirection * rayDistance, Color.red);
                _raycastHit = _raycast(ray, rayDirection, rayDistance, mask);
                if (_raycastHit)
                {
                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.y = _raycastHit.point.y - ray.y;
                    rayDistance     = Mathf.Abs(deltaMovement.y);

                    // remember to remove the skinWidth from our deltaMovement
                    if (isGoingUp)
                    {
                        deltaMovement.y      -= _skinWidth;
                        collisionState.above =  true;
                    }
                    else
                    {
                        deltaMovement.y      += _skinWidth;
                        collisionState.below =  true;
                        groundedRays ++;
                    }

                    _raycastHitsThisFrame.Add(_raycastHit);

                    // this is a hack to deal with the top of slopes. if we walk up a slope and reach the apex we can get in a situation
                    // where our ray gets a hit that is less then skinWidth causing us to be ungrounded the next frame due to residual velocity.
                    if (!isGoingUp && deltaMovement.y > 0.00001f)
                        _isGoingUpSlope = true;

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    // don't use if has edgeCheck
                    //if (_edgeCheck == 0 && rayDistance < _skinWidth + k_SkinWidthFloatFudgeFactor)break;
                }
            }

            if (_edgeCheck > 0)
                collisionState.edge = (totalVerticalRays - groundedRays) >= _edgeCheck;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RaycastHit2D _raycast(in Vector2 origin, in Vector2 direction, float distance, int layerMask)
        {
            return Physics2D.Raycast(origin, direction, distance, layerMask);
            /*s_ContactFilter2D.layerMask = layerMask;
            if (Physics2D.Raycast(origin, direction, s_ContactFilter2D, s_RaycastResult, distance) > 0)
                return s_RaycastResult[0];

            return default;*/
        }
    }
}