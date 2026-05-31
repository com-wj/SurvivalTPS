using System.Collections.Generic;
using UnityEngine;

public class RoundManager : Singleton<RoundManager>
{
	#region 인스펙터
	[Header("목표 킬 수")]
	[SerializeField] private int _targetKillCount = 10;

	[Header("적 스포너")]
	[SerializeField] private EnemySpawner _enemySpawner;

	[Header("라운드 데이터")]
	[SerializeField] private List<RoundDataSO> _roundDatas = new List<RoundDataSO>();

	[Header("의존성")]
	[SerializeField] private ScoreManager _scoreManager;
	[SerializeField] private WaveManager _waveManager;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	protected override void Awake()
	{
		base.Awake();
		if (_roundDatas == null ||
			_roundDatas.Count == 0)
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
		if (_waveManager == null)
		{
			_waveManager = WaveManager.Instance;
		}

		_waveManager.InitWaveData(_roundDatas[0].WaveDatas);
	}

	private void CheckClearCondition(int killCount)
	{
		if (killCount >= _targetKillCount)
		{
			//OnRoundClear();
		}
	}

	public void OnRoundClear()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] 라운드 클리어.");
		}

		_scoreManager.ResetKillCount();
		_enemySpawner.StopAndClearEnemies();
	}
}