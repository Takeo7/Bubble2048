using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class BubbleScript : MonoBehaviour
{
    [Header("Stats")]
    [Space]
    [SerializeField]
    int number;
    [Space]
    [Header("Components")]
    [SerializeField]
    TextMeshProUGUI numberText;
    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    Collider2D col;
    [SerializeField]
    LayerMask lm;
    ContactFilter2D cf;
    Collider2D[] aroundCol = new Collider2D[6];
    bool isShooted = true;
    bool collidedEnd = false;
    Transform spot;

    public void SetStats(int n)
    {
        number = n;
        SetText();
        if (number == 2048)
        {
            cf.SetLayerMask(lm);
            Physics2D.OverlapCircle(transform.position, 0.7f, cf, aroundCol);
            int length = aroundCol.Length;
            for (int i = 0; i < length; i++)
            {
                if (aroundCol[i] != null)
                {
                    aroundCol[i].GetComponent<BubbleScript>().Explode();
                }               
            }
            Explode();
        }
        
    }

    public int GetNumber()
    {
        return number;
    }

    public void SetText()
    {
        numberText.text = number.ToString();
    }

    public void SetIsShooted(bool s)
    {
        isShooted = s;
        if (isShooted)
        {
            StartCoroutine("CheckSpotDistance");
        }
    }

    public void SetSpot(Transform spotRaycasted)
    {
        spot = spotRaycasted;
    }

    public void Explode()
    {
        if (spot != null)
        {
            spot.GetComponent<Collider2D>().enabled = true;
        }
        
        BubbleController.instance.SetPoints(number);
        //Make some particles
        Destroy(gameObject);
    }


    IEnumerator CheckSpotDistance()
    {
        while (collidedEnd == false)
        {
            float distance = (spot.position - transform.position).magnitude;
            if (distance < 1f)
            {
                collidedEnd = true;
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                transform.position = spot.position;
                spot.GetComponent<Collider2D>().enabled = false;
                BubbleController.instance.SetCanShoot(true);
                
            }
            yield return new WaitForEndOfFrame();
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Bubble"))
    //    {
    //        BubbleController.instance.SetCanShoot(true);
    //        if (collision.collider.GetComponent<BubbleScript>().GetNumber() == number && isShooted)
    //        {
    //            collision.collider.GetComponent<BubbleScript>().SetStats(number * 2);
    //            BubbleController.instance.SetPoints(number);
    //            //Make some particles
    //            Destroy(gameObject);
    //        }
    //        else if(isShooted)
    //        {
    //            rb.velocity = Vector3.zero;
    //            rb.isKinematic = true;                
    //            collidedEnd = true;
    //            isShooted = false;
    //        }

    //    }else if (collision.collider.CompareTag("Floor"))
    //    {
    //        BubbleController.instance.SetPoints(number);
    //        //Make some particles
    //        spot.GetComponent<Collider2D>().enabled = true;
    //        Destroy(gameObject);
    //    }
    //    else if (collision.collider.CompareTag("Ceiling"))
    //    {
    //        BubbleController.instance.SetCanShoot(true);
    //        rb.velocity = Vector3.zero;
    //        rb.isKinematic = true;
    //        collidedEnd = true;
    //        isShooted = false;
    //    }
    //}
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    spot = collision.transform;
    //    if (collidedEnd)
    //    {
    //        transform.position = spot.position;
    //        spot.GetComponent<Collider2D>().enabled = false;
    //        collidedEnd = false;
    //    }
    //}


}
