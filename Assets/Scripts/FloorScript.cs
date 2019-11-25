using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Spot"))
        {
            if (collision.transform.childCount == 1)
            {
                Destroy(collision.transform.parent);
            }
        }
    }
}
