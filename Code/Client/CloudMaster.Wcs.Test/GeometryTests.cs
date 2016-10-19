using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudMaster.Wcs.Test
{
	[TestClass]
	public class GeometryTests
	{
		[TestMethod]
		public void TestGeometry()
		{
			try
			{
				var value = "F1M234.083,184.279L204.833,184.279C204.833,174.446204.833,164.612206.958,154.904209.083,145.196213.333,135.612217.583,126.029L243.583,136.529C240.333,144.279237.083,152.029235.499,159.987233.916,167.946233.999,176.112234.083,184.279z";
				PathFigureCollection.Parse(value);
			}
			catch (Exception ex)
			{
				throw;
			}
		
		}
	}
}
