using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

using SF = UnityEngine.SerializeField;

namespace Code
{
	public class ScrollAnimationRunner : MonoBehaviour
	{
		[SF] private Button _scrollAnimButton;
		[SF] private Button _shrinkAnimButton;

		[SF] private RectTransform _content;
		
		[SF] private InfinityScrollController _infinityScrollController;

		[SF] private float _scrollAnimDuration;
		[SF] private float _shrinkAnimDuration;


		private void OnEnable()
		{
			_scrollAnimButton.onClick.AddListener(StartScrollAnimation);
			_shrinkAnimButton.onClick.AddListener(StartShrinkAnimation);
		}
		
		private void OnDisable()
		{
			_scrollAnimButton.onClick.RemoveListener(StartScrollAnimation);
			_shrinkAnimButton.onClick.RemoveListener(StartShrinkAnimation);
		}
		

		private void StartScrollAnimation()
		{
			var newPlayerPosition = _infinityScrollController.GetNewPlayerContentPosition();

			_content.DOAnchorPosY(newPlayerPosition, _scrollAnimDuration).SetEase(Ease.InOutSine);
		}

		private void StartShrinkAnimation()
		{
			if (_infinityScrollController.TryGetLastPlayerWidget(out Widget lastPlayerWidget))
			{
				lastPlayerWidget.transform.DOScaleY(0, _shrinkAnimDuration).SetEase(Ease.InOutSine);
				StartMoveToCurrentPlayerAnimation();
			}
			else
			{
				Debug.LogError("Last player widgets can't be get");
			}
		}

		private void StartMoveToCurrentPlayerAnimation()
		{
			var widgets = _infinityScrollController.GetBelowPlayerWidgets();
			var lastPlayerPosition = _infinityScrollController.GetLastPlayerContentPosition();

			for (var index = 0; index < widgets.Count; index++)
			{
				Widget widget = widgets[index];
				widget.Rect.DOAnchorPosY(lastPlayerPosition - (index * widget.GetHeight()), 5f);
			}
		}
	}
}