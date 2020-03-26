using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerCollisions pColl;
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] float movementSpeed = 1, jumpHeight = 3f;
    [SerializeField] float maxHorizSpeed = 8.0f, maxVertSpeed = 25.0f;
    [SerializeField] float fallMultiplier = 3f;

    int currWallDir = 0;

    float lastGroundTouchTime = 0.0f;

    float xRaw = 0, yRaw = 0;
    float gravity = -17f, gravityMultiplier = 1f, lowJumpMultiplier = 2f;

    bool isDashing = false, hasDashed = false;
    bool isWallJumping = false;
    bool canMove = true;
    bool gravityActive = true;
    bool jumpRequest = false;

    Vector2 currVelocity;

    Coroutine moveRoutine, dashRoutine, gravRoutine;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pColl = GetComponent<PlayerCollisions>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        PlayerHandler.Player = this;
        PlayerHandler.SaveCheckpoint(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");

        //Ground Touch Time / Coyote Time
        if (!pColl.OnGround && lastGroundTouchTime < 0.1f)
        {
            lastGroundTouchTime += Time.deltaTime;
        }
        else if(pColl.OnGround)
        {
            lastGroundTouchTime = 0.0f;
        }

        //Check For Collisons
        if (pColl.OnAnyWall) // TODO - Detect corner clipping and make slid on top of ledge
        {
            var wallDir = pColl.OnRightWall ? 1 : -1;

            if (currWallDir != wallDir) // TODO - fix so that they can jump off same wall incase they make it back to the wall
            {
                isWallJumping = false;
                currWallDir = wallDir;

                if(hasDashed)
                {
                    if (dashRoutine != null) StopCoroutine(dashRoutine);
                    dashRoutine = StartCoroutine(SlowPlayerVelocity(1.5f, 0));
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequest = true;
        }
    }

    void FixedUpdate()
    {
        //Handle Player Movement
        HandleMovement();
    }

    private void HandleMovement()
    {
        currVelocity = rb.velocity;
        currVelocity.x /= 1.01f; // slows velocity over time to prevent slippery feel

        //Check if grounded and set velocity and other variables relative
        if (pColl.OnGround)
        {
            gravityMultiplier = 1;
            isWallJumping = false;
            hasDashed = false;

            anim.SetBool("IsInAir", false);
        }
        else
        {
            gravityMultiplier += Time.deltaTime;

            anim.SetBool("IsInAir", true);
        }

        //Check if player can move and calc their velocity and set sprite
        if (canMove && xRaw != 0)
        {
            currVelocity.x += movementSpeed * (int)xRaw;
            SetMovementAnimationState(true);
            FlipSprite((int)xRaw);
        }
        else if (xRaw == 0 && pColl.OnGround)
        {
            SetMovementAnimationState(false);
            currVelocity.x = 0;
        }

        //Check if player should Jump - might change to fixed update
        if (jumpRequest && (pColl.OnGround || lastGroundTouchTime < 0.1f)) // Todo - very tiny period where can doublejump, maybe add hasJumped bool
        {
            jumpRequest = false;
            currVelocity.y = Mathf.Sqrt(4f * jumpHeight);
        }
        else if (jumpRequest && pColl.OnAnyWall && !isWallJumping && !pColl.OnGround)
        {
            jumpRequest = false;
            currVelocity = WallJump(currVelocity);
        }

        //Manually set gravity
        currVelocity = HandleGravity(currVelocity);

        //Cap max speed
        currVelocity = CapVelocity(currVelocity);

        //Finally set the players position after all  the calculations
        rb.velocity = currVelocity;

        jumpRequest = false;
    }

    private void FlipSprite(int side)
    {
        sprite.flipX = side == -1;
    }

    private void SetMovementAnimationState(bool isActive)
    {
        if (pColl.OnGround && isActive)
        {
            anim.SetBool("isWalking", isActive);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    //private Vector2 BeginDashing(float xRaw, float yRaw, Vector2 currVelocity)
    //{
    //    hasDashed = true;
    //    isDashing = true;

    //    //Gravity coroutine
    //    if (gravRoutine != null) StopCoroutine(gravRoutine);
    //    if(moveRoutine != null) StopCoroutine(moveRoutine);

    //    gravRoutine = StartCoroutine(DisableGravity(0.22f));
    //    moveRoutine = StartCoroutine(DisableMovement(0.2f));

    //    gravityMultiplier = 1;
    //    currVelocity.y = 0;
    //    currVelocity.x = 0;

    //    var tempDashSpeed = dashSpeed;

    //    if(xRaw != 0 && yRaw != 0)
    //    {
    //        currVelocity.x = xRaw * (tempDashSpeed / 1.3f);
    //        currVelocity.y = yRaw * (tempDashSpeed / 1.7f);
    //    }
    //    else if(xRaw != 0 || yRaw != 0)
    //    {
    //        currVelocity.x = xRaw * tempDashSpeed * 1.1f;
    //        currVelocity.y = yRaw * tempDashSpeed / 1.7f;
    //    }
    //    else
    //    {
    //        if (sprite.flipX)
    //            currVelocity.x = -1 * (tempDashSpeed * 1.1f);
    //        else
    //            currVelocity.x = 1 * tempDashSpeed;
    //    }

    //    //slow down player after 0.16 seconds using coroutine 
    //    if (dashRoutine != null) StopCoroutine(dashRoutine);
    //    dashRoutine = StartCoroutine(SlowPlayerVelocity(2, 0.02f));

    //    return currVelocity;
    //}

    private Vector2 WallJump(Vector2 currVelocity)
    {
        if(true)
        {
            if (moveRoutine != null) StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(DisableMovement(0.2f));

            isWallJumping = true;
            gravityMultiplier = 1.68f;
            currVelocity.y = jumpHeight / 1.5f;
            currVelocity.x += 5 * (-currWallDir * 1.7f);
            FlipSprite(-currWallDir);
        }

        return currVelocity;
    }

    private Vector2 CapVelocity(Vector2 currVelocity)
    {
        if (currVelocity.x > 0)
            currVelocity.x = Mathf.Min(currVelocity.x, maxHorizSpeed);
        else if (currVelocity.x < 0)
            currVelocity.x = Mathf.Max(currVelocity.x, -maxHorizSpeed);

        if (currVelocity.y > 0)
            currVelocity.y = Mathf.Min(currVelocity.y, maxVertSpeed);
        else if (currVelocity.y < 0)
            currVelocity.y = Mathf.Max(currVelocity.y, -maxVertSpeed);
        return currVelocity;
    }

    private Vector2 HandleGravity(Vector2 currVelocity)
    {
        if (gravityActive)
        {
            var downscale = 0.018f;

            if (!pColl.OnGround && currVelocity.y < 0 && pColl.OnAnyWall)
            {
                // set slow wall gravity
                currVelocity.y += (gravity / 2) * gravityMultiplier * downscale;
            }
            else if (currVelocity.y < 0)
            {
                currVelocity.y += gravity * gravityMultiplier * downscale;
            }
            else if (currVelocity.y > 0 && !Input.GetKey(KeyCode.Space) /* && !isWallJumping && !isDashing*/ )
            {
                currVelocity.y += gravity * gravityMultiplier * downscale;
            }
            else
            {
                currVelocity.y += gravity * gravityMultiplier * downscale;
            }
        }

        return currVelocity;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;

        yield return new WaitForSeconds(time);

        canMove = true;
    }

    IEnumerator DisableGravity(float time)
    {
        gravityActive = false;

        yield return new WaitForSeconds(time);

        gravityActive = true;
    }

    IEnumerator SlowPlayerVelocity(float amount, float time)
    {
        yield return new WaitForSeconds(time);

        var currVelocity = rb.velocity;

        currVelocity.x /= amount;
        currVelocity.y /= amount * 1.8f;

        rb.velocity = currVelocity;

        isDashing = false;
    }
}
