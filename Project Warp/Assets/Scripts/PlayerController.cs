using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public struct Attack
{
    public int startupFrames;
    public int activeFrames;
    public int recoveryFrames;
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    private InputAction move;
    private InputAction jump;
    private InputAction crouch;
    private InputAction normalAtk;
    private InputAction specialAtk;

    [SerializeField] private float groundedSpeed;
    [SerializeField] private float airSpeed;

    private bool isMoving;
    private bool isCrouching;
    private bool isAttacking;
    private bool isGrounded;

    [SerializeField] private HitBox hitBox;
    [SerializeField] private HurtBox hurtBox;
    private Attack[] attacks;
    private int nChain;
    private int attackType;
    private int startupFrameCounter;
    private int activeFrameCounter;
    private int recoveryFrameCounter;

    private int currentSprite;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] jumpSprites;
    [SerializeField] private Sprite[] fallSprites;
    [SerializeField] private Sprite[] crouchSprites;

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

    [SerializeField] private Sprite[] standSSprites;
    [SerializeField] private Sprite[] crouchSSprites;
    [SerializeField] private Sprite[] forwardSSprites;
    [SerializeField] private Sprite[] backSSprites;

    // Start is called before the first frame update
    void Start()
    {
        playerInput.currentActionMap.Enable();

        move = playerInput.currentActionMap.FindAction("MoveLeftRight");
        jump = playerInput.currentActionMap.FindAction("Jump");
        crouch = playerInput.currentActionMap.FindAction("Crouch");
        normalAtk = playerInput.currentActionMap.FindAction("NormalAttack");
        specialAtk = playerInput.currentActionMap.FindAction("SpecialAttack");

        move.started += Handle_MoveStart;
        jump.performed += Handle_Jump;
        crouch.started += Handle_CrouchStart;
        normalAtk.performed += Handle_Normal;
        specialAtk.performed += Handle_Special;

        move.canceled += Handle_MoveEnd;
        crouch.canceled += Handle_CrouchEnd;

        isMoving = false;
        isCrouching = false;
        isAttacking = false;
        isGrounded = true;

        attacks = new Attack[17];
        nChain = 0;
        attackType = 0;
        startupFrameCounter = 0;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        currentSprite = 0;

        setUpAttacks();
    }

    private void setUpAttacks()
    {
        //5N
        attacks[1].startupFrames = 6;
        attacks[1].activeFrames = 3;
        attacks[1].recoveryFrames = 10;

        //5NN
        attacks[2].startupFrames = 7;
        attacks[2].activeFrames = 6;
        attacks[2].recoveryFrames = 19;

        //5NNN
        attacks[3].startupFrames = 12;
        attacks[3].activeFrames = 10;
        attacks[3].recoveryFrames = 21;

        //2N
        attacks[4].startupFrames = 7;
        attacks[4].activeFrames = 3;
        attacks[4].recoveryFrames = 14;

        //2NN
        attacks[5].startupFrames = 11;
        attacks[5].activeFrames = 6;
        attacks[5].recoveryFrames = 18;

        //2NNN
        attacks[6].startupFrames = 13;
        attacks[6].activeFrames = 3;
        attacks[6].recoveryFrames = 21;

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
        attacks[10].startupFrames = 7;
        attacks[10].activeFrames = 4;
        attacks[10].recoveryFrames = 8;

        //j.NN
        attacks[11].startupFrames = 8;
        attacks[11].activeFrames = 2;
        attacks[11].recoveryFrames = 13;

        //j.NNN/j.6N
        attacks[12].startupFrames = 13;
        attacks[12].activeFrames = 4;
        attacks[12].recoveryFrames = 19;

        //5S
        attacks[13].startupFrames = 10;
        attacks[13].activeFrames = 6;
        attacks[13].recoveryFrames = 2;

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

    // Update is called once per frame
    void Update()
    {
        updateAnimation();

        if(startupFrameCounter <= 0 && activeFrameCounter <= 0 && recoveryFrameCounter <= 0)
        {
            isAttacking = false;
            attackType = 0;
            nChain = 0;
        }

        if((isMoving && !isAttacking) || (!isGrounded && isAttacking))
        {
            float moveDir = move.ReadValue<float>();
            float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;

            if(isGrounded)
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveDir * groundedSpeed, currentY);
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveDir * airSpeed, currentY);
            }
        }
        else
        {
            float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);
        }

        startupFrameCounter--;

        if(startupFrameCounter == 0)
        {
            hitBox.startHitBox(attackType);
            activeFrameCounter = attacks[attackType].activeFrames + 1;
            //EditorApplication.isPaused = true;
            print("Starting attack");
        }

        activeFrameCounter--;

        if(activeFrameCounter == 0)
        {
            hitBox.stopHitBox();
            recoveryFrameCounter = attacks[attackType].recoveryFrames + 1;
            print("Done");
        }

        recoveryFrameCounter--;

        if(recoveryFrameCounter == 0)
        {
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            print("Done with Recovery");
        }
    }

    private void updateAnimation()
    {
        currentSprite++;
        Sprite s = null;
        gameObject.GetComponent<Animator>().enabled = false;

        if(isAttacking)
        {
            switch(attackType)
            {
                case 1: //5N
                    currentSprite %= standNSprites.Length;
                    s = standNSprites[currentSprite];
                    break;
                case 2: //5NN
                case 8: //6NN
                    currentSprite %= standNNSprites.Length;
                    s = standNNSprites[currentSprite];
                    break;
                case 3: //5NNN
                    currentSprite %= standNNNSprites.Length;
                    s = standNNNSprites[currentSprite];
                    break;
                case 4: //2N
                    currentSprite %= crouchNSprites.Length;
                    s = crouchNSprites[currentSprite];
                    break;
                case 5: //2NN
                    currentSprite %= crouchNNSprites.Length;
                    s = crouchNNSprites[currentSprite];
                    break;
                case 6: //2NNN
                    currentSprite %= crouchNNNSprites.Length;
                    s = crouchNNNSprites[currentSprite];
                    break;
                case 7: //6N
                    /*currentSprite %= forwardNSprites.Length;
                    s = forwardNSprites[currentSprite];
                    break;*/
                case 9: //6NNN
                    /*currentSprite %= forwardNNNSprites.Length;
                    s = forwardNNNSprites[currentSprite];
                    break;*/
                case 10: //j.N
                    currentSprite %= airNSprites.Length;
                    s = airNSprites[currentSprite];
                    break;
                case 11: //j.NN
                    currentSprite %= airNNSprites.Length;
                    s = airNNSprites[currentSprite];
                    break;
                case 12: //j.NNN/j.6N
                    currentSprite %= airNNNSprites.Length;
                    s = airNNNSprites[currentSprite];
                    break;
                case 13: //5S
                    currentSprite %= standSSprites.Length;
                    s = standSSprites[currentSprite];
                    break;
                case 14: //2S
                    /*currentSprite %= crouchSSprites.Length;
                    s = crouchSSprites[currentSprite];
                    break;*/
                case 15: //6S
                    currentSprite %= forwardSSprites.Length;
                    s = forwardSSprites[currentSprite];
                    break;
                case 16: //4S
                    currentSprite %= backSSprites.Length;
                    s = backSSprites[currentSprite];
                    break;
                default:
                    break;
            }
        }
        else if(!isGrounded)
        {
            float yVelo = gameObject.GetComponent<Rigidbody2D>().velocity.y;

            if(yVelo > 0)
            {
                if(currentSprite >= jumpSprites.Length)
                {
                    currentSprite = jumpSprites.Length - 1;
                }

                s = jumpSprites[currentSprite];
            }
            else
            {
                if (currentSprite >= fallSprites.Length)
                {
                    currentSprite = fallSprites.Length - 1;
                }

                s = fallSprites[currentSprite];
            }
        }
        else if(isCrouching)
        {
            currentSprite %= crouchSprites.Length;
            s = crouchSprites[currentSprite];
        }
        else if(isMoving)
        {
            currentSprite %= walkSprites.Length;
            s = walkSprites[currentSprite];
        }
        else
        {
            gameObject.GetComponent<Animator>().enabled = true;
        }

        gameObject.GetComponent<SpriteRenderer>().sprite = s;
    }

    private void Handle_MoveStart(InputAction.CallbackContext obj)
    {
        if(!isCrouching)
        {
            isMoving = true;
        }
    }

    private void Handle_Jump(InputAction.CallbackContext obj)
    {
        if (isGrounded)
        {
            float currentX = gameObject.GetComponent<Rigidbody2D>().velocity.x;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(currentX, 10);
            isGrounded = false;
        }
    }

    private void Handle_CrouchStart(InputAction.CallbackContext obj)
    {
        isCrouching = true;
    }

    private void Handle_Normal(InputAction.CallbackContext obj)
    {
        if (nChain < 3)
        {
            attackType = 1; //standings

            if (!isGrounded) //aerials
            {
                attackType = 10;
            }
            else if (isCrouching) //lows
            {
                attackType = 4;
            }
            else if (isMoving) //forwards
            {
                attackType = 7;
            }
            

            attackType += nChain;

            startupFrameCounter = attacks[attackType].startupFrames;
            currentSprite = -1;
            isAttacking = true;
            nChain++;
        }
    }

    private void Handle_Special(InputAction.CallbackContext obj)
    {
        attackType = 13;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        if(isCrouching)
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
        }

        startupFrameCounter = attacks[attackType].startupFrames;
        currentSprite = -1;
        isAttacking = true;
    }

    private void Handle_MoveEnd(InputAction.CallbackContext obj)
    {
        isMoving = false;
    }

    private void Handle_CrouchEnd(InputAction.CallbackContext obj)
    {
        isCrouching = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.75f, 1, 1));
    }
}
