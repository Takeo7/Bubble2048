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


    Collider2D[] aroundCol = new Collider2D[1];
    bool isShooted = true;


    public void SetStats(int n)
    {
        number = n;
        if (number == 2048)
        {
            int length = aroundCol.Length;
            for (int i = 0; i < length; i++)
            {
                aroundCol[i].GetComponent<BubbleScript>().Explode();
            }
            Explode();
        }
        SetText();
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
    }

    public void Explode()
    {
        BubbleController.instance.SetPoints(number);
        //Make some particles
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bubble"))
        {
            BubbleController.instance.SetCanShoot(true);
            if (collision.collider.GetComponent<BubbleScript>().GetNumber() == number && isShooted)
            {
                collision.collider.GetComponent<BubbleScript>().SetStats(number * 2);
                collision.collider.GetComponent<BubbleScript>().SetText();
                BubbleController.instance.SetPoints(number);
                //Make some particles
                Destroy(gameObject);
            }
            else if(isShooted)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                isShooted = false;
            }
            else
            {
                
                if (aroundCol.Length == 1)
                {
                    aroundCol[0] = collision.collider;
                }
                else
                {
                    Collider2D[] temp = aroundCol;
                    aroundCol = new Collider2D[temp.Length + 1];
                    aroundCol = temp;
                    aroundCol[aroundCol.Length - 1] = collision.collider;
                }               
            }

        }else if (collision.collider.CompareTag("Floor"))
        {
            BubbleController.instance.SetPoints(number);
            //Make some particles
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Ceiling"))
        {
            BubbleController.instance.SetCanShoot(true);
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            isShooted = false;
        }
    }
}
