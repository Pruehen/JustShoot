using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour, IDamagable
{
    public GameObject hitSFX;
    public EnemyCombat combat = new EnemyCombat();

    public bool isDie = false;
    [SerializeField] protected Player player;
    protected Transform enemyTrf;
    [SerializeField] protected Transform playerTrf;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Statemachine statemachine;
    protected Collider col;
    protected AudioSource audioSource;
    private List<Rigidbody> rigidbodies = new List<Rigidbody>();

    protected readonly int hashTrace = Animator.StringToHash("IsTrace");
    protected readonly int hashAttack = Animator.StringToHash("IsAttack");
    protected readonly int hashHit = Animator.StringToHash("Hit");
    protected readonly int hashMoving = Animator.StringToHash("IsMoving");
    protected readonly int hashDead = Animator.StringToHash("Dead");
    protected readonly int hashIsDead = Animator.StringToHash("IsDead");

    public AudioClip idleSFX;
    public AudioClip traceSFX;
    public AudioClip attackSFX;
    public AudioClip deadSFX;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    protected virtual void Awake()
    {
        player = Player.Instance;
        playerTrf = player.transform;
        enemyTrf = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        GetAllChildComponent(transform, rigidbodies);

        SkinnedMeshRenderer[] sms = transform.GetChild(0).GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(SkinnedMeshRenderer sms2 in sms)
        {
            if(sms2.enabled == true )
            {
                skinnedMeshRenderer = sms2;
                break;
            }
        }

    }
    protected virtual void Start()
    {
        combat.OnDamaged += PlayHitAnim;
        combat.OnDamagedWDamage += Player.Instance.combat.AddDealCount;
        combat.OnDead += Player.Instance.combat.AddKillCount;
        combat.OnDead += Dead;
    }
    protected virtual void OnEnable()
    {
        skinnedMeshRenderer.updateWhenOffscreen = false;
        foreach(var item in rigidbodies)
        {
            item.gameObject.layer = LayerMask.NameToLayer("Regdoll");
        }
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        animator.SetBool(hashIsDead, false);
    }
    public void TakeDamage(float damage)
    {
        if (combat.TakeDamage(damage))
        {
            animator.SetTrigger(hashHit);
        }
        SFXManager.Instance.SoundOnAttach(Player.Instance.transform, hitSFX);
    }
    private void PlayHitAnim()
    {
        animator.SetTrigger(hashHit);
    }
    protected virtual void Dead()
    {
        skinnedMeshRenderer.updateWhenOffscreen = true;

        foreach (var item in rigidbodies)
        {
            item.gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
        isDie = true;
        animator.enabled = false;
        agent.updatePosition = false;
        agent.updateRotation = false;
        EffectManager.Instance.DeadEffectGenerate(transform.position);


        StartCoroutine(ReturnToPool());
    }

    protected bool IsTargetVisible(Transform origin)
    {
        Vector3 targetPos = player.transform.position + Vector3.up;
        Vector3 targetDir = (-origin.position + targetPos).normalized;
        float dist = Vector3.Distance(origin.position, player.transform.position);
        Vector3 originPos = origin.position + targetDir;


        //bool isAimeAligned = Vector3.Dot(targetDir, transform.forward) >= .99f;
        //if (isAimeAligned)
        //{
        //    this.isAimeAligned = true;
        //}

        Ray sightRay = new Ray(originPos, targetDir);
        Physics.Raycast(sightRay, out RaycastHit hit, dist);
        Debug.DrawRay(sightRay.origin, sightRay.direction * dist, Color.yellow);

        if (hit.collider == null)
        {
            Debug.Log("NothingHit");
            return true;
        }
        else if (hit.collider.CompareTag("Player"))
        {
            hit.point.DrawSphere(1f, Color.blue);
            return true;
        }
        else
        {
            hit.point.DrawSphere(1f, Color.red);
            return false;
        }
    }

    protected virtual IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(15f);
        ObjectPoolManager.Instance.EnqueueObject(gameObject);
    }

    private void GetAllChildComponent<T>(Transform parent, List<T> list) 
    {
        T[] res = parent.GetComponentsInChildren<T>();
        foreach(var item in res)
        {
            list.Add(item);
        }

        if(parent.childCount == 0)
        {
            return;
        }
    }
}
