using System;
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
		
		[SF] private int _currentPlayerIndex;

		[SF] private int _totalPlayers;

		private int _previousPlayerIndex;
		private float _widgetHeight;
		private List<Widget> _widgets = new();

		private void Update()
		{
			if (_currentPlayerIndex != _previousPlayerIndex)
			{
				Setup(_currentPlayerIndex);
				
				_previousPlayerIndex = _currentPlayerIndex;
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

		private int GetMaxWidgetsAmountInViewPort()
		{
			_widgetHeight = _widgetPrefab.GetComponent<RectTransform>().rect.height;
			var viewPort = _viewPort.rect.height;
			var widgetCount = Mathf.FloorToInt(viewPort / _widgetHeight);

			return widgetCount;
		}
		
		private void SpawnWidgets(int currentPlayerIndex, int widgetsAmount)
		{
			var extraWidget = 1;
			
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
			var currentWidgetPosition = _currentPlayerIndex * (_widgetHeight + _spacing);
			var viewPortCenter = _viewPort.rect.height * 0.5f;
			var widgetHalfHeight = _widgetHeight * 0.5f;

			var contentPosition = currentWidgetPosition - viewPortCenter + widgetHalfHeight;
			
			_contentRect.anchoredPosition = new Vector2(_contentRect.anchoredPosition.x, contentPosition);
		}
	}
}