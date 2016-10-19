using System;

namespace WCS.Shared.Schedule
{
	/// <summary>
	/// Encapsulates all the mouse drag interaction
	/// </summary>
	internal class MouseManipulation
	{
		private double _xInternalClickOffset;

		public double StartPosition { get; set; }
		public double DragDelta { get; set; }
		public DateTime LastUserModified { get; set; }

		public MouseManipulation(double xInternalClickOffset, double startPosition, bool isInitialDrag)
		{
			_xInternalClickOffset = xInternalClickOffset;

			DragDelta = 0;
			StartPosition = startPosition;
			LastUserModified = DateTime.Now;
			IsInitialDrag = isInitialDrag;
		}

		internal void Update(double delta)
		{
			DragDelta = delta - _xInternalClickOffset;
			LastUserModified = DateTime.Now;

			//Console.Write("DELTA: ");
			//Console.WriteLine(DragDelta);
		}

		public bool HasMoved { get { return DragDelta != 0; } }

		public bool IsInitialDrag { get; set; }
	}
}
