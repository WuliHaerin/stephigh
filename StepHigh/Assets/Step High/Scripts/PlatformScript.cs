using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script which controls the behaviour ofplatform like falling , deactivaating , etc
/// </summary>
public class PlatformScript : MonoBehaviour {

    private float currentTime; //store time after which the platform start falling
    private bool startTimer = false; //decide when to start the falling timer
    private Rigidbody2D myBody; //ref to rigidbody
    private GameObject cameraPos; //ref to camera position
    private Quaternion defaulRotation; //ref to default rotation of platform
    public Vector3 StartPos;

    public bool StartTimer //getter and setter
    {
        get { return startTimer; }
        set { startTimer = value; }
    }

    public Rigidbody2D MyBody //getter and setter
    {
        get { return myBody; }
        set { myBody = value; }
    }

    // Use this for initialization
    void Awake ()
    {   //get the rigidbody
        myBody = GetComponent<Rigidbody2D>();
        defaulRotation = transform.rotation; //stores the default rotation
	}

    void Start()
    {   //get ref to camera position
        cameraPos = GameObject.FindGameObjectWithTag("MainCamera");
    }

	// Update is called once per frame
	void Update ()
    {   
        //if game is not started the return
        if (GameUI.instance.GameStarted == false || !PlayerController.instance.StartedMoving)
            return;

        if (startTimer) //if time is on
        {   //reduce the current time every second
            currentTime -= Time.deltaTime;
            //check if current time is zero and rigidbody is not dynamic
            if (currentTime <= 0 && myBody.bodyType != RigidbodyType2D.Dynamic)
            {   //make body dynamic
                myBody.bodyType = RigidbodyType2D.Dynamic;
                StartCoroutine(Deactivate()); //start deactivation coroutine
            }
        }
        //if the distance between camera and platform is less than -4
        if (transform.position.y - cameraPos.transform.position.y < -4f)
        {   //start deactivation coroutine
            StartCoroutine(Deactivate());
        }
	}

    //basic setting done when platform is spawned
    public void BasicSettings(float timeToFall)
    {   //reset the rotation
        transform.rotation = defaulRotation;
        currentTime = timeToFall;//reset the time
        startTimer = false;   //reset the bool
    }
    public void BackStart(float timeToFall)
    {
        gameObject.SetActive(true);
        transform.position = StartPos;
        transform.rotation = defaulRotation;
        myBody.bodyType = RigidbodyType2D.Static;
        currentTime = timeToFall;
        //startTimer = false;
    }
    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
