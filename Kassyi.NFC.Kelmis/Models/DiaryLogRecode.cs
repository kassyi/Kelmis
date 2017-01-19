using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace Kassyi.NFC.Kelmis.Model
{
    class DiaryLogRecode
    {
        /// <summary>
        /// 一日の合計利用額
        /// </summary>
        public ReactiveProperty<int> DiaryCharge = new ReactiveProperty<int>();
    }
}
