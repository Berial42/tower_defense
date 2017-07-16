using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIControllerScript : MonoBehaviour
{

    private GameManagerScript gameManagerScript;

    // UI segments
    public GameObject mainMenu;
    public GameObject gameOverMenu;
    public GameObject playerHud;
    public GameObject buildMenu;
    private Image[] enemyHealth;
    //health bar
    public Image healthBar;
    //text
    public Text waveCounter;
    public Text stagetText;
    public Text currencyText;
    //buttons
    public Button endPreparation;
    public Button playButton;
    public Button mainMenuButton;
    public Button closeBuildMenuBtn;
    public Button buildTurretBtn;
    // timer values
    private int remainingSeconds;

    // Use this for initialization
    void Start()
    {
        // get game manager
        gameManagerScript = FindObjectOfType<GameManagerScript>();

        // assign listeners to the buttons
        endPreparation.onClick.AddListener(delegate { EndPreparation(); });
        mainMenuButton.onClick.AddListener(delegate { MainMenu(); });
        playButton.onClick.AddListener(delegate { StartPlay(); });
        closeBuildMenuBtn.onClick.AddListener(delegate { CloseBuildMenu(); });
        buildTurretBtn.onClick.AddListener(delegate { BuildTower(); });

        // activates main menu
        mainMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerScript.wavePhase == GameManagerScript.EWavePhase.EMainMenu)
        {
            mainMenu.SetActive(true);
        }
        else if (gameManagerScript.wavePhase == GameManagerScript.EWavePhase.EPreparation)
        {
            playerHud.SetActive(true);
            endPreparation.enabled = true;
        }
        else if (gameManagerScript.wavePhase == GameManagerScript.EWavePhase.ECombat)
        {
            EnemyHPBar();
        }
    }

    public void MainMenu()
    {
        gameManagerScript.wavePhase = GameManagerScript.EWavePhase.EMainMenu;
        gameOverMenu.SetActive(false);
        mainMenu.SetActive(true);
        ResertEnemyHPBars();
        gameManagerScript.ResetGame();        
    }

    // starts preparation timer
    public void PreparationPhase()
    {
        InvokeRepeating("PreparationTimer", 1f, 1f);
        remainingSeconds = 10;
        endPreparation.gameObject.SetActive(true);
        ResertEnemyHPBars();
    }

    // checks remaining time and after that begins combat phase
    private void PreparationTimer()
    {
        if (remainingSeconds > 0)
        {
            stagetText.text = "Preparation\n" + remainingSeconds.ToString();
            remainingSeconds -= 1;
        }
        else
        {
            CancelInvoke();
            stagetText.text = "";
            gameManagerScript.wavePhase = GameManagerScript.EWavePhase.ECombat;
            endPreparation.gameObject.SetActive(false);
        }
    }

    // updates player HUD during preparation and combat phase
    public void UpdatePlayerHUD(float playerHP, float currency, int waveCount)
    {
        healthBar.fillAmount = playerHP;
        currencyText.text = "$ " + currency;
        waveCounter.text = "Wave " + waveCount.ToString();
    }

    // starts game from main menu
    public void StartPlay()
    {
        gameManagerScript.wavePhase = GameManagerScript.EWavePhase.EPreparation;
        PreparationPhase();
        mainMenu.SetActive(false);
    }

    // ends preparation phase, cancels timers
    private void EndPreparation()
    {
        CancelInvoke();
        stagetText.text = "";
        gameManagerScript.wavePhase = GameManagerScript.EWavePhase.ECombat;
        endPreparation.gameObject.SetActive(false);
        buildMenu.SetActive(false);
    }
    // activates game over screen
    public void ShowGameOverScreen()
    {
        ResertEnemyHPBars();
        gameOverMenu.SetActive(true);
        playerHud.SetActive(false);
    }
    // opens build menu
    public void OpenBuildMenu()
    {
        buildMenu.SetActive(true);
    }
    // closes build menu
    public void CloseBuildMenu()
    {
        gameManagerScript.playerCtrl.selectedTurret = null;
        buildMenu.SetActive(false);
    }
    // calls build function for the selected turret in the player controller
    public void BuildTower()
    {
        gameManagerScript.playerCtrl.selectedTurret.BuildTurret();
    }
    // sets enemy hp bar
    void EnemyHPBar()
    {
        if (gameManagerScript.spawnedEnemyList.Count > 0)
        {
            for (int i = 0; i < gameManagerScript.spawnedEnemyList.Count; i++)
            {
                if (gameManagerScript.spawnedEnemyList[i].gameObject.activeInHierarchy)
                {
                    Vector3 viewPos = FindObjectOfType<Camera>().WorldToViewportPoint(gameManagerScript.spawnedEnemyList[i].transform.position);
                    if (viewPos.x > 0 && viewPos.y > 0)
                    {
                        gameManagerScript.pooler.enemyHealthList[i].enabled = true;
                        gameManagerScript.pooler.enemyHealthList[i].rectTransform.position = gameManagerScript.playerCtrl.activeCamera.
                                                                                                WorldToScreenPoint(gameManagerScript.spawnedEnemyList[i].transform.position +
                                                                                                new Vector3(0,6,0));
                        gameManagerScript.pooler.enemyHealthList[i].fillAmount = gameManagerScript.spawnedEnemyList[i].currentHP / gameManagerScript.spawnedEnemyList[i].maxHP;
                    }
                    else
                    {
                        gameManagerScript.pooler.enemyHealthList[i].enabled = false;
                    }
                }
                else
                {
                    gameManagerScript.pooler.enemyHealthList[i].enabled = false;
                }
            }
        }
    }
    // disables enemy bars
    void ResertEnemyHPBars()
    {
        for (int i = 0; i < gameManagerScript.pooler.enemyHealthList.Count; i++)
        {
            gameManagerScript.pooler.enemyHealthList[i].enabled = false;
        }
    }
}
