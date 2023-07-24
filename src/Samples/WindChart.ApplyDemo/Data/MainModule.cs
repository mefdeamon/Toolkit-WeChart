using MeiMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindChart.ApplyDemo.PageModels;

namespace WindChart.ApplyDemo.Data
{
    internal class MainModule : IModule
    {
        public void OnLoad(IBinder binder)
        {
            binder.BindSingleton<WelcomePageModel>();
            binder.BindSingleton<LinegramPageModel>();
            binder.BindSingleton<BargramPageModel>();
            binder.BindSingleton<DotgramPageModel>();
        }

    }
}
