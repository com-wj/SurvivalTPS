using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private Camera _mainCamera;
	[SerializeField] private Gun _currentGun; // 장착한 총기
	#endregion

	private void Awake()
	{
		if (_mainCamera == null)
		{
			_mainCamera = Camera.main;
		}

		if (_mainCamera == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}

	public void TryShoot()
	{
		if (_currentGun == null)
		{
			Debug.LogWarning($"[{name}] 장착중인 총기가 없습니다.");
			return;
		}

		if (!_currentGun.CanFire) return;

		// 목표 지점 계산
		// 조준 위치를 카메라 화면 중앙 조준점과 일치시키기 위함

		Vector3 targetPos; // 목표 지점
		Ray cameraRay = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // 카메라 중앙 레이

		if (Physics.Raycast(cameraRay, out RaycastHit hit))
		{
			targetPos = hit.point;
		}
		else
		{
			targetPos = cameraRay.GetPoint(500f);
		}

		_currentGun.OnFire(targetPos);
	}
}