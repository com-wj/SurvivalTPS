using UnityEngine;

public class ExpandSize : MonoBehaviour
{
	[SerializeField] private RectTransform _targetRectTr;
	[SerializeField] private RectTransform _TMPRectTr;
	[SerializeField] private float _spacing = 0f;

#if UNITY_EDITOR
	private void OnValidate()
	{
		FitRectSize();
	}
#endif
	private void Start()
	{
		FitRectSize();
	}

	private void FitRectSize()
	{
		if (_TMPRectTr == null || _targetRectTr == null) return;

		Vector2 sizeDelta = _TMPRectTr.sizeDelta;
		sizeDelta.x += _spacing;
		_targetRectTr.sizeDelta = sizeDelta;
	}
}