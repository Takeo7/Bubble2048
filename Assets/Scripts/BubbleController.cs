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
    LayerMask defaultLayer;
    [SerializeField]
    LayerMask SpotsMask;
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
                //hit = Physics2D.Raycast(launchPosition.position, dir, 1000f, defaultLayer);
                hit = Physics2D.CircleCast(launchPosition.position, 0.121f, dir, 1000f, defaultLayer);

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Wall-Left") || hit.collider.CompareTag("Wall-Right"))
                    {
                        Vector2 hitResult;
                        if (hit.collider.CompareTag("Wall-Left"))
                        {
                            hitResult = hit.point + new Vector2(0.3f, 0);
                        }
                        else
                        {
                            hitResult = hit.point + new Vector2(-0.3f, 0);
                        }
                        line.SetPosition(1, hit.point);
                        RaycastHit2D hit2;
                        //hit2 = Physics2D.Raycast(hitResult, Vector3.Reflect(dir, hit.normal), 10000f, defaultLayer);
                        hit2 = Physics2D.CircleCast(hitResult, 0.121f, Vector3.Reflect(dir, hit.normal), 1000f, defaultLayer);
                        line.SetPosition(2, hit2.point);
                        if (hit2.collider.CompareTag("Bubble") || hit2.collider.CompareTag("Ceiling"))
                        {
                            //line.SetPosition(2, hit2.point);
                        }
                    }
                    else if (hit.collider.CompareTag("Ceiling") || hit.collider.CompareTag("Bubble"))
                    {                       
                        line.SetPosition(1, hit.point);
                        line.SetPosition(2, hit.point);
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
                line.SetPosition(0, Vector3.zero);
                line.SetPosition(1, Vector3.zero);
                if (line.positionCount == 3)
                {
                    line.SetPosition(2, Vector3.zero);
                }
                
            }
        }
        
        
    }

    void CreateBubble()
    {
        /*
         * x+ = 0,65
         * y+ = 0,6
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
