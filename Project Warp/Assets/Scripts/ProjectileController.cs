using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [SerializeField] private HitBox hitBox;
    [SerializeField] private int speed;
    public bool movingRight;

    // Start is called before the first frame update
    void Start()
    {
        hitBox.StartHitBox(15, movingRight, 3, 7, new Vector2(0.3f, 0.3f));
    }

    // Update is called once per frame
    void Update()
    {
        if (movingRight)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject.name != collision.gameObject.name)
        {
            Destroy(gameObject);
        }
    }
}
