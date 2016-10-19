using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Path = System.Windows.Shapes.Path;

namespace Cloudmaster.WCS.Galway.Core
{
	public class RfidMapResource
	{
		#region Public
		
		public static Brush GetLocationBrush(string location)
		{
			var canvas = GetCanvas(location);
			return new VisualBrush(canvas);
		}
		public static Brush GetLocationAnnotationBrush(string wardCode)
		{
			var canvas = GetCanvas(string.Format("{0}_Labels", wardCode));
			return new VisualBrush(canvas);
		}

		public static Path GetLocationPath(string locationCode)
		{
			return GetPath(locationCode);
		}

		#endregion

		#region Interpret the layout file

		private static volatile XDocument _instance;
		private static object _syncRoot = new Object();

		public static XDocument Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_syncRoot)
					{
						if (_instance == null)
							_instance = XDocument.Load(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Media\Schematics\rfidfloorplan.xml");
					}
				}

				return _instance;
			}
		}
		private static Path GetPath(string pathName)
		{
			try
			{
				var path = (from c in Instance.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Path")
							where c.HasAttributes && c.Attributes().Any(a => a.Value == pathName)
							select c).FirstOrDefault();

				if (path == null) return null;
				var pathXml = path.ToString();
				var pathStream = new MemoryStream(ASCIIEncoding.Default.GetBytes(pathXml));
				var xmlReader = XmlReader.Create(pathStream);
				return (Path)XamlReader.Load(xmlReader);
			}
			catch (Exception ex)
			{

				throw;
			}

		}

		public static Geometry GetLocationGeometry(string locationName)
		{
			try
			{
				var canvas = (from c in Instance.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Canvas")
							  where c.HasAttributes && c.Attributes().Any(a => a.Value == locationName)
							  select c).FirstOrDefault();

				if (canvas == null) return null;

				var path = (from p in canvas.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Path")
							 select p).FirstOrDefault();

				if (path == null) return null;


				var dataAttribute = path.Attributes("Data").FirstOrDefault();
				if (dataAttribute == null) return null;
				return Geometry.Parse(dataAttribute.Value);
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public static Canvas GetCanvas(string locationName)
		{
			try
			{
				var canvas = (from c in Instance.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Canvas")
							  where c.HasAttributes && c.Attributes().Any(a => a.Value == locationName)
							  select c).FirstOrDefault();

				if (canvas == null) return null;
				var canvasXml = canvas.ToString();
				var canvasStream = new MemoryStream(Encoding.Default.GetBytes(canvasXml));
				var xmlReader = XmlReader.Create(canvasStream);
				var canvasCtrl = (Canvas) XamlReader.Load(xmlReader);
				return canvasCtrl;
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public static double GetCanvasWidth(string locationName)
		{
			try
			{
				var canvas = (from c in Instance.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Canvas")
							  where c.HasAttributes && c.Attributes().Any(a => a.Value == locationName)
							  select c).FirstOrDefault();

				if (canvas == null) return 0.0;

				var paths = (from p in canvas.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Path")
						select new{Width = Double.Parse(p.Attributes("Width").Max().Value), Left = Double.Parse(p.Attributes("Canvas.Left").Min().Value)} ).ToList();
				var maxWidth = paths.Max(p => (p.Left + p.Width));
				var minOffset = paths.Min(p => (p.Left));
				return maxWidth - minOffset;
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public static double GetCanvasHeight(string locationName)
		{
			try
			{
				var canvas = (from c in Instance.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Canvas")
							  where c.HasAttributes && c.Attributes().Any(a => a.Value == locationName)
							  select c).FirstOrDefault();

				if (canvas == null) return 0.0;

				var paths = (from p in canvas.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Path")
					select new{Width = Double.Parse(p.Attributes("Height").Max().Value), Left = Double.Parse(p.Attributes("Canvas.Top").Min().Value)} ).ToList();

				var maxWidth = paths.Max(p => (p.Left + p.Width));
				var minOffset = paths.Min(p => (p.Left));
				return maxWidth - minOffset;
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public static Thickness GetCanvasOffset(string locationName)
		{
			try
			{
				var canvas = (from c in Instance.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Canvas")
							  where c.HasAttributes && c.Attributes().Any(a => a.Value == locationName)
							  select c).FirstOrDefault();

				if (canvas == null) return new Thickness(0,0,0,0);

				var left = (from p in canvas.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Path")
							let l = Double.Parse(p.Attributes("Canvas.Left").First().Value)
							select l).Min();

				var top = (from p in canvas.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Path")
						   let l = Double.Parse(p.Attributes("Canvas.Top").First().Value)
							select l).Min();
				return new Thickness(left,top,0,0);

			}
			catch (Exception ex)
			{

				throw;
			}
		}

		#endregion

		public static object GetLabelsWidth()
		{
			var canvas = (from c in Instance.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Canvas")
						  where c.HasAttributes && c.Attributes().Any(a => a.Value == "Labels")
						  select c).FirstOrDefault();
			if (canvas == null) return 0.0;

			var paths = (from p in canvas.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Rectangle")
						 select new { Width = Double.Parse(p.Attributes("Width").Max().Value), Left = Double.Parse(p.Attributes("Canvas.Left").Min().Value) }).ToList();
			var maxWidth = paths.Max(p => (p.Left + p.Width));
			var minOffset = paths.Min(p => (p.Left));
			return maxWidth - minOffset;

		}

		public static double GetLabelsHeight()
		{
			try
			{
				var canvas = (from c in Instance.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Canvas")
							  where c.HasAttributes && c.Attributes().Any(a => a.Value == "Labels")
							  select c).FirstOrDefault();

				if (canvas == null) return 0.0;

				var paths = (from p in canvas.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Rectangle")
							 select new { Width = Double.Parse(p.Attributes("Height").Max().Value), Left = Double.Parse(p.Attributes("Canvas.Top").Min().Value) }).ToList();

				var maxWidth = paths.Max(p => (p.Left + p.Width));
				var minOffset = paths.Min(p => (p.Left));
				return maxWidth - minOffset;
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public static Thickness GetLabelsOffet()
		{
			try
			{
				var canvas = (from c in Instance.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Canvas")
							  where c.HasAttributes && c.Attributes().Any(a => a.Value == "Labels")
							  select c).FirstOrDefault();

				if (canvas == null) return new Thickness(0, 0, 0, 0);

				var left = (from p in canvas.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Rectangle")
							let l = Double.Parse(p.Attributes("Canvas.Left").First().Value)
							select l).Min();

				var top = (from p in canvas.Descendants("{http://schemas.microsoft.com/winfx/2006/xaml/presentation}Rectangle")
						   let l = Double.Parse(p.Attributes("Canvas.Top").First().Value)
						   select l).Min();
				return new Thickness(left, top, 0, 0);

			}
			catch (Exception ex)
			{

				throw;
			}
		}
	}
}
