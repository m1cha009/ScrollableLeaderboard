using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code
{
	public class InfinityScrollController : MonoBehaviour
	{
		[SF] private Widget _widgetPrefab;
		[SF] private RectTransform _viewPort;
		[SF] private RectTransform _contentRect;
		[SF] private float _spacing;
		
		[SF] private int _lastPlayerIndex;
		[SF] private int _newPlayerIndex;

		[SF] private int _totalPlayers;

		private int _previousPlayerIndex;
		private float _widgetHeight;
		private List<Widget> _widgets = new();
		
		private int _firstWidgetIndex;
		private int _lastWidgetIndex;
		
		private Vector2 _previousContentPosition = Vector2.zero;

		private void Start()
		{
			_previousContentPosition = _contentRect.anchoredPosition;
		}

		private void Update()
		{
			if (_lastPlayerIndex != _previousPlayerIndex)
			{
				Setup(_lastPlayerIndex);
				
				_previousPlayerIndex = _lastPlayerIndex;
				
				_previousContentPosition = _contentRect.anchoredPosition;
			}

			if (IsContentNeedUpdate())
			{
				_previousContentPosition = _contentRect.anchoredPosition;
			}
		}

		private void CleanWidgets()
		{
			foreach (var widget in _widgets)
			{
				Destroy(widget.gameObject);
			}
			
			_widgets.Clear();
		}

		private void Setup(int currentPlayerIndex)
		{
			CleanWidgets();
			
			var widgetsAmount = GetMaxWidgetsAmountInViewPort();
			
			SpawnWidgets(currentPlayerIndex, widgetsAmount);
			
			SetupContentSize();

			AlignContentCurrentPlayerWithViewPort();

		}

		public float GetPlayerContentPosition()
		{
			var currentWidgetPosition = _newPlayerIndex * (_widgetHeight + _spacing);
			var viewPortCenter = _viewPort.rect.height * 0.5f;
			var widgetHalfHeight = _widgetHeight * 0.5f;
			
			var contentPosition = currentWidgetPosition - viewPortCenter + widgetHalfHeight;

			return contentPosition;
		}
		
		private int GetMaxWidgetsAmountInViewPort()
		{
			_widgetHeight = _widgetPrefab.GetComponent<RectTransform>().rect.height;
			var viewPort = _viewPort.rect.height;
			var widgetCount = Mathf.FloorToInt(viewPort / _widgetHeight);

			return widgetCount;
		}
		
		private void SpawnWidgets(int currentPlayerIndex, int widgetsAmount)
		{
			var extraWidget = 2;
			
			var halfAmount = Mathf.FloorToInt(widgetsAmount / 2f);
			var startIndex = currentPlayerIndex - halfAmount - extraWidget;
			var endIndex = currentPlayerIndex + halfAmount + extraWidget + 1; 
			
			Debug.Log($"Current Player Index: {currentPlayerIndex} Start: {startIndex}, End: {endIndex}, Amount: {widgetsAmount}");
			
			for (int i = startIndex; i < endIndex; i++)
			{
				var widget = SpawnWidget(i);
				
				widget.SetPosition(new Vector2(0, -i * (_widgetHeight + _spacing)));
				
				_widgets.Add(widget);
			}
			
			_firstWidgetIndex = 0;
			_lastWidgetIndex = _widgets.Count - 1;
		}

		private Widget SpawnWidget(int index)
		{
			var widget = Instantiate(_widgetPrefab, _contentRect);
			widget.SetName(index);

			return widget;
		}

		private void SetupContentSize()
		{
			var totalHeight = _totalPlayers * (_widgetHeight + _spacing);
			_contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, totalHeight);
		}

		private void AlignContentCurrentPlayerWithViewPort()
		{
			var currentWidgetPosition = _lastPlayerIndex * (_widgetHeight + _spacing);
			var viewPortCenter = _viewPort.rect.height * 0.5f;
			var widgetHalfHeight = _widgetHeight * 0.5f;

			var contentPosition = currentWidgetPosition - viewPortCenter + widgetHalfHeight;
			
			_contentRect.anchoredPosition = new Vector2(_contentRect.anchoredPosition.x, contentPosition);
		}

		private bool IsContentNeedUpdate()
		{
			var currentContentPosition = _contentRect.anchoredPosition;

			var upperThreshold = _previousContentPosition.y + _widgetHeight + _spacing;
			var lowerThreshold = _previousContentPosition.y - _widgetHeight - _spacing;
			
			if (_contentRect.anchoredPosition.y > upperThreshold)
			{
				MoveWidgetToBottom();

				return true;
			}
			
			if (_contentRect.anchoredPosition.y < lowerThreshold)
			{
				Debug.Log($"Moved widget. {lowerThreshold}, Content pos: {_contentRect.anchoredPosition.y}");
				
				MoveWidgetToTop();
				
				return true;
			}

			return false;
		}

		private void MoveWidgetToBottom()
		{
			var firstWidget = _widgets[_firstWidgetIndex];
			var lastWidget = _widgets[_lastWidgetIndex];
			
			var newPosition = lastWidget.GetPosition().y + -lastWidget.GetHeight() + _spacing;
			
			firstWidget.SetPosition(new Vector2(0, newPosition));
			firstWidget.SetName(lastWidget.GetIndex() + 1);
			
			_lastWidgetIndex = _firstWidgetIndex;
			_firstWidgetIndex = (_firstWidgetIndex + 1) % _widgets.Count;
		}
		
		private void MoveWidgetToTop()
		{
			Debug.Log($"Move to the top");

			var firstWidget = _widgets[_firstWidgetIndex];
			var lastWidget = _widgets[_lastWidgetIndex];
			
			var newPosition = firstWidget.GetPosition().y + firstWidget.GetHeight() + _spacing;
			lastWidget.SetPosition(new Vector2(0, newPosition));
			lastWidget.SetName(firstWidget.GetIndex() - 1);
			
			_firstWidgetIndex = _lastWidgetIndex;
			_lastWidgetIndex = (_lastWidgetIndex - 1 + _widgets.Count) % _widgets.Count;
		}

	}
}