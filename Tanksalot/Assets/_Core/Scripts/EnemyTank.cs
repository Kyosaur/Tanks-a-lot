
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Add: Respawn option, networking,
public enum ENEMY_AI_TYPE
{
    PASSIVE = 0,
    AGGRO = 1,
    AGGRO_ON_DAMAGE = 2,
    AGGRO_ON_SIGHT = 3
}

public class EnemyTank : Damagable
{

    [Space(15)]
    [Header("--Tank Data--")]
    [Space(10)]
    public Color m_Color = Color.white;
    [Space(5)]

    public GameObject m_TurretBone;
    public Transform m_GunEnd;
    public Rigidbody m_Projectile;
    private Renderer[] m_Renderer;


    [Space(5)]
    public float m_Speed = 10f;
    public float m_RotateSpeed = 90f;

    public float m_ProjectileSpeed = 80f;
    public float m_FireDelayTime = 700;
    private bool m_FireDelay = false;

    private NavMeshAgent m_NavAgent;

    [Space(15)]
    [Header("--Enemy AI Control--")]
    [Space(10)]

    public ENEMY_AI_TYPE m_EnemyType;


    [Range(0.01f, 1.0f)]
    public float m_FireAccuracy = 0.75f;

    public float m_EnemySightDistance = 25f;
    public float m_StoppingDistance = 10.0f;

    public Transform[] m_Path;
    private int m_PathIdx = 0;
    private bool m_PathComplete = false;

    private Rigidbody m_Rigidbody;
    private Animator m_Animator;

    private GameObject m_Target;
    private Vector3 m_RandomDirection;


    // Use this for initialization
    new void Start()
    {
        base.Start(); 

        m_NavAgent = GetComponentInChildren<NavMeshAgent>();

        m_Rigidbody = GetComponentInChildren<Rigidbody>();
        m_Animator = GetComponentInChildren<Animator>();
        m_Renderer = GetComponentsInChildren<Renderer>();

        if (m_Renderer.Length > 0)
        {
            foreach (Renderer r in m_Renderer)
            {
                r.material.color = m_Color;
            }
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log("Navmesh info -- Has Path: " + m_NavAgent.hasPath + " PathPending: " + m_NavAgent.pathPending + " Going to: " + m_NavAgent.destination + " Velocity: " + m_NavAgent.velocity + " On Mesh: " + m_NavAgent.isOnNavMesh + " Is stopped: " + m_NavAgent.isStopped);

        if (m_EnemyType == ENEMY_AI_TYPE.AGGRO)
        {
            m_Target = FindNearestPlayer();

            if (m_Target != null)
            {
                if (IsInView(m_Target))
                {
                    LookAt(m_Target.transform.position);
                    Fire();
                }

                MoveTowards(m_Target.transform.position, m_StoppingDistance);
            }       
        }

        if (m_EnemyType == ENEMY_AI_TYPE.AGGRO_ON_SIGHT)
        {
            m_Target = FindNearestPlayer();

            if (m_Target != null)
            {
                if (Vector3.Distance(m_Target.transform.position, this.transform.position) <= m_EnemySightDistance)
                {
                    if (IsInView(m_Target))
                    {
                        LookAt(m_Target.transform.position);
                        Fire();
                    }

                    MoveTowards(m_Target.transform.position, m_StoppingDistance);
                }
                else FollowPresetPath();
            }
        }

        if(m_EnemyType == ENEMY_AI_TYPE.PASSIVE)
        {
            FollowPresetPath();
        }

        m_Animator.SetFloat("Move", (m_NavAgent.velocity.magnitude != 0) ? 1 : 0);


    }

    new void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }



