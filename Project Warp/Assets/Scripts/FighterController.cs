using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Vector2 knockback;
}

public class FighterController : MonoBehaviour
{
    protected int hp;
    protected int hitstunFrames;
    protected Attack[] attacks;

    [SerializeField] protected HurtBox hurtBox;

    protected Vector2 saveTransform;
    protected bool transformSaved;

    // Start is called before the first frame update
    void Start()
    {
        saveTransform = transform.position;
        transformSaved = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // <summary>
    // Fills our Attack array with appropriate frame data
    // More aspects of each Attack will be added at a later date
    // </summary>
    protected void SetUpAttacks()
    {
        attacks = new Attack[25];

        //blank
        attacks[0].startupFrames = 0;
        attacks[0].activeFrames = 0;
        attacks[0].recoveryFrames = 0;
        attacks[0].damage = 0;
        attacks[0].hitstun = 0;
        attacks[0].knockback = Vector2.zero;

        //PW-350 5N
        attacks[1].startupFrames = 6;
        attacks[1].activeFrames = 3;
        attacks[1].recoveryFrames = 10;
        attacks[1].damage = 1;
        attacks[1].hitstun = 5;
        attacks[1].knockback = Vector2.zero;

        //PW-350 5NN
        attacks[2].startupFrames = 8;
        attacks[2].activeFrames = 5;
        attacks[2].recoveryFrames = 19;
        attacks[2].damage = 3;
        attacks[2].hitstun = 8;
        attacks[2].knockback = Vector2.zero;

        //PW-350 5NNN
        attacks[3].startupFrames = 14;
        attacks[3].activeFrames = 8;
        attacks[3].recoveryFrames = 21;
        attacks[3].damage = 7;
        attacks[3].hitstun = 20;
        attacks[3].knockback = new Vector2(6, 0.3f);

        //PW-350 2N
        attacks[4].startupFrames = 8;
        attacks[4].activeFrames = 2;
        attacks[4].recoveryFrames = 14;
        attacks[4].damage = 2;
        attacks[4].hitstun = 6;
        attacks[4].knockback = Vector2.zero;

        //PW-350 2NN
        attacks[5].startupFrames = 12;
        attacks[5].activeFrames = 5;
        attacks[5].recoveryFrames = 18;
        attacks[5].damage = 3;
        attacks[5].hitstun = 9;
        attacks[5].knockback = Vector2.zero;

        //PW-350 2NNN
        attacks[6].startupFrames = 14;
        attacks[6].activeFrames = 3;
        attacks[6].recoveryFrames = 20;
        attacks[6].damage = 7;
        attacks[6].hitstun = 20;
        attacks[6].knockback = new Vector2(0.3f, 6);

        //PW-350 6N
        attacks[7].startupFrames = 15;
        attacks[7].activeFrames = 6;
        attacks[7].recoveryFrames = 6;
        attacks[7].damage = 10;
        attacks[7].hitstun = 25;
        attacks[7].knockback = new Vector2(0.2f, 0);

        //PW-350 6NN
        attacks[8].startupFrames = 7;
        attacks[8].activeFrames = 6;
        attacks[8].recoveryFrames = 19;
        attacks[8].damage = 3;
        attacks[8].hitstun = 8;
        attacks[8].knockback = Vector2.zero;

        //PW-350 6NNN
        attacks[9].startupFrames = 14;
        attacks[9].activeFrames = 8;
        attacks[9].recoveryFrames = 21;
        attacks[9].damage = 7;
        attacks[9].hitstun = 20;
        attacks[9].knockback = new Vector2(6, 0.3f);

        //PW-350 j.N
        attacks[10].startupFrames = 8;
        attacks[10].activeFrames = 3;
        attacks[10].recoveryFrames = 8;
        attacks[10].damage = 1;
        attacks[10].hitstun = 7;
        attacks[10].knockback = new Vector2(0.1f, 0.2f);

        //PW-350 j.NN
        attacks[11].startupFrames = 7;
        attacks[11].activeFrames = 3;
        attacks[11].recoveryFrames = 13;
        attacks[11].damage = 3;
        attacks[11].hitstun = 8;
        attacks[11].knockback = new Vector2(0.1f, 0.2f);

        //PW-350 j.NNN
        attacks[12].startupFrames = 13;
        attacks[12].activeFrames = 4;
        attacks[12].recoveryFrames = 19;
        attacks[12].damage = 6;
        attacks[12].hitstun = 14;
        attacks[12].knockback = new Vector2(0.4f, -6);

        //PW-350 5S
        attacks[13].startupFrames = 11;
        attacks[13].activeFrames = 1;
        attacks[13].recoveryFrames = 6;
        attacks[13].damage = 0;
        attacks[13].hitstun = 0;
        attacks[13].knockback = Vector2.zero;

        //PW-350 2S
        attacks[14].startupFrames = 10;
        attacks[14].activeFrames = 18;
        attacks[14].recoveryFrames = 25;
        attacks[14].damage = 0;
        attacks[14].hitstun = 0;
        attacks[14].knockback = Vector2.zero;

        //PW-350 6S
        attacks[15].startupFrames = 13;
        attacks[15].activeFrames = 14;
        attacks[15].recoveryFrames = 15;
        attacks[15].damage = 3;
        attacks[15].hitstun = 7;
        attacks[15].knockback = new Vector2(0.3f, 0.3f);

        //HUND Bite
        attacks[16].startupFrames = 7;
        attacks[16].activeFrames = 3;
        attacks[16].recoveryFrames = 10;
        attacks[16].damage = 1;
        attacks[16].hitstun = 6;
        attacks[16].knockback = Vector2.zero;

        //HUND Pounce
        attacks[17].startupFrames = 7;
        attacks[17].activeFrames = 2;
        attacks[17].recoveryFrames = 1;
        attacks[17].damage = 3;
        attacks[17].hitstun = 20;
        attacks[17].knockback = new Vector2(6, 0.3f);

        //Hazmat Whack
        attacks[18].startupFrames = 0;
        attacks[18].activeFrames = 0;
        attacks[18].recoveryFrames = 0;
        attacks[18].damage = 0;
        attacks[18].hitstun = 0;
        attacks[18].knockback = Vector2.zero;

        //Hazmat Punch
        attacks[19].startupFrames = 0;
        attacks[19].activeFrames = 0;
        attacks[19].recoveryFrames = 0;
        attacks[19].damage = 0;
        attacks[19].hitstun = 0;
        attacks[19].knockback = Vector2.zero;

        //Shroom Kick
        attacks[20].startupFrames = 0;
        attacks[20].activeFrames = 0;
        attacks[20].recoveryFrames = 0;
        attacks[20].damage = 0;
        attacks[20].hitstun = 0;
        attacks[20].knockback = Vector2.zero;

        //Shroom Spore
        attacks[21].startupFrames = 0;
        attacks[21].activeFrames = 0;
        attacks[21].recoveryFrames = 0;
        attacks[21].damage = 0;
        attacks[21].hitstun = 0;
        attacks[21].knockback = Vector2.zero;

        //BB-Hands Kick
        attacks[22].startupFrames = 0;
        attacks[22].activeFrames = 0;
        attacks[22].recoveryFrames = 0;
        attacks[22].damage = 0;
        attacks[22].hitstun = 0;
        attacks[22].knockback = Vector2.zero;

        //BB-Hands Fire
        attacks[23].startupFrames = 0;
        attacks[23].activeFrames = 0;
        attacks[23].recoveryFrames = 0;
        attacks[23].damage = 0;
        attacks[23].hitstun = 0;
        attacks[23].knockback = Vector2.zero;

        //BB-Hands Slam
        attacks[24].startupFrames = 0;
        attacks[24].activeFrames = 0;
        attacks[24].recoveryFrames = 0;
        attacks[24].damage = 0;
        attacks[24].hitstun = 0;
        attacks[24].knockback = Vector2.zero;
    }

    // <summary>
    // Reduces our HP
    // Used by HurtBox when taking damage
    // </summary>
    // <param name="damage"> The amount of damage we take from the attack
    public void ReduceHP(int damage)
    {
        // Reduce our HP
        hp -= damage;

        // If we have overkill damage, set our HP to 0
        if (hp < 0)
        {
            hp = 0;
        }
    }

    public void ApplyHitstun(int hitstun)
    {
        hitstunFrames = hitstun;
    }

    public void ApplyKnockback(Vector2 knockback)
    {
        GetComponent<Rigidbody2D>().AddForce(knockback);
    }

    public void InvertHurtboxes()
    {
        BoxCollider2D[] boxColliders = GetComponentsInChildren<BoxCollider2D>();

        for(int i = 0;i < boxColliders.Length;i++)
        {
            float xOffset = boxColliders[i].offset.x * -1;
            float yOffset = boxColliders[i].offset.y;

            boxColliders[i].offset = new Vector2(xOffset, yOffset);
        }
    }

    public void AdjustTransform(float xChange, float yChange)
    {
        if (!transformSaved)
        {
            saveTransform = transform.position;
            transformSaved = true;

            float currentX = transform.position.x;
            float currentY = transform.position.y;

            transform.position = new Vector2(currentX + xChange, currentY + yChange);

            BoxCollider2D[] boxColliders = GetComponentsInChildren<BoxCollider2D>();

            for (int i = 0; i < boxColliders.Length; i++)
            {
                float xOffset = boxColliders[i].offset.x - xChange;
                float yOffset = boxColliders[i].offset.y - yChange;

                boxColliders[i].offset = new Vector2(xOffset, yOffset);
            }

            Camera c = Camera.main;

            c.transform.position = new Vector3(c.transform.position.x + xChange, 
                c.transform.position.y + yChange, -10);
        }
    }

    public void UndoAdjustTransform()
    {
        float xChange = saveTransform.x - transform.position.x;
        float yChange = saveTransform.y - transform.position.y;

        transformSaved = false;
        AdjustTransform(xChange, yChange);
        saveTransform = transform.position;
        transformSaved = false;
    }
}
