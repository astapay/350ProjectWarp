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

    // Other properties
    public bool invulnerableHead;
    public bool invulnerableAll;
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
        attacks = new Attack[19];

        //blank
        attacks[0].startupFrames = 0;
        attacks[0].activeFrames = 0;
        attacks[0].recoveryFrames = 0;
        attacks[0].damage = 0;
        attacks[0].hitstun = 0;
        attacks[0].knockback = Vector2.zero;
        attacks[0].invulnerableHead = false;
        attacks[0].invulnerableAll = false;

        //PW-350 5N
        attacks[1].startupFrames = 6;
        attacks[1].activeFrames = 3;
        attacks[1].recoveryFrames = 10;
        attacks[1].damage = 1;
        attacks[1].hitstun = 5;
        attacks[1].knockback = Vector2.zero;
        attacks[1].invulnerableHead = false;
        attacks[1].invulnerableAll = false;

        //PW-350 5NN
        attacks[2].startupFrames = 8;
        attacks[2].activeFrames = 5;
        attacks[2].recoveryFrames = 19;
        attacks[2].damage = 3;
        attacks[2].hitstun = 8;
        attacks[2].knockback = Vector2.zero;
        attacks[2].invulnerableHead = false;
        attacks[2].invulnerableAll = false;

        //PW-350 5NNN
        attacks[3].startupFrames = 14;
        attacks[3].activeFrames = 8;
        attacks[3].recoveryFrames = 21;
        attacks[3].damage = 7;
        attacks[3].hitstun = 20;
        attacks[3].knockback = new Vector2(6, 0.3f);
        attacks[3].invulnerableHead = false;
        attacks[3].invulnerableAll = false;

        //PW-350 2N
        attacks[4].startupFrames = 8;
        attacks[4].activeFrames = 2;
        attacks[4].recoveryFrames = 14;
        attacks[4].damage = 2;
        attacks[4].hitstun = 6;
        attacks[4].knockback = Vector2.zero;
        attacks[4].invulnerableHead = false;
        attacks[4].invulnerableAll = false;

        //PW-350 2NN
        attacks[5].startupFrames = 12;
        attacks[5].activeFrames = 5;
        attacks[5].recoveryFrames = 18;
        attacks[5].damage = 3;
        attacks[5].hitstun = 9;
        attacks[5].knockback = Vector2.zero;
        attacks[5].invulnerableHead = false;
        attacks[5].invulnerableAll = false;

        //PW-350 2NNN
        attacks[6].startupFrames = 14;
        attacks[6].activeFrames = 3;
        attacks[6].recoveryFrames = 20;
        attacks[6].damage = 7;
        attacks[6].hitstun = 20;
        attacks[6].knockback = new Vector2(0.3f, 6);
        attacks[6].invulnerableHead = true;
        attacks[6].invulnerableAll = false;

        //PW-350 6N
        attacks[7].startupFrames = 20;
        attacks[7].activeFrames = 6;
        attacks[7].recoveryFrames = 4;
        attacks[7].damage = 10;
        attacks[7].hitstun = 25;
        attacks[7].knockback = new Vector2(0.2f, 0);
        attacks[7].invulnerableHead = false;
        attacks[7].invulnerableAll = false;

        //PW-350 6NN
        attacks[8].startupFrames = 7;
        attacks[8].activeFrames = 6;
        attacks[8].recoveryFrames = 19;
        attacks[8].damage = 3;
        attacks[8].hitstun = 8;
        attacks[8].knockback = Vector2.zero;
        attacks[8].invulnerableHead = false;
        attacks[8].invulnerableAll = false;

        //PW-350 6NNN
        attacks[9].startupFrames = 14;
        attacks[9].activeFrames = 8;
        attacks[9].recoveryFrames = 21;
        attacks[9].damage = 7;
        attacks[9].hitstun = 20;
        attacks[9].knockback = new Vector2(6, 0.3f);
        attacks[9].invulnerableHead = false;
        attacks[9].invulnerableAll = false;

        //PW-350 j.N
        attacks[10].startupFrames = 8;
        attacks[10].activeFrames = 3;
        attacks[10].recoveryFrames = 8;
        attacks[10].damage = 1;
        attacks[10].hitstun = 7;
        attacks[10].knockback = new Vector2(0.1f, 0.2f);
        attacks[10].invulnerableHead = false;
        attacks[10].invulnerableAll = false;

        //PW-350 j.NN
        attacks[11].startupFrames = 7;
        attacks[11].activeFrames = 3;
        attacks[11].recoveryFrames = 13;
        attacks[11].damage = 3;
        attacks[11].hitstun = 8;
        attacks[11].knockback = new Vector2(0.1f, 0.2f);
        attacks[11].invulnerableHead = false;
        attacks[11].invulnerableAll = false;

        //PW-350 j.NNN
        attacks[12].startupFrames = 13;
        attacks[12].activeFrames = 4;
        attacks[12].recoveryFrames = 19;
        attacks[12].damage = 6;
        attacks[12].hitstun = 14;
        attacks[12].knockback = new Vector2(0.4f, -6);
        attacks[12].invulnerableHead = false;
        attacks[12].invulnerableAll = false;

        //PW-350 5S
        attacks[13].startupFrames = 11;
        attacks[13].activeFrames = 1;
        attacks[13].recoveryFrames = 6;
        attacks[13].damage = 0;
        attacks[13].hitstun = 0;
        attacks[13].knockback = Vector2.zero;
        attacks[13].invulnerableHead = false;
        attacks[13].invulnerableAll = false;

        //PW-350 2S
        attacks[14].startupFrames = 9;
        attacks[14].activeFrames = 17;
        attacks[14].recoveryFrames = 46;
        attacks[14].damage = 0;
        attacks[14].hitstun = 0;
        attacks[14].knockback = Vector2.zero;
        attacks[14].invulnerableHead = false;
        attacks[14].invulnerableAll = false;

        //PW-350 6S
        attacks[15].startupFrames = 13;
        attacks[15].activeFrames = 14;
        attacks[15].recoveryFrames = 15;
        attacks[15].damage = 0;
        attacks[15].hitstun = 0;
        attacks[15].knockback = Vector2.zero;
        attacks[15].invulnerableHead = false;
        attacks[15].invulnerableAll = false;

        //PW-350 4S
        attacks[16].startupFrames = 4;
        attacks[16].activeFrames = 25;
        attacks[16].recoveryFrames = 15;
        attacks[16].damage = 0;
        attacks[16].hitstun = 0;
        attacks[16].knockback = Vector2.zero;
        attacks[16].invulnerableHead = false;
        attacks[16].invulnerableAll = false;

        //HUND Bite
        attacks[17].startupFrames = 7;
        attacks[17].activeFrames = 3;
        attacks[17].recoveryFrames = 10;
        attacks[17].damage = 1;
        attacks[17].hitstun = 6;
        attacks[17].knockback = Vector2.zero;
        attacks[17].invulnerableHead = false;
        attacks[17].invulnerableAll = false;

        //HUND Pounce
        attacks[18].startupFrames = 7;
        attacks[18].activeFrames = 2;
        attacks[18].recoveryFrames = 1;
        attacks[18].damage = 3;
        attacks[18].hitstun = 20;
        attacks[18].knockback = new Vector2(6, 0.3f);
        attacks[18].invulnerableHead = false;
        attacks[18].invulnerableAll = false;

        //Hazmat Whack

        //Hazmat Grab

        //Shroom Kick

        //Shroom Spore
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
