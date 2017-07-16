using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour {

    // phase of the turret
    public enum ETurretPhase
    {
        EPlot,
        ETower
    }
    public ETurretPhase TurretPhase;

    // game controller
    private GameManagerScript gameCtrl;
    // turret object
    public GameObject turret;
    // tower part of the turret
    public GameObject tower;
    // projectile object
    public GameObject projectile;
    //projectile speed
    public float projSpeed;
    // fire direction
    public Vector3 fireDir;
    // muzzle transform
    public Transform spawnTransform;
    // target location
    private Vector3 targetLocation;
    // fire rate
    public float fireRate;
    // last shot time
    private float lastShotTime;
    // current target
    public GameObject targetEnemy;
    // enemy layer 
    public LayerMask enemylayer;
    //maximum fire distance of the turret
    public float fireDistance;


    // Use this for initialization
    void Start ()
    {
        gameCtrl = FindObjectOfType<GameManagerScript>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (TurretPhase == ETurretPhase.ETower && gameCtrl.wavePhase == GameManagerScript.EWavePhase.ECombat)
        {
            if (targetEnemy != null && targetEnemy.activeInHierarchy)
            {
                if (CheckDistance(targetEnemy.transform.position) < fireDistance)
                {
                    TurretFaceDirection();
                    if (lastShotTime + fireRate < Time.time)
                    {
                        Fire();
                        lastShotTime = Time.time;
                    }
                }
                else
                {
                    targetEnemy = null; 
                }
            }
            else
            {
                targetEnemy = GetEnemy();               
            }
        }		
	}
    // activates child objects and sets TurretPhase enum
    // removes currency from the game manager
    public void BuildTurret()
    {
        if (gameCtrl.earnedCurrency >= 100)
        {
            tower.SetActive(true);
            TurretPhase = ETurretPhase.ETower;
            gameCtrl.earnedCurrency -= 100;
        }
    }
    //reset turret
    public void ResetTurret()
    {
        tower.SetActive(false);
        TurretPhase = ETurretPhase.EPlot;
        targetEnemy = null;
    }
    // loops through spawned enemies list and looks for active and in range enemy
    private GameObject GetEnemy()
    {
        GameObject target;
        for (int i = 0; i < gameCtrl.spawnedEnemyList.Count; i++)
        {
            target = gameCtrl.spawnedEnemyList[i].gameObject;
            if (target != null)
            {
                if (CheckDistance(gameCtrl.spawnedEnemyList[i].transform.position) < fireDistance)
                {
                    if (target.activeInHierarchy)
                    {
                        return target;
                    }
                }
            }
            
        }
        return null;
    }
    // turns turret towards target
    private void TurretFaceDirection()
    {
        Vector3 direction = targetEnemy.transform.position - turret.transform.position;
        direction.y = 0;
        turret.transform.rotation = Quaternion.LookRotation(direction);
    }
    // gets inactive bullet from the pool, sets its position and set its velocity towards enemy position
    public void Fire()
    {
        GameObject proj = gameCtrl.pooler.ReturnProjectile();
        proj.transform.position = spawnTransform.position;
        targetLocation = targetEnemy.transform.position + new Vector3(0,2,0);

        proj.SetActive(true);
        proj.GetComponent<Rigidbody>().velocity = (targetLocation - spawnTransform.position).normalized * projSpeed;
        proj.GetComponent<ProjectileScript>().Side = ProjectileScript.ESide.EPlayer;
    }
    // distance check between turret and passed position
    private float CheckDistance(Vector3 target)
    {
        float dist = Vector3.Distance(transform.position, target);
        return dist;
    }
}
