using System;
using UnityEngine;

public class PlayerBase : MonoBehaviour, IDamageable
{
	#region 인스펙터
	[SerializeField] private float _currentHp = 100.0f;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	[SerializeField] private bool _debugMode = false;
	#endregion

	#region 내부 변수
	private float _maxHp = 100f;
	private bool _isDead = false;
	#endregion

	public bool IsDead => _isDead;

	public event Action<float, float> HPChanged; // 현재, 전체 체력
	public event Action Dead;
	public event Action<EDamageType> DeadByCause;

	private void Awake()
	{
		_currentHp = _maxHp;
	}

	private void Start()
	{
		HPChanged?.Invoke(_currentHp, _maxHp);
	}

#if UNITY_EDITOR
	// For Test
	private void Update()
	{
		if (_debugMode)
		{
			if (Input.GetKeyDown(KeyCode.F4))
			{
				Die(EDamageType.Normal);
			}
		}
	}
#endif

	public void TakeDamage(float damage, EDamageType damageType)
	{
		if (_isDead) return;

		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayOneShot("PlayerHit");
		}

		_currentHp -= damage;
		if (_printLog)
		{
			Debug.Log($"[{name}] {damage} 피해 입음. 남은 체력:{_currentHp}");
		}

		if (_currentHp <= 0)
		{
			_currentHp = 0;
			Die(damageType);
		}

		HPChanged?.Invoke(_currentHp, _maxHp);
	}

	private void Die(EDamageType damageType)
	{
		if (_isDead) return;

		_isDead = true;

		if (_printLog)
		{
			Debug.Log($"[{name}] 사망");
		}

		DeadByCause?.Invoke(damageType);
		Dead?.Invoke();
	}
}