/*************************************************************************
// File Name:       GameManager.cs
// Author:          Andrew Stapay
// Creation Date:   September 29, 2023
//
// Description:     A Spore attack released by a Shroom enemy
*************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SporeController : FighterController
{
    // Sprites
    [SerializeField] private Sprite[] sprites;
    private int currentSprite;

    // <summary>
    // Start is called before the first frame update
    // Used to initialize variables and start its hitbox
    // </summary>
    void Start()
    {
        // Set currentSprite
        currentSprite = -1;

        // Start hitbox
        HitBox hitBox = GetComponentInChildren<HitBox>();
        hitBox.StartHitBox(21, true, 0, 40, Vector2.zero);
    }

    // <summary>
    // Update is called once per frame
    // Used to keep track of the life of the spores
    // </summary>
    void Update()
    {
        currentSprite++;

        // Continue sprites during life
        if (currentSprite < sprites.Length)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[currentSprite];
        }
        // When we are done, stop the hitbox and destroy
        else
        {
            HitBox hitBox = GetComponentInChildren<HitBox>();
            hitBox.StopHitBox();

            Destroy(gameObject);
        }
    }
}
