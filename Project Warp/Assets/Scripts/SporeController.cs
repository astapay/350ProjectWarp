using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SporeController : FighterController
{
    [SerializeField] private Sprite[] sprites;
    private int currentSprite;

    // Start is called before the first frame update
    void Start()
    {
        currentSprite = -1;

        HitBox hitBox = GetComponentInChildren<HitBox>();
        hitBox.StartHitBox(21, true, 0, 40, Vector2.zero);
    }

    // Update is called once per frame
    void Update()
    {
        currentSprite++;

        if (currentSprite < sprites.Length)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[currentSprite];
        }
        else
        {
            HitBox hitBox = GetComponentInChildren<HitBox>();
            hitBox.StopHitBox();

            Destroy(gameObject);
        }

        EditorApplication.isPaused = true;
    }
}
