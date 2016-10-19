using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resonance.Metro.Controls.Cleaning.Convertors;

namespace Resonance.Metro.Controls.Tests.Cleaning.Convertors
{
    [TestClass]
    public class TimelineItemToBrushConvertorTests
    {
        private FrameworkElement frameworkElement = new FrameworkElement();

        private TimelineItemToBrushConvertor convertor = new TimelineItemToBrushConvertor();

        ResourceDictionary resourceDictionary = new ResourceDictionary { Source = new Uri("/Resonance.Metro.Controls;component/Cleaning/Resources/Brushes.xaml", UriKind.RelativeOrAbsolute) };

        [TestInitialize]
        public void Init()
        {
            frameworkElement.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        [TestMethod]
        public void TestRequiresDeepCleanReturnsWarningBrush()
        {
            var expectedResult = frameworkElement.TryFindResource("warningColor");

            object result = convertor.Convert(new object[] { frameworkElement, BedStatus.RequiresDeepClean, true }, typeof(Brush), string.Empty, Thread.CurrentThread.CurrentCulture);

            Assert.AreSame(expectedResult, result);
        }

        [TestMethod]
        public void TestCleanReturnsInActiveBrush()
        {
            var expectedResult = frameworkElement.TryFindResource("inactiveColor");

            object result = convertor.Convert(new object[] { frameworkElement, BedStatus.Clean, true }, typeof(Brush), string.Empty, Thread.CurrentThread.CurrentCulture);

            Assert.AreSame(expectedResult, result);
        }

        [TestMethod]
        public void TestDirtyAndAvailableReturnsActiveBrush()
        {
            var expectedResult = frameworkElement.TryFindResource("activeColor");

            object result = convertor.Convert(new object[] { frameworkElement, BedStatus.Dirty, true }, typeof(Brush), string.Empty, Thread.CurrentThread.CurrentCulture);

            Assert.AreSame(expectedResult, result);
        }

        [TestMethod]
        public void TestDirtyAndNotAvailableReturnsInActiveBrush()
        {
            var expectedResult = frameworkElement.TryFindResource("pendingColor");

            object result = convertor.Convert(new object[] { frameworkElement, BedStatus.Dirty, false }, typeof(Brush), string.Empty, Thread.CurrentThread.CurrentCulture);

            Assert.AreSame(expectedResult, result);
        }
    }
}
