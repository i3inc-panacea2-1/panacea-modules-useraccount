using Panacea.Modularity.UiManager;
using Panacea.Modules.UserAccount.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(WarningPopup))]
    class WarningViewModel:PopupViewModelBase<object>
    {
        public WarningViewModel(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
