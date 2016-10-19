using System;

namespace WCS.Shared.Converters
{
    internal static class ConverterHelper
    {
        internal static void ValidateArguments(object value, bool isNullValueAllowed, Type expectedValueType, Type targetType, Type expectedTargetType)
        {
            ValidateArguments(value, isNullValueAllowed, expectedValueType, targetType, expectedTargetType, false, null, null);
        }

        internal static void ValidateArguments(object value, bool isNullValueAllowed, Type expectedValueType, Type targetType, Type expectedTargetType, object parameter, Type expectedParameterType)
		{
            ValidateArguments(value, isNullValueAllowed, expectedValueType, targetType, expectedTargetType, true, parameter, expectedParameterType);
        }

        internal static void ValidateArguments(object value, bool isNullValueAllowed, Type expectedValueType, Type targetType, Type expectedTargetType, bool isParameterIncluded, object parameter, Type expectedParameterType)
        {
            if (!isNullValueAllowed && value == null)
                throw new ArgumentNullException("value");

            if (value != null && !expectedValueType.IsInstanceOfType(value))
                throw new InvalidOperationException(String.Format("The value type must be of type {0}.", expectedValueType));

            if (targetType != expectedTargetType)
                throw new InvalidOperationException(String.Format("The target type must be of type {0}.", expectedTargetType));

            if (isParameterIncluded)
            {
                if (parameter == null)
                    throw new ArgumentNullException("parameter");

                if (parameter.GetType() != expectedParameterType)
                    throw new InvalidOperationException(String.Format("The parameter type must be of type {0}.", expectedTargetType));
            }
        }
    }
}
