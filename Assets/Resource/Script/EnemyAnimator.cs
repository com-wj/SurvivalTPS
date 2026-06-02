using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private EnemyBase _enemyBase;
	[SerializeField] private Animator _animator;

	[Header("애니메이터 파라미터")]
	[SerializeField] private string _paramMoveSpeed = "fMoveSpeed";
	[SerializeField] private string _paramAttack = "tAttack";
	[SerializeField] private string _paramDie = "tDie";

	[Header("애니메이터 튜닝")]
	[SerializeField] private float _speedDamp = 0.12f;
	#endregion

	#region 내부 변수
	private int _hashMoveSpeed;
	private int _hashAttack;
	private int _hashDie;

	private bool _hasAttackParam;
	#endregion

	private void Awake()
	{
		if (_enemyBase == null ||
			_animator == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}

		// 해시 캐싱
		_hashMoveSpeed = Animator.StringToHash(_paramMoveSpeed);

		_hasAttackParam = !string.IsNullOrEmpty(_paramAttack);
		if (_hasAttackParam)
		{
			_hashAttack = Animator.StringToHash(_paramAttack);
		}

		_hashDie = Animator.StringToHash(_paramDie);
	}

	private void OnEnable()
	{
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
	}

	// Animator에서 threshold를 제곱값으로 입력할 것.
	public void OnMove(float sqrMoveSpeed)
	{
		_animator.SetFloat(_hashMoveSpeed, sqrMoveSpeed, _speedDamp, Time.deltaTime);
	}

	public void SetMoveSpeedParam(float sqrMoveSpeed)
	{
		_animator.SetFloat(_hashMoveSpeed, sqrMoveSpeed);
	}

	public void OnAttack()
	{
		if (_hasAttackParam)
		{
			_animator.SetTrigger(_hashAttack);
		}
	}

	private void OnDead()
	{
		_animator.SetTrigger(_hashDie);
	}
}