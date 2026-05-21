using System.Collections;
using UnityEngine;

public class LifeTimeObejct : PooledObject
{
	#region 인스펙터
	[Header("수명")]
	[SerializeField] private float _lifeTime = 1.0f;
	[SerializeField] private float _elapse; // 경과 시간
	#endregion

	#region 내부 변수
	private Coroutine _routine;
	#endregion

	protected override void OnDisable()
	{
		base.OnDisable();

		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_elapse = 0f;
	}

	public override void Init(PooledObject origin)
	{
		base.Init(origin);

		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_routine = StartCoroutine(Co_LifeTimer(_lifeTime));
	}

	private IEnumerator Co_LifeTimer(float lifetime)
	{
		_elapse = 0f;
		while (_elapse < lifetime)
		{
			_elapse += Time.deltaTime;
			yield return null;
		}

		ReturnToPool();
		_routine = null;
	}
}