#define USE_UNITASK
// 코루틴과 동일한 기능을 하기위해서
// TaskUtil 에 사용된 UniTask 2.3.1 은 GetCancellationTokenOnDisable 이 추가된 버전입니다.

#if USE_UNITASK
using System.Threading;
using Cysharp.Threading.Tasks;
#endif

using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public static class TaskUtil
{
    public class TaskMethod
    {
        protected MonoBehaviour owner;
        protected bool isKill;

#if USE_UNITASK
        protected CancellationTokenSource tokenSource;
#else
        protected Coroutine coroutine;        
#endif

        public void Kill()
        {
            if (isKill)
                return;

            isKill = true;

#if USE_UNITASK
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
#else
            if (coroutine != null)
                owner.StopCoroutine(coroutine);
#endif
        }
    }

    public class WhileTaskMethod : TaskMethod
    {
        public WhileTaskMethod(MonoBehaviour owner, float interval, float startDelay, Action task, Func<bool> condition)
        {
            this.owner = owner;
            SetIntervalTime(interval);
#if USE_UNITASK
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, owner.GetCancellationTokenOnDestroy(), owner.GetCancellationTokenOnDisable());
            Method(interval, startDelay, task, condition).Forget();
#else
            coroutine = owner.StartCoroutine(Method(interval, startDelay, task, condition));
#endif
        }

        protected float intervalTime;

#if !USE_UNITASK
        WaitForSeconds loopTime;
#endif

#if USE_UNITASK
        async UniTaskVoid Method(float interval, float startDelay, Action task, Func<bool> condition)
#else
        IEnumerator Method(float interval, float startDelay, Action task, Func<bool> condition)
#endif
        {
            if (startDelay > 0)
            {
#if USE_UNITASK
                await UniTask.Delay(Mathf.RoundToInt(startDelay * 1000), cancellationToken: tokenSource.Token);
#else
                yield return new WaitForSeconds(startDelay);
#endif
            }

            while (true)
            {
                if (condition == null || condition.Invoke())
                    task?.Invoke();
                else
                {
                    Kill();
                    break;
                }

                if (isKill)
                    break;

#if USE_UNITASK
                await UniTask.Delay(Mathf.RoundToInt(intervalTime * 1000), cancellationToken: tokenSource.Token);
#else
                yield return loopTime;
#endif
            }
        }

        public float GetIntervalTime() => intervalTime;

        public void SetIntervalTime(float interval)
        {
            if (intervalTime == interval)
                return;

            intervalTime = interval;
#if !USE_UNITASK
            loopTime = new WaitForSeconds(interval);
#endif
        }
    }

    public class DelayTaskMethod : TaskMethod
    {
        public DelayTaskMethod(MonoBehaviour owner, float time, Action task)
        {
            this.owner = owner;
#if USE_UNITASK
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, owner.GetCancellationTokenOnDestroy(), owner.GetCancellationTokenOnDisable());
            Method(time, task).Forget();
#else
            coroutine = owner.StartCoroutine(Method(time, task));
#endif
        }

#if USE_UNITASK
        async UniTaskVoid Method(float time, Action task)
#else
        IEnumerator Method(float time, Action task)
#endif
        {
#if USE_UNITASK
            await UniTask.Delay(Mathf.RoundToInt(time * 1000), cancellationToken: tokenSource.Token);
            Kill();
#else
            yield return new WaitForSeconds(time);
#endif
            task?.Invoke();
        }
    }

    public class WaitUntilTaskMethod : TaskMethod
    {
        public WaitUntilTaskMethod(MonoBehaviour owner, Action task, Func<bool> condition)
        {
            this.owner = owner;
#if USE_UNITASK
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, owner.GetCancellationTokenOnDestroy(), owner.GetCancellationTokenOnDisable());
            Method(task, condition).Forget();
#else
            coroutine = owner.StartCoroutine(Method(task, condition));
#endif
        }

#if USE_UNITASK
        async UniTaskVoid Method(Action task, Func<bool> condition)
#else
        IEnumerator Method(Action task, Func<bool> condition)
