using System.Collections;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
	#region 인스펙터
	[Header("수명")]
	[SerializeField] private float _lifeTime = 1.0f;
	[SerializeField] private float _elapse; // 경과 시간

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	#region
	private PooledObject _origin;
	private Coroutine _routine;
	#endregion

	private void OnDisable()
	{
		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_elapse = 0f;
		_origin = null;
	}

	public void Init(PooledObject origin)
	{
		_origin = origin;

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

	private void ReturnToPool()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] pool로 복귀함.");
		}

		if (PoolManager.Instance == null) return;

		PoolManager.Instance.Push(_origin, this);
	}
}