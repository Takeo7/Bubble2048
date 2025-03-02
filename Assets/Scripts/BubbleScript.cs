﻿using System.Collections;
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
    public int bounces;
    bool collidedEnd = false;
    Transform spot;
    [SerializeField]
    GameObject explosionParticles;


    public bool isCeilingConnected;

    public void SetStats(int n,Color c)
    {
        GetComponent<SpriteRenderer>().color = c;
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
                    if (aroundCol[i].GetComponent<BubbleScript>() != this)
                    {
                        return true;
                    }
                    
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
        Debug.Log("Fall");
        transform.SetParent(null);
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
            //spot.GetComponent<SpriteRenderer>().enabled = false;
            spot.transform.GetChild(0).gameObject.SetActive(false);
        }       
        BubbleController.instance.SetPoints(number);
        //Make some particles
        BubbleController.instance.RemoveBubbleFramGlobalList(this);
        ParticleSystem particleExplosionTemp = Instantiate(explosionParticles, transform.position, transform.rotation).GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particleExplosionTemp.main;
        main.startColor = GetComponent<SpriteRenderer>().color;
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
            }
            yield return new WaitForEndOfFrame();
        }
        col.isTrigger = false;
        gameObject.layer = 16;
        transform.SetParent(spot);
        BubbleController.instance.AddBubbleToGlobalList(this);
        ChechForFusion(true);
    }

    public void SetBubbleInNewLine(Transform s)
    {
        spot = s;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        transform.position = spot.position;
        spot.GetComponent<Collider2D>().enabled = false;        
        col.isTrigger = false;
        gameObject.layer = 16;
        transform.SetParent(spot);
        BubbleController.instance.AddBubbleToGlobalList(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor") && transform.parent == null)
        {
            Explode();
        }
        else if (collision.CompareTag("Floor") && transform.parent != null)
        {
            Debug.LogError("GAMEOVER", gameObject);
            BubbleController.instance.GameOver();
        }        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall-Left") || collision.collider.CompareTag("Wall-Left"))
        {
            bounces--;
            if (bounces == 0)
            {
                col.isTrigger = true;
            }
        }
    }
}
