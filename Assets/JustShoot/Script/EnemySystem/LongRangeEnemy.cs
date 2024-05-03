using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LongRangeEnemy : MonoBehaviour
{
    [SerializeField] protected Player player;
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

    public float hp = 100f;

    private void Awake()
    {
        enemyTrf = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        statemachine = gameObject.AddComponent<Statemachine>();
        statemachine.AddState(State.IDLE, new IdleState(this));
        statemachine.AddState(State.TRACE, new TraceState(this));
        statemachine.AddState(State.ATTACK, new AttackState(this));
        statemachine.InitState(State.IDLE);

        agent.destination = playerTrf.position;
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
    }

    private void IsAimedPlayer()
    {
        //이거 재사용될 수 있음 fire 할때도 체크해야됨
        Vector3 targetDir = (-transform.position + playerTrf.position).normalized;
        transform.LookAt(targetDir);
        bool isAimed = Vector3.Dot(targetDir, transform.forward) >= .99f;
        if (isAimed)
        {
            isAimed = true;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDie)
        {
            return;
        }
        hp-=damage;
        if (hp <= 0f)
        {
            isDie = true;
            //OnDead
        }
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

    class AttackState : BaseEnemyState
    {
        public AttackState(LongRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetBool(owner.hashAttack, true);
            Debug.Log("Shoot");
        }
        public override void Update()
        {
            //플레이어 조준 완료시 state Aimed 조건을 설정함
            owner.IsAimedPlayer();
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
        }
    }
    class DeadState : BaseEnemyState
    {
        public DeadState(LongRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetBool(owner.hashAim, true);
            Debug.Log("Aim");
        }

        public override void Update()
        {
            //플레이어 조준 완료시 state Aimed 조건을 설정함
            owner.IsAimedPlayer();
        }
    }
}
