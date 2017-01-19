using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System.Reflection;

namespace Kassyi.NFC.Kelmis.Models
{
    /// <summary>
    /// value != null -> Collapsed
    /// value == null -> Visible
    /// </summary>
    public class IsNullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
        /// </summary>
        public IsNullToVisibilityConverter()
        {
            this.DefaultValue = Visibility.Visible;
        }

        /// <summary>
        /// Gets or sets the default value when the value to be converted
        /// is not a <see cref="Boolean"/>.
        /// </summary>
        public Visibility DefaultValue { get; set; }

        /// <summary>Converts the specified value to a visibility.</summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns><see cref="Visibility.Visible"/> if value is <c>true</c>;
        /// <see cref="Visibility.Collapsed"/> if value is <c>false</c>;
        /// otherwise returns the value of <see cref="DefaultValue"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>Back conversion is not supported.</summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Nothing is returned</returns>
        /// <exception cref="System.NotSupportedException">Back conversion is
        /// not supported.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// value == null -> Collapsed
    /// value != null -> Visible
    /// </summary>
    public class IsNOTNullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
        /// </summary>
        public IsNOTNullToVisibilityConverter()
        {
            this.DefaultValue = Visibility.Visible;
        }

        /// <summary>
        /// Gets or sets the default value when the value to be converted
        /// is not a <see cref="Boolean"/>.
        /// </summary>
        public Visibility DefaultValue { get; set; }

        /// <summary>Converts the specified value to a visibility.</summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns><see cref="Visibility.Visible"/> if value is <c>true</c>;
        /// <see cref="Visibility.Collapsed"/> if value is <c>false</c>;
        /// otherwise returns the value of <see cref="DefaultValue"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>Back conversion is not supported.</summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Nothing is returned</returns>
        /// <exception cref="System.NotSupportedException">Back conversion is
        /// not supported.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
