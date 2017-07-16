using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPoolerScript : MonoBehaviour {

    // objects to pool
    // enemies
    public GameObject enemy;

    // projectiles
    public GameObject projectile;

    // enemy health bars
    public Image healthImg;
    // size of pool
    public float poolSize;

    // object List
    // enemies
    private List<GameObject> enemiesList;  

    // projectiles
    private List<GameObject> projectileList;

    // health bars
    public List<Image> enemyHealthList;

	// Use this for initialization
	void Start ()
    {
        // create projectile pool
        projectileList = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(projectile);
            proj.SetActive(false);
            projectileList.Add(proj);
        }

        // create enemies pool
        enemiesList = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemyPref = Instantiate(enemy);
            enemyPref.SetActive(false);
            enemiesList.Add(enemyPref);
        }

        // create HP bars pool
        enemyHealthList = new List<Image>();
        for (int i = 0; i < poolSize; i++)
        {
            Image hpBar = Instantiate(healthImg, FindObjectOfType<UIControllerScript>().transform);
            hpBar.enabled = false;
            enemyHealthList.Add(hpBar);
        }
    }
	
    // returns inactive enemy object in the scene
    public GameObject ReturnEnemy()
    {
        for (int i = 0; i < enemiesList.Count; i++)
        {
            if (!enemiesList[i].activeInHierarchy)
            {
                return enemiesList[i];
            }
        }
        return null;
    }
    // returns inactive projectile in the scene
    public GameObject ReturnProjectile()
    {
        for (int i = 0; i < projectileList.Count; i++)
        {
            if (!projectileList[i].activeInHierarchy)
            {
                return projectileList[i];
            }
        }
        return null;
    }
}
