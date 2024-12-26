using System;
using UnityEngine;

public abstract class Timer
{
    protected float initialTime;
    protected float Time { get; set; }
    public bool IsRunning { get; protected set; }

    public float Progress => Time / initialTime;

    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    protected Timer(float value)
    {
        initialTime = value;
        IsRunning = false;
    }

    public void Start()
    {
        Time = initialTime;
        if (!IsRunning) {
            IsRunning = true;
            // 发送Timer开始活动
            OnTimerStart.Invoke();
        }
    }

    public void Stop()
    {
        if (IsRunning) {
            IsRunning = false;
            OnTimerStop.Invoke();
        }
    }

    public void Resume() => IsRunning = true;

    public void Pause() => IsRunning = false;

    public abstract void Tick(float deltaTime);

    // 倒计时
    public class CountdownTimer : Timer 
    {
        public CountdownTimer(float value) : base(value) {}

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0) {
                Time -= deltaTime;
            }

            if (IsRunning && Time <= 0) {
                Stop();
            }
        }

        public void Reset() => Time = initialTime;

        public void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }
    }

    // 定时器
    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0) {}

        public override void Tick(float deltaTime)
        {
            if (IsRunning) {
                Time += deltaTime;
            }
        }

        public void Reset() => Time = 0;

        public float GetTime() => Time;
    }
}
