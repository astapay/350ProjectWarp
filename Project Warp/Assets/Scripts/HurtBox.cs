using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f);

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(1,1,1));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        getHit(1);
    }

    public void getHit(int damage)
    {
        Dummy parent = GetComponentInParent<Dummy>();
        parent.reduceHP(damage);
    }
}
