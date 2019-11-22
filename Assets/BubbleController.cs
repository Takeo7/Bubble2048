using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{

    #region Singleton
    public static BubbleController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion


    [Header("Bubble")]
    [SerializeField]
    GameObject bubblePrefab;

    [Space]
    [Space]

    [Header("Launcher")]
    [SerializeField]
    Transform launchPosition;
    GameObject tempLaunch;
    [SerializeField]
    float force;
    [SerializeField]
    LineRenderer line;
    [SerializeField]
    LayerMask layer;
    bool canShoot = true;

    [Space]
    [Space]

    [Header("Points")]
    [SerializeField]
    int points;

    private void Update()
    {
        if (canShoot)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 dir = Input.mousePosition;
                dir.z = 0f;
                dir = Camera.main.ScreenToWorldPoint(dir);
                line.SetPosition(0, launchPosition.position);
                dir = dir - launchPosition.position;

                RaycastHit2D hit;
                hit = Physics2D.Raycast(launchPosition.position, dir, 1000f, layer);

                Debug.Log(hit.collider);
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Wall"))
                    {
                        line.SetPosition(1, hit.point);
                        line.positionCount = 3;
                        RaycastHit2D hit2;
                        hit2 = Physics2D.Raycast(hit.point, Vector3.Reflect(dir, hit.normal), 1000f, layer);
                        Debug.DrawRay(hit.point, Vector3.Reflect(dir, hit.normal) * 1000f, Color.green, 5f);
                        if (hit2.collider.CompareTag("Bubble") || hit2.collider.CompareTag("Ceiling"))
                        {
                            line.SetPosition(2, hit2.point);
                        }
                    }
                    else if (hit.collider.CompareTag("Ceiling") || hit.collider.CompareTag("Bubble"))
                    {
                        line.positionCount = 2;
                        line.SetPosition(1, hit.point);
                    }
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                tempLaunch = Instantiate(bubblePrefab, launchPosition.position, Quaternion.identity);
                tempLaunch.GetComponent<BubbleScript>().SetStats(RandomNumber());



            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector3 dir = Input.mousePosition;
                dir.z = 0f;
                dir = Camera.main.ScreenToWorldPoint(dir);
                dir = dir - launchPosition.position;
                dir = dir.normalized;
                tempLaunch.GetComponent<Rigidbody2D>().AddForce(dir * force, ForceMode2D.Impulse);
                tempLaunch.layer = 0;
                canShoot = false;
                Debug.Log("ResetLine");
                line.SetPosition(0, Vector3.zero);
                line.SetPosition(1, Vector3.zero);
                line.SetPosition(2, Vector3.zero);
            }
        }
        
        
    }

    void CreateBubble()
    {
        /*
         * x+ = 0,65
         * y+ = 4
         * 
         * xini = -2,4
         * yini = 4,6
         */
    }

    public void SetPoints(int p)
    {
        points += p;
    }

    int RandomNumber()
    {
        // 2 - 4 - 8 - 16 - 32 - 64 - 128 - 256 - 512 - 1024 - 2048
        int rand = Random.Range(1, 11);
        if (rand == 11)
        {
            rand = 1;
        }
        int result = (int)Mathf.Pow(2, rand);
        return result;
    }

    public void SetCanShoot(bool cs)
    {
        canShoot = cs;
    }

}
