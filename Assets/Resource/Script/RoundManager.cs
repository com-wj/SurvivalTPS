using TMPro;
using UnityEngine;

public class RoundManager : Singleton<RoundManager>
{
	#region 인스펙터
	[Header("목표 킬 수")]
	[SerializeField] private int _targetKillCount = 10;

	[Header("적 스포너")]
	[SerializeField] private EnemySpawner _enemySpawner;

	[Header("스폰 정보")]
	[SerializeField] private PooledObject _prefab;
	[SerializeField] private int _unitCountPerSpawn;
	[SerializeField] private float _spawnInterval;
	[SerializeField] private int _spawnCount;

	[Header("의존성")]
	[SerializeField] private ScoreManager _scoreManager;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	protected override void Awake()
	{
		base.Awake();
		if (_prefab == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			gameObject.SetActive(false);
			return;
		}
	}

	private void OnEnable()
	{
		if (_scoreManager == null && ScoreManager.Instance != null)
		{
			_scoreManager = ScoreManager.Instance;
		}

		if (_scoreManager != null)
		{
			_scoreManager.KillCountChanged += CheckClearCondition;
		}
	}

	private void OnDisable()
	{
		if (_scoreManager != null)
		{
			_scoreManager.KillCountChanged -= CheckClearCondition;
		}
	}
	private void Start()
	{
		if (_enemySpawner == null)
		{
			_enemySpawner = EnemySpawner.Instance;
		}
		if (_scoreManager == null)
		{
			_scoreManager = ScoreManager.Instance;
			_scoreManager.KillCountChanged += CheckClearCondition;
		}

		_enemySpawner.StartSpawn(_prefab, _unitCountPerSpawn, _spawnInterval, _spawnCount);
	}

	private void CheckClearCondition(int killCount)
	{
		if (killCount >= _targetKillCount)
		{
			if (_printLog)
			{
				Debug.Log($"[{name}] 라운드 클리어.");
			}

			_scoreManager.ResetKillCount();
			_enemySpawner.StopAndClearEnemies();
		}
	}
}