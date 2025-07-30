using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

using SF = UnityEngine.SerializeField;

namespace Code
{
	public class ScrollAnimationRunner : MonoBehaviour
	{
		[SF] private Button _wholeSeqAnimButton;

		[SF] private RectTransform _content;
		
		[SF] private Widget _currentPlayerWidget;
		
		[SF] private InfinityScrollController _infinityScrollController;

		[SF] private float _scrollAnimDuration;
		[SF] private float _shrinkAnimDuration;
		[SF] private float _expandAnimDuration;
		[SF] private float _translateAnimDuration;
		[SF] private float _currentPlayerScaleAnimDuration;


		private void OnEnable()
		{
			_wholeSeqAnimButton.onClick.AddListener(HandleWholeSeqClicked);
		}

		private void OnDisable()
		{
			_wholeSeqAnimButton.onClick.RemoveListener(HandleWholeSeqClicked);
		}

		private void HandleWholeSeqClicked()
		{
			StartWholeSeqAnimation().Forget();
		}
		
		private async UniTaskVoid StartWholeSeqAnimation()
		{
			StartCurrentPlayerScaleWidgetAnimation().Forget();

			await StartShrinkAnimation();
			
			Debug.Log("First part finished. Starting second");

			await StartScrollAnimation();
			
			Debug.Log("Scroll finished");
			
			await StartExpandWidgetAnimation();
			
			Debug.Log("Second part finished");
			
		}
		
		
		private async UniTask StartScrollAnimation()
		{
			var newPlayerRank = _infinityScrollController.NewPlayerRank;
			
			var newPlayerPosition = _infinityScrollController.GetPlayerYCenterPositionInViewPort(newPlayerRank);

			await _content.DOAnchorPosY(newPlayerPosition, _scrollAnimDuration)
				.SetEase(Ease.InOutSine)
				.SetUpdate(true)
				.AsyncWaitForCompletion();
		}

		private async UniTask StartShrinkAnimation()
		{
			var lastPlayerRank = _infinityScrollController.LastPlayerRank;
			
			if (_infinityScrollController.TryGetPlayerWidget(lastPlayerRank, out Widget lastPlayerWidget))
			{
				await UniTask.WhenAll(new List<UniTask>()
				{
					ScaleDownAnimation(lastPlayerWidget),
					StartTranslateWidgetsAnimation(lastPlayerRank, true)
				});
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

		private async UniTask StartExpandWidgetAnimation()
		{
			var newPlayerRank = _infinityScrollController.NewPlayerRank;
			
			_infinityScrollController.UpdateWidgetData(newPlayerRank);
			_currentPlayerWidget.UpdateRank(newPlayerRank);
			
			var newPlayerPosition = _infinityScrollController.GetPlayerPosition(newPlayerRank);
			
			var widget = _infinityScrollController.GetInactiveWidget();
			widget.Setup(newPlayerRank);
			widget.SetScaleY(0);
			widget.SetPosition(new Vector2(0, -newPlayerPosition));

			await UniTask.WhenAll(new List<UniTask>
			{
				ScaleUpAnimation(widget),
				StartTranslateWidgetsAnimation(newPlayerRank, false)
			});
		}

		private async UniTask ScaleDownAnimation(Widget widget)
		{
			await widget.transform
				.DOScaleY(0, _shrinkAnimDuration)
				.SetEase(Ease.InOutSine).OnComplete(() =>
				{
					RemoveLastPlayerWidget(widget);
				})
				.AsyncWaitForCompletion();
		}

		private async UniTask ScaleUpAnimation(Widget widget)
		{
			await widget.transform.DOScaleY(1, _expandAnimDuration)
				.SetEase(Ease.InOutSine)
				.SetUpdate(true)
				.AsyncWaitForCompletion();
		}
		
		private async UniTask StartTranslateWidgetsAnimation(int playerRank, bool isMoveUp)
		{
			var widgets = _infinityScrollController.GetPlayerWidgetsBelowCurrentRank(playerRank);

			foreach (var widget in widgets)
			{
				if (isMoveUp)
				{
					widget.Rect.DOAnchorPosY(widget.GetPosition().y + widget.GetHeight(), _translateAnimDuration)
						.SetUpdate(true);
				}
				else
				{
					widget.Rect.DOAnchorPosY( widget.GetPosition().y - widget.GetHeight(), _translateAnimDuration).SetUpdate(true);
				}
				
			}

			await UniTask.CompletedTask;
		}
		
		private async UniTask StartCurrentPlayerScaleWidgetAnimation()
		{
			await _currentPlayerWidget.transform
				.DOScale(Vector3.one * 1.5f, _currentPlayerScaleAnimDuration)
				.SetEase(Ease.InOutSine)
				.SetUpdate(true)
				.OnComplete(() =>
				{
					_currentPlayerWidget.transform
						.DOScale(Vector3.one, _currentPlayerScaleAnimDuration)
						.SetDelay(_scrollAnimDuration)
						.SetEase(Ease.InOutSine)
						.SetUpdate(true);
				})
				.AsyncWaitForCompletion();
		}
		
	}
}