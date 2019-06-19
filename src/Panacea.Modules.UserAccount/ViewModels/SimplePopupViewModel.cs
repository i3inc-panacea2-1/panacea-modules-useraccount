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
    [View(typeof(SimplePopup))]
    public class SimplePopupViewModel : PopupViewModelBase<object>
    {
        public string Text { get; set; }
        public SimplePopupViewModel(string text)
        {
            this.Text = text;
        }
    }
}
