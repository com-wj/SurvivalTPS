using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
	#region 인스펙터
	[Header("(this) 마스터 볼륨 슬라이더")]
	[SerializeField] private Slider _masterVolumeSlider;
	[Header("(this) 마스터 볼륨 슬라이더 TMP")]
	[SerializeField] private TMP_Text _masterText;

	[Header("(this) BGM 볼륨 슬라이더")]
	[SerializeField] private Slider _bgmVolumeSlider;
	[Header("(this) BGM 볼륨 슬라이더 TMP")]
	[SerializeField] private TMP_Text _bgmText;

	[Header("(this) SFX 볼륨 슬라이더")]
	[SerializeField] private Slider _sfxVolumeSlider;
	[Header("(this) SFX 볼륨 슬라이더 TMP")]
	[SerializeField] private TMP_Text _sfxText;
	#endregion

	void Awake()
	{
		if (_masterVolumeSlider == null ||
			_masterText == null ||
			_bgmVolumeSlider == null ||
			_bgmText == null ||
			_sfxVolumeSlider == null ||
			_sfxText == null)
		{
			Debug.LogWarning("SettingUI) 인스펙터 null 감지.");
			gameObject.SetActive(false);
			return;
		}
	}

	void OnEnable()
	{
		//if (AudioManager.Instance == null)
		{
			Debug.LogWarning("SettingUI) 오디오매니저 인스턴스 null.");
			gameObject.SetActive(false);
			return;
		}

		//_masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.1f);
		//_bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.1f);
		//_sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.1f);

		//AudioManager.Instance.PlayOneShot("PuaseUIPop");
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log("SettingUI) ESC감지. UI 종료");
			gameObject.SetActive(false);
		}
	}

	public void OnClickOptionQuitButton()
	{
		Debug.Log("SettingUI)Option 종료 버튼 클릭 감지");
		gameObject.SetActive(false);
	}

	void OnDisable()
	{
		//PlayerPrefs.SetFloat("MasterVolume", _masterVolumeSlider.value);
		//PlayerPrefs.SetFloat("BGMVolume", _bgmVolumeSlider.value);
		//PlayerPrefs.SetFloat("SFXVolume", _sfxVolumeSlider.value);
		//PlayerPrefs.Save();

		//AudioManager.Instance.PlayOneShot("PauseUIClose");
	}

	public void OnChangeMasterSliderValue(float value)
	{
		//AudioManager.Instance.SetVolume("MasterVolume", value);
		_masterText.text = (value >= 0.01) ? value.ToString("###%") : value.ToString("0%");
	}

	public void OnChangeBGMSliderValue(float value)
	{
		//AudioManager.Instance.SetVolume("BGMVolume", value);
		_bgmText.text = (value >= 0.01) ? value.ToString("###%") : value.ToString("0%");
	}

	public void OnChangeSFXSliderValue(float value)
	{
		//AudioManager.Instance.SetVolume("SFXVolume", value);
		_sfxText.text = (value >= 0.01) ? value.ToString("###%") : value.ToString("0%");
	}
}