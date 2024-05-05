using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour, IDamagable
{
    public GameObject hitSFX;
    public EnemyCombat combat;

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
    private void Dead()
    {
        isDie = true;
        col.enabled = false;
        StartCoroutine(ReturnToPool());
    }
    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(15f);
        ObjectPoolManager.Instance.EnqueueObject(gameObject);
    }
}
