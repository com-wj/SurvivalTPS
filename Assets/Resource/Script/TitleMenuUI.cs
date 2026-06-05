using UnityEngine;
using UnityEngine.UI;

public class TitleMenuUI : MonoBehaviour
{
	#region 인스펙터
	[Header("옵션 UI 패널")]
	[SerializeField] private GameObject _settingPanel;

	[Header("버튼")]
	[SerializeField] private Button _startButton;
	[SerializeField] private Button _settingButton;
	[SerializeField] private Button _exitButton;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	private void Awake()
	{
		if (
			_settingPanel == null ||
			_startButton == null ||
			_settingButton == null ||
			_exitButton == null
			)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null.");
			gameObject.SetActive(false);
			return;
		}

		Time.timeScale = 1.0f;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	void OnEnable()
	{
		_startButton.onClick.AddListener(OnClickStartButton);
		_settingButton.onClick.AddListener(OnClickOptionSettingButton);
		_exitButton.onClick.AddListener(OnClickExitButton);
	}

	void OnDisable()
	{
		_startButton.onClick.RemoveListener(OnClickStartButton);
		_settingButton.onClick.RemoveListener(OnClickOptionSettingButton);
		_exitButton.onClick.RemoveListener(OnClickExitButton);
	}

	public void OnClickStartButton()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] Start 버튼 클릭 감지");
		}

		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayOneShot("UIButtonClick");
		}

		if (SceneFlowManager.Instance != null)
		{
			SceneFlowManager.Instance.TryLoadScene(ESceneID.Game);
		}
	}

	public void OnClickOptionSettingButton()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] Option 버튼 클릭 감지");
		}
		if (_settingPanel != null)
		{
			_settingPanel.SetActive(true);
		}
		else
		{
			Debug.LogWarning($"[{name}] 설정 창 null. 인스펙터 연결 확인");
		}
	}

	public void OnClickExitButton()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] Exit 버튼 클릭 감지");
		}
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}