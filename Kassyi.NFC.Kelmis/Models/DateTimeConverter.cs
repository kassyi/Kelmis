using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Kassyi.NFC.Kelmis.Models
{
    /// <summary>
    /// DateTimeからカレンダーの日付を列挙するクラス
    /// </summary>
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is DateTime ? Convert((DateTime)value) : new List<DateTime>();
        }

        public static DateBlock GetCalenderStartAndEndDate(DateTime value)
        {
            var date = value;

            // 該当月の1日と末日 
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var lastDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            // カレンダー表示の開始日と終了日 
            var startDate = firstDay.AddDays(DayOfWeek.Sunday - firstDay.DayOfWeek);
            var endDate = lastDay.AddDays(DayOfWeek.Saturday - lastDay.DayOfWeek);
            return new DateBlock() { StartDate = startDate, EndDate = endDate };
        }

        public static IEnumerable<DateTime> Convert(DateTime value)
        {
            // 今までnullを気にしてなかった  
            if (value == null)
            {
                return new List<DateTime>();
            }

            var date = GetCalenderStartAndEndDate((DateTime)value);

            return Enumerable.Range(0, (date.EndDate- date.StartDate).Days + 1)
                             .Select(n => date.StartDate.AddDays(n));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
