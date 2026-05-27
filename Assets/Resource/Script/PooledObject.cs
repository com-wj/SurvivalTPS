using System;
using UnityEngine;

[Serializable]
public class PooledObject : MonoBehaviour
{
	#region 인스펙터
	[Header("디버그")]
	[SerializeField] protected bool _printLog = false;
	#endregion

	#region 내부 변수
	protected PooledObject _origin;
	#endregion

	protected virtual void OnDisable()
	{
		_origin = null;
	}

	public virtual void Init(PooledObject origin)
	{
		_origin = origin;
	}

	protected virtual void ReturnToPool()
	{
		if (PoolManager.Instance == null || _origin == null) return;

		if (_printLog)
		{
			Debug.Log($"[{name}] pool로 복귀함.");
		}

		PoolManager.Instance.Push(_origin, this);
	}
}