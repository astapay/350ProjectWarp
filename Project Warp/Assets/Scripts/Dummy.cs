using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Dummy : MonoBehaviour
{
    private int hp;

    private int currentSprite;
    [SerializeField] private Sprite[] deathSprites;

    // Start is called before the first frame update
    void Start()
    {
        hp = 1;
        currentSprite = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if(hp == 0)
        {
            deathAnimation();
        }
    }

    private void deathAnimation()
    {
        gameObject.GetComponent<Animator>().enabled = false;
        currentSprite++;

        if(currentSprite >= deathSprites.Length)
        {
            currentSprite = deathSprites.Length - 1;
        }

        gameObject.GetComponent<SpriteRenderer>().sprite = deathSprites[currentSprite];
    }

    public void reduceHP(int damage)
    {
        hp -= damage;

        if(hp < 0)
        {
            hp = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(1,1,1));
    }
}
