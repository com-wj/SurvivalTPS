using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[SerializeField] private PlayerBase _playerBase;

	public PlayerBase PlayerBase => _playerBase;

	protected override void Awake()
	{
		base.Awake();
		if (_playerBase == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}

	private void OnEnable()
	{
		if (_playerBase != null)
		{
			_playerBase.Dead += OnPlayerDead;
		}
	}

	private void OnDisable()
	{
		if (_playerBase != null)
		{
			_playerBase.Dead -= OnPlayerDead;
		}
	}

	private void OnPlayerDead()
	{
		if (RoundManager.Instance != null)
		{
			RoundManager.Instance.OnRoundFail();
		}
	}
}