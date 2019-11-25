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
    int pow;
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

    public bool isCeilingConnected;

    public void SetStats(int n)
    {
        int result = (int)Mathf.Pow(2, n);
        number = result;
        pow = n;
        SetText();
        if (number == 2048)
        {
            cf.SetLayerMask(lm);
            aroundCol = new Collider2D[7];
            Physics2D.OverlapCircle(transform.position, 0.7f, cf, aroundCol);
            int length = aroundCol.Length;
            for (int i = 0; i < length; i++)
            {
                if (aroundCol[i] != null && aroundCol[i].GetComponent<BubbleScript>() == true)
                {
                    aroundCol[i].GetComponent<BubbleScript>().Explode();
                }               
            }
            Explode();
        }
        
    }

    public void ChechForFusion(bool isFirst)
    {
        if (isFirst)
        {
            BubbleController.instance.AddBubbleFusionList(this);
        }       
        cf.SetLayerMask(lm);
        aroundCol = new Collider2D[7];
        Physics2D.OverlapCircle(transform.position, 0.7f, cf, aroundCol);
        int length = aroundCol.Length;
        for (int i = 0; i < length; i++)
        {
            if (aroundCol[i] != null && aroundCol[i].GetComponent<BubbleScript>() == true)
            {
                if (aroundCol[i].GetComponent<BubbleScript>().GetNumber() == number)
                {
                    if (BubbleController.instance.CheckExistBubbleFusionList(aroundCol[i].GetComponent<BubbleScript>()) == false)
                    {
                        BubbleController.instance.AddBubbleFusionList(aroundCol[i].GetComponent<BubbleScript>());
                    }
                }
            }
        }
        BubbleController.instance.UpCounter();    
    }

    public bool CheckAround()
    {
        cf.SetLayerMask(lm);
        aroundCol = new Collider2D[7];
        Physics2D.OverlapCircle(transform.position, 0.7f, cf, aroundCol);
        int length = aroundCol.Length;
        for (int i = 0; i < length; i++)
        {
            if (aroundCol[i] != null && aroundCol[i].GetComponent<BubbleScript>() == true)
            {
                if (aroundCol[i].GetComponent<BubbleScript>().GetNumber() == number)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void CheckAroundCeiling()
    {
        cf.SetLayerMask(lm);
        aroundCol = new Collider2D[7];
        Physics2D.OverlapCircle(transform.position, 0.7f, cf, aroundCol);
        int length = aroundCol.Length;
        for (int i = 0; i < length; i++)
        {
            if (aroundCol[i] != null && aroundCol[i].GetComponent<BubbleScript>() == true)
            {
                if (aroundCol[i].GetComponent<BubbleScript>().isCeilingConnected == false)
                {
                    aroundCol[i].GetComponent<BubbleScript>().isCeilingConnected = true;
                    aroundCol[i].GetComponent<BubbleScript>().CheckAroundCeiling();
                }                
            }
        }
    }

    public void Fall()
    {
        if (spot != null)
        {
            spot.GetComponent<Collider2D>().enabled = true;
            spot.GetComponent<Collider2D>().isTrigger = false;
            spot.GetComponent<SpriteRenderer>().enabled = false;
        }
        col.isTrigger = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;
    }

    public void Fusion()
    {
        Explode();
    }

    public int GetNumber()
    {
        return number;
    }

    public int GetPow()
    {
        return pow;
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
            spot.GetComponent<Collider2D>().isTrigger = false;
            spot.GetComponent<SpriteRenderer>().enabled = false;
        }       
        BubbleController.instance.SetPoints(number);
        //Make some particles
        BubbleController.instance.RemoveBubbleFramGlobalList(this);
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
        col.isTrigger = false;
        gameObject.layer = 16;
        BubbleController.instance.AddBubbleToGlobalList(this);
        ChechForFusion(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
        {
            Explode();
        }
    }
}
