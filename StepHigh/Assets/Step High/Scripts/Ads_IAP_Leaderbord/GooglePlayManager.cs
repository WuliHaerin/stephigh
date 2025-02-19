using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#if GooglePlayDef
using GooglePlayGames;        
#endif
/// <summary>
/// ***********Uncomment lines after importing google play services*******************
/// </summary>

public class GooglePlayManager : MonoBehaviour
{

    public static GooglePlayManager singleton;

    private AudioSource sound;

    [HideInInspector]
    public managerVars vars;

    void OnEnable()
    {
        vars = Resources.Load<managerVars>("managerVarsContainer");
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void Awake()
    {
        MakeInstance();
    }

    void MakeInstance()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    // Use this for initialization
    void Start()
    {
        sound = GetComponent<AudioSource>();
#if GooglePlayDef
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                InitializeAchievements();
            }
        });
#endif
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        ReportScore(GameManager.instance.currentScore);
    }

    public void OpenLeaderboardsScore()
    {
        Debug.Log("Working");

#if GooglePlayDef
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(vars.leaderBoardID);
        }
#endif
    }

    void ReportScore(int score)
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, vars.leaderBoardID, (bool success) => { });
        }
    }

}