#endif
        {
#if USE_UNITASK
            await UniTask.WaitUntil(condition, cancellationToken: tokenSource.Token);
            Kill();
#else
            yield return new WaitUntil(condition);
#endif
            task?.Invoke();
        }
    }

    public class CollectionTaskMethod<T> : TaskMethod
    {
        public CollectionTaskMethod(MonoBehaviour owner, float interval, ICollection<T> collection, Action<T, int> task)
        {
            this.owner = owner;
            this.collection = collection;
#if USE_UNITASK
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, owner.GetCancellationTokenOnDestroy(), owner.GetCancellationTokenOnDisable());
            Method(interval, task).Forget();
#else
            loopTime = new WaitForSeconds(interval);
            coroutine = owner.StartCoroutine(Method(interval, task));
#endif
        }

        ICollection<T> collection;
#if !USE_UNITASK
        WaitForSeconds loopTime;
#endif

#if USE_UNITASK
        async UniTaskVoid Method(float interval, Action<T, int> task)
#else
        IEnumerator Method(float interval, Action<T, int> task)
#endif
        {
            int index = 0;
            foreach (var item in collection)
            {
                task?.Invoke(item, index);

                if (isKill)
                    break;

#if USE_UNITASK
                await UniTask.Delay(Mathf.RoundToInt(interval * 1000), cancellationToken: tokenSource.Token);
#else
                yield return loopTime;
#endif
                index++;
            }
#if USE_UNITASK
            Kill();
#endif
        }
    }

    public class ForTaskMethod : TaskMethod
    {
        public ForTaskMethod(MonoBehaviour owner, float interval, int startValue, int endValue, int iterator, Action<int> task)
        {
            this.owner = owner;
#if USE_UNITASK
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, owner.GetCancellationTokenOnDestroy(), owner.GetCancellationTokenOnDisable());
            Method(interval, startValue, endValue, iterator, task).Forget();
#else
            loopTime = new WaitForSeconds(interval);
            coroutine = owner.StartCoroutine(Method(interval, startValue, endValue, iterator, task));
#endif
        }

#if !USE_UNITASK
        WaitForSeconds loopTime;
#endif

#if USE_UNITASK
        async UniTaskVoid Method(float interval, int startValue, int endValue, int iterator, Action<int> task)
#else
        IEnumerator Method(float interval, int startValue, int endValue, int iterator, Action<int> task)
#endif
        {
            for (int i = startValue; i < endValue; i += iterator)
            {
                task?.Invoke(i);

                if (isKill)
                    break;

#if USE_UNITASK
                await UniTask.Delay(Mathf.RoundToInt(interval * 1000), cancellationToken: tokenSource.Token);
#else
                yield return loopTime;
#endif
            }
#if USE_UNITASK
            Kill();
#endif
        }
    }

    #region Extension Method

    /// <summary>
    /// 지정된 시간 대기 후 작업을 실행합니다.
    /// </summary>
    public static DelayTaskMethod TaskDelay(this MonoBehaviour owner,
        float time, Action task) =>
        new DelayTaskMethod(owner, time, task);

    /// <summary>
    /// condition 이 null 이거나 true 일 때 지정된 시간 대기 후 작업을 실행합니다.
    /// false 가 되었을 때 Kill 이 실행됩니다.
    /// </summary>
    /// <param name="startDelay">while 실행 전 딜레이</param>
    public static WhileTaskMethod TaskWhile(this MonoBehaviour owner,
        float interval, float startDelay, Action task, Func<bool> condition = null) =>
        new WhileTaskMethod(owner, interval, startDelay, task, condition);

    /// <summary>
    /// 컬렉션 요소를 지정된 시간을 대기하며 작업을 실행합니다.
    /// </summary>
    public static CollectionTaskMethod<T> TaskCollection<T>(this MonoBehaviour owner,
        float interval, ICollection<T> collection, Action<T, int> task) =>
        new CollectionTaskMethod<T>(owner, interval, collection, task);

    /// <summary>
    /// 조건이 만족 될 때까지 대기 후 실행합니다.
    /// </summary>
    public static WaitUntilTaskMethod TaskWaitUntil(this MonoBehaviour owner,
        Action task, Func<bool> condition) =>
        new WaitUntilTaskMethod(owner, task, condition);

    /// <summary>
    /// for (v = startValue; v < endValue; v += iterator)
    /// </summary>
    public static ForTaskMethod TaskFor(this MonoBehaviour owner,
        float interval, int startValue, int endValue, int iterator, Action<int> task) =>
        new ForTaskMethod(owner, interval, startValue, endValue, iterator, task);

    #endregion
}