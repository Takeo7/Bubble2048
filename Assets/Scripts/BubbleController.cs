using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [SerializeField]
    Color[] colors;

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
    LayerMask layerCast;
    [SerializeField]
    LayerMask spotLayer;
    
    Collider2D spotRaycsted;
    RaycastHit2D[] hitsSpots;
    bool canShoot = true;
    bool isPressed = false;

    [Space]
    [Space]

    [Header("Points")]
    [SerializeField]
    int points;
    [SerializeField]
    TextMeshProUGUI pointsText;
    [SerializeField]
    GameObject perfectText;

    [Space]
    [Space]

    [Header("SpotGrid")]
    [SerializeField]
    GameObject spotGridPrefab;
    [SerializeField]
    GameObject gridSpots;
    bool isGridLeft = true;
    bool isFusion = true;
    int bounces = 0;

    [Space]
    [Space]

    [Header("Game Over")]
    [SerializeField]
    GameObject GameOverText;

    //Fusion Bubbles List
    List<BubbleScript> bubblesToFusion = new List<BubbleScript>();
    int listCounter = 0;

    //Global Bubbles List
    List<BubbleScript> allBubbles = new List<BubbleScript>();

    float circleCastRad = 0.121f;



    private void Start()
    {
        NewBubble();
    }

    private void Update()
    {

        if (canShoot && isPressed == false)
        {
            #region while Button is pressed down
            if (Input.GetMouseButton(0))
            {
                #region Gets the direction to shoot the bubble
                Vector3 dir = Input.mousePosition;
                dir.z = -.1f;
                dir = Camera.main.ScreenToWorldPoint(dir);                
                dir = dir - launchPosition.position;

                line.SetPosition(0, launchPosition.position);
                #endregion

                #region First CircleCast (hit)
                RaycastHit2D hit;
                hit = Physics2D.CircleCast(launchPosition.position, circleCastRad, dir, 2000f, raycastHitsLayer);
                #endregion

                #region Correction of hit
                Vector2 hitResult;
                if (hit.collider.CompareTag("Wall-Left"))
                {
                    hitResult = hit.point + new Vector2(0.3f, 0);
                }
                else
                {
                    hitResult = hit.point + new Vector2(-0.3f, 0);
                }
                #endregion

                line.SetPosition(1, hit.point);

                if (hit.collider != null)
                {
                    #region if hits first a wall
                    if (hit.collider.CompareTag("Wall-Left") || hit.collider.CompareTag("Wall-Right"))
                    {
                        bounces = 1;

                        #region Second CircleCast (hit2)
                        RaycastHit2D hit2;
                        hit2 = Physics2D.CircleCast(hitResult, circleCastRad, Vector3.Reflect(dir, hit.normal), 1000f, raycastHitsLayer);
                        #endregion

                        #region Correction of the hit2
                        Vector2 hitResult2;
                        if (hit2.collider.CompareTag("Wall-Left"))
                        {
                            hitResult2 = hit2.point + new Vector2(0.3f, 0);
                        }
                        else
                        {
                            hitResult2 = hit2.point + new Vector2(-0.3f, 0);
                        }
                        #endregion

                        line.SetPosition(2, hit2.point);
                        line.SetPosition(3, hit2.point);

                        #region if hits second a Bubble or Ceiling
                        if (hit2.collider.CompareTag("Bubble") || hit2.collider.CompareTag("Ceiling"))
                        {
                            #region Circle Cast For Spots
                            ContactFilter2D cf = new ContactFilter2D();
                            cf.SetLayerMask(layerCast);
                            hitsSpots = new RaycastHit2D[20];
                            int numbHits = Physics2D.CircleCast(hitResult, circleCastRad, Vector3.Reflect(dir, hit.normal) * 20000f, cf, hitsSpots);
                            RaycastHit2D[] tempSpots = hitsSpots;
                            hitsSpots = new RaycastHit2D[numbHits];
                            hitsSpots = tempSpots;

                            Debug.DrawRay(hitResult, Vector3.Reflect(dir, hit.normal) * 20000f, Color.green, 2f);
                            #endregion

                            #region Set the spot
                            Collider2D currentSpot = new Collider2D();
                            int length = hitsSpots.Length;
                            for (int i = 0; i < length; i++)
                            {
                                if (hitsSpots[i].collider != null)
                                {
                                    if (hitsSpots[i].collider.CompareTag("Bubble") || hitsSpots[i].collider.CompareTag("Ceiling") || hitsSpots[i].collider.CompareTag("Wall-Left") || hitsSpots[i].collider.CompareTag("Wall-Right"))
                                    {
                                        if (i != 0)
                                        {
                                            currentSpot = hitsSpots[i - 1].collider;
                                            break;
                                        }
                                    }
                                }                                
                            }
                            if (currentSpot != spotRaycsted)
                            {
                                if (spotRaycsted != null)
                                {    
                                    if (spotRaycsted.CompareTag("Spot"))
                                    {
                                        spotRaycsted.transform.GetChild(0).gameObject.SetActive(false);
                                    }
                                    
                                }
                                if (currentSpot != null)
                                {
                                    spotRaycsted = currentSpot;
                                }
                                
                                spotRaycsted.transform.GetChild(0).gameObject.SetActive(true);
                                if (spotRaycsted.CompareTag("Spot"))
                                {
                                    spotRaycsted.transform.GetChild(0).GetComponent<SpriteRenderer>().color = tempLaunch.GetComponent<SpriteRenderer>().color;
                                    tempLaunch.GetComponent<BubbleScript>().SetSpot(spotRaycsted.transform);
                                }
                                #region if doesnt gets a spot
                                else
                                {
                                    RaycastHit2D r;
                                    

                                    if (hit.collider.CompareTag("Wall-Left"))
                                    {
                                        hitResult = hit.point + new Vector2(0.3f, 0);
                                    }
                                    else
                                    {
                                        hitResult = hit.point + new Vector2(-0.3f, 0);
                                    }
                                    r = Physics2D.CircleCast(hitResult, circleCastRad/2,hit.normal,10f,spotLayer);
                                    spotRaycsted = r.collider;
                                    Debug.DrawRay(hitResult+new Vector2(0,-0.6f), hit.normal*10f,Color.blue,2f);
                                    spotRaycsted.transform.GetChild(0).gameObject.SetActive(true);
                                    spotRaycsted.transform.GetChild(0).GetComponent<SpriteRenderer>().color = tempLaunch.GetComponent<SpriteRenderer>().color;
                                    tempLaunch.GetComponent<BubbleScript>().SetSpot(spotRaycsted.transform);
                                }
                                #endregion

                            }
                            #endregion
                        }
                        #endregion

                        #region If hits second a Wall
                        else
                        {
                            bounces = 2;

                            #region Third Circlecast (hit3)                    
                            RaycastHit2D hit3;
                            hit3 = Physics2D.CircleCast(hitResult2, circleCastRad, Vector3.Reflect(Vector3.Reflect(dir, hit.normal), hit2.normal), 1000f, raycastHitsLayer);
                            #endregion

                            #region Correction fo hit3             
                            Vector2 hitResult3;
                            if (hit3.collider.CompareTag("Wall-Left"))
                            {
                                hitResult3 = hit3.point + new Vector2(0.3f, 0);
                            }
                            else if(hit3.collider.CompareTag("Wall-Right"))
                            {
                                hitResult3 = hit3.point + new Vector2(-0.3f, 0);
                            }
                            #endregion

                            line.SetPosition(3, hit3.point);

                            if (hit3.collider.CompareTag("Bubble") || hit3.collider.CompareTag("Ceiling"))
                            {
                                ContactFilter2D cf = new ContactFilter2D();
                                cf.SetLayerMask(layerCast);
                                hitsSpots = new RaycastHit2D[20];
                                int numbHits = Physics2D.CircleCast(hitResult2, circleCastRad, Vector3.Reflect(Vector3.Reflect(dir, hit.normal), hit2.normal) * 20000f, cf, hitsSpots);
                                RaycastHit2D[] tempSpots = hitsSpots;
                                hitsSpots = new RaycastHit2D[numbHits];
                                hitsSpots = tempSpots;

                                Debug.DrawRay(hitResult2, Vector3.Reflect(Vector3.Reflect(dir, hit.normal), hit2.normal) * 20000f, Color.blue, 2f);

                                Collider2D currentSpot = new Collider2D();
                                int length = hitsSpots.Length;
                                for (int i = 0; i < length; i++)
                                {
                                    if (hitsSpots[i].collider != null)
                                    {
                                        if (hitsSpots[i].collider.CompareTag("Bubble") || hitsSpots[i].collider.CompareTag("Ceiling") || hitsSpots[i].collider.CompareTag("Wall-Left") || hitsSpots[i].collider.CompareTag("Wall-Right"))
                                        {
                                            if (i != 0)
                                            {
                                                currentSpot = hitsSpots[i - 1].collider;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (currentSpot != spotRaycsted)
                                {
                                    if (spotRaycsted != null)
                                    {
                                        //spotRaycsted.GetComponent<SpriteRenderer>().enabled = false;     
                                        if (spotRaycsted.CompareTag("Spot"))
                                        {
                                            spotRaycsted.transform.GetChild(0).gameObject.SetActive(false);
                                        }

                                    }
                                    if (currentSpot != null)
                                    {
                                        spotRaycsted = currentSpot;
                                    }

                                    //spotRaycsted.GetComponent<SpriteRenderer>().enabled = true;
                                    spotRaycsted.transform.GetChild(0).gameObject.SetActive(true);
                                    if (spotRaycsted.CompareTag("Spot"))
                                    {
                                        spotRaycsted.transform.GetChild(0).GetComponent<SpriteRenderer>().color = tempLaunch.GetComponent<SpriteRenderer>().color;
                                        tempLaunch.GetComponent<BubbleScript>().SetSpot(spotRaycsted.transform);
                                    }
                                    else
                                    {
                                        RaycastHit2D r;


                                        if (hit2.collider.CompareTag("Wall-Left"))
                                        {
                                            hitResult = hit2.point + new Vector2(0.3f, -0.5f);
                                        }
                                        else
                                        {
                                            hitResult = hit2.point + new Vector2(-0.3f, -0.5f);
                                        }
                                        r = Physics2D.Raycast(hitResult, hit2.normal, 10f, spotLayer);
                                        spotRaycsted = r.collider;
                                        Debug.DrawRay(hitResult, hit2.normal, Color.blue, 2f);
                                        Debug.Log(spotRaycsted);
                                        spotRaycsted.transform.GetChild(0).gameObject.SetActive(true);
                                        spotRaycsted.transform.GetChild(0).GetComponent<SpriteRenderer>().color = tempLaunch.GetComponent<SpriteRenderer>().color;
                                        tempLaunch.GetComponent<BubbleScript>().SetSpot(spotRaycsted.transform);
                                    }

                                }
                            }
                        }
                        #endregion

                    }
                    #endregion
                    #region If Raycast Hits First Ceiling or Other Bubble
                    else if (hit.collider.CompareTag("Ceiling") || hit.collider.CompareTag("Bubble"))
                    {
                        bounces = 0;

                        line.SetPosition(1, hit.point);
                        line.SetPosition(2, hit.point);
                        line.SetPosition(3, hit.point);

                        ContactFilter2D cf = new ContactFilter2D();
                        cf.SetLayerMask(layerCast);
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
                            if (hitsSpots[i].collider.CompareTag("Bubble") || hitsSpots[i].collider.CompareTag("Ceiling") || hitsSpots[i].collider.CompareTag("Wall-Left") || hitsSpots[i].collider.CompareTag("Wall-Right"))
                            {

                                if (i != 0)
                                {
                                    currentSpot = hitsSpots[i - 1].collider;
                                    break;
                                }
                            }
                        }
                        if (currentSpot != spotRaycsted)
                        {
                            if (spotRaycsted != null)
                            {
                                //spotRaycsted.GetComponent<SpriteRenderer>().enabled = false;     
                                if (spotRaycsted.CompareTag("Spot"))
                                {
                                    spotRaycsted.transform.GetChild(0).gameObject.SetActive(false);
                                }
                                    
                            }
                            spotRaycsted = currentSpot;
                            //spotRaycsted.GetComponent<SpriteRenderer>().enabled = true;
                            spotRaycsted.transform.GetChild(0).gameObject.SetActive(true);
                            if (spotRaycsted.CompareTag("Spot"))
                            {
                                spotRaycsted.transform.GetChild(0).GetComponent<SpriteRenderer>().color = tempLaunch.GetComponent<SpriteRenderer>().color;
                                tempLaunch.GetComponent<BubbleScript>().SetSpot(spotRaycsted.transform);
                            }
                        }
                    }
                    #endregion
                    else
                    {
                        spotRaycsted.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }
            #endregion
            #region when button is released
            else if (Input.GetMouseButtonUp(0) && spotRaycsted != null)
            {
                spotRaycsted.GetComponent<Collider2D>().isTrigger = true;
                Vector3 dir = Input.mousePosition;
                dir.z = 0f;
                dir = Camera.main.ScreenToWorldPoint(dir);
                dir = dir - launchPosition.position;
                dir = dir.normalized;
                tempLaunch.GetComponent<Rigidbody2D>().AddForce(dir * force, ForceMode2D.Impulse);
                tempLaunch.layer = 0;
                if (bounces == 0)
                {
                    tempLaunch.GetComponent<Collider2D>().isTrigger = true;
                }
                else
                {
                    tempLaunch.GetComponent<BubbleScript>().bounces = bounces;
                }
                canShoot = false;
                line.SetPosition(0, Vector3.zero);
                line.SetPosition(1, Vector3.zero);
                line.SetPosition(2, Vector3.zero);
                line.SetPosition(3, Vector3.zero);
                tempLaunch.GetComponent<BubbleScript>().SetIsShooted(true);
            }
            #endregion
        }


    }

    void CreateLine()
    {
        /*
         * x+ = 0.325f
         * y+ = 0.6f

         * yini = 3.6f
         */
    }

    public void NewBubble()
    {
        if (isFusion)
        {
            StartNewLine();
            isFusion = false;
        } 
        bubblesToFusion.Clear();
        listCounter = 0;
        tempLaunch = Instantiate(bubblePrefab, launchPosition.position, Quaternion.identity);
        int rn = RandomNumber();
        tempLaunch.GetComponent<BubbleScript>().SetStats(rn,colors[rn-1]);
        canShoot = true;
    }

    public void SetPoints(int p)
    {
        points += p;
        pointsText.text = points + " Points";
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

    void CheckNextInFusionList()
    {
        if (listCounter < bubblesToFusion.Count)
        {
            
            bubblesToFusion[listCounter].ChechForFusion(false);
        }
        else if(listCounter == bubblesToFusion.Count && bubblesToFusion.Count == 1)
        {
            
            NewBubble();
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
        isFusion = true;
        int length = bubblesToFusion.Count - 1;
        for (int i = 0; i < length; i++)
        {
            bubblesToFusion[i].Explode();
        }
        if (bubblesToFusion[length].GetPow() < 11)
        {
            int numb = bubblesToFusion[length].GetPow() + length;
            bubblesToFusion[length].SetStats(numb,colors[numb-1]);
            Recheck(bubblesToFusion[length]);
        }
    }

    void Recheck(BubbleScript b)
    {
        BubbleScript bs = b;
        if (bs.CheckAround())
        {
            bubblesToFusion.Clear();
            listCounter = 0;
            bs.ChechForFusion(true);           
        }
        else
        {
            CheckFallBubbles();
            NewBubble();
        }
        
    }

    #region CheckFall

    public void AddBubbleToGlobalList(BubbleScript b)
    {
        allBubbles.Add(b);
    }

    public void RemoveBubbleFramGlobalList(BubbleScript b)
    {
        allBubbles.Remove(b);
        if (allBubbles.Count == 0)
        {
            perfectText.SetActive(true);
        }
    }

    void CheckFallBubbles()
    {
        foreach (BubbleScript item in allBubbles)
        {
            item.isCeilingConnected = false;
        }
        List<BubbleScript> attachedToCeiling = new List<BubbleScript>();
        foreach (BubbleScript item in allBubbles)
        {
            if (item.transform.position.y > 3.2f)
            {
                attachedToCeiling.Add(item);
            }
        }

        foreach (BubbleScript item in attachedToCeiling)
        {
            item.CheckAroundCeiling();
        }

        foreach (BubbleScript item in allBubbles)
        {
            if (item.isCeilingConnected == false)
            {
                item.Fall();
            }
        }

    }

    #endregion

    #region NewLineSpots

    void StartNewLine()
    {
        DownLines();
    }

    void DownLines()
    {
        gridSpots.transform.position -= new Vector3(0, 0.6f);
        NewLineSpots();
    }

    void NewLineSpots()
    {
        GameObject temp;
        if (isGridLeft)
        {
            temp = Instantiate(spotGridPrefab, new Vector3(0.325f, -0.97f), Quaternion.identity);
            isGridLeft = false;
        }
        else
        {
            temp = Instantiate(spotGridPrefab, new Vector3(0, -0.97f), Quaternion.identity);
            isGridLeft = true;
        }
        
        temp.transform.SetParent(gridSpots.transform);
        
        int length = temp.transform.childCount;
        for (int i = 0; i < length; i++)
        {
            GameObject tempBubble = Instantiate(bubblePrefab, temp.transform.GetChild(i).position, Quaternion.identity);
            tempBubble.GetComponent<BubbleScript>().SetBubbleInNewLine(temp.transform.GetChild(i));
            int rn = RandomNumber();
            tempBubble.GetComponent<BubbleScript>().SetStats(rn, colors[rn - 1]);
        }
    }


    #endregion

    #region Game Over

    public void GameOver()
    {
        canShoot = false;
        GameOverText.SetActive(true);
    }

    #endregion
}
