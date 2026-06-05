using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
	[Serializable]
	private class SoundFile
	{
		public string soundName;
		public AudioClip clip;
	}

	#region 인스펙터
	[Header("BGM List")]
	[SerializeField] private SoundFile[] _bgmFiles;
	[Header("BGM Audio Source")]
	[SerializeField] private AudioSource _bgmAudioSource;

	[Header("SFX List")]
	[SerializeField] private SoundFile[] _sfxFiles;
	[Header("SFX Audio Source")]
	[SerializeField] private AudioSource _sfxAudioSource;

	/*
	[Header("타워 SFX Audio Source")]
	[SerializeField] private AudioSource _towerSfxAudioSource;
	[Header("타워 SFX 재생 간격")]
	[SerializeField] private float _towerSfxCooldown = 0.1f;
	*/

	[Header("오디오 믹서")]
	[SerializeField] private AudioMixer _audioMixer;

	[Header("음소거 경계 슬라이더 값")]
	[SerializeField] private const float MUTE_THRESHOLD_VALUE = 0.001f;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	#region 내부 변수
	private readonly Dictionary<string, AudioClip> _nameToClip = new Dictionary<string, AudioClip>();

	private bool _LoadingEnd = false;
	//private float _nextTowerSfxPlayTime = 0.0f;
	#endregion

	public AudioMixer Mixer => _audioMixer;

	protected override void Awake()
	{
		base.Awake();
		if (_bgmFiles == null ||
			_bgmFiles.Length == 0 ||
			_bgmAudioSource == null ||
			_sfxFiles == null ||
			_sfxFiles.Length == 0 ||
			_sfxAudioSource == null
			//||
			//_towerSfxAudioSource == null
			)
		{
			Debug.LogWarning($"[{name}] 인스펙터 null.");
			gameObject.SetActive(false);
			return;
		}

		CachingSound();
	}

	private void CachingSound()
	{
		for (int i = 0; i < _bgmFiles.Length; i++)
		{
			string fileName = _bgmFiles[i].soundName;
			AudioClip clip = _bgmFiles[i].clip;

			if (string.IsNullOrEmpty(fileName) ||
				clip == null)
				continue;

			_nameToClip[fileName] = clip;
		}

		for (int i = 0; i < _sfxFiles.Length; i++)
		{
			string fileName = _sfxFiles[i].soundName;
			AudioClip clip = _sfxFiles[i].clip;

			if (string.IsNullOrEmpty(fileName) ||
				clip == null)
				continue;

			_nameToClip[fileName] = clip;
		}

		if (_printLog)
		{
			Debug.Log($"[{name}] 캐싱 완료.");
		}
	}

	void Update()
	{
		if (_LoadingEnd ||
			_audioMixer == null)
			return;

		_LoadingEnd = true; // Mixer 로드 시점에 적용

		//SetVolume("MasterVolume", PlayerPrefs.GetFloat("MasterVolume", 0.1f));
		//SetVolume("BGMVolume", PlayerPrefs.GetFloat("BGMVolume", 0.1f));
		//SetVolume("SFXVolume", PlayerPrefs.GetFloat("SFXVolume", 0.1f));
	}

	private bool HaveAudio(string audioName)
	{
		if (!_nameToClip.ContainsKey(audioName))
		{
			Debug.LogWarning($"[{name}] [{audioName}] 사운드 파일이 없습니다.");
			return false;
		}

		return true;
	}

	public void PlayLoop(string audioName)
	{
		if (!HaveAudio(audioName))
			return;

		_bgmAudioSource.clip = _nameToClip[audioName];
		_bgmAudioSource.Play();
	}

	public void PlayOneShotBGM(string audioName)
	{
		if (!HaveAudio(audioName))
			return;

		_bgmAudioSource.PlayOneShot(_nameToClip[audioName]);
	}

	public void Stop()
	{
		_bgmAudioSource.Stop();
	}

	public void PlayOneShot(string audioName)
	{
		if (!HaveAudio(audioName))
			return;

		_sfxAudioSource.PlayOneShot(_nameToClip[audioName]);
	}

	/*
	public void PlayOneShotTower(string audioName)
	{
		if (!HaveAudio(audioName))
			return;

		if (Time.time < _nextTowerSfxPlayTime)
			return;

		_nextTowerSfxPlayTime = Time.time + _towerSfxCooldown;

		_towerSfxAudioSource.pitch += UnityEngine.Random.Range(-0.05f, 0.05f);
		_towerSfxAudioSource.PlayOneShot(_nameToClip[audioName]);

		_towerSfxAudioSource.pitch = 1.0f;
	}
	*/

	public void SetVolume(string audioName, float sliderValue)
	{
		float dB;
		if (sliderValue > MUTE_THRESHOLD_VALUE)
		{
			dB = Mathf.Log10(sliderValue) * 20;
		}
		else
		{
			dB = -80.0f;
		}

		if (_printLog)
		{
			Debug.Log($"[{name}] 볼륨 설정. {audioName} : {dB}");
		}
		_audioMixer.SetFloat(audioName, dB);
	}
}