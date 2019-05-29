using Panacea.Core;
using Panacea.Modularity.UserAccount;
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
