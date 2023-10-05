using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "PW-350")
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();

            pc.IncreaseGems();

            Destroy(gameObject);
        }
    }
}
