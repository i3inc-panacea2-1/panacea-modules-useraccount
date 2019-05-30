using Panacea.Controls;
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
    [View(typeof(NewPasswordPopup))]
    class NewPasswordViewModel:PopupViewModelBase<object>
    {
        public string Password { get; }

        public RelayCommand CloseCommand { get; }
        public NewPasswordViewModel(string password)
        {
            Password = password;
            CloseCommand = new RelayCommand(arg => SetResult(null));
        }
    }
}
