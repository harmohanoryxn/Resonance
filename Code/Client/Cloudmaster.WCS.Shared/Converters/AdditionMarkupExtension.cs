using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace WCS.Shared.Converters
{
    /// <summary>
    /// Markup extension that adds a number to a number
    /// </summary>
    public class AdditionMarkupExtension : Binding
    {
        public AdditionMarkupExtension()
        {
            Initialize();
        }

        public AdditionMarkupExtension(string path)
            : base(path)
        {
            Initialize();
        }

        public AdditionMarkupExtension(string path, object amount)
            : base(path)
        {
            Initialize();
            this.Amoumt = amount; 
        }

        private void Initialize()
        {
            this.Amoumt = Binding.DoNothing;
            this.Converter = new SwitchConverter(this);
        }

        [ConstructorArgument("amount")]
        public object Amoumt { get; set; }
         

        private class SwitchConverter : IValueConverter
        {
            private AdditionMarkupExtension _switch;

            public SwitchConverter(AdditionMarkupExtension switchExtension)
            {
                _switch = switchExtension;
            }


            #region IValueConverter Members

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                try
                {
                    if (value == null) return null;

                    var originalAmount = System.Convert.ToDouble(value);
                    var amount = System.Convert.ToDouble(_switch.Amoumt);
                    return originalAmount + amount;
                }
                catch
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return Binding.DoNothing;
            }

            #endregion
        }
    }
}