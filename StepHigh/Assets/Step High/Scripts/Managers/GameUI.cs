using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using StarkSDKSpace;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;

public class GameUI : MonoBehaviour
{

    public static GameUI instance;

    public GameObject mainMenu, gameOverMenu, gamePanel, removeAdsBtn, adsAndGiftPanel;
    public UIObjects UIO; //ref to the UIObject

    private bool gameStarted = false;
    private int i = 0; //this is to make game over coroutine fun only once
    public GameObject AddStaminaPanel;
    private StarkAdManager starkAdManager;

    public string clickid;
    public bool GameStarted
    {
        get { return gameStarted; }
        set { gameStarted = value; }
    }

    [HideInInspector]
    public managerVars vars;

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
    {
        #if AdmobDef
        if (GameManager.instance.canShowAds) AdmobAdsScript.instance.ShowBannerAds();
        else if (!GameManager.instance.canShowAds) AdmobAdsScript.instance.DestroyBannerAds();
        #endif
        gameStarted = false;
        GameManager.instance.gameOver = false;
        GameManager.instance.currentScore = 0;
        GameManager.instance.currentPoints = 0;
        //sound button
        if (GameManager.instance.isMusicOn == true)
        {
            AudioListener.volume = 1;
            UIO.mainMenuUI.soundImage.sprite = vars.soundOffButton;
        }
        else
        {
            AudioListener.volume = 0;
            UIO.mainMenuUI.soundImage.sprite = vars.soundOnButton;
        }

        if (GameManager.instance.gameRestart)
        {
            GameManager.instance.gameRestart = false;
            PlayBtn();
        }

        UIO.gameMenuUI.scoreText.text = "" + GameManager.instance.currentScore;
        UIO.gameMenuUI.diamondText.text = "" + GameManager.instance.currentPoints;
    } 

    void Update()
    {
        if (GameManager.instance.gameOver == false && gameStarted == true)
        {
            UIO.gameMenuUI.scoreText.text = "" + GameManager.instance.currentScore;
            UIO.gameMenuUI.diamondText.text = "" + GameManager.instance.currentPoints;
        }

        if (GameManager.instance.gameOver && i == 0)
        {
            i++;
            StartCoroutine(GameOver());
            ShowInterstitialAd("1lcaf5895d5l1293dc",
            () => {
                Debug.Log("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
        }

        if (GameManager.instance.canShowAds)
        {
            //removeAdsBtn.SetActive(true);
        }
        else
        {
            removeAdsBtn.SetActive(false);
        }

    }

#region MainMenu
    public void PlayBtn()
    {
        if(StaminaTimer.currentStamina>=1)
        {
            UIObjects.instance.ButtonPress();
            gamePanel.SetActive(true);
            mainMenu.SetActive(false);
            gameStarted = true;
            StaminaTimer.subtract(1);
        }
        else
        {
            AddStaminaPanel.SetActive(true);
        }
        
    }

    public void LeaderboardBtn()
    {
        UIObjects.instance.ButtonPress();
#if UNITY_ANDROID
        GooglePlayManager.singleton.OpenLeaderboardsScore();
#elif UNITY_IOS
        LeaderboardiOSManager.instance.ShowLeaderboard();
#endif
    }

    public void RemoveAdsBtn()
    {
        UIObjects.instance.ButtonPress();
        //uncomment below line after adding Unity IAP
        //Purchaser.instance.BuyNoAds();
    }

    public void SoundBtn()
    {
        if (GameManager.instance.isMusicOn == true)
        {
            GameManager.instance.isMusicOn = false;
            AudioListener.volume = 0;
            UIO.mainMenuUI.soundImage.sprite = vars.soundOnButton;
            GameManager.instance.Save();
        }
        else
        {
            GameManager.instance.isMusicOn = true;
            AudioListener.volume = 1;
            UIO.mainMenuUI.soundImage.sprite = vars.soundOffButton;
            GameManager.instance.Save();
        }

        UIObjects.instance.ButtonPress();
    }
#endregion

#region GameOver Menu

    public void ShareBtn()
    {
        UIObjects.instance.ButtonPress();
        ShareScreenShot.instance.ButtonShare();
    }

    public void HomeBtn()
    {
        UIObjects.instance.ButtonPress();
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void RateBtn()
    {
        UIObjects.instance.ButtonPress();
        Application.OpenURL(vars.rateButtonUrl);
    }

    public void RestartBtn()
    {
        UIObjects.instance.ButtonPress();
        GameManager.instance.gameRestart = true;
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void AdsBtn()
    {
        UIObjects.instance.ButtonPress();
        //call the reward ads methode here
        UnityAds.instance.ShowRewardedAd();
        AdsTimeTracker.instance.TrackTime();  //start the timer
        UIObjects.instance.gameOverMenuUI.adsBtn.interactable = false; //make button interactable false
    }

    public void GiftBtn()
    {
        UIObjects.instance.ButtonPress();
        GameManager.instance.points += 20;      //increase the coins
        GameManager.instance.Save();           //save it
        GiftTimeTracker.instance.TrackTime();  //start the timer
        UIObjects.instance.gameOverMenuUI.giftBtn.interactable = false; //make button interactable false
    }

    public void CloseGiftAdsPanel()
    {
        UIObjects.instance.ButtonPress();
        adsAndGiftPanel.SetActive(false);
    }

#endregion

    private IEnumerator GameOver()
    {
        if (GameManager.instance.currentScore > GameManager.instance.bestScore)
        {
            GameManager.instance.bestScore = GameManager.instance.currentScore;
            GameManager.instance.Save();
        }

        GameManager.instance.lastScore = GameManager.instance.currentScore;

        UIO.gameOverMenuUI.gameOverScoreText.text = "" + GameManager.instance.currentScore;
        UIO.gameOverMenuUI.gameOverBestScoreText.text = "" + GameManager.instance.bestScore;
        UIO.gameOverMenuUI.gameOverDiamondText.text = "+" + GameManager.instance.currentPoints;
        UIO.shopMenuUI.diamondText.text = "" + GameManager.instance.points;

        yield return new WaitForSeconds(1.5f);
        gameOverMenu.SetActive(true);
    }

    public void ActivateAdsGiftPanel()
    {
        if (GameManager.instance.gameOver)
        {
            if (AdsTimeTracker.instance.adsReady || GiftTimeTracker.instance.giftReady)
            {
                //adsAndGiftPanel.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }

}
