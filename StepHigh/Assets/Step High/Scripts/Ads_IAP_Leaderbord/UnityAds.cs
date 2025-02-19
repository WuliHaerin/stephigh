using UnityEngine;
using System.Collections;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class UnityAds : MonoBehaviour
{

    public static UnityAds instance;

    private int i = 0;

    [HideInInspector]
    public managerVars vars;

    void OnEnable()
    {
        vars = Resources.Load("managerVarsContainer") as managerVars;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Use this for initialization
    void Start()
    {
        i = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance == null)
            return;

        if (GameManager.instance.gameOver == true)
        {
            //we want only one ad to be shown so we put condition that when i is 0 we show ad.
            if (i == 0)
            {
                i++;
                GameManager.instance.gamesPlayed++;

                if (GameManager.instance.gamesPlayed >= vars.showInterstitialAfter && GameManager.instance.canShowAds == true)
                {
                    GameManager.instance.gamesPlayed = 0;
                    //use any one of them
                    //admob ads
#if AdmobDef
                    AdmobAdsScript.instance.ShowInterstitial();
#endif
                    //unity ads
#if UNITY_ADS
                    if(!vars.admobActive)
                        ShowAd();
#endif
                }
            }
        }
    }

    public void ShowAd()
    {
#if UNITY_ADS
            if (Advertisement.IsReady())
            {
                Advertisement.Show();
            }
#endif
    }

    //use this function for showing reward ads
    public void ShowRewardedAd()
    {
#if UNITY_ADS
            if (Advertisement.IsReady("rewardedVideo"))
            {
                var options = new ShowOptions { resultCallback = HandleShowResult };
                Advertisement.Show("rewardedVideo", options);
            }
            else
            {
                Debug.Log("Ads not ready");
            }
#endif
    }

#if UNITY_ADS
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");

    /*here we give 50 poinst as reward*/
                GameManager.instance.points += 5;
                GameManager.instance.Save();

                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");

                break;
        }
    }
#endif

}
