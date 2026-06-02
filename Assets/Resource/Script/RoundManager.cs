using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : Singleton<RoundManager>
{
	#region 인스펙터
	[Header("적 스포너")]
	[SerializeField] private EnemySpawner _enemySpawner;

	[Header("라운드 데이터")]
	[SerializeField] private List<RoundDataSO> _roundDatas = new List<RoundDataSO>();

	[Header("현재 라운드")]
	[SerializeField] private int _roundIndex = 0;

	[Header("의존성")]
	[SerializeField] private ScoreManager _scoreManager;
	[SerializeField] private WaveManager _waveManager;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	[SerializeField] private bool _debugMode = false;
	#endregion

	#region 내부 변수
	private Coroutine _routine;
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
		if (ScoreManager.Instance != null)
		{
			if (
				_scoreManager == null
				||
			(_scoreManager != null &&
			_scoreManager != ScoreManager.Instance)
			) // Missing
			{
				_scoreManager = ScoreManager.Instance;
			}
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

		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
	}
	private void Start()
	{
		if (_enemySpawner == null)
		{
			_enemySpawner = EnemySpawner.Instance;
		}
		if (_scoreManager == null ||
			_scoreManager != ScoreManager.Instance) // Missing
		{
			_scoreManager = ScoreManager.Instance;
			_scoreManager.KillCountChanged += CheckClearCondition;
		}

		if (_waveManager == null)
		{
			_waveManager = WaveManager.Instance;
		}

		_scoreManager.Init();
		
		RoundStart(_roundIndex);
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (_debugMode)
		{
			if (Input.GetKeyDown(KeyCode.F6))
			{
				Debug.Log($"[{name}] 라운드 클리어");
				OnRoundClear();
			}
		}
	}
#endif

	private void CheckClearCondition(int killCount)
	{
		//if (killCount >= _targetKillCount)
		{
			//OnRoundClear();
		}
	}

	private void RoundStart(int roundIndex)
	{
		_waveManager.InitWaveData(_roundDatas[roundIndex].WaveDatas);
		_scoreManager.StartTimer();
	}

	public void OnRoundClear()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] 라운드 클리어.");
		}

		_waveManager.SetWaveRunning(false);
		_scoreManager.StopTimer();
		_scoreManager.ResetKillCount();
		_enemySpawner.StopAndClearEnemies();

		_roundIndex++;
		bool isFinalRound = (_roundIndex == _roundDatas.Count);

		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_routine = StartCoroutine(Co_RoundEnd(5f, isFinalRound));
	}

	public void OnRoundFail()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] 라운드 실패.");
		}

		_waveManager.SetWaveRunning(false);
		_scoreManager.StopTimer();
		_scoreManager.ResetKillCount();

		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		_routine = StartCoroutine(Co_RoundEnd(3f, true));
	}

	private IEnumerator Co_RoundEnd(float time, bool loadResult)
	{
		float elasped = 0;
		while (elasped < time)
		{
			elasped += Time.deltaTime;
			yield return null;
		}

		if (loadResult)
		{
			if (SceneFlowManager.Instance != null)
			{
				SceneFlowManager.Instance.TryLoadScene(ESceneID.Result);
			}
			_routine = null;
			yield break;
		}

		// 다음 라운드 시작 코드
		RoundStart(_roundIndex);
		_routine = null;
	}
}