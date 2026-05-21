using UnityEngine;

public class EnemyBase : PooledObject, IDamageable
{
	#region 인스펙터
	[SerializeField] private float _currentHp;
	[SerializeField] private float _moveSpeed = 5f;

	[Header("공격")]
	[SerializeField] private float _attackDamage = 5f;
	[SerializeField] private float _attackInterval = 1.5f;
	[SerializeField] private float _attackRange = 5f;
	[SerializeField] private float _attackActionDuration = 0.8f;
	[SerializeField] private float _hitPredelay;
	#endregion

	#region 인스펙터
	private bool _isDead = false;
	#endregion

	#region 프로퍼티
	public float MoveSpeed => _moveSpeed;
	public float AttackDamage => _attackDamage;
	public float AttackInterval => _attackInterval;
	public float AttackRange => _attackRange;
	public float AttackActionDuration => _attackActionDuration;
	public float HitPredelay => _hitPredelay;
	public bool IsDead => _isDead;
	#endregion

	public void TakeDamage(float damage)
	{
		if (_isDead) return;

		_currentHp -= damage;
		if (_printLog)
		{
			Debug.Log($"[{name}] {damage} 피해 입음. 남은 체력:{_currentHp}");
		}

		if (_currentHp < 0)
		{
			_currentHp = 0;
			Die();
		}
	}

	private void Die()
	{
		_isDead = true;
		if (_printLog)
		{
			Debug.Log($"[{name}] 사망");
		}

		ReturnToPool();
	}
}