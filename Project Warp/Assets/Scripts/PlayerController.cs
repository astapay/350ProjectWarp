/***************************************************************************
// File Name:       PlayerController.cs
// Author:          Andrew Stapay
// Creation Date:   September 9, 2023
//
// Description:     Represents the main character in the game. The player is
                    able to control this character. He can move, attack,
                    jump, etc.
***************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Transactions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : FighterController
{
    // Variables for input from the user
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameManager gm;
    private InputAction move;
    private InputAction jump;
    private InputAction crouch;
    private InputAction normalAtk;
    private InputAction specialAtk;
    private InputAction restart;
    private InputAction quit;

    // Speed variables for movement
    [SerializeField] private float groundedSpeed;
    [SerializeField] private float airSpeed;

    private int gemCount;

    // Bool values that will determine what the
    // character can or can't do
    private bool facingRight;
    private bool canMoveLeft;
    private bool canMoveRight;
    private bool canAttack;
    private bool isMoving;
    private bool isJumping;
    private bool isCrouching;
    private bool isAttacking;
    private bool isGrounded;
    private bool doneWithEntrance;
    private bool exitStarted;

    // Attack-related information
    // Includes out hitbox, hurtbox, and array of Attacks
    // As well as counters to keep track of frames
    [SerializeField] private HitBox hitBox;
    private int nChain;
    private int attackType;
    private int startupFrameCounter;
    private int activeFrameCounter;
    private int recoveryFrameCounter;

    // A counter to keep track of the next sprite to play
    private int currentSprite;

    // Movement/misc. sprites
    [SerializeField] private Sprite[] entranceSprites;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] jumpSprites;
    [SerializeField] private Sprite[] fallSprites;
    [SerializeField] private Sprite[] crouchSprites;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] private Sprite[] exitSprites;

    // Normal attack sprites
    [SerializeField] private Sprite[] standNSprites;
    [SerializeField] private Sprite[] standNNSprites;
    [SerializeField] private Sprite[] standNNNSprites;
    [SerializeField] private Sprite[] crouchNSprites;
    [SerializeField] private Sprite[] crouchNNSprites;
    [SerializeField] private Sprite[] crouchNNNSprites;
    [SerializeField] private Sprite[] forwardNSprites;
    [SerializeField] private Sprite[] airNSprites;
    [SerializeField] private Sprite[] airNNSprites;
    [SerializeField] private Sprite[] airNNNSprites;

    // Special attack sprites
    [SerializeField] private Sprite[] standSSprites;
    [SerializeField] private Sprite[] crouchSSprites;
    [SerializeField] private Sprite[] forwardSSprites;

    // <summary>
    // Start is called before the first frame update
    // Initializes all of our private variables
    // </summary>
    void Start()
    {
        // Enable the action map for the player
        playerInput.currentActionMap.Enable();

        // Assign actions to each of our InputActions
        move = playerInput.currentActionMap.FindAction("MoveLeftRight");
        jump = playerInput.currentActionMap.FindAction("Jump");
        crouch = playerInput.currentActionMap.FindAction("Crouch");
        normalAtk = playerInput.currentActionMap.FindAction("NormalAttack");
        specialAtk = playerInput.currentActionMap.FindAction("SpecialAttack");
        restart = playerInput.currentActionMap.FindAction("Restart");
        quit = playerInput.currentActionMap.FindAction("Quit");

        // Assign handlers for the beginning of our actions
        move.started += Handle_MoveStart;
        jump.started += Handle_JumpStart;
        crouch.started += Handle_CrouchStart;
        normalAtk.performed += Handle_Normal;
        specialAtk.performed += Handle_Special;
        restart.performed += Handle_Restart;
        quit.performed += Handle_Quit;

        // Assign handlers for the ending of our actions
        move.canceled += Handle_MoveEnd;
        crouch.canceled += Handle_CrouchEnd;
        jump.canceled += Handle_JumpEnd;

        // Initialize booleans
        facingRight = true;
        canMoveLeft = true;
        canMoveRight = true;
        canAttack = true;
        isMoving = false;
        isJumping = false;
        isCrouching = false;
        isAttacking = false;
        isGrounded = true;

        //Initialize attack-related data
        nChain = 0;
        attackType = 0;
        startupFrameCounter = 0;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        // Start the sprite counter
        currentSprite = -1;

        // Initialize superclass variables
        hp = 50;
        hitstunFrames = 0;
        saveTransform = transform.position;
        transformSaved = false;
        SetUpAttacks();
    }

    // <summary>
    // Update is called once per frame
    // Used to update information about
    // the character, including
    // playing animations, keeping track of attacking frames, movement, etc
    // <summary>
    void Update()
    {
        if (exitStarted)
        {
            currentSprite++;

            if (currentSprite < exitSprites.Length)
            {
                Sprite s = exitSprites[currentSprite];
                gameObject.GetComponent<SpriteRenderer>().sprite = s;
            }
            else
            {
                gm.FreezeTime(30);
                Application.Quit();
                EditorApplication.Exit(0);
            }
        }
        else if (doneWithEntrance)
        {
            // Check to see if we fall too far off of the map
            // (This is possible at only one point of the level)
            if (transform.position.y < -4)
            {
                transform.position = new Vector3(14, 1, 0);
            }

            if (hitstunFrames <= 0)
            {
                // Movement if statements
                if ((isMoving && !isAttacking) || (!isGrounded && isAttacking))
                {
                    // We can move, generally speaking
                    // We need our direction and current Y velocity
                    float moveDir = move.ReadValue<float>();
                    float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;

                    // Check to see if we can move in the inputted direction
                    if ((canMoveLeft && moveDir < 0) || (canMoveRight && moveDir > 0))
                    {
                        // Check to see if we are in the air or not
                        // This determines the applicable speed
                        if (isGrounded)
                        {
                            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveDir * groundedSpeed, currentY);
                        }
                        else
                        {
                            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveDir * airSpeed, currentY);
                        }
                    }
                }
                else
                {
                    // We are not trying to move
                    // Still, we must fall at a constant rate

                    float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);
                }

                // Now, we handle our attack if we are indeed attacking
                // Reduce our startup
                startupFrameCounter--;

                // If startup is over, we begin drawing the hitbox
                if (startupFrameCounter == 0)
                {
                    hitBox.StopHitBox();

                    if (attackType != 15)
                    {
                        // Begin drawing the hitbox for the attack
                        hitBox.StartHitBox(attackType, facingRight, attacks[attackType].damage,
                            attacks[attackType].hitstun, attacks[attackType].knockback);
                    }

                    // Set our active frame counter to keep track of
                    // how long the hitbox exists for
                    activeFrameCounter = (attacks[attackType].activeFrames + 1);

                    //This line is used for debugging hitbox positions
                    //EditorApplication.isPaused = true;
                }

                // Reduce our active frames
                activeFrameCounter--;

                if (attackType != 0 && activeFrameCounter == attacks[attackType].activeFrames - 1)
                {
                    canAttack = true;
                }

                // If active frames are over, we must stop drawing the hitbox
                if (activeFrameCounter == 0)
                {
                    if (attackType == 13)
                    {
                        UndoAdjustTransform();
                    }

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

                    if (attackType >= 1 && attackType <= 3 || attackType == 7 || attackType == 15)
                    {
                        UndoAdjustTransform();
                    }

                    canAttack = true;
                    isAttacking = false;
                    attackType = 0;
                    nChain = 0;
                }

                // Begin the special effect of an attack, if applicable
                SpecialStart();
            }
            else
            {
                hitstunFrames--;
            }
            
            // Update our current animation, if there is one playing
            UpdateAnimation();
        }
        else
        {
            currentSprite++;

            if (currentSprite < entranceSprites.Length)
            {
                Sprite s = entranceSprites[currentSprite];
                gameObject.GetComponent<SpriteRenderer>().sprite = s;
            }
            else
            {
                doneWithEntrance = true;
                currentSprite = -1;
            }
        }
    }

    // <summary>
    // Updates the currently playing animation, if applicable
    // This method is not fully implemented:
    // 6N, 6NNN, and 2S currently use different animations,
    // but the same frame data
    // </summary>
    private void UpdateAnimation()
    {
        // Increment our sprite counter
        currentSprite++;

        // General variable to store our next sprite to display
        Sprite s = null;

        // Temporarily disable the Animator for the idle animation
        gameObject.GetComponent<Animator>().enabled = false;

        if (hitstunFrames <= 0)
        {
            transform.rotation = Quaternion.identity;

            // First, check to see if we must play an attacking animation
            if (isAttacking)
            {
                // Switch on the global variable attackType
                // to determine which attack to display
                switch (attackType)
                {
                    case 1: //5N
                        {
                            currentSprite %= standNSprites.Length;
                            s = standNSprites[currentSprite];

                            if (facingRight)
                            {
                                AdjustTransform(0.155f, 0);
                            }
                            else
                            {
                                AdjustTransform(-0.155f, 0);
                            }
                        }
                        break;
                    case 2: //5NN
                    case 8: //6NN
                        {
                            currentSprite %= standNNSprites.Length;
                            s = standNNSprites[currentSprite];

                            if (facingRight)
                            {
                                AdjustTransform(0.155f, 0);
                            }
                            else
                            {
                                AdjustTransform(-0.155f, 0);
                            }
                        }
                        break;
                    case 3: //5NNN
                    case 9: //6NNN
                        {
                            currentSprite %= standNNNSprites.Length;
                            s = standNNNSprites[currentSprite];

                            if (facingRight)
                            {
                                AdjustTransform(0.155f, 0);
                            }
                            else
                            {
                                AdjustTransform(-0.155f, 0);
                            }
                        }
                        break;
                    case 4: //2N
                        {
                            currentSprite %= crouchNSprites.Length;
                            s = crouchNSprites[currentSprite];
                        }
                        break;
                    case 5: //2NN
                        {
                            currentSprite %= crouchNNSprites.Length;
                            s = crouchNNSprites[currentSprite];
                        }
                        break;
                    case 6: //2NNN
                        {
                            currentSprite %= crouchNNNSprites.Length;
                            s = crouchNNNSprites[currentSprite];
                        }
                        break;
                    case 7: //6N
                        {
                            currentSprite %= forwardNSprites.Length;
                            s = forwardNSprites[currentSprite];

                            if (facingRight)
                            {
                                AdjustTransform(0.155f, 0);
                            }
                            else
                            {
                                AdjustTransform(-0.155f, 0);
                            }
                        }
                        break;
                    case 10: //j.N
                        {
                            currentSprite %= airNSprites.Length;
                            s = airNSprites[currentSprite];
                        }
                        break;
                    case 11: //j.NN
                        {
                            currentSprite %= airNNSprites.Length;
                            s = airNNSprites[currentSprite];
                        }
                        break;
                    case 12: //j.NNN/j.6N
                        {
                            currentSprite %= airNNNSprites.Length;
                            s = airNNNSprites[currentSprite];
                        }
                        break;
                    case 13: //5S
                        {
                            currentSprite %= standSSprites.Length;
                            s = standSSprites[currentSprite];

                            if (recoveryFrameCounter < 0)
                            {
                                if (facingRight)
                                {
                                    AdjustTransform(0.155f, 0);
                                }
                                else
                                {
                                    AdjustTransform(-0.155f, 0);
                                }
                            }
                        }
                        break;
                    case 14: //2S
                        {
                            currentSprite %= crouchSSprites.Length;
                            s = crouchSSprites[currentSprite];
                        }
                        break;
                    case 15: //6S
                        {
                            currentSprite %= forwardSSprites.Length;
                            s = forwardSSprites[currentSprite];

                            if (facingRight)
                            {
                                AdjustTransform(0.155f, 0);
                            }
                            else
                            {
                                AdjustTransform(-0.155f, 0);
                            }
                        }
                        break;
                    // If we somehow find a different attack,
                    // Simply play the Animator again
                    default:
                        gameObject.GetComponent<Animator>().enabled = true;
                        break;
                }
            }
            // Next, we check for the jumping animation
            else if (!isGrounded)
            {
                // We need the Y velocity to see if we are jumping or falling
                float yVelo = gameObject.GetComponent<Rigidbody2D>().velocity.y;

                // Check to see if we are jumping or falling
                if (yVelo > 0)
                {
                    // We are jumping

                    // So that we don't seem to jump again in midair,
                    // we will continuously play the last animation if we
                    // are still moving up
                    if (currentSprite >= jumpSprites.Length)
                    {
                        currentSprite = jumpSprites.Length - 1;
                    }

                    s = jumpSprites[currentSprite];
                }
                else
                {
                    //We are falling

                    // So that we don't seem to fall off invisible platforms,
                    // we will continuously play the last animation if we
                    // are still moving down
                    if (currentSprite >= fallSprites.Length)
                    {
                        currentSprite = fallSprites.Length - 1;
                    }

                    s = fallSprites[currentSprite];
                }
            }
            // Next, crouching sprites
            else if (isCrouching)
            {
                currentSprite %= crouchSprites.Length;
                s = crouchSprites[currentSprite];
            }
            // Next, walking
            else if (isMoving)
            {
                currentSprite %= walkSprites.Length;
                s = walkSprites[currentSprite];

                // We also need to update which direction we are looking
                if (facingRight)
                {
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;
                }
            }
            // Finally, if the rest fail,
            // we turn on the Animator for the idle animation
            else
            {
                gameObject.GetComponent<Animator>().enabled = true;
            }
        }
        else
        {
            s = deathSprites[7];
            hitstunFrames--;
        }

        // Apply the chosen sprite to the SpriteRenderer
        // (If s is null and the Animator is enabled, nothing happens)
        gameObject.GetComponent<SpriteRenderer>().sprite = s;

        /*if (attackType == 13)
        {
            EditorApplication.isPaused = true;
        }*/
    }

    // <summary>
    // Provides the effect of a special attack, if applicable
    // For now, only standing special is implemented
    // </summary>
    private void SpecialStart()
    {
        // Check to see if we are attacking in the first place
        if (isAttacking)
        {
            // Next, check to see if we are attacking
            // with the standing special and our active frames
            // have started
            if (attackType == 13 && activeFrameCounter > 0)
            {
                float currentX = transform.position.x;
                float currentY = transform.position.y;
                float xOffset = 0;
                float yOffset = 0;

                if (isMoving)
                {
                    float moveDir = move.ReadValue<float>();

                    if (moveDir < 0)
                    {
                        if (isJumping)
                        {
                            xOffset = -Mathf.Sqrt(2) / 2.0f;
                            yOffset = Mathf.Sqrt(2) / 2.0f;
                        }
                        else if (isCrouching)
                        {
                            xOffset = -Mathf.Sqrt(2) / 2.0f;
                            yOffset = -Mathf.Sqrt(2) / 2.0f;
                        }
                        else
                        {
                            xOffset = -1;
                        }
                    }
                    else if (moveDir > 0)
                    {
                        if (isJumping)
                        {
                            xOffset = Mathf.Sqrt(2) / 2.0f;
                            yOffset = Mathf.Sqrt(2) / 2.0f;
                        }
                        else if (isCrouching)
                        {
                            xOffset = Mathf.Sqrt(2) / 2.0f;
                            yOffset = -Mathf.Sqrt(2) / 2.0f;
                        }
                        else
                        {
                            xOffset = 1;
                        }
                    }
                }
                else
                {
                    if (isJumping)
                    {
                        yOffset = 1;
                    }
                    else if (isCrouching)
                    {
                        yOffset = -1;
                    }
                }

                xOffset *= 2;
                yOffset *= 2;

                transform.position = new Vector3(currentX + xOffset, currentY + yOffset, 0);

                currentX = saveTransform.x;
                currentY = saveTransform.y;

                saveTransform = new Vector2(currentX + xOffset, currentY + yOffset);
            }
            else if (attackType == 14)
            {
                if (currentSprite <= 32)
                {
                    hurtBox.TurnOff();
                }
                else
                {
                    hurtBox.TurnOn();
                }
            }
            else if (attackType == 15 && activeFrameCounter > 0)
            {
                if (activeFrameCounter % 4 == 1)
                {
                    float xOffset = transform.position.x + 1;
                    float yOffset = transform.position.y;

                    if (!facingRight)
                    {
                        xOffset = transform.position.x - 1;
                    }

                    Vector2 temp = new Vector2(xOffset, yOffset);

                    GameObject pc = Instantiate(projectilePrefab, temp, Quaternion.identity);

                    pc.GetComponent<ProjectileController>().pwProjectile = true;

                    if (facingRight)
                    {
                        pc.GetComponent<ProjectileController>().movingRight = true;
                    }
                    else
                    {
                        pc.GetComponent<ProjectileController>().movingRight = false;
                    }
                }
            }
        }
    }

    public void IncreaseGems()
    {
        gemCount++;
    }

    // <summary>
    // The handler for beginning movement
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_MoveStart(InputAction.CallbackContext obj)
    {
        if (hitstunFrames <= 0)
        {
            // We want to be able to move
            // as long as we are not crouching
            if (!isGrounded || !isCrouching)
            {
                isMoving = true;
            }

            if (isGrounded && !isAttacking)
            {
                // We need to turn around if we are trying to move in a new direction
                float moveDir = move.ReadValue<float>();

                if (moveDir < 0)
                {
                    if (facingRight)
                    {
                        InvertHurtboxes();
                        facingRight = false;
                    }
                }
                else if (moveDir > 0)
                {
                    if (!facingRight)
                    {
                        InvertHurtboxes();
                        facingRight = true;
                    }
                }
            }
        }
    }

    // <summary>
    // The handler for performing a jump
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_JumpStart(InputAction.CallbackContext obj)
    {
        isJumping = true;

        if (hitstunFrames <= 0)
        {
            // We want to be able to jump
            // as long as we are on the ground
            if (isGrounded)
            {
                // We need to keep our current X velocity and carry it
                // into the jump
                float currentX = gameObject.GetComponent<Rigidbody2D>().velocity.x;

                // Apply our new velocity
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(currentX, 10);

                // Since we jumped, we are no longer on the ground
                isGrounded = false;
            }
        }
    }

    // <summary>
    // The handler for beginning a crouch
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_CrouchStart(InputAction.CallbackContext obj)
    {
        if (hitstunFrames <= 0)
        {
            isCrouching = true;
        }
    }

    // <summary>
    // The handler for beginning a normal attack
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_Normal(InputAction.CallbackContext obj)
    {
        if (hitstunFrames <= 0)
        {
            // nChain keeps track of how many normal attacks
            // we have strung in a row
            // If we have done too many in a row, we can't do it again
            if (nChain < 3)
            {
                if (canAttack)
                {
                    if (attackType >= 1 && attackType <= 3 || attackType == 7 || attackType == 15)
                    {
                        UndoAdjustTransform();
                    }

                    // Now, we need to determine which attack to perform
                    // We will default to our standing normals
                    attackType = 1;

                    // First, we will check for aerials
                    if (!isGrounded)
                    {
                        attackType = 10;
                    }
                    // Then, lows
                    else if (isCrouching)
                    {
                        attackType = 4;
                    }
                    // Finally, forwards
                    else if (isMoving)
                    {
                        attackType = 7;
                    }

                    // We can modify which attack in the string
                    // to do based on nChain
                    attackType += nChain;

                    // We need to begin our startup frames
                    startupFrameCounter = attacks[attackType].startupFrames;
                    activeFrameCounter = 0;
                    recoveryFrameCounter = 0;

                    // Reset our sprite counter
                    currentSprite = -1;

                    // We are obviously now attacking
                    canAttack = false;
                    isAttacking = true;

                    // Increment nChain to keep track of how
                    // many normals we've done
                    nChain++;
                }
            }
        }
    }

    // <summary>
    // The handler for beginning a special attack
    // For now, only standing special is implemented
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_Special(InputAction.CallbackContext obj)
    {
        if (gemCount >= 3)
        {
            exitStarted = true;
            currentSprite = -1;
        }
        else if (hitstunFrames <= 0)
        {
            if (canAttack)
            {
                // Start with standing
                attackType = 13;

                // A special property: the character will levitate during the special
                gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

                // Conditions for other specials
                if(isCrouching)
                {
                    attackType = 14;
                }
                else if(isMoving)
                {
                    attackType = 15;
                }

                // Start the startup frames
                startupFrameCounter = attacks[attackType].startupFrames;
                activeFrameCounter = 0;
                recoveryFrameCounter = 0;

                // Reset the frame counter
                currentSprite = -1;

                // We are now attacking
                canAttack = false;
                isAttacking = true;
            }
        }
    }

    // <summary>
    // The handler for performing a restart
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_Restart(InputAction.CallbackContext obj)
    {
        // We will simply reload the scene
        SceneManager.LoadScene(0);
    }

    // <summary>
    // The handler for performing a quit
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_Quit(InputAction.CallbackContext obj)
    {
        // We will simply quit the application
        Application.Quit();
    }

    // <summary>
    // The handler for ending movement
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_MoveEnd(InputAction.CallbackContext obj)
    {
        // We are now no longer moving
        isMoving = false;
    }

    // <summary>
    // The handler for ending a crouch
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_CrouchEnd(InputAction.CallbackContext obj)
    {
        // We are now no longer crouching
        isCrouching = false;
    }

    private void Handle_JumpEnd(InputAction.CallbackContext obj)
    {
        isJumping = false;
    }

    // <summary>
    // This method is called whenever we start
    // colliding with something
    // We will use this for detecting the ground
    // </summary>
    // <param name="other"> The other collider involved in the collision
    // In this case, another pushbox, beit the ground or an enemy </param>
    public void OnCollisionEnter2D(Collision2D collision)
    {
        // If we begin colliding with the ground,
        // then we are grounded
        if(collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    // <summary>
    // This method is called whenever we stop
    // colliding with something
    // We will use this for detecting the ground
    // </summary>
    // <param name="other"> The other collider involved in the collision
    // In this case, another pushbox, beit the ground or an enemy </param>
    public void OnCollisionExit2D(Collision2D collision)
    {
        // If we are no longer colliding with the ground,
        // then we are not grounded
        // and there are no walls to impede movement
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
            canMoveLeft = true;
            canMoveRight = true;
        }
    }

    // <summary>
    // This method is called for each frame we are colliding with something
    // We will use this as a supplemental method
    // for detecting the ground and walls
    // </summary>
    // <param name="other"> The other collider involved in the collision
    // In this case, another pushbox, beit the ground or an enemy </param>
    public void OnCollisionStay2D(Collision2D collision)
    {
        // We need to be able to detect floors and walls
        // for movement purposes
        // So, we need to count the number of collisions
        // with any such floors and walls
        // These counters will be used to keep track of these collisions
        int ctBelow = 0;
        int ctLeft = 0;
        int ctRight = 0;

        // We need to determine what X and Y values
        // represent below, to the left, and to the right of the character
        // We can do a bit of math with the bounds
        // of the BoxCollider to determine these points
        float belowMe = gameObject.GetComponent<BoxCollider2D>().bounds.center.y - 
            gameObject.GetComponent<BoxCollider2D>().bounds.extents.y;
        float leftOfMe = gameObject.GetComponent<BoxCollider2D>().bounds.center.x - 
            gameObject.GetComponent<BoxCollider2D>().bounds.extents.x;
        float rightOfMe = gameObject.GetComponent<BoxCollider2D>().bounds.center.x + 
            gameObject.GetComponent<BoxCollider2D>().bounds.extents.x;

        foreach (ContactPoint2D pt in collision.contacts)
        {
            // First, we will count the number of collisions below
            if (pt.point.y <= belowMe)
            {
                ctBelow++;
            }

            // Next, to the left
            if (pt.point.x <= leftOfMe)
            {
                ctLeft++;
            }

            // Finally, to the right
            if (pt.point.x >= rightOfMe)
            {
                ctRight++;
            }
        }

        // If there are 2 or more collisions below us,
        // then we are on the ground
        // Otherwise, we are in the air
        // If we are not colliding with the ground,
        // then we can ignore this instance
        if (ctBelow >= 2 || collision.gameObject.tag != "Ground")
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // If there are 2 or more collisions to the left of us,
        // then there is a wall to the left and we cannot move left
        // Otherwise, we can move left
        if (ctLeft >= 2)
        {
            canMoveLeft = false;
        }
        else
        {
            canMoveLeft = true;
        }

        // The same logic for going left
        // applies for going right
        if (ctRight >= 2)
        {
            canMoveRight = false;
        }
        else
        {
            canMoveRight = true;
        }
    }

    // <summary>
    // Draws the player's pushbox in Scene View
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

    // <summary>
    // Called when this object is destroyed
    // Used to remove handlers for our actions
    // </summary>
    private void OnDestroy()
    {
        // Remove all handlers
        move.started -= Handle_MoveStart;
        jump.performed -= Handle_JumpStart;
        crouch.started -= Handle_CrouchStart;
        normalAtk.performed -= Handle_Normal;
        specialAtk.performed -= Handle_Special;
        restart.performed -= Handle_Restart;
        quit.performed -= Handle_Quit;
        move.canceled -= Handle_MoveEnd;
        crouch.canceled -= Handle_CrouchEnd;
        jump.canceled -= Handle_JumpEnd;
    }
}
