using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code
{
	public class ControllableScroller : MonoBehaviour
	{
		[SF] private RectTransform viewPort;
		[SF] private RectTransform content;
		[SF] private Widget widgetPrefab;
		[SF] private float spacing;
		[SF] private float paddingTop;
		[SF] private float paddingBottom;
		
		[SF] private int previousPosition;
		[SF] private int newPosition;
		
		private int _widgetsAmount;
		
		[SF] private float currentScrollPosition;

		private float _widgetHeight;

		private TournamentHandler _tournamentHandler;
		private List<Widget> _widgets = new();
		
		private Widget _currentPlayerWidget;
		
		private void Start()
		{
			_tournamentHandler = new TournamentHandler();
			_tournamentHandler.GetTournamentUsers();
			
			_widgetHeight = widgetPrefab.GetComponent<RectTransform>().rect.height;

			SetupWidgets();
		}

		private void Update()
		{
			// UpdateWidgetsPosition();
		}

		private void SetupWidgets()
		{
			var widgetHeightWithSpacingAndPadding = _widgetHeight + spacing + paddingTop + paddingBottom;
			
			Debug.Log($"Widget with all: {widgetHeightWithSpacingAndPadding}");
			
			_widgetsAmount = Mathf.FloorToInt(viewPort.rect.height / _widgetHeight + spacing);

			currentScrollPosition = previousPosition;
			
			float viewPortCenter = viewPort.rect.height * 0.5f;
			float widgetHalfHeight = _widgetHeight * 0.5f;
			float initialWidgetPosition = viewPortCenter - widgetHalfHeight;

			for (var i = 0; i < _widgetsAmount; i++)
			{
				Widget widget = Instantiate(widgetPrefab, content);
				
				float widgetPositionInViewPort = initialWidgetPosition - paddingTop - (i * (_widgetHeight + spacing));
				widget.SetPosition(Vector2.up * widgetPositionInViewPort);
				
				_widgets.Add(widget);
			}
		}


		private void UpdateWidgetsPosition()
		{
			float viewPortCenter = viewPort.rect.height * 0.5f;
			float widgetHalfHeight = _widgetHeight * 0.5f;
			
			float playerPositionInViewPort = viewPortCenter - widgetHalfHeight - currentScrollPosition * viewPort.rect.height;
			
			_currentPlayerWidget.SetPosition(Vector2.up * playerPositionInViewPort);
		}
	}
}