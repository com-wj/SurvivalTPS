using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
	#region 인스펙터
	[Header("적 스포너")]
	[SerializeField] private EnemySpawner _enemySpawner;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;

	[Header("의존성")]
	[SerializeField] private PlayerBase _playerBase;
	#endregion

	#region 내부 변수
	private IReadOnlyList<WaveDataSO> _waveDatas;

	private int _waveIndex;
	private float _waveTimer;
	private bool _isWaveRunning = false;
	#endregion

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
		_isWaveRunning = false;
	}

	private void Start()
	{
		if (_enemySpawner == null)
		{
			_enemySpawner = EnemySpawner.Instance;
		}
	}

	// 현재 라운드 웨이브 데이터 주입
	public void InitWaveData(IReadOnlyList<WaveDataSO> waves)
	{
		if (waves == null || waves.Count == 0) return;

		if (_printLog)
		{
			Debug.Log($"[{name}] 웨이브 데이터 초기화");
		}

		_waveDatas = waves;
		_waveIndex = 0;
		_waveTimer = 0f;
		_isWaveRunning = true;

		StartWave(_waveIndex);
	}

	// 웨이브 시작
	private void StartWave(int index)
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] {index}웨이브 시작");
		}

		_waveIndex = index;

		if (_enemySpawner == null)
		{
			if (EnemySpawner.Instance != null)
			{
				_enemySpawner = EnemySpawner.Instance;
			}
		}

		WaveDataSO wave = _waveDatas[index];

		_enemySpawner.StartSpawn(
			wave.Prefab,
			wave.UnitCountPerSpawn,
			wave.SpawnInterval,
			-1
			);
	}

	private void Update()
	{
		if (!_isWaveRunning) return;

		_waveTimer += Time.deltaTime;

		// 현재 웨이브 지속시간보다 길면
		if (_waveTimer >= _waveDatas[_waveIndex].Duration)
		{
			SetToNextWave();
		}
	}

	// 다음 웨이브로 전환
	private void SetToNextWave()
	{
		int nextIndex = _waveIndex + 1;
		if (nextIndex < _waveDatas.Count)
		{
			StartWave(nextIndex);
		}
		else // 모든 웨이브 클리어.
		{
			_isWaveRunning = false;

			WaveDataSO currentWave = _waveDatas[_waveIndex];
			if (!currentWave.IsBossWave) // 보스 웨이브는 시간 영향을 받지 않음.
			{
				if (RoundManager.Instance != null)
				{
					RoundManager.Instance.OnRoundClear();
				}
			}
		}
	}
}