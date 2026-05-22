using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[SerializeField] private Transform _playerTr;

	public Transform PlayerTr => _playerTr;

	protected override void Awake()
	{
		base.Awake();
		if (_playerTr == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}
}