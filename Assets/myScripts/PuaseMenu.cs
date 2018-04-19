using UnityEngine;
using UnityEngine.UI;

public class PuaseMenu : MonoBehaviour {
    public static PuaseMenu pm;
    // Boolean flag to contol pausing and unpausing
    public bool isPaused;

    public bool onUpgrades;
    public bool onHighScores;
    public bool onSplash;

    public GameObject splashScreen;
    public GameObject pauseScreen;
    public GameObject highscoreScreen;
    public GameObject highscoreScreeInput;
    public GameObject upgrades;
    public GameObject gameOverScreen;

    void Awake() {
        pm = this;
    }

    // Use this for initialization
    void Start() {
        //Start the game paused
        Time.timeScale = 0.0f;

        // 
        isPaused = false;
        onUpgrades = false;
        onHighScores= false;

        // start on splash screen
        splashScreen.SetActive(true);
        onSplash = true;
    }

    // Update is called once per frame
    void Update() {
        // escape key toggle pause and unpause
        if (Input.GetKeyDown(KeyCode.Escape) && !onSplash && !onHighScores && !onUpgrades) {
            isPaused = !isPaused;
            Time.timeScale = (isPaused) ? 0.0f : 1.0f;
            pauseScreen.SetActive(isPaused);
        }

        // when an upgrade is chosen, hide window and unpause the game
        if (ProgressionUnlocks.pu.picked) {
            upgrades.SetActive(false);
            Time.timeScale = 1.0f;
            ProgressionUnlocks.pu.picked = false;
        }

        if (PlayerController.pc.isDead) {
            gameOverScreen.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }

    // Function that starts game from splash screen
    public void StartGame() {
        onSplash = false;
        splashScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }

    // Function that displays the highscore screen
    public void DisplayHighScores() {
        ObjectPools.SharedInstance.ResetPools();
        print("CALLED HIGH SCORE FUNCTION");
        onHighScores = !onHighScores;
        highscoreScreen.SetActive(true);
        highscoreScreeInput.SetActive(true);
        Time.timeScale = 0.0f;
    }

    //Function to return to the spalsh screen
    public void GoBackToSplash() {
        LevelGen.lg.levelNum = 0;
        onSplash = true;
        splashScreen.SetActive(true);
        // disable the high score screen
        onHighScores = !onHighScores;
        highscoreScreen.SetActive(false);
        // pause game
        Time.timeScale = 0.0f;
    }

    //Function to return to the spalsh screen
    public void GoToUpgrades() {
        onUpgrades = true;
        upgrades.SetActive(true);

        // pause game
        Time.timeScale = 0.0f;
    }

    public void Restart() {
        print("here");
        LevelGen.lg.levelNum = 0;
        PlayerController.pc.Init();
        ObjectPools.SharedInstance.ResetPools();
        LeviathanController.lc.Init();
        LevelGen.lg.Init();
        ProgressionUnlocks.pu.ResetUnlocks();

        gameOverScreen.SetActive(false);
        splashScreen.SetActive(true);
        onSplash = true;
        // pause game
        Time.timeScale = 0.0f;
    }
}
