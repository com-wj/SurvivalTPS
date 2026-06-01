using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
	#region 인스펙터
	[Header("일시정지 UI")]
	[SerializeField] private GameObject _pausePanel;

	[Header("옵션 UI 패널")]
	[SerializeField] private GameObject _settingPanel;

	[Header("버튼")]
	[SerializeField] private Button _resumeButton;
	[SerializeField] private Button _settingButton;
	[SerializeField] private Button _goToTitleButton;
	[SerializeField] private Button _exitButton;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	void Awake()
	{
		if (_settingPanel == null ||
			_resumeButton == null ||
			_settingButton == null ||
			_goToTitleButton == null ||
			_exitButton == null
			)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null.");
			gameObject.SetActive(false);
			return;
		}
	}

	void OnEnable()
	{
		_resumeButton.onClick.AddListener(OnClickResumeButton);
		_settingButton.onClick.AddListener(OnClickOptionSettingButton);
		_goToTitleButton.onClick.AddListener(OnClickGotoTitleButton);
		_exitButton.onClick.AddListener(OnClickExitButton);
	}

	void OnDisable()
	{
		_resumeButton.onClick.RemoveListener(OnClickResumeButton);
		_settingButton.onClick.RemoveListener(OnClickOptionSettingButton);
		_goToTitleButton.onClick.RemoveListener(OnClickGotoTitleButton);
		_exitButton.onClick.RemoveListener(OnClickExitButton);
	}

	private void OnPauseUIOpen()
	{
		//AudioManager.Instance.PlayOneShot("PuaseUIPop");
		Time.timeScale = 0.0f;

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		_pausePanel.SetActive(true);
	}

	private void OnPauseUIClose()
	{
		//AudioManager.Instance.PlayOneShot("PauseUIClose");
		Time.timeScale = 1.0f;

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		_pausePanel.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (_printLog)
			{
				Debug.Log($"[{name}] ESC감지.");
			}

			if (!_settingPanel.activeSelf)
			{
				if(!_pausePanel.activeSelf)
					OnPauseUIOpen();
				else
					OnPauseUIClose();
			}
		}
	}

	public void OnClickResumeButton()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] Resume 버튼 감지");
		}
		//AudioManager.Instance.PlayOneShot("UIButtonClick");
		OnPauseUIClose();
	}

	public void OnClickOptionSettingButton()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] OptionSetting 버튼 감지");
		}
		_settingPanel.SetActive(true);
	}

	public void OnClickGotoTitleButton()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] GotoTitle 버튼 감지");
		}
		SceneFlowManager sceneFlowManager = SceneFlowManager.Instance;

		if (sceneFlowManager == null)
		{
			Debug.LogWarning($"[{name}] 씬 플로우 매니저 인스턴스 null");
			return;
		}

		OnPauseUIClose();
		//AudioManager.Instance.PlayOneShot("MoveSceneButtonClick");
		if (SceneFlowManager.Instance != null)
		{
			sceneFlowManager.TryLoadScene(ESceneID.Title);
			Time.timeScale = 0f;
		}
	}

	public void OnClickExitButton()
	{
		if (_printLog)
		{
			Debug.Log("Exit 버튼 클릭 감지");
		}
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}