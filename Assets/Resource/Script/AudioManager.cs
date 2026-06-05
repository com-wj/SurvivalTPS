using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : Singleton<AudioManager>
{
	[Serializable]
	private class SoundFile
	{
		public string soundName;
		public AudioClip clip;

		[Range(0f, 1f)]
		public float volume = 1f;
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

	[Header("오디오 믹서")]
	[SerializeField] private AudioMixer _audioMixer;

	[Header("음소거 경계 슬라이더 값")]
	[SerializeField] private const float MUTE_THRESHOLD_VALUE = 0.001f;

	[Header("디버그")]
	[SerializeField] private bool _printLog = false;
	#endregion

	#region 내부 변수
	private readonly Dictionary<string, SoundFile> _nameToSound = new Dictionary<string, SoundFile>();
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
			SoundFile file = _bgmFiles[i];
			string fileName = file.soundName;

			if (string.IsNullOrEmpty(fileName) ||
				file.clip == null)
				continue;

			_nameToSound[fileName] = file;
		}

		for (int i = 0; i < _sfxFiles.Length; i++)
		{
			SoundFile file = _sfxFiles[i];
			string fileName = file.soundName;

			if (string.IsNullOrEmpty(fileName) ||
				file.clip == null)
				continue;

			_nameToSound[fileName] = file;
		}

		if (_printLog)
		{
			Debug.Log($"[{name}] 캐싱 완료.");
		}
	}

	private bool HaveAudio(string audioName)
	{
		if (!_nameToSound.ContainsKey(audioName))
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

		_bgmAudioSource.clip = _nameToSound[audioName].clip;
		_bgmAudioSource.Play();
	}

	public void PlayOneShotBGM(string audioName)
	{
		if (!HaveAudio(audioName))
			return;

		SoundFile file = _nameToSound[audioName];
		_bgmAudioSource.PlayOneShot(file.clip, file.volume);
	}

	public void Stop()
	{
		_bgmAudioSource.Stop();
	}

	public void PlayOneShot(string audioName)
	{
		if (!HaveAudio(audioName))
			return;

		SoundFile file = _nameToSound[audioName];
		_sfxAudioSource.PlayOneShot(file.clip, file.volume);
	}

	/// <summary>
	/// 오버로딩 : 외부에서 직접 오디오 클립 재생
	/// </summary>
	public void PlayOneShot(AudioClip clip, float volume = 1f)
	{
		_sfxAudioSource.PlayOneShot(clip, volume);
	}

	/// <summary>
	/// clip 목록에서 랜덤으로 재생시킴.
	/// </summary>
	public void PlayOneShotRandom(AudioClip[] clips, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
	{
		if (clips == null || clips.Length == 0) return;

		_sfxAudioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);

		int index = UnityEngine.Random.Range(0, clips.Length);
		_sfxAudioSource.PlayOneShot(clips[index], volume);

		_sfxAudioSource.pitch = 1.0f;
	}

	public void SetVolume(string audioParam, float sliderValue)
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
			Debug.Log($"[{name}] 볼륨 설정. {audioParam} : {dB}");
		}
		_audioMixer.SetFloat(audioParam, dB);
	}
}