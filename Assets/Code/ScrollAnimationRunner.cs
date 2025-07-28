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
			var newPlayerPosition = _infinityScrollController.GetPlayerContentPosition();

			_content.DOAnchorPosY(newPlayerPosition, _scrollAnimDuration).SetEase(Ease.InOutSine);
		}

		private void StartShrinkAnimation()
		{
			if (_infinityScrollController.TryGetLastPlayerWidget(out Widget lastPlayerWidget))
			{
				lastPlayerWidget.transform.DOScaleY(0, _shrinkAnimDuration).SetEase(Ease.InOutSine);
			}
			else
			{
				Debug.LogError("Last player widgets can't be get");
			}
		}
	}
}