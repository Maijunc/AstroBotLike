// Predicate 是一个函数 测试一个条件，并根据测试结果返回布尔值 true or false
using System;

public class FuncPredicate : IPredicate
{

    // Func 是一个委托，它可以接受0个或多个参数，并返回一个值
    readonly Func<bool> func;
    public FuncPredicate(Func<bool> func)
    {
        this.func = func;
    }

    // Evaluate方法用于测试一个条件，并根据测试结果返回布尔值 true or false
    public bool Evaluate() => func.Invoke();
}