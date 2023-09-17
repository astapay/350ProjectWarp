using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    [SerializeField] private GameObject playerChar;
    [SerializeField] private Dummy dummy;
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
    private int attackType;
    private int startupFrameCounter;
    private int activeFrameCounter;
    private int recoveryFrameCounter;

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

        attacks = new Attack[5];
        attackType = 0;
        startupFrameCounter = 0;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        setUpAttacks();
    }

    private void setUpAttacks()
    {
        attacks[1].startupFrames = 6;
        attacks[1].activeFrames = 3;
        attacks[1].recoveryFrames = 13;

        attacks[2].startupFrames = 7;
        attacks[2].activeFrames = 3;
        attacks[2].recoveryFrames = 14;

        attacks[3].startupFrames = 20;
        attacks[3].activeFrames = 6;
        attacks[3].recoveryFrames = 4;

        attacks[4].startupFrames = 6;
        attacks[4].activeFrames = 3;
        attacks[4].recoveryFrames = 4;
    }

    // Update is called once per frame
    void Update()
    {
        if(startupFrameCounter <= 0)
        {
            isAttacking = false;
        }

        if(isMoving)
        {
            float moveDir = move.ReadValue<float>();
            float currentY = playerChar.GetComponent<Rigidbody2D>().velocity.y;

            if(isGrounded)
            {
                playerChar.GetComponent<Rigidbody2D>().velocity = new Vector2(moveDir * groundedSpeed, currentY);
            }
            else
            {
                playerChar.GetComponent<Rigidbody2D>().velocity = new Vector2(moveDir * airSpeed, currentY);
            }
        }
        else
        {
            float currentY = playerChar.GetComponent<Rigidbody2D>().velocity.y;
            playerChar.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);
        }

        startupFrameCounter--;

        if(startupFrameCounter == 0)
        {
            hitBox.startHitBox(attackType);
            activeFrameCounter = attacks[attackType].activeFrames + 1;
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
            print("Done with Recovery");
        }
    }

    private void Handle_MoveStart(InputAction.CallbackContext obj)
    {
        isMoving = true;
    }

    private void Handle_Jump(InputAction.CallbackContext obj)
    {
        if (isGrounded)
        {
            float currentX = playerChar.GetComponent<Rigidbody2D>().velocity.x;
            playerChar.GetComponent<Rigidbody2D>().velocity = new Vector2(currentX, 10);
            isGrounded = false;
        }
    }

    private void Handle_CrouchStart(InputAction.CallbackContext obj)
    {
        isCrouching = true;
    }

    private void Handle_Normal(InputAction.CallbackContext obj)
    {
        attackType = 1; //standings

        if(isCrouching) //lows
        {
            attackType = 2;
        }
        else if(isMoving) //forwards
        {
            attackType = 3;
        }
        else if(!isGrounded) //aerials
        {
            attackType = 4;
        }

        startupFrameCounter = attacks[attackType].startupFrames;
    }

    private void Handle_Special(InputAction.CallbackContext obj)
    {

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
        if (collision.otherCollider.tag == "Ground")
        {
            isGrounded = true;
        }
    }
}
