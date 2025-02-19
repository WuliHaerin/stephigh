using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpriteType
{
    normalS, fireS, iceS, grassS
}
/// <summary>
/// This script modifies the platform like changing the platform theme , etc
/// </summary>
public class PlatformModifier : MonoBehaviour {

    [SerializeField]
    private SpriteRenderer[] platformImg; //ref to all the object having platform sprite on them
    [SerializeField]
    [Header("Keep it empty if there is no obstacle")]
    private GameObject obstacle; //ref to the obstacle

    private bool reverseObstaclePos = false; //bool to change the position of obstacle from left and right
    private float obstacleXPos; //change obstacle x value
    private SpriteType platformSprite;

    public bool ReverseObstaclePos //getter and setter
    {
        get { return reverseObstaclePos; }
        set { reverseObstaclePos = value; }
    }

    public managerVars vars; //ref to managerVar which store game design

    void OnEnable()
    {
        vars = Resources.Load<managerVars>("managerVarsContainer");//getting the managerVar
    }

    // Use this for initialization
    void Awake ()
    {
        if (obstacle != null) //checking if obstacle field is not empty
            obstacleXPos = obstacle.transform.localPosition.x;//get the local x pos
	}
    //called when the platfrom is spawned
    public void BasicSettings(SpriteType typeOfSprite)
    {
        platformSprite = typeOfSprite;//set the platform sprite
        //depending on platform sprite we set the sprite
        switch (platformSprite)
        {
            case SpriteType.normalS:
                for (int i = 0; i < platformImg.Length; i++)
                {   //here we put vars.platforms[0] , the 0 index sprite in var has normal platform sprite
                    platformImg[i].sprite = vars.platforms[0].platformSprite;
                }
                break;

            case SpriteType.fireS:
                for (int i = 0; i < platformImg.Length; i++)
                {   //here we put vars.platforms[1] , the 1 index sprite in var has fire platform sprite
                    platformImg[i].sprite = vars.platforms[1].platformSprite;
                }
                break;

            case SpriteType.iceS:
                for (int i = 0; i < platformImg.Length; i++)
                {   //here we put vars.platforms[3] , the 3 index sprite in var has grass platform sprite
                    platformImg[i].sprite = vars.platforms[3].platformSprite;
                }
                break;

            case SpriteType.grassS:
                for (int i = 0; i < platformImg.Length; i++)
                {   //here we put vars.platforms[2] , the 2 index sprite in var has grass platform sprite
                    platformImg[i].sprite = vars.platforms[2].platformSprite;
                }
                break;
        }
        //if reverse is true we make obstacle on right side(by default its on left side)
        if (reverseObstaclePos && obstacle != null)
            obstacle.transform.localPosition = new Vector3(-obstacleXPos, obstacle.transform.localPosition.y, 0f);

    }
}
