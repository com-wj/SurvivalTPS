using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public enum ESceneID
{
	Title,
	Game,
	Result,
}

public class SceneFlowManager : Singleton<SceneFlowManager>
{
	#region 인스펙터
	[Header("씬 목록")]
	[SerializeField] private AssetReference _titleSceneRef;
	[SerializeField] private AssetReference _gameSceneRef;
	[SerializeField] private AssetReference _resultSceneRef;

	[Header("씬 로드 모드")]
	[SerializeField] private LoadSceneMode _loadMode = LoadSceneMode.Single;

	[Header("로딩 완료 후 즉시 전환")]
	[SerializeField] private bool _activeOnLoad = false;

	[Header("페이드 캔버스")]
	[SerializeField] private CanvasGroup _fadeGroup;
	[SerializeField] private float _fadeDuration = 0.5f;

	[Header("디버그")]
	[SerializeField] private bool _printLog = true;
	[SerializeField] private bool _debugMode = false;
	#endregion

	#region 내부 변수
	private bool _isLoading = false;

	//private AsyncOperationHandle<SceneInstance> _loadedSceneHandle;
	//private bool _hasLoadedScene = false;

	private CancellationTokenSource _cts;
	#endregion

	protected override void Awake()
	{
		base.Awake();
		if (_fadeGroup == null)
		{
			Debug.LogWarning($"[{name} 인스펙터 null]");
		}
	}

	private void OnDisable()
	{
		DisposeCTS();
	}

	private void DisposeCTS()
	{
		if (_cts == null) return;

		if (!_cts.IsCancellationRequested)
		{
			_cts.Cancel();
		}

		_cts.Dispose(); // 반납. 메모리 릭 방지.
		_cts = null;
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (_debugMode)
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				TryLoadScene(ESceneID.Title);
			}
			if (Input.GetKeyDown(KeyCode.F2))
			{
				TryLoadScene(ESceneID.Game);
			}
			if (Input.GetKeyDown(KeyCode.F3))
			{
				TryLoadScene(ESceneID.Result);
			}
		}
	}
#endif

	// 씬 로드
	public void TryLoadScene(ESceneID sceneId)
	{
		if (_isLoading) return;

		AssetReference sceneRef = null;
		switch (sceneId)
		{
			case ESceneID.Title:
				sceneRef = _titleSceneRef;
				break;
			case ESceneID.Game:
				sceneRef = _gameSceneRef;
				break;
			case ESceneID.Result:
				sceneRef = _resultSceneRef;
				break;
		}

		if (!sceneRef.RuntimeKeyIsValid())
		{
			Debug.LogWarning($"[{name}] 인스펙터 null");
			return;
		}

		DisposeCTS();
		_cts = new CancellationTokenSource();

		LoadSceneAysnc(sceneRef, _cts.Token).Forget();
	}

	private async UniTaskVoid LoadSceneAysnc(AssetReference sceneRef, CancellationToken token)
	{
		_isLoading = true;
		AsyncOperationHandle<SceneInstance> handle = default;

		try
		{
			await FadeAsync(1f, _fadeDuration, token); // 페이드 아웃

			if (_printLog)
			{
				Debug.Log($"[{name}] 씬 로드 시작");
			}

			handle = Addressables.LoadSceneAsync(sceneRef, _loadMode, _activeOnLoad);

			await handle.Task.AsUniTask().AttachExternalCancellation(token);

			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				/*
				if (_hasLoadedScene && _loadedSceneHandle.IsValid())
				{
					Addressables.Release(_loadedSceneHandle);
				}

				_loadedSceneHandle = handle;
				_hasLoadedScene = true;
				*/

				string loadedSceneName = handle.Result.Scene.name;
				if (_printLog)
				{
					Debug.Log($"[{name}] ({loadedSceneName}) 씬 로드 완료");
				}

				await UniTask.Yield(PlayerLoopTiming.Update, token);

				await FadeAsync(0f, _fadeDuration, token); // 페이드 인
			}
			else
			{
				Addressables.Release(handle);
				Debug.LogWarning($"[{name}] 씬 로드 실패");
			}
		}
		catch (OperationCanceledException)
		{
			Debug.Log($"[{name}] 씬 로드 취소");

			if (handle.IsValid())
			{
				Addressables.Release(handle);
			}
		}
		catch (Exception e)
		{
			Debug.LogError($"[{name}] 예외 발생 : {e.Message}");

			if (handle.IsValid())
			{
				Addressables.Release(handle);
			}
		}
		finally
		{
			_isLoading = false;
			if (_fadeGroup != null)
			{
				_fadeGroup.alpha = 0f;
				_fadeGroup.gameObject.SetActive(false);
			}
		}
	}

	private async UniTask FadeAsync(float targetAlpha, float duration, CancellationToken token)
	{
		if (_fadeGroup == null) return;

		float startAlpha = _fadeGroup.alpha;

		if (duration <= 0)
		{
			_fadeGroup.alpha = targetAlpha;
			if (targetAlpha <= 0.01f)
			{
				_fadeGroup.gameObject.SetActive(false);
			}
			return;
		}

		if (targetAlpha >= 0.01f)
		{
			_fadeGroup.gameObject.SetActive(true);
		}

		float t = 0f;
		while (t < duration)
		{
			t += Time.unscaledDeltaTime;
			float lerpT = Mathf.Clamp01(t / duration);
			_fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerpT);

			await UniTask.Yield(PlayerLoopTiming.Update, token);
		}

		_fadeGroup.alpha = targetAlpha;
		if (targetAlpha <= 0.01f)
		{
			_fadeGroup.gameObject.SetActive(false);
		}
	}
}