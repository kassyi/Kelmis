using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using StrawhatNet.NFC.FelicaReader.Suica;
using System.ComponentModel.DataAnnotations;

namespace Kassyi.NFC.Kelmis.Models
{
    public class MonthlyLogbook
    {
        public ReactiveProperty<DateTime> CurrentMonth = new ReactiveProperty<DateTime>() { Value = DateTime.Now };
        public ReactiveCollection<DailyLogRecode> Log = new ReactiveCollection<DailyLogRecode>();
        public ReactiveCollection<string> CardIdms = new ReactiveCollection<string>();
        public ReactiveProperty<string> CardSelecterSelectedIdm = new ReactiveProperty<string>();

        public ReactiveCommand AddMonthCommand { get; }
        public ReactiveCommand SubtractMonthCommadn { get; }

        enum BalanceCategory
        {
            Fare, Charge, ProductSales, Other
        }

        Dictionary<ProsessIdEnum, BalanceCategory> category = new Dictionary<ProsessIdEnum, BalanceCategory>(){
{ProsessIdEnum.チャージ    ,BalanceCategory.Charge},
{ProsessIdEnum.入A_入場時オートチャージ   ,BalanceCategory.Charge},
{ProsessIdEnum.出A_出場時オートチャージ   ,BalanceCategory.Charge},
{ProsessIdEnum.入金_バスチャージ   ,BalanceCategory.Charge},
{ProsessIdEnum.ポイントチャージ    ,BalanceCategory.Charge},
{ProsessIdEnum.入金_レジ入金 ,BalanceCategory.Charge},

{ProsessIdEnum.運賃支払_改札出場   ,BalanceCategory.Fare},
{ProsessIdEnum.券購_磁気券購入    ,BalanceCategory.Fare},
{ProsessIdEnum.精算  ,BalanceCategory.Fare},
{ProsessIdEnum.精算_入場精算 ,BalanceCategory.Fare},
{ProsessIdEnum.窓出_改札窓口処理   ,BalanceCategory.Fare},
{ProsessIdEnum.バス_PiTaPa系  ,BalanceCategory.Fare},
{ProsessIdEnum.バス_IruCa系   ,BalanceCategory.Fare},
{ProsessIdEnum.支払_新幹線利用    ,BalanceCategory.Fare},
{ProsessIdEnum.券購_バス路面電車企画券購入  ,BalanceCategory.Fare},
{ProsessIdEnum.精算_他社精算 ,BalanceCategory.Fare},
{ProsessIdEnum.精算_他社入場精算   ,BalanceCategory.Fare},

{ProsessIdEnum.物販  ,BalanceCategory.ProductSales},
{ProsessIdEnum.物販取消    ,BalanceCategory.ProductSales},
{ProsessIdEnum.入物_入場物販 ,BalanceCategory.ProductSales},
{ProsessIdEnum.物現_現金併用物販   ,BalanceCategory.ProductSales},
{ProsessIdEnum.入物_入場現金併用物販 ,BalanceCategory.ProductSales},

{ProsessIdEnum.新規_新規発行, BalanceCategory.Other },
{ProsessIdEnum.控除_窓口控除 ,BalanceCategory.Other},
{ProsessIdEnum.再発_再発行処理    ,BalanceCategory.Other},

};
        public MonthlyLogbook()
        {
            CardIdms.CollectionChanged +=
                (_, __) => CardSelecterSelectedIdm.Value = CardIdms.FirstOrDefault();
            CardIdms.AddRangeOnScheduler(KelmisLogDb.Current.GetSaveedCardIdms());
            CurrentMonth.Subscribe(d => LoadFromDb(d));

            AddMonthCommand = CurrentMonth.ObserveHasErrors.ToReactiveCommand();
        }

        /// <summary>
        /// 指定さた月のデータを読み込みます
        /// </summary>
        /// <param name="date"></param>
        void LoadFromDb(DateTime date)
        {
            var log = KelmisLogDb.Current.GetLog(CardSelecterSelectedIdm.Value, CalenderGenelater.GetCalenderStartAndEndDate(date))
                .GroupBy(l => l.Date)
                .Select(day =>
                {
                    var recode = new DailyLogRecode();
                    recode.Date = day.First().Date;

                    var map = day.Select(r => new Tuple<BalanceCategory, KelmisLogRecode>(category[(ProsessIdEnum)r.ProssesId], r));
                    recode.SumCharge = map.Where(t => t.Item1 == BalanceCategory.Charge)
                                          .Sum(t => t.Item2.Balance);
                    recode.SumFare = map.Where(t => t.Item1 == BalanceCategory.Fare)
                                          .Sum(t => t.Item2.Balance);
                    recode.SumOther = map.Where(t => t.Item1 == BalanceCategory.Other)
                                          .Sum(t => t.Item2.Balance);
                    recode.SumProductSales = map.Where(t => t.Item1 == BalanceCategory.ProductSales)
                                                .Sum(t => t.Item2.Balance);
                    return recode;
                });
            var calender = GelelateCalender(date)
                .Select(d => new DailyLogRecode() { Date = d })
                .Union(log)
                .GroupBy(d => d.Date)
                .Select(g => g.FindMax(r => r.TotalDayPay));

            
            Log.Clear();
            Log.AddRangeOnScheduler(calender);

        }

        static IEnumerable<DateTime> GelelateCalender(DateTime date)
        {
            // 今までnullを気にしてなかった  
            if (date == null)
            {
                return new List<DateTime>();
            }

            // 該当月の1日と末日 
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var lastDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            // カレンダー表示の開始日と終了日 
            var startDate = firstDay.AddDays(DayOfWeek.Sunday - firstDay.DayOfWeek);
            var endDate = lastDay.AddDays(DayOfWeek.Saturday - lastDay.DayOfWeek);
            return Enumerable.Range(0, (endDate - startDate).Days + 1)
                             .Select(n => startDate.AddDays(n));
        }
    }

    /// <summary>
    /// Suicaの利用履歴を1日単位でまとめるクラス
    /// </summary>
    public class DailyLogRecode
    {
        public DateTime Date;
        /// <summary>
        /// 一日の運賃合計額
        /// </summary>
        public int SumFare { get; set; }
        public int SumCharge { get; set; }
        public int SumProductSales { get; set; }
        public int SumOther { get; set; }

        public int TotalDayPay => SumFare + SumCharge + SumProductSales + SumOther;
    }
}
