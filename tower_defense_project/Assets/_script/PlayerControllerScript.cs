using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

    // game controller
    private GameManagerScript gameCtrl;
    //active camera
    public Camera activeCamera;
    // turret object
    public GameObject turret;

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

    //player health
    public float playerMaxHealth;
    public float playerHealth;

    // selected tower plot
    public TurretScript selectedTurret;

	// Use this for initialization
	void Start ()
    {
        gameCtrl = FindObjectOfType<GameManagerScript>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // check stage of the wave
        if (gameCtrl.wavePhase == GameManagerScript.EWavePhase.ECombat)
        {
            if (Input.GetButton("Fire1"))
            {
                // checks time of the last shot 
                // if the certain amount of time has passed fire the projectile
                if (lastShotTime + fireRate < Time.time)
                {
                    FireGun();
                    lastShotTime = Time.time;
                }
            }
        }
        else if (gameCtrl.wavePhase == GameManagerScript.EWavePhase.EPreparation)
        {
            if (Input.GetButton("Fire1"))
            {
                // if the raycast from mouse is turret
                RaycastHit hit = ReturnHitPoint();
                if (hit.collider.gameObject != null)
                {
                    if (hit.collider.tag == "Turret")
                    {
                        selectedTurret = hit.collider.gameObject.GetComponentInParent<TurretScript>();
                    }
                }
            }
        }
        // rotates turret towards mouse position
        TurretFaceDirection();      
	}

    // gets inactive projectile from the pool, sets its position, set it active and set velocity to its rigidbody
    public void FireGun()
    {
        GameObject proj = gameCtrl.pooler.ReturnProjectile();
        proj.transform.position = spawnTransform.position;
        targetLocation = GetTargetLocation();
        proj.SetActive(true);
        proj.GetComponent<Rigidbody>().velocity = (targetLocation - spawnTransform.position).normalized * projSpeed;
        proj.GetComponent<ProjectileScript>().Side = ProjectileScript.ESide.EPlayer;
    }

    //turret direction based on its target
    private void TurretFaceDirection()
    {
        Vector3 direction = GetTargetLocation() - turret.transform.position;
        direction.y = 0;
        turret.transform.rotation = Quaternion.LookRotation(direction);
    }
    // gets target location from raycast info
    private Vector3 GetTargetLocation()
    {
        Vector3 location = Vector3.zero;

        RaycastHit rayHit = ReturnHitPoint();

        if (rayHit.point != null)
        {
            location = rayHit.point;
            //location.y += 1f;
        }
        return location;
    }
    // gets raycast from input position
    private RaycastHit ReturnHitPoint()
    {
        RaycastHit rayHit;
        Vector3 inputPosition;

        //gets input position based on the platform
#if UNITY_ANDROID
        inputPosition = Input.GetTouch(0).position;
#elif UNITY_STANDALONE_WIN
        inputPosition = Input.mousePosition;
#else
        inputPosition = Input.mousePosition;
#endif

        Ray cameraRay = activeCamera.ScreenPointToRay(inputPosition);
        Physics.Raycast(cameraRay, out rayHit);

        return rayHit;
    }

}
