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

        public async Task<bool> LoginAsync()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                var source = new TaskCompletionSource<bool>();
                ui.Navigate(new LoginViewModel(_core, source));
                var res = await source.Task;
                ui.GoBack();
                return res;
            }
            return false;
        }

        public async Task<bool> LogoutAsync(bool force = false)
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                await ui.DoWhileBusy(() => _core.UserService.LogoutAsync());
                return true;
            }
            return false;
        }

        public void NavigateToMyAccount()
        {

        }

        public void NavigateToRegister()
        {

        }

        public Task<bool> RequestLoginAsync(string text)
        {
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
