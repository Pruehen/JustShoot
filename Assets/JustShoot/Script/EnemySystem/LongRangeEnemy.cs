using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LongRangeEnemy : BaseEnemy, IDamagable
{
    public enum State
    {
        IDLE, TRACE, ATTACK, DEAD
    }
    public State state = State.IDLE;

    public bool isAimeAligned = false;

    public float traceDistance = 9999f;
    public float attackDistance = 10f;
    public float aimRotateSpeed = 30f;

    public float maxHp = 60f;

    [SerializeField] Transform shootTransform;
    [SerializeField] GameObject projectilePrefab;


    protected override void Awake()
    {
        base.Awake();
        combat.Init(transform, maxHp);

        statemachine = gameObject.AddComponent<Statemachine>();
        statemachine.AddState(State.IDLE, new IdleState(this));
        statemachine.AddState(State.TRACE, new TraceState(this));
        statemachine.AddState(State.ATTACK, new AttackState(this));
        statemachine.AddState(State.DEAD, new DeadState(this));
        statemachine.InitState(State.IDLE);

        agent.destination = playerTrf.position;

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        audioSource.clip = traceSFX;
        audioSource.Play();
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(CheckEnemyState());
    }

    protected virtual IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            float distance = Vector3.Distance(playerTrf.position, enemyTrf.position);
            if (distance <= attackDistance && IsTargetVisible(shootTransform))
            {
                statemachine.ChangeState(State.ATTACK);
                state = State.ATTACK;
            }
            else if (distance <= traceDistance)
            {
                statemachine.ChangeState(State.TRACE);
                state = State.TRACE;
            }
            else
            {
                statemachine.ChangeState(State.IDLE);
                state = State.IDLE;
            }
        }
        statemachine.ChangeState(State.DEAD);
        state = State.DEAD;
    }

    public void OnAnimationAttack()
    {
        //FireProjectile
        audioSource.PlayOneShot(attackSFX);

        GameObject bulletIst = ObjectPoolManager.Instance.DequeueObject(projectilePrefab);
        bulletIst.transform.position = shootTransform.position;

        float initialSpeed = 10f;

        Vector3 velocityForIntecept = ProjectileCalc.CalculateInitialVelocity(Player.Instance.transform, shootTransform, initialSpeed, Vector3.up);
        bulletIst.transform.rotation = Quaternion.LookRotation(velocityForIntecept);
        bulletIst.transform.GetComponent<EnemyProjectile>().Init(10, velocityForIntecept.magnitude, 10f);

        EffectManager.Instance.FireEffectGenenate(shootTransform.position, shootTransform.rotation);
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
            owner.animator.SetBool(owner.hashTrace, true);
            owner.animator.SetBool(owner.hashAttack, false);
            owner.agent.SetDestination(owner.playerTrf.position);
        }

        public override void Update()
        {
            //공격중이면 이동안함
            if (owner.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                return;
            owner.agent.isStopped = false;
            owner.agent.SetDestination(owner.playerTrf.position);
        }
    }
    class AttackState : BaseEnemyState
    {
        public AttackState(LongRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.agent.isStopped = true;

            owner.animator.SetBool(owner.hashTrace, true);
            owner.animator.SetBool(owner.hashAttack, true);
        }
        public override void Update()
        {
            //플레이어 조준 완료시 state Aimed 조건을 설정함
            Vector3 pos = owner.transform.position;
            Vector3 target = owner.playerTrf.position;
            Vector3 desiredDir = -pos + target;
            desiredDir = new Vector3(desiredDir.x, 0f, desiredDir.z);
            desiredDir = desiredDir.normalized;
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(desiredDir),Time.deltaTime * owner.aimRotateSpeed);
        }
    }
    class DeadState : BaseEnemyState
    {
        public DeadState(LongRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetTrigger(owner.hashDead);
            owner.animator.SetBool(owner.hashIsDead, true);
            owner.agent.isStopped = true;
            owner.audioSource.Stop();
            owner.audioSource.PlayOneShot(owner.deadSFX);
        }
    }
}
