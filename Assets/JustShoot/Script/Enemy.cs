using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        IDLE, TRACE, ATTACK, DIE
    }
    public State state = State.IDLE;

    public float traceDistance = 10;
    public float attackDistance = 2;

    public bool isDie = false;

    Transform enemyTrf;
    [SerializeField]Transform playerTrf;
    NavMeshAgent agent;
    Animator animator;
    Statemachine statemachine;

    readonly int hashTrace = Animator.StringToHash("IsTrace");
    readonly int hashAttack = Animator.StringToHash("IsAttack");

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

    IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            if(state == State.DIE)
            {
                statemachine.ChangeState(State.DIE);
                yield break;
            }

            float distance = Vector3.Distance(playerTrf.position, this.transform.position);

            if(distance <= attackDistance)
            {
                statemachine.ChangeState(State.ATTACK);
            }
            else if(distance <= traceDistance)
            {
                statemachine.ChangeState(State.TRACE);
            }
            else
            {
                statemachine.ChangeState(State.IDLE);
            }
        }
    }


    class BaseEnemyState : BaseState
    {
        protected Enemy owner;
        public BaseEnemyState(Enemy owner)
        {
            this.owner = owner;
        }
    }

    class IdleState : BaseEnemyState
    {
        public IdleState(Enemy owner) : base(owner) { }

        public override void Enter() 
        {
            owner.agent.isStopped = true;
            owner.animator.SetBool(owner.hashTrace, false);
        }
    }

    class TraceState : BaseEnemyState
    {
        public TraceState(Enemy owner) : base(owner) { }

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
        public AttackState(Enemy owner) : base(owner) { }

        public override void Enter()
        {                        
            owner.animator.SetBool(owner.hashAttack, true);
            Debug.Log("Shoot");
        }
    }
}
