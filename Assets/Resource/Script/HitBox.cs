using UnityEngine;

public class HitBox : MonoBehaviour, IDamageable
{
	private enum HitLocation
	{
		Default,
		Head,
		Body,
		Arm,
		Leg,
	}

	#region 인스펙터
	[Header("부위")]
	[SerializeField] private HitLocation _hitLocation;

	[Header("디버그")]
	[SerializeField] private bool _printLog;
	#endregion

	#region 내부 변수
	private IDamageable _root; // 피해 주체
	#endregion

	private void Awake()
	{
		// 피해 주체 탐색
		_root = transform.root.GetComponent<IDamageable>();

		if (_root == null)
		{
			if (transform.parent != null)
			{
				_root = transform.parent.GetComponentInParent<IDamageable>();
			}
		}
		else
		{
			if (_printLog)
			{
				Debug.Log($"[{name}] 최상위 root 탐색. {_root}");
			}
		}

		if (_root == null)
		{
			Debug.LogWarning($"[{name}] root 컴포넌트 확인");
			gameObject.SetActive(false);
			return;
		}
	}

	public void TakeDamage(float damage)
	{
		if (_root == null) return;

		float multiplier = GetDamageMultiplier(_hitLocation);

		if (_printLog)
		{
			Debug.Log($"[{name}] {_hitLocation} 히트");
		}
		_root.TakeDamage(damage * multiplier);
	}

	/// <summary>
	/// 부위별 데미지 가중치 계산
	/// </summary>
	private float GetDamageMultiplier(HitLocation location)
	{
		float multiplier = 1;
		
		switch (location)
		{
			case HitLocation.Default:
				multiplier = 1f;
				break;
			case HitLocation.Head:
				multiplier = 1.5f;
				break;
			case HitLocation.Body:
				multiplier = 1f;
				break;
			case HitLocation.Arm:
				multiplier = 0.8f;
				break;
			case HitLocation.Leg:
				multiplier = 0.7f;
				break;
		}

		return multiplier;
	}
}