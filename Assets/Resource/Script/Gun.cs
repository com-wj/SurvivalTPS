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
	[SerializeField] private int _damage; // 피해량
	[SerializeField] private float _fireInterval; // 발사 간격
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
	private Vector3 _toTarget; // 디버그용
	#endregion

	public Transform LeftHandMount => _leftHandMountTr != null ? _leftHandMountTr : null;
	public bool CanFire => Time.time > _lastFireTime + _fireInterval;

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

			SummonImpactEffect(hit);
		}

		// 총구 화염
		if (_muzzleFlash == null) return;

		_muzzleFlash.Play();
	}

	// 타격 이펙트
	private void SummonImpactEffect(RaycastHit hit)
	{

		int layer = hit.collider.gameObject.layer;
		// VFXManager.instance.SummonImpactEffect(layer);
	}

	private void OnDrawGizmos()
	{
		if (!_drawRay) return;

		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(_firePoint.position, _toTarget * _maxDistance);
	}
}