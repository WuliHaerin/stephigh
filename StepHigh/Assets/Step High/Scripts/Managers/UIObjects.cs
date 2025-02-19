using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class MainMenuUI
{
    public Image mainMenuBG, titleImage, playImage, leaderboardBtnImage, noAdsImage, soundImage;
}

[System.Serializable]
public class ShopMenuUI
{
    public Image shopMenuBG, diamondImage, shopCloseImage;
    public Text diamondText;
}

[System.Serializable]
public class GameMenuUI
{
    public Image diamondImage;
    public Text diamondText, scoreText;
}

[System.Serializable]
public class GameOverMenuUI
{
    public Image gameOverMenuBG, gameOverDiamondImage, shareImage, shopBtnImage, leaderboardBtnImage, homeImage, rateImage;
    public Text gameOverDiamondText, gameOverScoreText,  adsTimer, giftTimer;
    public TextMeshProUGUI gameOverBestScoreText;
    public Button adsBtn, giftBtn;
}

public class UIObjects : MonoBehaviour
{

    public static UIObjects instance;
    private AudioSource audioS;
    private AudioClip buttonClick;

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
        audioS = GetComponent<AudioSource>();       
    }

    void Start()
    {
        buttonClick = vars.buttonSound;
    }

    public MainMenuUI mainMenuUI;
    public ShopMenuUI shopMenuUI;
    public GameMenuUI gameMenuUI;
    public GameOverMenuUI gameOverMenuUI;
	public Text[] mainFont, secondFont;

    public void ButtonPress()
    {
        audioS.PlayOneShot(buttonClick);
    }

}
