using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    // game object references
    public PlayerControllerScript playerCtrl;  // player controller
    private UIControllerScript uiCtrl;         // UI controller
    public ObjectPoolerScript pooler;       // object pooling script
    // time length of the wave
    public float waveLength;
    // wave count
    private int waveNumber;
    // spawn delay during the wave
    public float spawnDelay;
    private float lastSpawnTime;
    //spawn point
    public Transform spawnPoint;
    // number of enemies to spawn
    public int enemySpawnCount;
    public int waveSpawnCount;
    public List<EnemyControllerScript> spawnedEnemyList;     //list of spawned enemies
    // phase the wave is in
    public enum EWavePhase
    {
        EMainMenu,
        EPreparation,
        ECombat,
        EEndWave,
        EGameOver
    }
    public EWavePhase wavePhase;

    // enemy movement points
    public Transform[] checkpoints;

    // earned currency
    public float earnedCurrency;

	// Use this for initialization
	void Start ()
    {
        pooler = GameObject.FindObjectOfType<ObjectPoolerScript>();
        playerCtrl = FindObjectOfType<PlayerControllerScript>();
        uiCtrl = FindObjectOfType<UIControllerScript>();
        spawnedEnemyList = new List<EnemyControllerScript>();
        earnedCurrency = 200f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // behavior based on state
        if (wavePhase == EWavePhase.ECombat)
        {
            // gets enemies from the pool and activates them
            if (spawnedEnemyList.Count <= waveSpawnCount + waveNumber)
            {
                if (lastSpawnTime + spawnDelay < Time.time)
                {
                    SpawnEnemyFromPool();
                    enemySpawnCount += 1;
                    lastSpawnTime = Time.time;
                }
            }
            // ends wave if there is no active enemies
            if (enemySpawnCount == 0)
            {
                wavePhase = EWavePhase.EEndWave;
            }
            UpdateHUD();

            // ends game if the player health is 0
            if (playerCtrl.playerHealth == 0)
            {
                EndGameFunct();
            }
        }
        else if (wavePhase == EWavePhase.EPreparation)
        {
            //checks if the player has selected turret build plot and activates build menu accordingly
            if (playerCtrl.selectedTurret != null)
            {
                if (playerCtrl.selectedTurret.TurretPhase == TurretScript.ETurretPhase.EPlot)
                {
                    uiCtrl.OpenBuildMenu();
                }
                else
                {
                    uiCtrl.CloseBuildMenu();
                }
            }
            else
            {
                if (uiCtrl.buildMenu.activeInHierarchy)
                {
                    uiCtrl.CloseBuildMenu();
                }
            }
            UpdateHUD();
        }
        else if (wavePhase == EWavePhase.EEndWave)
        {
             // check if the wave count is above 20 and ends game or
             // starts another wave
            if (waveNumber < 20)
            {
                spawnedEnemyList.Clear();
                waveNumber += 1;
                UpdateHUD();
                wavePhase = EWavePhase.EPreparation;
                uiCtrl.PreparationPhase();
            }
            else
            {
                EndGameFunct();
            }
        }
	}
    // updates players health bar, currency counter and wave counter
    private void UpdateHUD()
    {
        float playerHPPercent = playerCtrl.playerHealth / playerCtrl.playerMaxHealth;
        uiCtrl.UpdatePlayerHUD(playerHPPercent, earnedCurrency, waveNumber);
    }
    // gets inactive enemy from pool, activates it, sets its position and
    // based on the random number sets if its range or melee
    private void SpawnEnemyFromPool()
    {
        GameObject enemyPref = pooler.ReturnEnemy();
        enemyPref.transform.position = spawnPoint.position;
        spawnedEnemyList.Add(enemyPref.GetComponent<EnemyControllerScript>());

        int rnd = Random.Range(0, 2);
        switch (rnd)
        {
            case 0:
                enemyPref.GetComponent<EnemyControllerScript>().EnemyType = EnemyControllerScript.EEnemyType.EMelee;

                break;

            case 1:
                enemyPref.GetComponent<EnemyControllerScript>().EnemyType = EnemyControllerScript.EEnemyType.ERange;

                break;
        }
        enemyPref.SetActive(true);
    }

    // ends game and opens game over screen
    public void EndGameFunct()
    {
        foreach (EnemyControllerScript e in spawnedEnemyList)
        {
            e.gameObject.SetActive(false);
        }
        wavePhase = EWavePhase.EGameOver;
        uiCtrl.ShowGameOverScreen();
    }

    public void ResetGame()
    {
        playerCtrl.playerHealth = playerCtrl.playerMaxHealth;
        earnedCurrency = 200f;
        spawnedEnemyList.Clear();
        enemySpawnCount = 0;
        waveNumber = 0;
        TurretScript[] turrets = FindObjectsOfType<TurretScript>();
        foreach (TurretScript t in turrets)
        {
            t.ResetTurret();
        }
    }
}
