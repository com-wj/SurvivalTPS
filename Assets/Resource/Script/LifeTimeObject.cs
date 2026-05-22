using System.Collections;
using UnityEngine;

public class LifeTimeObject : PooledObject
{
	#region 인스펙터
	[Header("소멸 타이머")]
	[SerializeField] protected float _lifeTime = 1.0f;
	[SerializeField] protected bool _timerAutoStart = false;
	#endregion

	#region 내부 변수
	protected Coroutine _routine;
	#endregion

	protected override void OnDisable()
	{
		base.OnDisable();

		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
	}

	public override void Init(PooledObject origin)
	{
		base.Init(origin);

		if (_timerAutoStart)
		{
			StartPoolingTimer();
		}
	}

	protected virtual void StartPoolingTimer()
	{
		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_routine = StartCoroutine(Co_LifeTimer(_lifeTime));
	}

	protected virtual IEnumerator Co_LifeTimer(float lifetime)
	{
		float elapsed = 0f;
		while (elapsed < lifetime)
		{
			elapsed += Time.deltaTime;
			yield return null;
		}

		_routine = null;
		ReturnToPool();
	}
}