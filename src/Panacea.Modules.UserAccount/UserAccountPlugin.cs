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
    public class UserAccountPlugin : IUserAccountPlugin
    {
        private readonly PanaceaServices _core;
        IUserAccountManager _manager;
        public UserAccountPlugin(PanaceaServices core)
        {
            _core = core;
        }

        public Task BeginInit()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
           
        }

        public Task EndInit()
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                ui.AddNavigationBarControl(new NavigationButtonViewModel(_core.UserService, GetUserAccountManager()));
                ui.AddSettingsControl(new SettingsControlViewModel(_core.UserService, GetUserAccountManager()));
            }
            return Task.CompletedTask;
        }

        public IUserAccountManager GetUserAccountManager()
        {
            return _manager = _manager ?? new UserAccountManager(_core);
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }
    }
}
