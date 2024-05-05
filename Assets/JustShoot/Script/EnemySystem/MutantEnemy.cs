using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MutantEnemy : BaseEnemy
{
    [SerializeField] protected Player player;
    private Combat combat;
    public enum State
    {
        IDLE, TRACE, LAUNCH, HOMING, ATTACK, DEAD
    }
    public State state = State.IDLE;

    public float traceDistance = 9999f;
    public float attackDistance = 16f;
    public float DamageDistance = 1f;
    public float aimRotateSpeed = 30f;
    public float maxHp = 300f;

    Rigidbody rb;

    readonly int hashLaunch = Animator.StringToHash("Launch");
    readonly int hashHoming = Animator.StringToHash("IsHoming");
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();

        statemachine = gameObject.AddComponent<Statemachine>();
        statemachine.AddState(State.IDLE, new IdleState(this));
        statemachine.AddState(State.TRACE, new TraceState(this));
        statemachine.AddState(State.ATTACK, new AttackState(this));
        statemachine.AddState(State.DEAD, new DeadState(this));
        statemachine.InitState(State.IDLE);

        agent.destination = playerTrf.position;
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
            if (distance <= attackDistance || animator.GetCurrentAnimatorStateInfo(0).IsName("MutantJumpAttack") || agent.isOnNavMesh == false)
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
    private void OnAnimationLaunch()
    {
        float timeScaleFactor = 0.3f;
        float distance = (playerTrf.position - transform.position).magnitude;
        float timeToTarget = distance * timeScaleFactor;
        Vector3 initialVelocity = ProjectileCalc.CalculateInitialVelocity(transform.position, playerTrf.position, timeToTarget, Physics.gravity.y);
        rb.velocity = initialVelocity;  
    }

    //적 공격 애니메이션에서 실행됨
    private void OnAimationAttak()
    {
        //DealDamage
        float distance = Vector3.Distance(player.transform.position, transform.position);
        bool closeEnogh = distance <= DamageDistance;

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
    class BaseEnemyState : BaseState
    {
        protected MutantEnemy owner;
        public BaseEnemyState(MutantEnemy owner)
        {
            this.owner = owner;
        }
    }

    class IdleState : BaseEnemyState
    {
        public IdleState(MutantEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.agent.isStopped = true;
            owner.animator.SetBool(owner.hashTrace, false);
        }
    }

    class TraceState : BaseEnemyState
    {
        public TraceState(MutantEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.agent.SetDestination(owner.playerTrf.position);

            owner.agent.isStopped = false;
            owner.animator.SetBool(owner.hashTrace, true);
            owner.animator.SetBool(owner.hashAttack, false);
            owner.agent.updatePosition = true;
            owner.agent.updateRotation = true;
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
        public AttackState(MutantEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetBool(owner.hashTrace, true);
            owner.animator.SetBool(owner.hashAttack, true);
            owner.agent.updatePosition = false;
            owner.agent.updateRotation = false;
        }
        public override void FixedUpdate()
        {
            float maxSpeed = 1f;
            float maxSteeringForce = 1f;
            Transform target = owner.playerTrf;
            Vector3 steeringForce = ProjectileCalc.CalculateSteeringForce(owner.transform.position, target.position, owner.rb.velocity, maxSpeed, maxSteeringForce);
            owner.rb.velocity = ProjectileCalc.ApplySteeringForce(owner.rb.velocity, steeringForce, maxSpeed);

            // Optionally adjust rotation to face velocity
            owner.rb.rotation = ProjectileCalc.CalculateRotation(owner.rb.velocity);
        }
    }
    class DeadState : BaseEnemyState
    {
        public DeadState(MutantEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetTrigger(owner.hashDead);
        }
    }
}
