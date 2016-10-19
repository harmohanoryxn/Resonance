using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCS.Core
{
	public interface ITimeDefinition : IIdentifable, IComparable<ITimeDefinition>
	{
		TimeSpan BeginTime { get; }
		TimeSpan Duration { get; }
	}
}
