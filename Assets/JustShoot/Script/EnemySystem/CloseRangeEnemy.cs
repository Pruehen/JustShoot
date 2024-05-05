using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CloseRangeEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] protected Player player;
    private Combat combat = new Combat();
    public enum State
    {
        IDLE, TRACE, ATTACK, DEAD
    }
    public State state = State.IDLE;

    public float traceDistance = 10;
    public float attackDistance = 2;
    public float aimRotateSpeed = 30f;

    public bool isDie = false;

    Transform enemyTrf;
    [SerializeField] Transform playerTrf;
    NavMeshAgent agent;
    Animator animator;
    Statemachine statemachine;

    readonly int hashTrace = Animator.StringToHash("IsTrace");
    readonly int hashAttack = Animator.StringToHash("IsAttack");
    readonly int hashHit = Animator.StringToHash("Hit");
    readonly int hashMoving = Animator.StringToHash("IsMoving");
    readonly int hashDead = Animator.StringToHash("Dead");
    private void Awake()
    {
        player = Player.Instance;
        playerTrf = player.transform;
        enemyTrf = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        statemachine = gameObject.AddComponent<Statemachine>();
        statemachine.AddState(State.IDLE, new IdleState(this));
        statemachine.AddState(State.TRACE, new TraceState(this));
        statemachine.AddState(State.ATTACK, new AttackState(this));
        statemachine.AddState(State.DEAD, new DeadState(this));
        statemachine.InitState(State.IDLE);

        agent.destination = playerTrf.position;
    }
    private void Start()
    {
        combat.Init(transform, 100f);

        combat.OnDamaged += PlayHitAnim;
        combat.OnDamagedWDamage += Player.Instance.combat.AddDealCount;
        combat.OnDead += Player.Instance.combat.AddKillCount;
        combat.OnDead += Dead;

        StartCoroutine(CheckEnemyState());
    }
    protected virtual IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            float distance = Vector3.Distance(playerTrf.position, enemyTrf.position);
            if (distance <= attackDistance)
            {
                statemachine.ChangeState(State.ATTACK);
            }
            else if (distance <= traceDistance)
            {
                statemachine.ChangeState(State.TRACE);
            }
            else
            {
                statemachine.ChangeState(State.IDLE);
            }
        }
        statemachine.ChangeState(State.DEAD);
    }

    //적 공격 애니메이션에서 실행됨
    public void OnAimationAttak()
    {
        //DealDamage
        float distance = Vector3.Distance(player.transform.position, transform.position);
        bool closeEnogh = distance <= attackDistance;

        Vector3 enemyToPlayerDir = (-transform.position + player.transform.position).normalized;
        //참고 https://www.falstad.com/dotproduct/
        bool inAttackDirection = Vector3.Dot(transform.forward, enemyToPlayerDir) > .8f; // dot product 로 적이 보는 방향과 적의 위치까지의 방향이 비슷하면 데미지

        bool damagable = closeEnogh && inAttackDirection;

        if (damagable)//Todo: 공격 거리 계산을 다시 하고 싶을 수 있음
        {
            //플레이어에게 데미지 추가
            player.TakeDamage(10f);
            int type = 1;
            Vector3 hitPosition = player.transform.position + Vector3.up;
            EffectManager.Instance.HitEffectGenenate(hitPosition, type);//착탄 이펙트 발생
        }
    }
    public void TakeDamage(float damage)
    {
        if (combat.TakeDamage(damage))
        {
            animator.SetTrigger(hashHit);
        }
    }
    private void PlayHitAnim()
    {
        animator.SetTrigger(hashHit);
    }
    private void Dead()
    {
        isDie = true;
    }
    class BaseEnemyState : BaseState
    {
        protected CloseRangeEnemy owner;
        public BaseEnemyState(CloseRangeEnemy owner)
        {
            this.owner = owner;
        }
    }

    class IdleState : BaseEnemyState
    {
        public IdleState(CloseRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.agent.isStopped = true;
            owner.animator.SetBool(owner.hashTrace, false);
        }
    }

    class TraceState : BaseEnemyState
    {
        public TraceState(CloseRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.agent.SetDestination(owner.playerTrf.position);

            owner.agent.isStopped = false;
            owner.animator.SetBool(owner.hashTrace, true);
            owner.animator.SetBool(owner.hashAttack, false);
        }
        public override void Update()
        {
            bool moving = owner.agent.velocity.magnitude >= .01f;
            if (moving)
            {
                owner.animator.SetBool(owner.hashMoving, true);
            }
            else
            {
                owner.animator.SetBool(owner.hashMoving, false);
            }

        }
    }

    class AttackState : BaseEnemyState
    {
        public AttackState(CloseRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetBool(owner.hashAttack, true);
        }
        public override void Update()
        {
            bool moving = owner.agent.velocity.magnitude >= .01f;
            if (moving)
            {
                owner.animator.SetBool(owner.hashMoving, true);
            }
            else
            {
                owner.animator.SetBool(owner.hashMoving, false);
            }

            Vector3 pos = owner.transform.position;
            Vector3 target = owner.playerTrf.position;
            Vector3 desiredDir = -pos + target;
            desiredDir = new Vector3(desiredDir.x, 0f, desiredDir.z);
            desiredDir = desiredDir.normalized;
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(desiredDir), Time.deltaTime * owner.aimRotateSpeed);
        }
    }
    class DeadState : BaseEnemyState
    {
        public DeadState(CloseRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetTrigger(owner.hashDead);
        }
    }
}
