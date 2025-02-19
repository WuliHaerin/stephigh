using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour {

    public GameObject scrollContent, shopItemPrefab, shopItemName, shopMenu, shopPlay, shopBuy;
    public Button closeShop, shopSelectButton, openShop;
    public Text shopSelectButtonText, dimondsText;
    public ScrollRect shopScroll;
    public string shopState;
    public ScrollRect scroll;
    public int scrollItemWidth, characterIndex, unlockableItemIndex;
    public Image shopBtnImage;
    public managerVars vars;
    public string clickid;
    private StarkAdManager starkAdManager;
    void OnEnable()
    {
        vars = Resources.Load<managerVars>("managerVarsContainer");
    }

    // Use this for initialization
    void Start ()
    {
        closeShop.GetComponent<Button>().onClick.AddListener(() => { CloseShopMenu(); });
        openShop.GetComponent<Button>().onClick.AddListener(() => { OpenShopMenu(); });
        shopSelectButton.GetComponent<Button>().onClick.AddListener(() => { SelectCharacter(); });
        dimondsText.text = "" + GameManager.instance.points;
        shopBtnImage.sprite = vars.characters[GameManager.instance.selectedSkin].shopCharacterSprite;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //current Location
        float curLoc = scroll.content.anchoredPosition.x / scrollItemWidth;
        //location to rach
        float locToReach = Mathf.Floor(curLoc);
        float posBetween = locToReach - curLoc;
        float type62 = posBetween * scrollItemWidth;

        // Update Pos
        if (Input.GetMouseButtonUp(0))
        {
            if (type62 >= -(scrollItemWidth / 2) + 1)
            {
                scroll.content.anchoredPosition = new Vector2(-Mathf.Floor(curLoc) * -scrollItemWidth, 0f);
            }
            else if (type62 <= -(scrollItemWidth / 2))
            {
                scroll.content.anchoredPosition = new Vector2(-Mathf.Ceil(curLoc) * -scrollItemWidth, 0f);
            }
        }

        // Update Index
        if (type62 >= -(scrollItemWidth / 2) + 1)
        {
            characterIndex = Mathf.Abs(Mathf.FloorToInt(curLoc));
        }
        else if (type62 <= -(scrollItemWidth / 2))
        {
            characterIndex = Mathf.Abs(Mathf.CeilToInt(curLoc));
        }
        //check if shop menu is active
        if (shopMenu.activeSelf)
        {   //if yes then we set the position of character images 
            for (int i = 0; i <= scrollContent.transform.childCount - 1; i++)
            {   //we make the selected image size to its full size
                if (i == characterIndex)
                {
                    scrollContent.transform.GetChild(characterIndex).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(160, 160);
                }
                else //and we make the un-selected image size to its half size
                {
                    scrollContent.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(80f, 80f);
                }
            }
            //we then sets the name of character
            shopItemName.GetComponent<TextMeshProUGUI>().text = vars.characters[characterIndex].characterName;
            //set the button 
            if (GameManager.instance.skinUnlocked[characterIndex] == true)
            {
                shopPlay.SetActive(true);
                shopBuy.SetActive(false);
            }
            else if (GameManager.instance.skinUnlocked[characterIndex] == false)
            {
                shopPlay.SetActive(false);
                shopBuy.SetActive(true);
                shopSelectButtonText.text = "" + vars.characters[characterIndex].characterPrice;
            }
        }

    }// Update

    //method called to open the shop
    public void OpenShopMenu()
    {
#if AdmobDef
        AdmobAdsScript.instance.HideBannerAds();
#endif
        UIObjects.instance.ButtonPress();
        shopMenu.SetActive(true);
        UpdateShopItems();
    }

    //method called to close the shop
    public void CloseShopMenu()
    {
        #if AdmobDef
        AdmobAdsScript.instance.ShowBannerAds();
        #endif
        UIObjects.instance.ButtonPress();
        foreach (Transform child in scrollContent.transform)
        {
            Destroy(child.gameObject);
        }

        shopMenu.SetActive(false);
    }

    //method called by select button to select the character
    public void SelectCharacter()
    {
        UIObjects.instance.ButtonPress();
        if (GameManager.instance.skinUnlocked[characterIndex] == true)
        {
            GameManager.instance.selectedSkin = characterIndex;
            GameManager.instance.Save();
            string sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName);
        }
        else if (GameManager.instance.points >= vars.characters[characterIndex].characterPrice)
        {
            GameManager.instance.points -= vars.characters[characterIndex].characterPrice;
            GameManager.instance.skinUnlocked[characterIndex] = true;
            GameManager.instance.selectedSkin = characterIndex;
            GameManager.instance.Save();
            dimondsText.text = "" + GameManager.instance.points;


            //we 1st destroy all the gameobjects
            foreach (Transform child in scrollContent.transform)
            {
                Destroy(child.gameObject);
            }
            //then update the shop
            UpdateShopItems();

        }
        else if (GameManager.instance.points < vars.characters[characterIndex].characterPrice)
        {
            Debug.Log("Buy Coins");
        }
        shopBtnImage.sprite = vars.characters[GameManager.instance.selectedSkin].shopCharacterSprite;
    }
    public void AddCoins()
    {
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    GameManager.instance.points += 10;
                    GameManager.instance.Save();
                    dimondsText.text = "" + GameManager.instance.points;

                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
    }
    //method which controls the movement and scrolling and spawning image prefabs 
    public void UpdateShopItems()
    {   //set the scrollContent parent size
        scrollContent.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2((vars.characters.Count * scrollItemWidth) + 400f, 684f);
        //set the scrollContent size
        scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2((vars.characters.Count * scrollItemWidth) + 400f, 260f);
        //loop through all the characters
        for (int i = 0; i <= (vars.characters.Count - 1); i++)
        {   //spawn the prefab
            GameObject shopItem = Instantiate(shopItemPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
            shopItem.transform.SetParent(scrollContent.transform);//set its parent
            shopItem.transform.localRotation = Quaternion.Euler(0, 0, 0);//set its rotation
            shopItem.transform.localScale = new Vector3(1, 1, 1);//set its scale
            if (i == 0) //the prefas at 0 index
            {   //set its position
                shopItem.transform.localPosition = new Vector3(160f, 0, 0);
            }
            else
            {   //set other next prefabs position
                shopItem.transform.localPosition = new Vector3((scrollItemWidth * i) + 160f, 0, 0);
            }
            //we get the tranform of the object which has Image component on it(image prefab)
            Transform x = shopItem.transform.GetChild(0).GetChild(0);
            if (i == 0)
            {   //set the image value
                x.GetComponent<Image>().sprite = vars.characters[0].shopCharacterSprite;
            }
            else
            {   //set the image value
                x.GetComponent<Image>().sprite = vars.characters[i].shopCharacterSprite;
                if (GameManager.instance.skinUnlocked[i] == false)
                {   //if the characters are unlocked then there color is set
                    x.GetComponent<Image>().color = new Color32(0, 0, 0, 150);
                    //x.GetComponent<Image>().color = new Color32(0, 0, 0, 150);
                }
            }
        }
        //when we open the shop the scroll is set to show the selected object in middle
        shopScroll.content.anchoredPosition = new Vector2(-(GameManager.instance.selectedSkin * scrollItemWidth), 0f);
    }
    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
}//class
