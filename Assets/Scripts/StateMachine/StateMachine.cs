using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine.XR;

public class StateMachine 
{
    // 代表当前节点 每个节点包含了一个状态（IState）和从该状态到其他状态的转换（ITransition）
    StateNode current;
    // 用于根据状态的类型 (Type) 查找对应的状态节点（StateNode）
    Dictionary<Type, StateNode> nodes = new();
    // anyTransitions 存储所有的全局转换条件（ITransition）
    // 这些条件不依赖于当前状态，任何状态都可能触发这些转换。
    // 例如：暂停、游戏结束等全局事件
    HashSet<ITransition> anyTransitions = new();

    // 不断转换状态
    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
            ChangeState(transition.To);

        // 这通常用来处理状态中的常规行为
        current.State.Update();
    }

    public void FixedUpdate()
    {
        current.State.FixedUpdate();
    }

    public void SetState(IState state)
    {
        this.current = nodes[state.GetType()];
        this.current.State?.OnEnter();
    }

    // 切换到新状态
    void ChangeState(IState state)
    {
        if (state == current.State)
            return;

        var previousState = current.State;
        var nextState = nodes[state.GetType()].State;

        previousState?.OnExit();
        nextState?.OnEnter();
        current = nodes[state.GetType()];
    }

    // 方法用于检查是否有满足条件的转换
    ITransition GetTransition()
    {
        // 首先检查全局转换条件 全局转换常常是更高优先级的行为 例如：用户按下“暂停”按钮、游戏结束等
        foreach (var transition in anyTransitions)
            if(transition.Condition.Evaluate())
                return transition;

        // 然后检查当前状态的转换条件
        foreach (var transition in current.Transitions)
            if (transition.Condition.Evaluate())
                return transition;

        return null;
    }

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(IState to, IPredicate condition)
    {
        anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
    }

    // 在字典里面找，找不到就创建一个新的
    StateNode GetOrAddNode(IState state)
    {
        var node = nodes.GetValueOrDefault(state.GetType());

        if (node == null)
        {
            node = new StateNode(state);
            nodes.Add(state.GetType(), node);
        }

        return node;
    }

    // 内部类 StateNode
    class StateNode 
    {
        public IState State { get; }
        // 包含了从该状态到其他状态的所有可能转换
        public HashSet<ITransition> Transitions { get; }

        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<ITransition>();
        }

        public void AddTransition(IState to, IPredicate condition)
        {
            Transitions.Add(new Transition(to, condition));
        }
    }
}
