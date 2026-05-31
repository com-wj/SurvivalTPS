using UnityEngine;

public class LeftHandIKHandler : MonoBehaviour
{
	#region 인스펙터
	[Header("의존성")]
	[SerializeField] private Animator _animator;
	[SerializeField] private Gun _gun;
	[SerializeField] private PlayerBase _playerBase;
	#endregion

	private void Awake()
	{
		if (_animator == null ||
			_gun == null ||
			_playerBase == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null 감지");
			gameObject.SetActive(false);
			return;
		}
	}

	public void EquipGun(Gun gun)
	{
		_gun = gun;
	}

	void OnAnimatorIK(int layerIndex)
	{
		if (_animator == null) return;

		// 왼손 마운트
		if (_gun == null) return;

		if (_gun.IsReloading) return;

		if (_playerBase.IsDead) return;

		Transform targetTr = _gun.LeftHandMount;

		if(targetTr == null) return;

		_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
		_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

		_animator.SetIKPosition(AvatarIKGoal.LeftHand, targetTr.position);
		_animator.SetIKRotation(AvatarIKGoal.LeftHand, targetTr.rotation);
	}
}