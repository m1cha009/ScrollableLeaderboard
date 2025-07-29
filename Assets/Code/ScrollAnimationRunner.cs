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
		[SF] private float _translateAnimDuration;


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
				
				StartTranslateWidgetsAnimation(lastPlayerRank, true);
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


		private void StartExpandWidgetAnimation()
		{
			var newPlayerRank = _infinityScrollController.NewPlayerRank;
			
			var newPlayerPosition = _infinityScrollController.GetPlayerPosition(newPlayerRank);
			
			var widget = _infinityScrollController.GetFirstInactiveWidget();
			widget.Setup(newPlayerRank);
			widget.SetScaleY(0);
			widget.SetPosition(new Vector2(0, -newPlayerPosition));
			
			widget.transform.DOScaleY(1, _expandAnimDuration).SetEase(Ease.InOutSine);
			
			// Change below players rank

			StartTranslateWidgetsAnimation(newPlayerRank, false);
		}
		
		private void StartTranslateWidgetsAnimation(int playerRank, bool isMoveUp)
		{
			var widgets = _infinityScrollController.GetPlayerWidgetsBelowCurrentRank(playerRank);

			foreach (var widget in widgets)
			{
				if (isMoveUp)
				{
					widget.Rect.DOAnchorPosY( widget.GetPosition().y + widget.GetHeight(), _translateAnimDuration);
				}
				else
				{
					widget.Rect.DOAnchorPosY( widget.GetPosition().y - widget.GetHeight(), _translateAnimDuration);
				}
				
			}
		}
		
	}
}