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

    protected virtual void Awake()
    {
        player = Player.Instance;
        playerTrf = player.transform;
        enemyTrf = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
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
        col.enabled = true;
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
        isDie = true;
        col.enabled = false;
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
}
