using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
	#region 인스펙터
	[Header("닫기 버튼")]
	[SerializeField] private Button _closeButton;

	[Header("마스터 볼륨")]
	[SerializeField] private Slider _masterVolumeSlider;
	[SerializeField] private TMP_Text _masterText;

	[Header("BGM 볼륨")]
	[SerializeField] private Slider _bgmVolumeSlider;
	[SerializeField] private TMP_Text _bgmText;

	[Header("SFX 볼륨")]
	[SerializeField] private Slider _sfxVolumeSlider;
	[SerializeField] private TMP_Text _sfxText;

	[Header("마우스 감도")]
	[SerializeField] private Slider _mouseSensitivitySlider;
	[SerializeField] private TMP_Text _mouseSensitivityText;

	[Header("의존성")]
	[SerializeField] private CameraController _normalCameraController;
	[SerializeField] private CameraController _aimCameraController;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	#region 내부 변수
	private SaveData _saveData;
	#endregion

	void Awake()
	{
		if (_closeButton == null ||
			_masterVolumeSlider == null ||
			_masterText == null ||
			_bgmVolumeSlider == null ||
			_bgmText == null ||
			_sfxVolumeSlider == null ||
			_sfxText == null ||
			_mouseSensitivitySlider == null ||
			_mouseSensitivityText == null
			)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null 감지.");
			gameObject.SetActive(false);
			return;
		}
	}

	void OnEnable()
	{
		_closeButton.onClick.AddListener(OnClickOptionQuitButton);
		_masterVolumeSlider.onValueChanged.AddListener(OnChangeMasterSliderValue);
		_bgmVolumeSlider.onValueChanged.AddListener(OnChangeBGMSliderValue);
		_sfxVolumeSlider.onValueChanged.AddListener(OnChangeSFXSliderValue);
		_mouseSensitivitySlider.onValueChanged.AddListener(OnChangeMouseSensitivitySliderValue);

		_saveData = DataManager.Load();

		_masterVolumeSlider.value = _saveData.MasterVolume;
		_bgmVolumeSlider.value = _saveData.BGMVolume;
		_sfxVolumeSlider.value = _saveData.SFXVolume;
		_mouseSensitivitySlider.value = _saveData.MouseSensitivity;

		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayOneShot("UIButtonClick");
		}
	}

	void OnDisable()
	{
		_saveData.MasterVolume = _masterVolumeSlider.value;
		_saveData.BGMVolume = _bgmVolumeSlider.value;
		_saveData.SFXVolume = _sfxVolumeSlider.value;
		_saveData.MouseSensitivity = _mouseSensitivitySlider.value;
		DataManager.Save(_saveData);

		_closeButton.onClick.RemoveListener(OnClickOptionQuitButton);
		_masterVolumeSlider.onValueChanged.RemoveListener(OnChangeMasterSliderValue);
		_bgmVolumeSlider.onValueChanged.RemoveListener(OnChangeBGMSliderValue);
		_sfxVolumeSlider.onValueChanged.RemoveListener(OnChangeSFXSliderValue);
		_mouseSensitivitySlider.onValueChanged.RemoveListener(OnChangeMouseSensitivitySliderValue);

		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayOneShot("UIClose");
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (_printLog)
			{
				Debug.Log($"[{name}] ESC감지. UI 종료");
			}
			gameObject.SetActive(false);
		}
	}

	private void OnClickOptionQuitButton()
	{
		if (_printLog)
		{
			Debug.Log($"[{name}] Option 종료 버튼 클릭 감지");
		}
		gameObject.SetActive(false);
	}

	private void OnChangeMasterSliderValue(float value)
	{
		UpdateVolume("MasterParam", value, _masterText);
	}

	private void OnChangeBGMSliderValue(float value)
	{
		UpdateVolume("BGMParam", value, _bgmText);
	}

	private void OnChangeSFXSliderValue(float value)
	{
		UpdateVolume("SFXParam", value, _sfxText);
	}

	// 볼륨 조절, 텍스트 갱신
	private void UpdateVolume(string audioParam, float value, TMP_Text tmp)
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.SetVolume(audioParam, value);
		}
		tmp.text = (value >= 0.01) ? value.ToString("###%") : value.ToString("0%");
	}

	private void OnChangeMouseSensitivitySliderValue(float value)
	{
		_mouseSensitivityText.text = value.ToString();

		if (_normalCameraController != null)
		{
			_normalCameraController.SetSensitivity(value);
		}
		if (_aimCameraController != null)
		{
			_aimCameraController.SetSensitivity(value);
		}
	}
}