using UnityEngine;

public enum PlatformType
{
    normal,ice
}
/// <summary>
/// Script which decide which platform to spawn, the pattern , etc
/// </summary>
public class PlatformSpawner : MonoBehaviour {

    public static PlatformSpawner instance;
    [HideInInspector]
    public managerVars vars;

    private float nextXPos = 0.544f, nextYPos = 0.657f;//ref to some value used to get next platform spawn position
    private Vector3 platformPosition, animSpikePlatformPos;//ref to platform pos 
    private PlatformType typeOfPlatform = PlatformType.normal;//decide platform theme
    //some bool variables
    private bool leftDir = false, startPlatform = true, animSpikeSpawn = false;//x = 1.65f
    private bool spikeOnLeft = false;
    private int platformCount, animPlatformCount;//used in specific situation
    private GameObject lastPlatform;

    [SerializeField]
    private float fallTime = 6f;//time after which platform start falling
    [SerializeField]
    private float minFallTime = 1f;//min time
    [SerializeField]
    private float fallTimeDecreaseMilestone; //score milestone which reduced fall time 
    [SerializeField]
    private float multiplier; //value with wich the fall time is multipied and reduced
    private float milestoneCount;//keep track of number of time fall time reduced

    [SerializeField]
    private Vector3 startPosition;   //start position where 1st platform will spawn
    [SerializeField]
    private SpriteType selectSpriteType;
    [SerializeField]
    private GameObject characterobj;//ref to player prefab
    [SerializeField]
    private Renderer bgPlaneMat; //ref to background material
    [SerializeField]
    private SpriteRenderer bgSprite;                                                       //*******Changed

    public GameObject ObjectPool;
    public PlatformType TypeOfPlatform //setter
    {
        set { typeOfPlatform = value; }
    }

    public bool LeftDir //getter and setter
    {
        get { return leftDir; }
        set { leftDir = value; }
    }

    public bool StartPlatform //getter and setter
    {
        get { return startPlatform; }
        set { startPlatform = value; }
    }

