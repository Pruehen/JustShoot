using System.Collections.Generic;
using UnityEngine;
using System;

public class Statemachine : MonoBehaviour
{
    Dictionary<string, BaseState> stateDic = new Dictionary<string, BaseState>();
    BaseState curState;

    private void Start()
    {
        curState.Enter();
    }
    private void Update()
    {
        curState.Update();
        curState.Transition();
    }
    private void LateUpdate()
    {
        curState.LateUpdate();
    }
    private void FixedUpdate()
    {
        curState.FixedUpdate();
    }

    public void InitState(string stateName)
    {
        curState = stateDic[stateName];
    }
    public void AddState(string stateName, BaseState state)
    {
        state.SetStateMachine(this);
        stateDic.Add(stateName, state);
    }
    public void StateUpdate()
    {
        curState.Update();
    }
    public void ChangeState(string stateName)
    {
        curState.Exit();
        curState = stateDic[stateName];
        curState.Enter();
    }
    public void InitState<T>(T stateType) where T: Enum
    {
        InitState(stateType.ToString());
    }
    public void AddState<T>(T stateType, BaseState state) where T : Enum
    {
        AddState(stateType.ToString(), state);
    }
    public void ChangeState<T>(T stateType) where T : Enum
    {
        ChangeState(stateType.ToString());
    }
}

public class BaseState
{
    Statemachine statemachine;

    public void SetStateMachine(Statemachine statemachine)
    {
        this.statemachine = statemachine;
    }
    protected void ChangeState(string stateName)
    {
        statemachine.ChangeState(stateName);
    }
    protected void ChangeState<T>(T stateType) where T: Enum
    {
        ChangeState(stateType.ToString());
    }
    public virtual void Enter()
    {
        
    }
    public virtual void Update()
    {

    }
    public virtual void LateUpdate()
    {

    }
    public virtual void FixedUpdate()
    {

    }
    public virtual void Exit()
    {

    }
    public virtual void Transition()
    {

    }
}
