using System;
using UnityEngine;

public class EnemyBase : LifeTimeObject, IDamageable
{
	#region 인스펙터
	[Header("능력치")]
	[SerializeField] protected float _maxHp;
	[SerializeField] protected float _currentHp;
	[SerializeField] protected float _moveSpeed = 5f;

	[Header("공격")]
	[SerializeField] protected float _attackDamage = 5f;
	[SerializeField] protected float _attackInterval = 1.5f;
	[SerializeField] protected float _attackRange = 5f;
	[SerializeField] protected float _attackActionDuration = 0.8f;
	[SerializeField] protected float _hitPredelay;

	[Header("모델 트랜스폼")]
	[SerializeField] protected Transform _modelTr;

	[Header("루트 캡슐 콜라이더")]
	[SerializeField] protected CapsuleCollider _collider;

	[Header("오디오 핸들러")]
	[SerializeField] protected EnemyAudioHandler _enemyAudioHandler;
	#endregion

	#region 내부 변수
	protected bool _isDead = false;
	#endregion

	public event Action Dead;

	#region 프로퍼티
	public float MoveSpeed => _moveSpeed;
	public float AttackDamage => _attackDamage;
	public float AttackInterval => _attackInterval;
	public float AttackRange => _attackRange;
	public float AttackActionDuration => _attackActionDuration;
	public float HitPredelay => _hitPredelay;
	public bool IsDead => _isDead;
	#endregion

	protected virtual void Awake()
	{
		if (_modelTr == null || _collider == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}

	protected virtual void OnEnable()
	{
		_currentHp = _maxHp;
		_collider.enabled = true;
		_modelTr.localPosition = Vector3.zero;
		_modelTr.localRotation = Quaternion.identity;
		_isDead = false;
	}

	public void TakeDamage(float damage, EDamageType damageType)
	{
		if (_isDead) return;

		_currentHp -= damage;
		if (_printLog)
		{
			Debug.Log($"[{name}] {damage} 피해 입음. 남은 체력:{_currentHp}");
		}

		if (_currentHp <= 0)
		{
			_currentHp = 0;
			Die();
		}
	}

	public virtual void Die(bool isPlayerKill = true)
	{
		if (_isDead) return;
		_isDead = true;

		if (_printLog)
		{
			Debug.Log($"[{name}] 사망");
		}

		_collider.enabled = false; // 사망 시 물리 충돌 제거

		if (isPlayerKill)
		{
			if (ScoreManager.Instance != null)
			{
				ScoreManager.Instance.AddKill();
			}
			if (EnemySpawner.Instance != null)
			{
				EnemySpawner.Instance.RemoveEnemyFromList(this);
			}
		}

		Dead?.Invoke();

		if (_enemyAudioHandler != null)
		{
			_enemyAudioHandler.PlayDeathClipRandom();
		}

		StartPoolingTimer();
	}
}