using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modularity.UserAccount;
using Panacea.Modules.UserAccount.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount
{
    public class UserAccountManager : IUserAccountManager
    {
        private readonly PanaceaServices _core;

        public UserAccountManager(PanaceaServices core)
        {
            _core = core;
        }

        public async Task<bool> LogoutAsync(bool force = false)
        {
            if (_core.UserService.User.Id == null) return true;
            if (!force)
            {
                var popup = new LogoutConfirmationViewModel();
                if (_core.TryGetUiManager(out IUiManager uiMgnr))
                {
                    var ok = await uiMgnr.ShowPopup<bool>(popup);
                    if (!ok)
                    {
                        return false;
                    }
                }
            }
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                await ui.DoWhileBusy(() => _core.UserService.LogoutAsync());
                return true;
            }
            return false;
        }

        public void NavigateToMyAccount()
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                var vm = new MyAccountViewModel(_core, this);
                ui.Navigate(vm);
            }
        }

        public void NavigateToRegister()
        {

        }

        public async Task<bool> LoginAsync()
        {
            if (_core.UserService.User.Id != null) return true;
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                var source = new TaskCompletionSource<bool>();
                var reg = new LoginViewModel(this, _core, source);
                ui.Navigate(reg, false);
                var res = await source.Task;
                if (res && _core.TryGetUiManager(out ui) && ui.CurrentPage == reg)
                {
                    ui.GoBack();
                }
                return res;
            }
            return false;
        }

        public async Task<bool> RegisterAsync()
        {
            if (_core.UserService.User.Id != null) return true;
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                var source = new TaskCompletionSource<bool>();
                var reg = new RegisterViewModel(this, _core, source);
                ui.Navigate(reg, false);
                var res = await source.Task;
                if (res && _core.TryGetUiManager(out ui) && ui.CurrentPage == reg)
                {
                    ui.GoBack();
                }
                return res;
            }
            return false;
        }
        public Task<bool> RequestLoginAsync(string text)
        {
            if (_core.UserService.User.Id != null) return Task.FromResult(true);
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                var source = new TaskCompletionSource<bool>();
                ui.ShowPopup(new RequestSignInViewModel(this, source, text));
                return source.Task;
            }
            return Task.FromResult(false);
        }


    }
}
