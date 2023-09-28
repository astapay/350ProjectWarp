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
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// <summary>
// A struct that we will use to store information
// for each attack our character does
// </summary>
public struct Attack
{
    // Frame data
    public int startupFrames;
    public int activeFrames;
    public int recoveryFrames;

    // Properties on hit
    public int damage;
    public int hitstun;

    // Other properties
    public bool overhead;
    public bool low;
    public bool invulnerableHead;
    public bool invulnerableAll;
}

public class PlayerController : MonoBehaviour
{
    // Variables for input from the user
    [SerializeField] private PlayerInput playerInput;
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

    // Bool values that will determine what the
    // character can or can't do
    private bool canMoveLeft;
    private bool canMoveRight;
    private bool isMoving;
    private bool isCrouching;
    private bool isAttacking;
    private bool isGrounded;

    // Attack-related information
    // Includes out hitbox, hurtbox, and array of Attacks
    // As well as counters to keep track of frames
    [SerializeField] private HitBox hitBox;
    [SerializeField] private HurtBox hurtBox;
    private Attack[] attacks;
    private int nChain;
    private int attackType;
    private int startupFrameCounter;
    private int activeFrameCounter;
    private int recoveryFrameCounter;
    private Vector3 saveTransform;

    // A counter to keep track of the next sprite to play
    private int currentSprite;

