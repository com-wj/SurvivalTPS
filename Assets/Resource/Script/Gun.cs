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

	[Header("사격")]
	[SerializeField] private EFireMode _fireMode; // 사격 모드
	[SerializeField] private int _damage; // 피해량
	[SerializeField] private float _fireInterval; // 발사 간격

	[Header("장전")]
	[SerializeField] private int _magCapacity; // 탄창 용량
	[SerializeField] private int _remainBulletCount; // 남은 장탄 수
	[SerializeField] private float _reloadInterval; // 장전 시간
	#endregion

	#region 내부 변수
	#endregion

	public Transform LeftHandMount => _leftHandMountTr != null ? _leftHandMountTr : null;
}