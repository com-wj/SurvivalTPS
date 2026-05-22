using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private EnemyBase _enemyBase;
	[SerializeField] private NavMeshAgent _navMeshAgent;
	[SerializeField] private EnemyAnimator _enemyAnimator;

	[Header("타겟(플레이어)")]
	[SerializeField] private Transform _targetTr;

	[Header("회전 속도")]
	[SerializeField] private float _rotateSharpness = 8.0f;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	#region 내부 변수
	private float _nextRefreshTime;
	private float _nextAttackTime;
	private bool _isAttacking = false;
	private Coroutine _routine;
	private Vector3 _toTarget = Vector3.zero;
	#endregion

	private float _sqrAttackRange => _enemyBase.AttackRange * _enemyBase.AttackRange;

	private void Awake()
	{
		if (_navMeshAgent == null ||
			_enemyBase == null ||
			_enemyAnimator == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}

	private void SetTarget()
	{
		if (GameManager.Instance == null ||
			GameManager.Instance.PlayerTr == null) return;

		_targetTr = GameManager.Instance.PlayerTr;
	}

	private void OnEnable()
	{
		_navMeshAgent.enabled = true;
		SetTarget();

		if (_enemyBase != null)
		{
			_enemyBase.Dead += OnDead;
		}
	}

	private void OnDisable()
	{
		if (_enemyBase != null)
		{
			_enemyBase.Dead -= OnDead;
		}

		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_targetTr = null;

		_isAttacking = false;
		_toTarget = Vector3.zero;
		_nextRefreshTime = 0f;
		_nextAttackTime = 0f;
	}

	private void Update()
	{
		if (_enemyBase == null ||
			_enemyBase.IsDead ||
			_targetTr == null) return;

		if (_isAttacking) return;

		if (_navMeshAgent.isStopped) // 공격 상태가 아니면 정지 해제
		{
			_navMeshAgent.isStopped = false;
		}

		RefreshDestination();

		if (!_navMeshAgent.pathPending &&
			_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
		{
			TickRotateToTarget();
			TryAttack();
		}

		UpdateMoveParam();
	}

	// 경로 재탐색
	private void RefreshDestination()
	{
		if (_nextRefreshTime > Time.time) return;

		_nextRefreshTime = Time.time + 0.2f;

		_navMeshAgent.SetDestination(_targetTr.position); // 동선 계산
	}

	private void TickRotateToTarget()
	{
		_toTarget = (_targetTr.position - transform.position);

		Vector3 toTarget = _toTarget;
		toTarget.y = 0;

		Quaternion targetRot = Quaternion.LookRotation(toTarget, Vector3.up); // 방향 벡터를 회전으로 전환

		float t = 1 - Mathf.Exp(-_rotateSharpness * Time.deltaTime);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);
	}

	private void TryAttack()
	{
		if (_nextAttackTime > Time.time) return;
		_nextAttackTime = Time.time + _enemyBase.AttackInterval;

		_isAttacking = true;
		_enemyAnimator.OnMove(0f);

		// 이동 정지
		_navMeshAgent.isStopped = true;
		_navMeshAgent.velocity = Vector3.zero;

		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_routine = StartCoroutine(Co_Attack());
	}

	private IEnumerator Co_Attack()
	{
		_enemyAnimator.OnAttack(); // 애니메이션 재생

		// 선딜레이
		float elasped = 0;
		while (elasped < _enemyBase.HitPredelay)
		{
			elasped += Time.deltaTime;
			yield return null;
		}

		// 공격 처리
		if (_enemyBase == null || 
			_enemyBase.IsDead ||
			_toTarget == Vector3.zero)
		{
			_routine = null;
			yield break;
		}

		if (Vector3.SqrMagnitude(_toTarget) <= _sqrAttackRange)
		{
			if (_targetTr.TryGetComponent(out IDamageable target))
			{
				target.TakeDamage(_enemyBase.AttackDamage);
			}

			if (_printLog)
			{
				Debug.Log($"[{name}] {target} 공격");
			}
		}

		// 후딜레이
		float post = Mathf.Max(0f, _enemyBase.AttackActionDuration - _enemyBase.HitPredelay);

		elasped = 0;
		while (elasped < post)
		{
			elasped += Time.deltaTime;
			yield return null;
		}

		_isAttacking = false; // 이동 정지 해제
		_routine = null;
	}

	private void UpdateMoveParam()
	{
		if (_enemyBase == null ||
			_enemyBase.IsDead ||
			_navMeshAgent == null ||
			_enemyAnimator == null) return;

		// 속도 벡터(velocity) 길이 = 현재 이동 속도
		float sqrMoveSpeed = _navMeshAgent.velocity.sqrMagnitude;
		_enemyAnimator.OnMove(sqrMoveSpeed);
	}

	private void OnDead()
	{
		_navMeshAgent.enabled = false;
	}
}