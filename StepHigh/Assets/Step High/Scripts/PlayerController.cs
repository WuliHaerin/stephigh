using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour {

    public static PlayerController instance;

    #region Set in editor;
    [SerializeField] //position from where ray start
    private Transform rayDown, rayLeft, rayRight;
    [SerializeField]//layers to decide the platform and obstacle
    private LayerMask platformLayer, obstacleLayer;
    #endregion Set in editor;

    private SpriteRenderer charImg; //character image
    private Rigidbody2D myBody;//ref to rigidbody
    private bool jumping = false,moveLeft = false; 
    //ref to position of platforms
    private Vector3 nextPlatformLeft, nextPlatformRight, currentPlatform;
    private GameObject curPlateformObj;
    private float offseX = 0.544f, offsetY = 0.657f; //this is to get next platform position (**dont change it)
    private AudioSource audioS;
    private bool startedMoving = false;

    public managerVars vars;

    public bool StartedMoving { get { return startedMoving; } }

    void Awake()
    {
        if (instance == null) instance = this;
        audioS = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start ()
    {   //getting the component attached to the gamobject
        myBody = GetComponent<Rigidbody2D>();
        charImg = GetComponent<SpriteRenderer>();
        CameraFollow.instance.PlayerSettings();
        charImg.sprite = vars.characters[GameManager.instance.selectedSkin].gameCharacterSprite;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //check if game started or not
        if (GameUI.instance.GameStarted == false || GameManager.instance.gameOver)
            return;
        //when mouse is clicked and player is not jumping
        if (Input.GetMouseButtonDown(0) && !jumping)
        {
            if (startedMoving == false) startedMoving = true;
            audioS.PlayOneShot(vars.jumpSound);
            //get the mouse position
            Vector2 mousePos = Input.mousePosition;
            //disable boxcollider
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            //check the mouse pos x value
            if (mousePos.x <= Screen.width / 2) //click on left side of mouse
            {
                moveLeft = true;
            }
            else if (mousePos.x >= Screen.width / 2) //click on right side of mouse
            {
                moveLeft = false;
            }
            //set jumping to true
            jumping = true;
            //
            if (PlatformSpawner.instance.StartPlatform == true)
            {
                PlatformSpawner.instance.StartPlatform = false;
            }
            //spawn the new platform
            PlatformSpawner.instance.DecidePath();
            //jump method is called
            Jump();

        }
        //when the player is falling and game is not over
        if (myBody.velocity.y < 0 && GameManager.instance.gameOver == false)
        {   //we unable the collider
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        //check is moving down , not platform at bottom and game is not over
        if (myBody.velocity.y < 0 && PlatformPresent() == false && GameManager.instance.gameOver == false)
        {
            dieType = 1;
            //game is over
            audioS.PlayOneShot(vars.fallSound);
            GameManager.instance.gameOver = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;//deactivate the collider
            charImg.sortingLayerName = "Default";//change the layer
            //start a coroutine
            StartCoroutine(DeactivatePlayer());
        }

        if ((transform.position.y - Camera.main.transform.position.y) <= -6f && GameManager.instance.gameOver == false)
        {
            dieType = 2;

            //game is over
            audioS.PlayOneShot(vars.fallSound);
            GameManager.instance.gameOver = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;//deactivate the collider
            charImg.sortingLayerName = "Default";//change the layer
            //start a coroutine
            StartCoroutine(DeactivatePlayer());
        }

        //if jumping and obstacle is detected
        if (jumping && ObstaclePresent() == true)
        {
            dieType = 3;

            //death effect is spawned
            GameObject deathEffect = ObjectPooling.instance.GetDeathEffect();
            deathEffect.transform.position = transform.position;//set its position
            deathEffect.SetActive(true);//activated it
            GameManager.instance.gameOver = true;//game over is true
            GameUI.instance.ActivateAdsGiftPanel(); //gift panel is one
            gameObject.SetActive(false);//deactivate the gameobject
        }

    }

    public Vector3 recordTrans;
    public int dieType = 0;
    public void Revive()
    {
        PlatformSpawner.instance.AllBackStart();
        transform.position = recordTrans;
    }

    public void DieAction()
    {
        if (dieType == 1)
        {
            //game is over
            audioS.PlayOneShot(vars.fallSound);
            GameManager.instance.gameOver = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;//deactivate the collider
            charImg.sortingLayerName = "Default";//change the layer
            //start a coroutine
            StartCoroutine(DeactivatePlayer());
        }
        else if (dieType == 2)
        {
            //game is over
            audioS.PlayOneShot(vars.fallSound);
            GameManager.instance.gameOver = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;//deactivate the collider
            charImg.sortingLayerName = "Default";//change the layer
            //start a coroutine
            StartCoroutine(DeactivatePlayer());
        }
        else if (dieType == 3)
        {
            //death effect is spawned
            GameObject deathEffect = ObjectPooling.instance.GetDeathEffect();
            deathEffect.transform.position = transform.position;//set its position
            deathEffect.SetActive(true);//activated it
            GameManager.instance.gameOver = true;//game over is true
            GameUI.instance.ActivateAdsGiftPanel(); //gift panel is one
            gameObject.SetActive(false);//deactivate the gameobject
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {   //detect the pickup
        if (other.gameObject.CompareTag("PickUp"))
        {   //deactivate the pickup
            audioS.PlayOneShot(vars.diamondSound);
            other.gameObject.SetActive(false);
            GameManager.instance.points++;//increase the points
            GameManager.instance.Save();//save it
            GameManager.instance.currentPoints++;//increase the current points
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {   //track the platform
        if (other.CompareTag("Platform"))
        {
            jumping = false;//set to false
            currentPlatform = other.gameObject.transform.position;//get platform position
            curPlateformObj = other.gameObject;
            //decide the next platform position on both left and right
            nextPlatformLeft = new Vector3(currentPlatform.x - offseX, currentPlatform.y + offsetY, 0f);
            nextPlatformRight = new Vector3(currentPlatform.x + offseX, currentPlatform.y + offsetY, 0f);
            //other.gameObject.GetComponent<PlatformScript>().StartTimer = true;
            recordTrans=transform.position;
            if (GameUI.instance.GameStarted && startedMoving)
                GameManager.instance.currentScore++;
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Platform"))
    //    {
    //        jumping = false;//set to false
    //        currentPlatform = collision.gameObject.transform.position;//get platform position
    //        curPlateformObj = collision.gameObject;
    //        //other.gameObject.GetComponent<PlatformScript>().StartTimer = true;
    //        recordTrans = transform.position;
    //    }
    //}
    /// <summary>
    /// Method which make the player jump
    /// </summary>
    void Jump()
    {
        if (jumping)
        {   //if moving left
            if (moveLeft) //left
            {   //set the scale
                transform.localScale = new Vector3(-1, 1, 1);
                transform.DOMoveX(nextPlatformLeft.x, 0.2f); //lerp x value
                transform.DOMoveY(nextPlatformLeft.y + 0.85f, 0.1f); //lerp y value
            }
            else if (!moveLeft) //right
            {
                transform.localScale = new Vector3(1, 1, 1);
                transform.DOMoveX (nextPlatformRight.x, 0.2f);
                transform.DOMoveY(nextPlatformRight.y + 0.85f, 0.1f);
            }
        }
    }

    bool PlatformPresent()
    {
        //create the raycast and provide few config like position , size layer etc.
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);
        Debug.DrawRay(rayDown.position, Vector2.down * 1f, Color.red); //to show on the game scene

        //here we check if the ray of the robot is detecting ground
        if (hit.collider != null)
        {
            //check which wall its has collided
            if (hit.collider.tag == "Platform")
            {
                return true; //if hitting platform we return true
            }
        }

        return false;// else retune false
    }

    bool ObstaclePresent()
    {
        //create the raycast and provide few config like position , size layer etc.
        RaycastHit2D hit1 = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        Debug.DrawRay(rayLeft.position, Vector2.left * 0.25f, Color.red); //to show on the game scene

        RaycastHit2D hit2 = Physics2D.Raycast(rayRight.position, Vector2.right, 0.15f, obstacleLayer);
        Debug.DrawRay(rayRight.position, Vector2.right * 0.25f, Color.red); //to show on the game scene

        //here we check if the ray of the robot is detecting ground
        if (hit1.collider != null)
        {
            //check which wall its has collided
            if (hit1.collider.tag == "Obstacle")
            {
                return true; //if hitting platform we return true
            }
        }

        //here we check if the ray of the robot is detecting ground
        if (hit2.collider != null)
        {
            //check which wall its has collided
            if (hit2.collider.tag == "Obstacle")
            {
                return true; //if hitting platform we return true
            }
        }

        return false;// else retune false
    }

    IEnumerator DeactivatePlayer()
    {
        yield return new WaitForSeconds(1f);
        GameUI.instance.ActivateAdsGiftPanel();//activate the gift panel
        gameObject.SetActive(false);
    }


}
