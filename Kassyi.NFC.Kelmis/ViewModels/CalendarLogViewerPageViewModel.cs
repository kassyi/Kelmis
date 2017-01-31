using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using System.Collections.ObjectModel;
using Kassyi.NFC.Kelmis.Models;

namespace Kassyi.NFC.Kelmis.ViewModels
{
    class CalendarLogViewerPageViewModel
    {
        public MonthlyLogbook Logbook = new MonthlyLogbook();

        public CalendarLogViewerPageViewModel()
        {
            
        }
    }
}