    void OnEnable()
    {
        vars = Resources.Load<managerVars>("managerVarsContainer");
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Use this for initialization
    void Start ()
    {   //set time milestone
        milestoneCount = fallTimeDecreaseMilestone;

        DecidePlatformTheme();//decide theme

        platformPosition = startPosition;//set the platform position

        for (int i = 0; i < 5; i++)
        {
            platformCount = 5; //set the number of platform the spawn in specific direction(here right)
            DecidePath();//spawn the platform
        }

        GameObject playerObj = Instantiate(characterobj);//spawn the player
        playerObj.transform.position = new Vector3(0, -2.2f, 0);//position at which player is spawned
    }

    void Update()
    {
        //if gameover is false we keep on checking milestone
        if (GameManager.instance.gameOver == false)
        {
            CheckMileStone();
        }
        
    }

    public void AllBackStart()
    {
        for (int i = 0; i < ObjectPool.transform.childCount; i++)
        {
            if (ObjectPool.transform.GetChild(i).tag=="Platform")
            {
                if(ObjectPool.transform.GetChild(i).gameObject.activeSelf==true)
                {
                    ObjectPool.transform.GetChild(i).GetComponent<PlatformScript>().BackStart(2);
                }
            }
        }
    }

    //methode which increase the milestone
    void CheckMileStone()
    {   //if the current score is more than milestone
        if (GameManager.instance.currentScore > milestoneCount)
        {   //we increase milestone
            milestoneCount += fallTimeDecreaseMilestone;
            //change the next milestone value
            fallTimeDecreaseMilestone += fallTimeDecreaseMilestone;
            fallTime *= multiplier; //reduce the falltime
            //check if fall time is less than minimum
            if (fallTime <= minFallTime)
            {   //then we set the fall time to minimum
                fallTime = minFallTime;
            }

        }
    }
    //method which spawn the platform
    public void SpawnPlatform()
    {
        int boolR = Random.Range(0, 2); //bool to deide on which side the obstacle must be (left or right)

        if (platformCount >= 1) //if platform count is equal or more than 1 
        {   //we spawn normal platform
            SpawnNormal(boolR);//spawn normal
        }//if platform count is equal to 0 
        else if (platformCount == 0)
        {   //we spawn any 1 from the othe platfomr
            int r = Random.Range(0, 3); //we decide the rndom number

            if (r == 0)//if its zero
            {   //we spawn common platform
                SpawnCommonPlatform(boolR);//spawn common
            }
            else if (r == 1) //if its 1
            {   //we spawn specific platform depending on theme
                switch (typeOfPlatform)
                {   //for normal we spawn grass theme
                    case PlatformType.normal:

                        SpawnGrassPlatform(boolR);//spawn grass

                        break;
                        //for ice we spawn ice theme
                    case PlatformType.ice:

                        SpawnIcePlatform(boolR);//spawn ice

                        break;
                }
            }
            else if (r == 2)//if its 2
            {   //new int value
                int valueR = 0;
                if (leftDir == true)//check the leftDir value
                {
                    valueR = 0; //decide the value
                }
                else
                {
                    valueR = 1;
                }
                //spawn the spike anim platform
                SpawnAnimSpike(valueR); //spawn anim spike platform
                animSpikeSpawn = true;//set the animSpikeSpawn true
                animPlatformCount = 3;//and count to 3
                //check which platform is spawned spike on left side or on right side
                if (spikeOnLeft)
                {   //depending on that normal platform are spawned
                    animSpikePlatformPos = new Vector3(platformPosition.x - 1.65f,
                       platformPosition.y + nextYPos, 0);
                    platformPosition = new Vector3(platformPosition.x + nextXPos, platformPosition.y + nextYPos, 0);
                }
                else
                {   //depending on that normal platform are spawned
                    animSpikePlatformPos = new Vector3(platformPosition.x + 1.65f,
                        platformPosition.y + nextYPos, 0);
                    platformPosition = new Vector3(platformPosition.x - nextXPos, platformPosition.y + nextYPos, 0);
                }

                return;
            }
        }
        //decide random number
        int dimondR = Random.Range(0, 12);

        if (dimondR == 2 && lastPlatform != null)//if its 2 we spawn the diamond
        {   //get the diamond obj from pooler
            GameObject diamond = ObjectPooling.instance.GetDiamond();
            //set its position
            diamond.transform.position = new Vector3(platformPosition.x, platformPosition.y + 0.5f, 0);
            diamond.SetActive(true); //activate it
        }
        //depending on dir the next platform position is set
        if (leftDir)
            platformPosition = new Vector3(platformPosition.x - nextXPos, platformPosition.y + nextYPos, 0);
        else
            platformPosition = new Vector3(platformPosition.x + nextXPos, platformPosition.y + nextYPos, 0);
    }
    //method which decide in which direction and how many platforms to spawn
    public void DecidePath()
    {   //if last platform spawned was anim spike
        if (animSpikeSpawn)
        {   //then we spawn the specific platform  and return from this method
            AnimSpikeSpawner();
            return;
        }
        //if platform count is more than or equal to zerp
        if (platformCount >= 0)
        {   //we spawn the platform
            SpawnPlatform();
            platformCount--; //reduce the count
        }
        else
        {   //else we decide the count , direction and then spawn 
            platformCount = Random.Range(1, 3);

            leftDir = !leftDir;

            SpawnPlatform();
        } 
    }

    //method which decide the theme of platform and bg
    public void DecidePlatformTheme()
    {   //we get the random number form the total platfomrs theme save in managerVars
        int r = Random.Range(0, vars.platforms.Count);
        //normalS, fireS, iceS, grassS
        if (r == 0)
        {
            selectSpriteType = SpriteType.normalS;
            typeOfPlatform = PlatformType.normal;
            //bgPlaneMat.material.mainTexture = vars.bgThemes[0].backgroundTexture;
            bgSprite.sprite = vars.bgThemes[0].backgroundSprite;                                     //*******Changed
        }
        else if (r == 1)
        {
            selectSpriteType = SpriteType.fireS;
            typeOfPlatform = PlatformType.normal;
            //bgPlaneMat.material.mainTexture = vars.bgThemes[1].backgroundTexture;
            bgSprite.sprite = vars.bgThemes[1].backgroundSprite;                                       //*******Changed
        }
        else if (r == 2)
        {
            selectSpriteType = SpriteType.iceS;
            typeOfPlatform = PlatformType.ice;
            //bgPlaneMat.material.mainTexture = vars.bgThemes[2].backgroundTexture;
            bgSprite.sprite = vars.bgThemes[2].backgroundSprite;                                       //*******Changed
        }
        else if (r == 3)
        {
            selectSpriteType = SpriteType.grassS;
            typeOfPlatform = PlatformType.normal;
            //bgPlaneMat.material.mainTexture = vars.bgThemes[0].backgroundTexture;
            bgSprite.sprite = vars.bgThemes[0].backgroundSprite;                                        //*******Changed
        }
    }

    //methode which spawn the normal platform
    void SpawnNormal(int boolR)
    {
        GameObject normalPlatform = ObjectPooling.instance.GetPlatfrom();
        normalPlatform.transform.position = platformPosition;

        //获取普通的初始坐标
        normalPlatform.GetComponent<PlatformScript>().StartPos = platformPosition;

        normalPlatform.GetComponent<PlatformScript>().MyBody.bodyType = RigidbodyType2D.Static;
        normalPlatform.SetActive(true);
        normalPlatform.GetComponent<PlatformScript>().BasicSettings(fallTime);
        if (boolR == 0)//left
            normalPlatform.GetComponent<PlatformModifier>().ReverseObstaclePos = false;
        else//right
            normalPlatform.GetComponent<PlatformModifier>().ReverseObstaclePos = true;
        normalPlatform.GetComponent<PlatformScript>().StartTimer = true;
        normalPlatform.GetComponent<PlatformModifier>().BasicSettings(selectSpriteType);
        lastPlatform = normalPlatform;
    }

    //methode which spawn the common platform
    void SpawnCommonPlatform(int boolR)
    {
        GameObject commonPlatform = ObjectPooling.instance.GetCommonPlatfrom();
        commonPlatform.transform.position = platformPosition;

        //获取普通的初始坐标
        commonPlatform.GetComponent<PlatformScript>().StartPos = platformPosition;


        commonPlatform.GetComponent<PlatformScript>().MyBody.bodyType = RigidbodyType2D.Static;
        commonPlatform.SetActive(true);
        commonPlatform.GetComponent<PlatformScript>().BasicSettings(fallTime);

        if (boolR == 0)
            commonPlatform.GetComponent<PlatformModifier>().ReverseObstaclePos = false;
        else
            commonPlatform.GetComponent<PlatformModifier>().ReverseObstaclePos = true;

        commonPlatform.GetComponent<PlatformScript>().StartTimer = true;
        commonPlatform.GetComponent<PlatformModifier>().BasicSettings(selectSpriteType);
        lastPlatform = commonPlatform;
    }

    //methode which spawn the grass platform
    void SpawnGrassPlatform(int boolR)
    {
        GameObject grassPlatform = ObjectPooling.instance.GetGrassPlatfrom();
        grassPlatform.transform.position = platformPosition;

        //获取普通的初始坐标
        grassPlatform.GetComponent<PlatformScript>().StartPos = platformPosition;

        grassPlatform.GetComponent<PlatformScript>().MyBody.bodyType = RigidbodyType2D.Static;
        grassPlatform.SetActive(true);
        grassPlatform.GetComponent<PlatformScript>().BasicSettings(fallTime);

        if (boolR == 0)
            grassPlatform.GetComponent<PlatformModifier>().ReverseObstaclePos = false;
        else
            grassPlatform.GetComponent<PlatformModifier>().ReverseObstaclePos = true;

        grassPlatform.GetComponent<PlatformScript>().StartTimer = true;
        grassPlatform.GetComponent<PlatformModifier>().BasicSettings(selectSpriteType);
        lastPlatform = grassPlatform;
    }

    //methode which spawn the ice platform
    void SpawnIcePlatform(int boolR)
    {
        GameObject icePlatform = ObjectPooling.instance.GetWinterPlatfrom();
        icePlatform.transform.position = platformPosition;

        //获取普通的初始坐标
        icePlatform.GetComponent<PlatformScript>().StartPos = platformPosition;

        icePlatform.GetComponent<PlatformScript>().MyBody.bodyType = RigidbodyType2D.Static;
        icePlatform.SetActive(true);
        icePlatform.GetComponent<PlatformScript>().BasicSettings(fallTime);

        if (boolR == 0)
            icePlatform.GetComponent<PlatformModifier>().ReverseObstaclePos = false;
        else
            icePlatform.GetComponent<PlatformModifier>().ReverseObstaclePos = true;

        icePlatform.GetComponent<PlatformScript>().StartTimer = true;
        icePlatform.GetComponent<PlatformModifier>().BasicSettings(selectSpriteType);
        lastPlatform = icePlatform;
    }

    //methode which spawn the anim spike platform
    void SpawnAnimSpike(int boolR)
    {
        GameObject animSpikePlatform = null;
        //left side
        if (boolR == 1)
        {
            animSpikePlatform = ObjectPooling.instance.GetAnimSpikePlatfromLeft();
            spikeOnLeft = true;
        }
        else
        {
            animSpikePlatform = ObjectPooling.instance.GetAnimSpikePlatfromRight();
            spikeOnLeft = false;
        }

        animSpikePlatform.transform.position = platformPosition;
        animSpikePlatform.GetComponent<PlatformScript>().MyBody.bodyType = RigidbodyType2D.Static;
        animSpikePlatform.SetActive(true);
        animSpikePlatform.GetComponent<PlatformScript>().BasicSettings(fallTime);
        animSpikePlatform.GetComponent<PlatformModifier>().BasicSettings(selectSpriteType);
        animSpikePlatform.GetComponent<PlatformScript>().StartTimer = true;
        string objName = animSpikePlatform.name;
        lastPlatform = animSpikePlatform;
    }

    //methode which spawn the normal platform after tha niam spike platform is spawned
    void AnimSpikeSpawner()
    {
        if (animPlatformCount > 0)
        {
            animPlatformCount--;
            for (int i = 0; i < 2; i++)
            {
                GameObject normalPlatform = ObjectPooling.instance.GetPlatfrom();
                if (i == 0)
                {   //right

                    if (spikeOnLeft == true)
                    {
                        normalPlatform.transform.position = platformPosition;
                        platformPosition = new Vector3(platformPosition.x + nextXPos, platformPosition.y + nextYPos, 0);
                    }
                    else
                    {
                        normalPlatform.transform.position = platformPosition;
                        platformPosition = new Vector3(platformPosition.x - nextXPos, platformPosition.y + nextYPos, 0);
                    }
                }
                else
                {   //left
                    if (spikeOnLeft == true)
                    {
                        normalPlatform.transform.position = animSpikePlatformPos;
                        animSpikePlatformPos = new Vector3(animSpikePlatformPos.x - nextXPos,
                            animSpikePlatformPos.y + nextYPos, 0);
                    }
                    else
                    {
                        normalPlatform.transform.position = animSpikePlatformPos;
                        animSpikePlatformPos = new Vector3(animSpikePlatformPos.x + nextXPos,
                            animSpikePlatformPos.y + nextYPos, 0);
                    }
                }
                normalPlatform.GetComponent<PlatformScript>().MyBody.bodyType = RigidbodyType2D.Static;
                normalPlatform.SetActive(true);
                normalPlatform.GetComponent<PlatformScript>().BasicSettings(fallTime);
                if (i == 0)
                    normalPlatform.GetComponent<PlatformModifier>().ReverseObstaclePos = true;
                else
                    normalPlatform.GetComponent<PlatformModifier>().ReverseObstaclePos = false;

                normalPlatform.GetComponent<PlatformScript>().StartTimer = true;
                normalPlatform.GetComponent<PlatformModifier>().BasicSettings(selectSpriteType);
            }
        }
        else
        {
            animSpikeSpawn = false;
            DecidePath();
        }
    }

}