    public bool IsInView(GameObject target)
    {
        if (Vector3.Distance(target.transform.position, this.transform.position) <= m_EnemySightDistance)
        {
            Debug.DrawLine(this.transform.position, target.transform.position, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, target.transform.position - this.transform.position, out hit, m_EnemySightDistance))
            {
                if (hit.collider.name == target.name)
                    return true;
            }
        }
        return false;
    }

    public GameObject FindNearestPlayer()
    {
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest_player = null;

        float closest_distance = float.MaxValue;

        foreach (GameObject target in player)
        {
            float distance = System.Math.Abs((target.transform.position.x + target.transform.position.y) - (this.transform.position.x + this.transform.position.y));

            if (distance < closest_distance)
            {
                closest_player = target;
                closest_distance = distance;
            }
        }
        return closest_player;
    }


    private void FollowPresetPath()
    {
        if (!(m_Path.Length > 0)) return;
        if (!HasReachedDestination()) return;

        
        if (m_PathIdx < 0)
        {
            m_PathIdx = 0;
            m_PathComplete = false;
        }

        if (m_PathIdx > m_Path.Length - 1)
        {
            m_PathIdx -= 2;
            m_PathComplete = true;
        }

        if (!m_PathComplete && m_PathIdx <= m_Path.Length - 1)
        {
            MoveTowards(m_Path[m_PathIdx].position, 0f);
            m_PathIdx++;
        }

        else if (m_PathComplete && m_PathIdx >= 0)
        {
            MoveTowards(m_Path[m_PathIdx].position, 0f);
            m_PathIdx--;
        }
        
        
    }


    private bool MoveTowards(Vector3 location, float stopDistance)
    {
        NavMeshHit pos;
        if (NavMesh.SamplePosition(location, out pos, 3.0f, NavMesh.AllAreas))
        {
            m_NavAgent.autoRepath = true;

            m_NavAgent.speed = m_Speed;
            m_NavAgent.angularSpeed = m_RotateSpeed;
            m_NavAgent.stoppingDistance = stopDistance;

            
            Debug.DrawRay(pos.position, transform.TransformDirection(Vector3.up) * 100, Color.yellow, 15F);

            m_NavAgent.SetDestination(pos.position);

            return true;
        }
        return false;
    }

    private bool HasReachedDestination()
    {
        //if (Vector3.Distance(m_Path[m_PathIdx].position, m_NavAgent.transform.position) <= m_NavAgent.stoppingDistance)
        if(m_NavAgent.remainingDistance <= m_NavAgent.stoppingDistance)
        {
            if (!m_NavAgent.hasPath || m_NavAgent.velocity.sqrMagnitude == 0f)
            {
                Debug.Log("destination reached");
                return true;
            }
        }
        return false;
    }

    private void LookAt(Vector3 target)
    {
        float random = Random.value;
        Vector3 random_direction = new Vector3(0, 0, 0);

        if (!(random <= m_FireAccuracy))
        {
            random_direction.x = Random.Range(-7, 7);
            random_direction.z = Random.Range(-7, 7);
        }

        if (!m_FireDelay) m_RandomDirection = target + random_direction - m_TurretBone.transform.position;

        Vector3 newDir = Vector3.RotateTowards(m_TurretBone.transform.forward, m_RandomDirection, 1f, 0);
        Quaternion ntarget = Quaternion.LookRotation(newDir);

        m_TurretBone.transform.rotation = Quaternion.Euler(-90, ntarget.eulerAngles.y, 0);
    }

    private void Fire()
    {
        //clean later
        if (!m_FireDelay)
        {
            var rot = m_TurretBone.transform.rotation * Quaternion.Euler(180, 0, 0);


            Rigidbody shellInstance = Instantiate(m_Projectile, m_GunEnd.position, rot) as Rigidbody;
            shellInstance.velocity = m_ProjectileSpeed * -m_TurretBone.transform.up;

            m_FireDelay = true;
            Invoke("ShootDelayReset", m_FireDelayTime / 1000);
        }

    }


    private void ShootDelayReset()
    {
        m_FireDelay = false;
    }


    public void SetAllCollidersStatus(bool active)
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = active;
        }
    }

    public override void DestroyObject()
    {
        m_NavAgent.enabled = false;
        this.enabled = false;

        m_Animator.SetInteger("Dead", 1);

        m_Rigidbody.isKinematic = true;
        SetAllCollidersStatus(false);

        HideHealthbars();
        Invoke("FadeOut", 1.5f);

    }

    public void FadeOut()
    {
        m_Rigidbody.isKinematic = false;
        Destroy(transform.root.gameObject, 1f);
    }
}
