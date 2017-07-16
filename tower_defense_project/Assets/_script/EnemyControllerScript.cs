using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerScript : MonoBehaviour {

    //rigidbody
    private Rigidbody rb;
    //gun object
    public GameObject gunObj;

    public Material meleeMaterial;
    public Material rangeMaterial;

    // movement speed
    public float speed;
    // movement direction
    private Vector3 moveDir;

    //obstacle layer
    public LayerMask obstaclelayer;

    // enemy type enum
    public enum EEnemyType
    {
        EMelee,
        ERange
    }
    public EEnemyType EnemyType;

    // projectile to fire if enemy is range
    public GameObject projectile;

    // enemy health var
    public float maxHP;
    public float currentHP;

    // stop distance
    public float stopDistance;

    // current move target
    private Vector3 moveToTarget;
    // player reference
    private PlayerControllerScript playerTarget;
    private GameManagerScript gms;
    // move checkpoints
    private List<Vector3> checkpoints;

    // fire rate
    public float attackRate;
    private float attackDistance;
    public float meleeAttackDistance;
    public float rangeAttackDistance;
    private float lastAttackTime;
    public Transform muzzlePos;
    // attack damage
    public float damage;

    // enemy states
    private enum EEnemyState
    {
        EMove,
        EAttack
    }
    private EEnemyState EnemyState;

    // move path index
    private int moveIndex;

    // Use this for initialization and reset
    private void OnEnable()
    {
        playerTarget = FindObjectOfType<PlayerControllerScript>();
        gms = FindObjectOfType<GameManagerScript>();
        rb = GetComponent<Rigidbody>();

        switch (EnemyType)
        {
            case EEnemyType.EMelee:
                attackDistance = meleeAttackDistance;
                MeshRenderer[] meshesM = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer m in meshesM)
                {
                    m.material = meleeMaterial;
                }
                gunObj.SetActive(false);
                break;

            case EEnemyType.ERange:
                attackDistance = rangeAttackDistance;
                MeshRenderer[] meshesR = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer m in meshesR)
                {
                    m.material = rangeMaterial;
                }
                gunObj.SetActive(true);
                break;
        }
        currentHP = maxHP;
        moveIndex = 0;
        moveToTarget = gms.checkpoints[0].position;
    }

    private void OnDisable()
    {
        moveToTarget = Vector3.zero;
    }

    // Update is called once per frame
    void Update ()
    {

        if (CheckDistance(playerTarget.transform.position) <= attackDistance)
        {
            rb.velocity = Vector3.zero;
            Vector3 dir = MovementDirection(playerTarget.transform.position);
            gameObject.transform.rotation = Quaternion.LookRotation(dir);
            if (lastAttackTime + attackRate < Time.time)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            if (CheckDistance(moveToTarget) <= stopDistance)
            {
                moveToTarget = GetNextMoveTarget();
            }
            else
            {
                MoveEnemy(moveToTarget);
            }
        }
	}
    // movement
    // add velocity to the rigidbody based on the forward vector
    public void MoveEnemy(Vector3 target)
    {
        Vector3 dir = MovementDirection(target);
        gameObject.transform.rotation = Quaternion.LookRotation(dir);
        // rotates object by 45 degrees on the up axis if other AI is in front
        if (CheckForObstacle())
        {
            transform.Rotate(Vector3.up * 45f);
        }
        rb.velocity = transform.forward * speed;

    }
    // distance between target and game object
    private float CheckDistance(Vector3 target)
    {
        float dist = Vector3.Distance(transform.position, target);
        return dist;
    }
    // get next move target
    private Vector3 GetNextMoveTarget()
    {
        Vector3 pos;
        if (moveIndex == gms.checkpoints.Length - 1)
        {
            pos = playerTarget.transform.position;
        }
        else
        {
            moveIndex += 1;
            pos = gms.checkpoints[moveIndex].position;
        }
        return pos;
    }
    //calculate movement direction
    private Vector3 MovementDirection(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;
        direction = direction.normalized;
        return direction;
    }
    // sphere cast in front of the character looking for another AI
    private bool CheckForObstacle()
    {
        RaycastHit hit;
        Vector3 dir = transform.forward;

        if (Physics.SphereCast(transform.position + new Vector3(0, 2f, 0), 2f, dir, out hit, 3f, obstaclelayer))
        {
            Debug.DrawLine(transform.position + new Vector3(0, 2f, 0), hit.point, Color.green);
            return true;
        }
        else
        {
            return false;
        }
    }
    // attack based on the type of the enemy
    private void Attack()
    {
        switch (EnemyType)
        {
            // reduces player health if in range
            case EEnemyType.EMelee:
                playerTarget.playerHealth -= damage;
                break;
            // fires projectile if in range
            case EEnemyType.ERange:
                FireProjectile();
                break;
        }
    }
    // gets inactive bullet from the pool, sets its position and set its velocity towards player position
    private void FireProjectile()
    {
        GameObject proj = gms.pooler.ReturnProjectile();
        proj.transform.position = muzzlePos.position;
        Vector3 targetPos = playerTarget.transform.position;
        targetPos.y += 5f;
        proj.SetActive(true);
        proj.GetComponent<Rigidbody>().velocity = (targetPos - muzzlePos.position).normalized * 100;
        proj.GetComponent<ProjectileScript>().Side = ProjectileScript.ESide.EEnemy;
    }
    // called when character is hit
    public void GotHit()
    {
        currentHP -= 1;
        if (currentHP == 0)
        {
            gms.enemySpawnCount -= 1;
            gms.earnedCurrency += 20f;
            gameObject.SetActive(false);
        }
        else { }
    }
}
