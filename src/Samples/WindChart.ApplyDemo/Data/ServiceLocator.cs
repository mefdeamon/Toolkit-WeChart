using MeiMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindChart.ApplyDemo.PageModels;

namespace WindChart.ApplyDemo.Data
{
    internal class ServiceLocator
    {
        public WelcomePageModel WelcomePageModel => ServiceProvider.Get<WelcomePageModel>();
        public LinegramPageModel LinegramPageModel => ServiceProvider.Get<LinegramPageModel>();
    }
}
