using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class HUNDController : FighterController
{
    [SerializeField] private int speed;
    [SerializeField] private GameManager gm;
    [SerializeField] private HitBox hitBox;
    private GameObject pw350;

    private bool facingRight;
    private bool isMoving;
    private bool canAttack;
    private bool isAttacking;

    private int currentSprite;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] biteSprites;
    [SerializeField] private Sprite[] pounceSprites;
    [SerializeField] private Sprite[] deathSprites;

    private int attackType;
    private int startupFrameCounter;
    private int activeFrameCounter;
    private int recoveryFrameCounter;

    // Start is called before the first frame update
    void Start()
    {
        currentSprite = -1;
        pw350 = gm.GetPW();

        facingRight = false;
        isMoving = false;
        canAttack = true;
        isAttacking = false;

        startupFrameCounter = 0;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        hp = 1;
        SetUpAttacks();
    }

    // Update is called once per frame
    void Update()
    {
        // If all of our attack frame counters are below 0,
        // then we surely aren't attacking
        if (startupFrameCounter <= 0 && activeFrameCounter <= 0 && recoveryFrameCounter <= 0)
        {
            canAttack = true;
            isAttacking = false;
            attackType = 0;
        }

        if (hp == 0)
        {
            DeathAnimation();
        }
        else
        {
            float distFromPW = gm.FindDistance(this.gameObject, pw350);

            if (distFromPW < 7)
            {
                if (canAttack && distFromPW < 1)
                {
                    StartBiteAttack();
                }
                else if (canAttack && distFromPW < 3)
                {
                    StartPounceAttack();
                }
                else if (!isAttacking)
                {
                    isMoving = true;
                    float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;

                    if (pw350.transform.position.x < transform.position.x)
                    {
                        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, currentY);

                        if (facingRight)
                        {
                            InvertHurtboxes();
                            facingRight = false;
                        }
                    }
                    else if (pw350.transform.position.x > transform.position.x)
                    {
                        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, currentY);

                        if (!facingRight)
                        {
                            InvertHurtboxes();
                            facingRight = true;
                        }
                    }
                    else
                    {
                        isMoving = false;
                        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);
                    }
                }
            }
            else
            {
                isMoving = false;
                float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);
            }
        }

        // Now, we handle our attack if we are indeed attacking
        // Reduce our startup
        startupFrameCounter--;

        // If startup is over, we begin drawing the hitbox
        if (startupFrameCounter == 0)
        {
            hitBox.StopHitBox();

            // Begin drawing the hitbox for the attack
            hitBox.StartHitBox(attackType, facingRight);

            // Set our active frame counter to keep track of
            // how long the hitbox exists for
            activeFrameCounter = (attacks[attackType].activeFrames + 1);

            //This line is used for debugging hitbox positions
            //EditorApplication.isPaused = true;
        }

        // Reduce our active frames
        activeFrameCounter--;

        // If active frames are over, we must stop drawing the hitbox
        if (activeFrameCounter == 0)
        {
            // Stop drawing the hitbox
            hitBox.StopHitBox();

            // Set our recovery frame counter to keep track of
            // our end lag (assuming that we didn't cancel the attack)
            recoveryFrameCounter = (attacks[attackType].recoveryFrames + 1);
        }

        // Reduce our recovery frames
        recoveryFrameCounter--;

        // If we are done with recovery, then we must be sure our
        // RigidBody2D has the correct constraints
        // (They are changed when we perform a special move)
        if (recoveryFrameCounter == 0)
        {
            // Reset the RigidBody2D constraints
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        currentSprite++;

        Sprite s = null;

        // Temporarily disable the Animator for the idle animation
        gameObject.GetComponent<Animator>().enabled = false;

        if (isAttacking)
        {
            switch (attackType)
            {
                case 17:
                    {
                        currentSprite %= biteSprites.Length;
                        s = biteSprites[currentSprite];
                    }
                    break;
                case 18:
                    {
                        currentSprite %= pounceSprites.Length;
                        s = pounceSprites[currentSprite];
                        transform.position = new Vector2(transform.position.x + 0.15f, transform.position.y);
                    }
                    break;
                default:
                    gameObject.GetComponent<Animator>().enabled = true;
                    break;
            }
        }
        else if (isMoving)
        {
            currentSprite %= walkSprites.Length;
            s = walkSprites[currentSprite];

            // We also need to update which direction we are looking
            if (facingRight)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        else
        {
            gameObject.GetComponent<Animator>().enabled = true;
        }

        gameObject.GetComponent<SpriteRenderer>().sprite = s;

        if (attackType == 18)
        {
            EditorApplication.isPaused = true;
        }
    }

    private void DeathAnimation()
    {
        // Disable its idle animation, pushbox, and RigidBody
        gameObject.GetComponent<Animator>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        // Increment our sprite tracker
        currentSprite++;

        // If we are going to go out of bounds in our sprite vector,
        // we are going to correct ourselves
        if (currentSprite >= deathSprites.Length)
        {
            currentSprite = deathSprites.Length - 1;
        }

        // Render the updated sprite through the SpriteRenderer
        gameObject.GetComponent<SpriteRenderer>().sprite = deathSprites[currentSprite];
    }

    private void StartBiteAttack()
    {
        attackType = 17;
        startupFrameCounter = attacks[attackType].startupFrames;

        currentSprite = -1;
        canAttack = false;
        isAttacking = true;
    }

    private void StartPounceAttack()
    {
        attackType = 18;
        startupFrameCounter = attacks[attackType].startupFrames;

        currentSprite = -1;
        canAttack = false;
        isAttacking = true;
    }

    // <summary>
    // Draws the HUND's pushbox in Scene View
    // Very useful for debugging
    // </summary>
    private void OnDrawGizmos()
    {
        // We will simply draw a cube around the Dummy's pushbox
        Gizmos.color = Color.white;

        Vector3 boxPos = GetComponent<BoxCollider2D>().bounds.center;
        Vector3 boxSize = GetComponent<BoxCollider2D>().size;

        Gizmos.DrawWireCube(boxPos, boxSize);
    }
}
