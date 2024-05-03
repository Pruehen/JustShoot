using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LongRangeEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] protected Player player;
    private Combat combat = new Combat();
    public enum State
    {
        IDLE, TRACE, Aim, ATTACK, DIE
    }
    public State state = State.IDLE;

    public bool isAimed = false;

    public float traceDistance = 10;
    public float attackDistance = 2;

    public bool isDie = false;

    Transform enemyTrf;
    [SerializeField] Transform playerTrf;
    NavMeshAgent agent;
    Animator animator;
    Statemachine statemachine;

    readonly int hashTrace = Animator.StringToHash("IsTrace");
    readonly int hashAttack = Animator.StringToHash("IsAttack");
    readonly int hashAim = Animator.StringToHash("IsAim");

    [SerializeField] Transform shootTransform;
    [SerializeField] GameObject projectilePrefab;

    private void Awake()
    {
        enemyTrf = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        statemachine = gameObject.AddComponent<Statemachine>();
        statemachine.AddState(State.IDLE, new IdleState(this));
        statemachine.AddState(State.TRACE, new TraceState(this));
        statemachine.AddState(State.Aim, new AimState(this));
        statemachine.AddState(State.ATTACK, new AttackState(this));
        statemachine.AddState(State.DIE, new DeadState(this));
        statemachine.InitState(State.IDLE);

        combat.Init(transform, 60f);

        player = Player.Instance;
        playerTrf = player.transform;
        agent.destination = playerTrf.position;

        combat.OnDead += Dead;
    }

    private void Start()
    {
        StartCoroutine(CheckEnemyState());
    }

    protected virtual IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            if (state == State.DIE)
            {
                statemachine.ChangeState(State.DIE);
                yield break;
            }

            float distance = Vector3.Distance(playerTrf.position, enemyTrf.position);
            if(isAimed)
            {
                statemachine.ChangeState(State.ATTACK);
            }
            else if (distance <= attackDistance)
            {
                statemachine.ChangeState(State.Aim);
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
        statemachine.ChangeState(State.DIE);
    }

    private void IsAimedPlayer()
    {
        //이거 재사용될 수 있음 fire 할때도 체크해야됨
        Vector3 targetDir = (-transform.position + player.transform.position).normalized;
        Debug.DrawLine(transform.position, transform.position + targetDir, Color.red, .1f);
        transform.LookAt(player.transform.position);
        bool isAimed = Vector3.Dot(targetDir, transform.forward) >= .99f;
        if (isAimed)
        {
            this.isAimed = true;
        }
    }

    public void TakeDamage(float damage)
    {
        combat.TakeDamage(damage);
    }

    public void OnAnimationAttack()
    {
        //FireProjectile

        GameObject bulletIst = ObjectPoolManager.Instance.DequeueObject(projectilePrefab);
        bulletIst.transform.position = shootTransform.position;
        bulletIst.transform.rotation = shootTransform.rotation;

        bulletIst.GetComponent<Rigidbody>().velocity = bulletIst.transform.forward * 500;

        EffectManager.Instance.FireEffectGenenate(shootTransform.position, shootTransform.rotation);

    }
    private void Dead()
    {
        isDie = true;
    }

    class BaseEnemyState : BaseState
    {
        protected LongRangeEnemy owner;
        public BaseEnemyState(LongRangeEnemy owner)
        {
            this.owner = owner;
        }
    }

    class IdleState : BaseEnemyState
    {
        public IdleState(LongRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.agent.isStopped = true;
            owner.animator.SetBool(owner.hashTrace, false);
        }
    }

    class TraceState : BaseEnemyState
    {
        public TraceState(LongRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.agent.SetDestination(owner.playerTrf.position);

            owner.agent.isStopped = false;
            owner.animator.SetBool(owner.hashTrace, true);
            owner.animator.SetBool(owner.hashAttack, false);
        }
    }
    class AimState : BaseEnemyState
    {
        public AimState(LongRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetBool(owner.hashAim, true);
            Debug.Log("Aim");
        }

        public override void Update()
        {
            //플레이어 조준 완료시 state Aimed 조건을 설정함
            owner.IsAimedPlayer();
            //Todo: 사격거리가 충분히 가까우면 정지하기
        }
    }
    class AttackState : BaseEnemyState
    {
        public AttackState(LongRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetBool(owner.hashAttack, true);
        }
        public override void Update()
        {
            //플레이어 조준 완료시 state Aimed 조건을 설정함
            owner.IsAimedPlayer();
        }
    }
    class DeadState : BaseEnemyState
    {
        public DeadState(LongRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            //Todo:
            //owner.animator.SetBool(owner.hashDead, true);
            Debug.Log("Dead");
        }
    }
}
