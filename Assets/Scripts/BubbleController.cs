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
    LayerMask raycastHitsLayer;
    [SerializeField]
    LayerMask SpotsMask;
    
    Collider2D spotRaycsted;
    RaycastHit2D[] hitsSpots;
    bool canShoot = true;
    bool isPressed = false;

    [Space]
    [Space]

    [Header("Points")]
    [SerializeField]
    int points;

    //Fusion Bubbles List
    List<BubbleScript> bubblesToFusion = new List<BubbleScript>();
    int listCounter = 0;

    float circleCastRad = 0.121f;

    private void Start()
    {
        NewBubble();
    }

    private void Update()
    {

        if (canShoot && isPressed == false)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 dir = Input.mousePosition;
                dir.z = -.1f;
                dir = Camera.main.ScreenToWorldPoint(dir);
                line.SetPosition(0, launchPosition.position);
                dir = dir - launchPosition.position;

                RaycastHit2D hit;
                //hit = Physics2D.Raycast(launchPosition.position, dir, 1000f, defaultLayer);
                hit = Physics2D.CircleCast(launchPosition.position, circleCastRad, dir, 2000f, raycastHitsLayer);

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
                        hit2 = Physics2D.CircleCast(hitResult, circleCastRad, Vector3.Reflect(dir, hit.normal), 1000f, raycastHitsLayer);
                        line.SetPosition(2, hit2.point);
                        if (hit2.collider.CompareTag("Bubble") || hit2.collider.CompareTag("Ceiling"))
                        {
                            ContactFilter2D cf = new ContactFilter2D();
                            cf.SetLayerMask(SpotsMask);
                            hitsSpots = new RaycastHit2D[20];
                            int numbHits = Physics2D.CircleCast(hitResult, circleCastRad, Vector3.Reflect(dir, hit.normal) * 2000f, cf, hitsSpots);
                            RaycastHit2D[] tempSpots = hitsSpots;
                            hitsSpots = new RaycastHit2D[numbHits];
                            hitsSpots = tempSpots;

                            Debug.DrawRay(hitResult, Vector3.Reflect(dir, hit.normal) * 2000f, Color.green, 2f);

                            Collider2D currentSpot = new Collider2D();
                            int length = hitsSpots.Length;
                            for (int i = 0; i < length; i++)
                            {
                                if (hitsSpots[i].collider.CompareTag("Bubble") || hitsSpots[i].collider.CompareTag("Ceiling"))
                                {
                                    currentSpot = hitsSpots[i - 1].collider;
                                    break;
                                }
                            }
                            if (currentSpot != spotRaycsted)
                            {
                                if (spotRaycsted != null)
                                {
                                    spotRaycsted.GetComponent<SpriteRenderer>().enabled = false;                                   
                                }
                                spotRaycsted = currentSpot;
                                spotRaycsted.GetComponent<SpriteRenderer>().enabled = true;
                                tempLaunch.GetComponent<BubbleScript>().SetSpot(spotRaycsted.transform);
                            }
                        }
                        
                    }
                    else if (hit.collider.CompareTag("Ceiling") || hit.collider.CompareTag("Bubble"))
                    {                       
                        line.SetPosition(1, hit.point);
                        line.SetPosition(2, hit.point);

                        ContactFilter2D cf = new ContactFilter2D();
                        cf.SetLayerMask(SpotsMask);
                        hitsSpots = new RaycastHit2D[30];
                        int numbHits = Physics2D.CircleCast(launchPosition.position + Vector3.up*0.5f, circleCastRad, dir * 10000f, cf, hitsSpots);
                        RaycastHit2D[] tempSpots = hitsSpots;
                        hitsSpots = new RaycastHit2D[numbHits];
                        hitsSpots = tempSpots;

                        Debug.DrawRay(launchPosition.position + Vector3.up*0.5f, dir*10000f, Color.red, 2f);

                        Collider2D currentSpot = new Collider2D();
                        int length = hitsSpots.Length;
                        for (int i = 0; i < length; i++)
                        {
                            if (hitsSpots[i].collider.CompareTag("Bubble") || hitsSpots[i].collider.CompareTag("Ceiling"))
                            {
                                currentSpot = hitsSpots[i - 1].collider;
                                break;
                            }
                        }
                        if (currentSpot != spotRaycsted)
                        {
                            if (spotRaycsted != null)
                            {
                                spotRaycsted.GetComponent<SpriteRenderer>().enabled = false;
                            }
                            spotRaycsted = currentSpot;
                            spotRaycsted.GetComponent<SpriteRenderer>().enabled = true;
                            tempLaunch.GetComponent<BubbleScript>().SetSpot(spotRaycsted.transform);
                        }
                    }
                    else
                    {
                        spotRaycsted.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                spotRaycsted.GetComponent<Collider2D>().isTrigger = true;
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
                tempLaunch.GetComponent<BubbleScript>().SetIsShooted(true);
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

    public void NewBubble()
    {
        bubblesToFusion.Clear();
        listCounter = 0;
        tempLaunch = Instantiate(bubblePrefab, launchPosition.position, Quaternion.identity);
        tempLaunch.GetComponent<BubbleScript>().SetStats(RandomNumber());
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
        
        return rand;
    }

    public void SetCanShoot(bool cs)
    {
        canShoot = cs;
    }

    public void AddBubbleFusionList(BubbleScript b)
    {
        bubblesToFusion.Add(b);
    }

    public bool CheckExistBubbleFusionList(BubbleScript b)
    {
        return bubblesToFusion.Contains(b);
    }

    public void UpCounter()
    {
        listCounter++;
        CheckNextInFusionList();
    }

    public bool CheckIfHaveToCounterUp()
    {
        if (listCounter < bubblesToFusion.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckNextInFusionList()
    {
        if (listCounter < bubblesToFusion.Count)
        {
            bubblesToFusion[listCounter].ChechForFusion();
        }
        else
        {
            FusionBubbles();
        }
        
    }

    public bool IndexListExist(int i)
    {
        if (bubblesToFusion[i] == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void FusionBubbles()
    {
        int length = bubblesToFusion.Count - 1;
        for (int i = 0; i < length; i++)
        {
            bubblesToFusion[i].Fusion();
        }
        if (bubblesToFusion[length].GetPow() < 11)
        {
            bubblesToFusion[length].SetStats(bubblesToFusion[length].GetPow() + length);
            //Recheck(bubblesToFusion[length]);
        }       
        NewBubble();
    }

    void Recheck(BubbleScript b)
    {
        BubbleScript bs = b;
        bubblesToFusion.Clear();
        listCounter = 0;
        b.ChechForFusion();
    }

}
