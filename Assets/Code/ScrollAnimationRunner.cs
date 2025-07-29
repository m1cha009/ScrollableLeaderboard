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
		[SF] private Button _expandAnimButton;

		[SF] private RectTransform _content;
		
		[SF] private InfinityScrollController _infinityScrollController;

		[SF] private float _scrollAnimDuration;
		[SF] private float _shrinkAnimDuration;
		[SF] private float _expandAnimDuration;


		private void OnEnable()
		{
			_scrollAnimButton.onClick.AddListener(StartScrollAnimation);
			_shrinkAnimButton.onClick.AddListener(StartShrinkAnimation);
			_expandAnimButton.onClick.AddListener(StartExpandWidgetAnimation);
		}
		
		private void OnDisable()
		{
			_scrollAnimButton.onClick.RemoveListener(StartScrollAnimation);
			_shrinkAnimButton.onClick.RemoveListener(StartShrinkAnimation);
			_expandAnimButton.onClick.RemoveListener(StartExpandWidgetAnimation);
		}
		

		private void StartScrollAnimation()
		{
			var newPlayerPosition = _infinityScrollController.GetPlayerCenterPosition();

			_content.DOAnchorPosY(newPlayerPosition, _scrollAnimDuration).SetEase(Ease.InOutSine);
		}

		private void StartShrinkAnimation()
		{
			var lastPlayerRank = _infinityScrollController.LastPlayerRank;
			
			if (_infinityScrollController.TryGetPlayerWidget(lastPlayerRank, out Widget lastPlayerWidget))
			{
				lastPlayerWidget.transform
					.DOScaleY(0, _shrinkAnimDuration)
					.SetEase(Ease.InOutSine).OnComplete(() =>
					{
						RemoveLastPlayerWidget(lastPlayerWidget);
					});
				
				StartTranslateWidgetsToTopAnimation();
			}
			else
			{
				Debug.LogError("Last player widgets can't be get");
			}
		}

		private void RemoveLastPlayerWidget(Widget lastPlayerWidget)
		{
			_infinityScrollController.RemoveWidget(lastPlayerWidget);
		}
		
		private void StartTranslateWidgetsToTopAnimation()
		{
			var widgets = _infinityScrollController.GetBelowPlayerWidgets();
			
			var lastPlayerRank = _infinityScrollController.LastPlayerRank;
			var lastPlayerPosition = _infinityScrollController.GetPlayerPosition(lastPlayerRank);

			for (var index = 0; index < widgets.Count; index++)
			{
				Widget widget = widgets[index];
				widget.Rect.DOAnchorPosY(-lastPlayerPosition - (index * widget.GetHeight()), 2.5f);
			}
		}


		private void StartExpandWidgetAnimation()
		{
			var newPlayerRank = _infinityScrollController.NewPlayerRank;
			
			var newPlayerPosition = _infinityScrollController.GetPlayerPosition(newPlayerRank);
			
			if (_infinityScrollController.TryGetPlayerWidget(newPlayerRank, out Widget newPlayerWidget))
			{
				Debug.Log($"New player widget found");
				
				newPlayerWidget.transform.DOScale(Vector3.one * 2, _expandAnimDuration).SetEase(Ease.InOutSine);
			}
			else
			{
				Debug.LogError($"New player not found in the current list");
			}
			
			// start expanding it
		}
		
		private void StartTranslateWidgetsToBottomAnimation()
		{
			// move widgets below provided index
		}
		
	}
}