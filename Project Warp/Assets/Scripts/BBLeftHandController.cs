using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BBLeftHandController : FighterController
{
    [SerializeField] private int speed;
    [SerializeField] private GameManager gm;
    [SerializeField] private GameObject projectilePrefab;
    private GameObject pw350;

    private bool facingRight;
    private bool canAttack;
    private bool isAttacking;

    private int currentSprite;
    [SerializeField] private Sprite[] fireSprites;

    private int attackType;
    private int startupFrameCounter;
    private int activeFrameCounter;
    private int recoveryFrameCounter;

    private int waitFrames;
    private int actionFrames;

    // Start is called before the first frame update
    void Start()
    {
        currentSprite = -1;
        pw350 = gm.GetPW();

        facingRight = true;
        canAttack = true;
        isAttacking = false;

        startupFrameCounter = 0;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        waitFrames = Random.Range(1, 90);
        actionFrames = Random.Range(1, 15);

        hp = 20;
        hitstunFrames = 0;
        saveTransform = transform.position;
        transformSaved = false;
        SetUpAttacks();
    }

    // Update is called once per frame
    void Update()
    {
        hitstunFrames = GetComponentInParent<BBHandsController>().GetHitstunFrames();

        if (actionFrames <= 0)
        {
            waitFrames = Random.Range(1, 90);
            actionFrames = Random.Range(1, 15);
        }

        // If all of our attack frame counters are below 0,
        // then we surely aren't attacking
        if (startupFrameCounter <= 0 && activeFrameCounter <= 0 && recoveryFrameCounter <= 0)
        {
            canAttack = true;
            isAttacking = false;
            attackType = 0;
        }

        if (waitFrames <= 0 && hitstunFrames <= 0)
        {
            float distFromPW = gm.FindDistance(this.gameObject, pw350);

            if (distFromPW < 7)
            {
                if (canAttack && Mathf.Abs(transform.position.y - pw350.transform.position.y) < 0.5f)
                {
                    StartFireAttack();
                }
                else if (!isAttacking)
                {
                    float xChange = pw350.transform.position.x - transform.position.x;
                    float yChange = pw350.transform.position.y - transform.position.y;

                    float mag = Mathf.Sqrt(xChange * xChange + yChange * yChange);

                    Vector2 temp1 = new Vector2 (xChange / mag, yChange / mag);
                    Vector2 temp2 = new Vector2 (speed * temp1.x, speed * temp1.y);

                    GetComponent<Rigidbody2D>().velocity = temp2;

                    if (temp2.x < 0)
                    {
                        facingRight = false;
                    }
                    else if (temp2.x > 0)
                    {
                        facingRight = true;
                    }
                }
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }

            actionFrames--;
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            hitstunFrames--;
        }

        waitFrames--;

        // Now, we handle our attack if we are indeed attacking
        // Reduce our startup
        startupFrameCounter--;

        // If startup is over, we begin drawing the hitbox
        if (startupFrameCounter == 0)
        {
            float xOffset = transform.position.x + 1;
            float yOffset = transform.position.y;

            if (!facingRight)
            {
                xOffset = transform.position.x - 1;
            }

            Vector2 temp = new Vector2(xOffset, yOffset);

            GameObject pc = Instantiate(projectilePrefab, temp, Quaternion.identity);

            pc.GetComponent<ProjectileController>().pwProjectile = false;

            if (facingRight)
            {
                pc.GetComponent<ProjectileController>().movingRight = true;
            }
            else
            {
                pc.GetComponent<ProjectileController>().movingRight = false;
            }

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
            // Set our recovery frame counter to keep track of
            // our end lag (assuming that we didn't cancel the attack)
            recoveryFrameCounter = (attacks[attackType].recoveryFrames + 1);
        }

        // Reduce our recovery frames
        recoveryFrameCounter--;

        if (recoveryFrameCounter == 0)
        {

        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        currentSprite++;

        Sprite s = null;

        // Temporarily disable the Animator for the idle animation
        gameObject.GetComponent<Animator>().enabled = false;

        if (hitstunFrames <= 0)
        {
            if (isAttacking)
            {
                switch (attackType)
                {
                    case 23:
                        {
                            currentSprite %= fireSprites.Length;
                            s = fireSprites[currentSprite];
                        }
                        break;
                    default:
                        gameObject.GetComponent<Animator>().enabled = true;
                        break;
                }
            }
            else
            {
                // We also need to update which direction we are looking
                if (facingRight)
                {
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;
                }

                gameObject.GetComponent<Animator>().enabled = true;
            }
        }

        gameObject.GetComponent<SpriteRenderer>().sprite = s;

        if (attackType == 23)
        {
            EditorApplication.isPaused = true;
        }
    }

    private void StartFireAttack()
    {
        attackType = 23;
        startupFrameCounter = attacks[attackType].startupFrames;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        currentSprite = -1;
        canAttack = false;
        isAttacking = true;
    }
}
