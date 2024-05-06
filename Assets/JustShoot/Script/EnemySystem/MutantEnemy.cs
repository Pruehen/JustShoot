using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MutantEnemy : BaseEnemy
{
    public enum State
    {
        IDLE, TRACE, LAUNCH, HOMING, ATTACK, DEAD
    }
    public State state = State.IDLE;


    public float traceDistance = 9999f;
    public float attackDistance = 16f;
    public float damageDistance = 2f;
    public float aimRotateSpeed = 30f;
    public float maxHp = 300f;

    Rigidbody rb;

    readonly int hashIsAlmostTarget = Animator.StringToHash("IsAlmostTarget"); 

    bool isGrouonded = false;
    [SerializeField] Transform sight;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();

        combat.Init(transform, maxHp);

        statemachine = gameObject.AddComponent<Statemachine>();
        statemachine.AddState(State.IDLE, new IdleState(this));
        statemachine.AddState(State.TRACE, new TraceState(this));
        statemachine.AddState(State.ATTACK, new AttackState(this));
        statemachine.AddState(State.DEAD, new DeadState(this));
        statemachine.InitState(State.IDLE);

    }
    protected override void Start()
    {
        base.Start();

        agent.destination = playerTrf.position;
        StartCoroutine(CheckEnemyState());
    }
    private void FixedUpdate()
    {
        isGrouonded = Physics.Raycast(transform.position, -Vector3.up, .3f);
    }
    protected virtual IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            float distance = Vector3.Distance(playerTrf.position, enemyTrf.position);
            if (distance <= attackDistance && IsTargetVisible(sight))
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


    //적 공격 애니메이션에서 실행됨
    private void OnAnimationLaunch()
    {
        Vector3 targetDir = (-transform.position + playerTrf.position).normalized;
        float angleV = Mathf.Atan2(targetDir.y, 1f);
        angleV = Mathf.Rad2Deg * angleV;
        angleV = (angleV > -15f) ? angleV + 30f : -angleV;
        rb.velocity = ProjectileCalc.CalcLaunch(transform.position, playerTrf.position, angleV);
        animator.SetBool(hashIsAlmostTarget, false);
        gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");

        audioSource.PlayOneShot(attackSFX);
    }

    //적 공격 애니메이션에서 실행됨
    private void OnAimationAttak()
    {
        gameObject.layer = LayerMask.NameToLayer("HugeEnemy");
        animator.SetBool(hashIsAlmostTarget, false);
        //DealDamage
        float distance = Vector3.Distance(player.transform.position, transform.position);
        bool closeEnogh = distance <= damageDistance;

        Vector3 enemyToPlayerDir = (-transform.position + player.transform.position).normalized;
        //참고 https://www.falstad.com/dotproduct/

        bool damagable = closeEnogh;

        if (damagable)//Todo: 공격 거리 계산을 다시 하고 싶을 수 있음
        {
            //플레이어에게 데미지 추가
            player.TakeDamage(10f);
            int type = 1;
            Vector3 hitPosition = player.transform.position + Vector3.up;
            EffectManager.Instance.HitEffectGenenate(hitPosition, type);//착탄 이펙트 발생
        }
    }

    protected override void Dead()
    {
        StartCoroutine(ReturnToPool());
        isDie = true;
        gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
    }

    protected override IEnumerator ReturnToPool()
    {
        yield return base.ReturnToPool();
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

            owner.agent.nextPosition = owner.transform.position;
            owner.agent.updatePosition = true;
            owner.agent.updateRotation = true;
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
            if(owner.isGrouonded)
            {
                owner.animator.SetBool(owner.hashIsAlmostTarget, true);
            }

        }
    }
    class AttackState : BaseEnemyState
    {
        public AttackState(MutantEnemy owner) : base(owner) { }


        Vector3 SentinelVec = new Vector3(-9999f, -9999f, -9999f);
        public override void Enter()
        {
            owner.animator.SetBool(owner.hashTrace, true);
            owner.animator.SetBool(owner.hashAttack, true);
            owner.agent.nextPosition = owner.transform.position;
            owner.agent.updatePosition = false;
            owner.agent.updateRotation = false;
            owner.agent.isStopped = true;
            prevPlayerPos = SentinelVec;
        }
        Vector3 prevPlayerPos = Vector3.positiveInfinity;
        public override void FixedUpdate()
        {
            Vector3 pos = owner.transform.position;
            Vector3 target = owner.playerTrf.position;
            Vector3 desiredDir = -pos + target;
            float distance = desiredDir.magnitude;
            desiredDir = new Vector3(desiredDir.x, 0f, desiredDir.z);
            desiredDir = desiredDir.normalized;
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(desiredDir), Time.deltaTime * owner.aimRotateSpeed);

            if (!owner.isGrouonded)
            {
                Vector3 curPlayerPos = owner.playerTrf.position;
                // 초기값
                if(prevPlayerPos == SentinelVec)
                    prevPlayerPos = curPlayerPos;


                bool isAlmostGrouonded = Physics.Raycast(owner.transform.position, -Vector3.up, .5f);
                isAlmostGrouonded = isAlmostGrouonded && owner.rb.velocity.y < 0f;
                if (distance < owner.damageDistance || isAlmostGrouonded)
                {
                    owner.animator.SetBool(owner.hashIsAlmostTarget, true);
                }
                else
                {
                    float deltaDist = (curPlayerPos - prevPlayerPos).magnitude;
                    Vector3 targetDir = (-owner.rb.position + owner.playerTrf.position).normalized;
                    Vector3 targetDirH = targetDir;
                    targetDirH.y = 0f;
                    targetDirH = targetDirH.normalized;

                    Vector3 velocityH = owner.rb.velocity;
                    velocityH.y = 0f;
                    float velocityHMag = velocityH.magnitude;

                    Vector3 newVelocityH = targetDirH * velocityHMag;

                    Vector3 result = new Vector3(newVelocityH.x, owner.rb.velocity.y, newVelocityH.z);
                    result += targetDirH * deltaDist * .5f;

                    owner.rb.velocity = result;
                }
                prevPlayerPos = curPlayerPos;
            }
            else
            {
                prevPlayerPos = SentinelVec;
                owner.animator.SetBool(owner.hashIsAlmostTarget, false);
            }
        }
    }
    class DeadState : BaseEnemyState
    {
        public DeadState(MutantEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetTrigger(owner.hashDead);
            owner.audioSource.PlayOneShot(owner.deadSFX);
        }
    }
}
