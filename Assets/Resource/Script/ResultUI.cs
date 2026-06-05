using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
	#region 인스펙터
	[Header("버튼")]
	[SerializeField] private Button _retryButton;
	[SerializeField] private Button _goToTitleButton;

	[Header("생존 여부")]
	[SerializeField] private TMP_Text _aliveText;
	[SerializeField] private string _successMessage = "SURVIVAL SUCCESS";
	[SerializeField] private string _failMessage = "SURVIVAL FAIL";

	//[Header("최종 라운드")]
	//[SerializeField] private TMP_Text _finalWave;

	[Header("플레이 타임")]
	[SerializeField] private TMP_Text _playTime;

	[Header("킬 수")]
	[SerializeField] private TMP_Text _killCount;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	void Awake()
	{
		if (_aliveText == null ||
			_playTime == null ||
			_killCount == null ||
			_playTime == null ||
			_killCount == null)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null.");
			gameObject.SetActive(false);
			return;
		}

		SetResultValue();

		AudioManager.Instance.Stop();
		//AudioManager.Instance.PlayOneShotBGM("result");
		Time.timeScale = 1.0f;

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	void OnEnable()
	{
		_retryButton.onClick.AddListener(OnClickRetryButton);
		_goToTitleButton.onClick.AddListener(OnClickGoToTitleButton);
	}

	void OnDisable()
	{
		_retryButton.onClick.RemoveListener(OnClickRetryButton);
		_goToTitleButton.onClick.RemoveListener(OnClickGoToTitleButton);
	}

	private void SetResultValue()
	{
		ScoreManager score = ScoreManager.Instance;
		if (score == null) return;

		_aliveText.text = score.IsSurvive ? _successMessage : _failMessage;

		//_finalWaveValue.text = score.FinalWave.ToString();
		_playTime.text = GetTimeToString(score.SurvivalTime);
		_killCount.text = score.TotalKillCount.ToString();
	}

	public string GetTimeToString(float time)
	{
		int hour = (int)time / 3600;
		int min = (int)time / 60;
		float sec = time % 60;

		string strTime = "";
		if (hour > 0)
		{
			strTime = $"{hour}:{min:D2}:{sec:00}";
		}
		else if (min > 0)
		{
			strTime = $"{min:D2}:{sec:00}";
		}
		else
		{
			strTime = $"{sec:00.00}";
		}

		return strTime;
	}

	public void OnClickRetryButton()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] Retry 버튼 클릭 감지");
		}

		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayOneShot("UIButtonClick");
		}

		if (SceneFlowManager.Instance != null)
		{
			SceneFlowManager.Instance.TryLoadScene(ESceneID.Game);
			HideCursor();
		}
	}

	public void OnClickGoToTitleButton()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] GoToTitle 버튼 클릭 감지");
		}

		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayOneShot("UIButtonClick");
		}

		if (SceneFlowManager.Instance != null)
		{
			SceneFlowManager.Instance.TryLoadScene(ESceneID.Title);
			HideCursor();
		}
	}

	private void HideCursor()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
}