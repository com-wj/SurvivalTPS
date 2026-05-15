using UnityEngine;

public enum EFireMode
{
	None,
	Single,
	Burst,
	FullAuto,
}

public class Gun : MonoBehaviour
{
	#region 인스펙터
	[Header("왼손 파지 위치")]
	[SerializeField] private Transform _leftHandMountTr;

	[Header("격발 위치")]
	[SerializeField] private Transform _firePoint;

	[Header("사격")]
	[SerializeField] private EFireMode _fireMode; // 사격 모드
	[SerializeField] private float _damage = 1f; // 피해량
	[SerializeField, Min(0.1f)] private float _rpm = 600f; // 분당 발사 수
	[SerializeField] private float _maxDistance; // 유효 거리

	[Header("장전")]
	[SerializeField] private int _magCapacity; // 탄창 용량
	[SerializeField] private int _currentAmmo; // 남은 장탄 수
	[SerializeField] private float _reloadInterval; // 장전 시간

	[Header("FX")]
	[SerializeField] private ParticleSystem _muzzleFlash; // 격발 VFX
	//[SerializeField] private MuzzleFlash _muzzleFlash; // 격발 VFX
	[SerializeField] private AudioClip _fireAudio; // 격발 SFX

	[Header("레이어 마스크")]
	[SerializeField] private LayerMask _targetLayer;

	[Header("디버그")]
	[SerializeField] private bool _drawRay = false;
	#endregion

	#region 내부 변수
	private float _lastFireTime;
	private float _fireInterval; // 발사 간격
	private Vector3 _toTarget; // 디버그용
	#endregion

	public Transform LeftHandMount => _leftHandMountTr != null ? _leftHandMountTr : null;
	public bool CanFire => Time.time > _lastFireTime + _fireInterval;

	private void Awake()
	{
		if (_leftHandMountTr == null ||
			_firePoint == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}

		CalculateFireInterval();
	}

	private void CalculateFireInterval()
	{
		if (_rpm <= 0)
		{
			Debug.LogWarning($"[{name}] rpm 수치가 너무 낮음. [{_rpm}]");
			_rpm = 1f;
		}

		_fireInterval = 60f / _rpm;
	}

	public void OnFire(Vector3 targetPos)
	{
		_lastFireTime = Time.time;

		if (_firePoint == null) return;

		Vector3 toTarget = _toTarget = (targetPos - _firePoint.position).normalized;
		if (Physics.Raycast(_firePoint.position, toTarget, out RaycastHit hit, _maxDistance, _targetLayer))
		{
			IDamageable target = hit.collider.GetComponent<IDamageable>();

			if (target != null)
			{
				target.TakeDamage(_damage);
			}

			if (VFXManager.Instance != null)
			{
				VFXManager.Instance.PlayHitImpact(hit);
			}
		}

		// 총구 화염
		if (_muzzleFlash == null) return;

		_muzzleFlash.Play();
	}

	private void OnDrawGizmos()
	{
		if (!_drawRay) return;

		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(_firePoint.position, _toTarget * _maxDistance);
	}
}