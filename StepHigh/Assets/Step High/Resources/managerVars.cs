using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class shopItemData{
	public string characterName;
	public Sprite gameCharacterSprite;
    public Sprite shopCharacterSprite;
    public int characterPrice;
}

[System.Serializable]
public class BGthemeData{
    public Texture backgroundTexture;
    public Sprite backgroundSprite;                                                     //*******Changed
}

[System.Serializable]
public class platformData
{
    public Sprite platformSprite;
    public string platformName;
}

public class managerVars : ScriptableObject {

    [SerializeField]
	public List<shopItemData> characters = new List<shopItemData>();

    [SerializeField]
	public List<BGthemeData> bgThemes = new List<BGthemeData>();

    [SerializeField]
    public List<platformData> platforms = new List<platformData>();

    [SerializeField]
    public Sprite soundOnButton, soundOffButton, leaderboardButton, shopButton, playButton,
        homeButton, rateButton, backButton, diamondImg, mainMenuBG, titleImage, noAdsImage, shopMenuBG, shopCloseImage,
        gameOverMenuBG, shareImage;

    [SerializeField]
    public Color32 gameOverScoreTextColor, gameOverBestScoreTextColor, inGameScoreTextColor, shopMenuDIamondTextColor,
        gameMenuDIamondTextColor, gameOverDIamondTextColor;

    [SerializeField]
	public Font mainFont, secondFont;

    [SerializeField]
    public AudioClip buttonSound, diamondSound, jumpSound, fallSound , deathSound;

    // Standart Vars
    [SerializeField]
    public string adMobInterstitialID, adMobBannerID, rateButtonUrl, leaderBoardID, admobAppID;
	[SerializeField]
	public int showInterstitialAfter, bannerAdPoisiton;
    [SerializeField]
    public bool admobActive, googlePlayActive;
}
