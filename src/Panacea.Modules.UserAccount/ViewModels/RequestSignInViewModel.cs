using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modularity.UserAccount;
using Panacea.Modules.UserAccount.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(RequestSignInPopup))]
    class RequestSignInViewModel:PopupViewModelBase<bool>
    {
        public RequestSignInViewModel(IUserAccountManager manager, TaskCompletionSource<bool> source, string text)
        {
            Text = text;
            CreateAccountCommand = new RelayCommand(async args =>
            {
                //source.SetResult(await manager.NavigateToRegister());
                taskCompletionSource.SetResult(true);
            });

            SignInCommand = new RelayCommand(async args =>
            {
                taskCompletionSource.SetResult(true);
                source.SetResult(await manager.LoginAsync());
            });
        }
        public RelayCommand CreateAccountCommand { get; }

        public RelayCommand SignInCommand { get; }

        public string Text { get; }
    }
}
