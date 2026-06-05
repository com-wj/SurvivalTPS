using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TitleManager : MonoBehaviour
{
	[Header("디버그")]
	[SerializeField] private bool _printLog = false;

	private void Start()
	{
		if (AudioManager.Instance != null)
		{
			// 초기 볼륨 세팅
			SaveData data = DataManager.Load();

			AudioManager.Instance.SetVolume("MasterParam", data.MasterVolume);
			AudioManager.Instance.SetVolume("BGMParam", data.BGMVolume);
			AudioManager.Instance.SetVolume("SFXParam", data.SFXVolume);
			
			if (_printLog)
			{
				Debug.Log($"[{name}] 초기 볼륨 설정 완료");
			}

			// Title BGM 실행
			if (AudioManager.Instance != null)
			{
				string key = "Title";
				AudioManager.Instance.PlayLoop(key);

				if (_printLog)
				{
					Debug.Log($"[{name}] {key} 재생");
				}
			}
		}
	}
}