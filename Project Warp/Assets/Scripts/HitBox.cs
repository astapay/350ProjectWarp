using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class HitBox : MonoBehaviour
{
    private UnityEngine.Color color;

    // Start is called before the first frame update
    void Start()
    {
        color = UnityEngine.Color.red;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startHitBox(int type)
    {
        float xOffset = 0;
        float yOffset = 0;
        float xSize = 0;
        float ySize = 0;

        switch(type)
        {
            case 1:
                xOffset = 1;
                yOffset = 0;
                xSize = 1;
                ySize = 0.5f;
                break;
            case 2:
                xOffset = 1;
                yOffset = -0.5f;
                xSize = 1;
                ySize = 0.5f;
                break;
            case 3:
                xOffset = 1;
                yOffset = 0.5f;
                xSize = 1;
                ySize = 0.5f;
                break;
            case 4:
                xOffset = 1;
                yOffset = 0;
                xSize = 1;
                ySize = 0.5f;
                break;
        }

        GetComponent<BoxCollider2D>().offset = new Vector2(xOffset, yOffset);
        GetComponent<BoxCollider2D>().size = new Vector2(xSize, ySize);
    }

    public void stopHitBox()
    {
        GetComponent<BoxCollider2D>().offset = Vector2.zero;
        GetComponent<BoxCollider2D>().size = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        Vector2 position = GetComponent<BoxCollider2D>().offset;
        Vector2 boxSize = GetComponent<BoxCollider2D>().size;

        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawCube(position, boxSize);
    }
}