    // Movement sprites
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] jumpSprites;
    [SerializeField] private Sprite[] fallSprites;
    [SerializeField] private Sprite[] crouchSprites;

    // Normal attack sprites
    [SerializeField] private Sprite[] standNSprites;
    [SerializeField] private Sprite[] standNNSprites;
    [SerializeField] private Sprite[] standNNNSprites;
    [SerializeField] private Sprite[] crouchNSprites;
    [SerializeField] private Sprite[] crouchNNSprites;
    [SerializeField] private Sprite[] crouchNNNSprites;
    [SerializeField] private Sprite[] forwardNSprites;
    [SerializeField] private Sprite[] forwardNNNSprites;
    [SerializeField] private Sprite[] airNSprites;
    [SerializeField] private Sprite[] airNNSprites;
    [SerializeField] private Sprite[] airNNNSprites;

    // Special attack sprites
    [SerializeField] private Sprite[] standSSprites;
    [SerializeField] private Sprite[] crouchSSprites;
    [SerializeField] private Sprite[] forwardSSprites;
    [SerializeField] private Sprite[] backSSprites;

    // <summary>
    // Start is called before the first frame update
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
        jump.performed += Handle_Jump;
        crouch.started += Handle_CrouchStart;
        normalAtk.performed += Handle_Normal;
        specialAtk.performed += Handle_Special;
        restart.performed += Handle_Restart;
        quit.performed += Handle_Quit;

        // Assign handlers for the ending of our actions
        move.canceled += Handle_MoveEnd;
        crouch.canceled += Handle_CrouchEnd;

        // Initialize booleans
        canMoveLeft = true;
        canMoveRight = true;
        isMoving = false;
        isCrouching = false;
        isAttacking = false;
        isGrounded = true;

        //Initialize attack-related data
        attacks = new Attack[17];
        nChain = 0;
        attackType = 0;
        startupFrameCounter = 0;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;
        saveTransform = transform.position;

        // Start the sprite counter
        currentSprite = 0;

        // Fill the Attack array
        SetUpAttacks();
    }

    // <summary>
    // Fills our Attack array with appropriate frame data
    // More aspects of each Attack will be added at a later date
    // </summary>
    private void SetUpAttacks()
    {
        //5N
        attacks[1].startupFrames = 6;
        attacks[1].activeFrames = 3;
        attacks[1].recoveryFrames = 10;
        attacks[1].damage = 1;
        attacks[1].hitstun = 5;
        attacks[1].overhead = false;
        attacks[1].low = false;
        attacks[1].invulnerableHead = false;
        attacks[1].invulnerableAll = false;

        //5NN
        attacks[2].startupFrames = 8;
        attacks[2].activeFrames = 5;
        attacks[2].recoveryFrames = 19;
        attacks[2].damage = 3;
        attacks[2].hitstun = 8;
        attacks[2].overhead = false;
        attacks[2].low = false;
        attacks[2].invulnerableHead = false;
        attacks[2].invulnerableAll = false;

        //5NNN
        attacks[3].startupFrames = 14;
        attacks[3].activeFrames = 8;
        attacks[3].recoveryFrames = 21;
        attacks[3].damage = 7;
        attacks[3].hitstun = 20;
        attacks[3].overhead = false;
        attacks[3].low = false;
        attacks[3].invulnerableHead = false;
        attacks[3].invulnerableAll = false;

        //2N
        attacks[4].startupFrames = 8;
        attacks[4].activeFrames = 2;
        attacks[4].recoveryFrames = 14;
        attacks[4].damage = 2;
        attacks[4].hitstun = 6;
        attacks[4].overhead = false;
        attacks[4].low = true;
        attacks[4].invulnerableHead = false;
        attacks[4].invulnerableAll = false;

        //2NN
        attacks[5].startupFrames = 12;
        attacks[5].activeFrames = 5;
        attacks[5].recoveryFrames = 18;
        attacks[5].damage = 3;
        attacks[5].hitstun = 9;
        attacks[5].overhead = false;
        attacks[5].low = true;
        attacks[5].invulnerableHead = false;
        attacks[5].invulnerableAll = false;

        //2NNN
        attacks[6].startupFrames = 14;
        attacks[6].activeFrames = 3;
        attacks[6].recoveryFrames = 20;
        attacks[6].damage = 7;
        attacks[6].hitstun = 20;
        attacks[6].overhead = false;
        attacks[6].low = false;
        attacks[6].invulnerableHead = true;
        attacks[6].invulnerableAll = false;

        //6N
        attacks[7].startupFrames = 20;
        attacks[7].activeFrames = 6;
        attacks[7].recoveryFrames = 4;

        //6NN
        attacks[8].startupFrames = 7;
        attacks[8].activeFrames = 6;
        attacks[8].recoveryFrames = 19;

        //6NNN
        attacks[9].startupFrames = 24;
        attacks[9].activeFrames = 1;
        attacks[9].recoveryFrames = 26;

        //j.N
        attacks[10].startupFrames = 8;
        attacks[10].activeFrames = 3;
        attacks[10].recoveryFrames = 8;

        //j.NN
        attacks[11].startupFrames = 7;
        attacks[11].activeFrames = 3;
        attacks[11].recoveryFrames = 13;

        //j.NNN
        attacks[12].startupFrames = 13;
        attacks[12].activeFrames = 4;
        attacks[12].recoveryFrames = 19;

        //5S
        attacks[13].startupFrames = 11;
        attacks[13].activeFrames = 1;
        attacks[13].recoveryFrames = 6;

        //2S
        attacks[14].startupFrames = 9;
        attacks[14].activeFrames = 17;
        attacks[14].recoveryFrames = 46;

        //6S
        attacks[15].startupFrames = 13;
        attacks[15].activeFrames = 14;
        attacks[15].recoveryFrames = 15;

        //4S
        attacks[16].startupFrames = 4;
        attacks[16].activeFrames = 25;
        attacks[16].recoveryFrames = 15;
    }

    // <summary>
    // Update is called once per frame
    // <summary>
    void Update()
    {
        // Update our current animation, if there is one playing
        UpdateAnimation();

        // Begin the special effect of an attack, if applicable
        SpecialStart();

        // Check to see if we fall too far off of the map
        // (This is possible at only one point of the level)
        if (transform.position.y < -4)
        {
            transform.position = new Vector3(14, 1, 0);
        }

        // If all of our attack frame counters are below 0,
        // then we surely aren't attacking
        if (startupFrameCounter <= 0 && activeFrameCounter <= 0 && recoveryFrameCounter <= 0)
        {
            isAttacking = false;
            attackType = 0;
            nChain = 0;
        }

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

            // Begin drawing the hitbox for the attack
            hitBox.StartHitBox(attackType);

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
            transform.position = saveTransform;
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
        saveTransform = transform.position;

        // Increment our sprite counter
        currentSprite++;

        // General variable to store our next sprite to display
        Sprite s = null;

        // Temporarily disable the Animator for the idle animation
        gameObject.GetComponent<Animator>().enabled = false;

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
                        transform.position = new Vector3(saveTransform.x + 0.155f, saveTransform.y, saveTransform.z);
                    }
                    break;
                case 2: //5NN
                case 8: //6NN
                    {
                        currentSprite %= standNNSprites.Length;
                        s = standNNSprites[currentSprite];
                        transform.position = new Vector3(saveTransform.x + 0.155f, saveTransform.y, saveTransform.z);
                    }
                    break;
                case 3: //5NNN
                    {
                        currentSprite %= standNNNSprites.Length;
                        s = standNNNSprites[currentSprite];
                        transform.position = new Vector3(saveTransform.x + 0.155f, saveTransform.y, saveTransform.z);
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
                case 7: //6N (not implemented)
                    /*currentSprite %= forwardNSprites.Length;
                    s = forwardNSprites[currentSprite];
                    break;*/
                case 9: //6NNN (not implemented)
                    /*currentSprite %= forwardNNNSprites.Length;
                    s = forwardNNNSprites[currentSprite];
                    break;*/
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
                    }
                    break;
                case 14: //2S (not implemented)
                    /*currentSprite %= crouchSSprites.Length;
                    s = crouchSSprites[currentSprite];
                    break;*/
                case 15: //6S
                    {
                        currentSprite %= forwardSSprites.Length;
                        s = forwardSSprites[currentSprite];
                    }
                    break;
                case 16: //4S
                    {
                        currentSprite %= backSSprites.Length;
                        s = backSSprites[currentSprite];
                    }
                    break;
                // If we somehow find a different attack,
                // Simply play the null sprite
                default:
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
                if(currentSprite >= jumpSprites.Length)
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
        }
        // Finally, if the rest fail,
        // we turn on the Animator for the idle animation
        else
        {
            gameObject.GetComponent<Animator>().enabled = true;
        }

        // Apply the chosen sprite to the SpriteRenderer
        // (If s is null and the Animator is enabled, nothing happens)
        gameObject.GetComponent<SpriteRenderer>().sprite = s;
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
            if (attackType >= 13 && activeFrameCounter > 0)
            {
                // This is the teleporting special, so we will teleport forward
                // More directions will be implemented at a later date
                float currentX = transform.position.x;
                transform.position = new Vector3(currentX + 5, transform.position.y, 0);
            }
        }
    }

    // <summary>
    // The handler for beginning movement
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_MoveStart(InputAction.CallbackContext obj)
    {
        // We want to be able to move
        // as long as we are not crouching
        if(!isCrouching)
        {
            isMoving = true;
        }
    }

    // <summary>
    // The handler for performing a jump
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_Jump(InputAction.CallbackContext obj)
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

    // <summary>
    // The handler for beginning a crouch
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_CrouchStart(InputAction.CallbackContext obj)
    {
        // We want to be able to crouch
        // as long as we are on the ground
        if (isGrounded)
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
        // nChain keeps track of how many normal attacks
        // we have strung in a row
        // If we have done too many in a row, we can't do it again
        if (nChain < 3)
        {
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

            // Reset our sprite counter
            currentSprite = -1;

            // We are obviously now attacking
            isAttacking = true;

            // Increment nChain to keep track of how
            // many normals we've done
            nChain++;
        }
    }

    // <summary>
    // The handler for beginning a special attack
    // For now, only standing special is implemented
    // </summary>
    // <param name="obj"> information as to what triggered this action </param>
    private void Handle_Special(InputAction.CallbackContext obj)
    {
        // Start with standing
        attackType = 13;

        // A special property: the character will levitate during the special
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        // Unimplemented conditions for other specials
        /*if(isCrouching)
        {
            attackType = 14;
        }
        else if(isMoving)
        {
            float moveDir = move.ReadValue<float>();

            if(moveDir > 0)
            {
                attackType = 15;
            }
            else
            {
                attackType = 16;
            }
        }*/

        // Start the startup frames
        startupFrameCounter = attacks[attackType].startupFrames;

        // Reset the frame counter
        currentSprite = -1;

        // We are now attacking
        isAttacking = true;
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

        // First, we will count the number of collisions below
        foreach (ContactPoint2D pt in collision.contacts)
        {
            if (pt.point.y <= belowMe)
            {
                ctBelow++;
            }
        }

        // Next, to the left
        foreach (ContactPoint2D pt in collision.contacts)
        {
            if (pt.point.x <= leftOfMe)
            {
                ctLeft++;
            }
        }

        // Finally, to the right
        foreach (ContactPoint2D pt in collision.contacts)
        {
            if (pt.point.x >= rightOfMe)
            {
                ctRight++;
            }
        }

        // If there are 2 or more collisions below us,
        // then we are on the ground
        // Otherwise, we are in the air
        if (ctBelow >= 2)
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
}
