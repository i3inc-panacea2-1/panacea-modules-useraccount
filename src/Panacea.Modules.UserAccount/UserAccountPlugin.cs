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
        TopBarButtonViewModel _topButton;
        NavigationButtonViewModel _navButton;

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
                _navButton = new NavigationButtonViewModel(_core.UserService, GetUserAccountManager());
                ui.AddNavigationBarControl(_navButton);
                //ui.AddSettingsControl(new SettingsControlViewModel(_core.UserService, GetUserAccountManager()));
                _topButton = new TopBarButtonViewModel(_core, GetUserAccountManager() as UserAccountManager);
                ui.AddMainPageControl(_topButton);
            }
            return Task.CompletedTask;
        }

        public IUserAccountManager GetUserAccountManager()
        {
            return _manager = _manager ?? new UserAccountManager(_core);
        }

        public Task Shutdown()
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                if (_navButton != null)
                {
                    ui.RemoveNavigationBarControl(_navButton);

                }
                if (_topButton != null)
                {
                    ui.RemoveMainPageControl(_topButton);
                }
            }
            return Task.CompletedTask;
        }
    }
}
