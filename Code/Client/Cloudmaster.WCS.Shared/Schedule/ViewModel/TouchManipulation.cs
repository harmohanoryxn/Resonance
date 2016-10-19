using System;

namespace WCS.Shared.Schedule
{
	internal class TouchManipulation
	{
		private double _xInternalClickOffset;

		public double StartPosition { get; set; }
		public DateTime LastUserModified { get; set; }
		public double DragDelta { get; set; }

		public TouchManipulation(double xInternalClickOffset, double startPosition, bool isInitialDrag)
		{
			_xInternalClickOffset = xInternalClickOffset;
			
			DragDelta = 0;
			StartPosition = startPosition;
			LastUserModified = DateTime.Now;
			IsInitialDrag = isInitialDrag;
		}

		internal void Update(double delta)
		{
			DragDelta = StartPosition - _xInternalClickOffset + delta;
			LastUserModified = DateTime.Now;
		}
		public bool IsInitialDrag { get; set; }
	}
}
