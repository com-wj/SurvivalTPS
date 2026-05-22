using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private Animator _animator;
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private PlayerShooter _playerShooter;

	[Header("애니메이터 파라미터")]
	// 이동
	[SerializeField] private string _paramMoveX = "fMoveX";
	[SerializeField] private string _paramMoveY = "fMoveY";

	// 점프
	[SerializeField] private string _paramJump = "tJump";
	[SerializeField] private string _paramVerticalVel = "fVerticalVel";
	[SerializeField] private string _paramIsGrounded = "bGrounded";

	// 조준 및 사격
	[SerializeField] private string _paramAim = "bAim";
	[SerializeField] private string _paramFire = "tFire";

	// 재장전
	[SerializeField] private string _paramReload = "tReload";
	[SerializeField] private string _paramReloadSpeed = "fReloadSpeed";

	[Header("애니메이션 태그")]
	[SerializeField] private string _tagHardLanding = "HardLanding";

	[Header("애니메이터 튜닝")]
	[SerializeField] private float _speedDamp = 0.12f;
	#endregion

	#region 내부 변수
	private int _hashMoveX;
	private int _hashMoveY;

	private int _hashJump;
	private int _hashVerticalVel;
	private int _hashIsGrounded;
	private int _hashHardLand;

	private int _hashAim;
	private int _hashFire;

	private int _hashReload;
	private int _hashReloadSpeed;

	private bool _hasJumpParam;
	private bool _hasAimParam;
	#endregion

	private void Awake()
	{
		if (_animator == null ||
			_playerController == null ||
			_playerShooter == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}

		_hashMoveX = Animator.StringToHash(_paramMoveX);
		_hashMoveY = Animator.StringToHash(_paramMoveY);

		_hasJumpParam = !string.IsNullOrEmpty(_paramJump) &&
			!string.IsNullOrEmpty(_paramIsGrounded) &&
			!string.IsNullOrEmpty(_paramVerticalVel) &&
			!string.IsNullOrEmpty(_tagHardLanding)
			;
		if (_hasJumpParam)
		{
			_hashJump = Animator.StringToHash(_paramJump);
			_hashIsGrounded = Animator.StringToHash(_paramIsGrounded);
			_hashVerticalVel = Animator.StringToHash(_paramVerticalVel);
			_hashHardLand = Animator.StringToHash(_tagHardLanding);
		}

		_hasAimParam = !string.IsNullOrEmpty(_paramAim) &&
			!string.IsNullOrEmpty(_paramFire) &&
			!string.IsNullOrEmpty(_paramReload) &&
			!string.IsNullOrEmpty(_paramReloadSpeed);
		if (_hasAimParam)
		{
			_hashAim = Animator.StringToHash(_paramAim);
			_hashFire = Animator.StringToHash(_paramFire);
			_hashReload = Animator.StringToHash(_paramReload);
			_hashReloadSpeed = Animator.StringToHash(_paramReloadSpeed);
		}
	}

	private void OnEnable()
	{
		if (_playerController != null)
		{
			_playerController.AimChanged += OnAim;
		}
		if (_playerShooter != null)
		{
			_playerShooter.Fire += OnFire;
			_playerShooter.Reload += OnReload;
		}
	}

	private void OnDisable()
	{
		if (_playerController != null)
		{
			_playerController.AimChanged -= OnAim;
		}
		if (_playerShooter != null)
		{
			_playerShooter.Fire -= OnFire;
			_playerShooter.Reload -= OnReload;
		}
	}

	public void OnMove(float moveX, float moveY)
	{
		_animator.SetFloat(_hashMoveX, moveX, _speedDamp, Time.deltaTime);
		_animator.SetFloat(_hashMoveY, moveY, _speedDamp, Time.deltaTime);
	}

	public void OnJump()
	{
		if (_hasJumpParam)
		{
			_animator.SetTrigger(_hashJump);
		}
	}

	public void OnAim(bool aiming)
	{
		if (_hasAimParam)
		{
			_animator.SetBool(_hashAim, aiming);
		}
	}

	public void OnFire()
	{
		if (_hasAimParam)
		{
			_animator.SetTrigger(_hashFire);
		}
	}

	public void OnReload(float animSpeedMultiplier)
	{
		if (_hasAimParam)
		{
			_animator.SetFloat(_hashReloadSpeed, animSpeedMultiplier);
			_animator.SetTrigger(_hashReload);
		}
	}

	public void UpdateAirParam(bool isGrounded, float verticalVel)
	{
		if (_hasJumpParam)
		{
			_animator.SetBool(_hashIsGrounded, isGrounded);
			_animator.SetFloat(_hashVerticalVel, verticalVel);
		}
	}

	public bool IsLanding()
	{
		return _animator.GetCurrentAnimatorStateInfo(0).tagHash == _hashHardLand;
	}
}