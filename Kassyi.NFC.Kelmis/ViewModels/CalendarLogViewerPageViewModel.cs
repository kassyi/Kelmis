using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using System.Collections.ObjectModel;

namespace Kassyi.NFC.Kelmis.Models
{
    class CalendarLogViewerPageViewModel
    {
        public ReactiveProperty<DateTime> CurrentMonth = new ReactiveProperty<DateTime>() { Value = DateTime.Now };

        public ReactiveCollection<string> CardIdms = new ReactiveCollection<string>();
        public ReactiveProperty<string> CardSelecterSelectedIdm = new ReactiveProperty<string>();

        public CalendarLogViewerPageViewModel()
        {
            InitializeProp();
        }

        void InitializeProp()
        {
            CardIdms.AddRangeOnScheduler(KelmisLogDb.Current.GetSaveedCardIdms());
        }
    }
}
