using NavCityBreda.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace NavCityBreda.ViewModels
{
    public class HelpVM : TemplateVM
    {
        public HelpVM() : base(Util.Loader.GetString("Help"))
        {

        }

        protected override void UpdatePropertiesToNewLanguage()
        {
            
        }
    }
}
