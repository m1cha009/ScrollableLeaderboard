using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

using SF = UnityEngine.SerializeField;

namespace Code
{
	public class ScrollAnimationRunner : MonoBehaviour
	{
		[SF] private Button _startAnimationButton;

		[SF] private RectTransform _content;
		
		[SF] private InfinityScrollController _infinityScrollController;

		[SF] private float _animationDuration;


		private void OnEnable()
		{
			_startAnimationButton.onClick.AddListener(StartAnimation);
		}
		
		private void OnDisable()
		{
			_startAnimationButton.onClick.RemoveListener(StartAnimation);
		}
		

		private void StartAnimation()
		{
			var newPlayerPosition = _infinityScrollController.GetPlayerContentPosition();

			_content.DOAnchorPosY(newPlayerPosition, _animationDuration).SetEase(Ease.InOutSine);
		}
	}
}