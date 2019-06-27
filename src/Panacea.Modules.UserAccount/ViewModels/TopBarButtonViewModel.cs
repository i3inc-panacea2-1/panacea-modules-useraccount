using Panacea.Controls;
using Panacea.Core;
using Panacea.Modules.UserAccount.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(TopBarButton))]
    class TopBarButtonViewModel : ViewModelBase
    {


        IUser _user;
        public IUser User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        bool _usernameLoggedOutVisible;
        private readonly PanaceaServices _core;
        private readonly UserAccountManager _manager;

        public bool UsernameLoggedOutVisible
        {
            get => _usernameLoggedOutVisible;
            set
            {
                _usernameLoggedOutVisible = value;
                OnPropertyChanged();
            }
        }

        public TopBarButtonViewModel(PanaceaServices core, UserAccountManager manager)
        {
            _core = core;

            _manager = manager;
            MainAccountButtonClickCommand = new RelayCommand(async args =>
            {
                if (User.Id == null)
                {
                    await manager.LoginAsync();
                }
                else
                {
                    manager.NavigateToMyAccount();
                }
            });
        }

        public override void Activate()
        {
            _core.UserService.UserLoggedIn += UserService_UserLoggedIn;
            _core.UserService.UserLoggedOut += UserService_UserLoggedOut;
            Update();
        }

        public override void Deactivate()
        {
            _core.UserService.UserLoggedIn -= UserService_UserLoggedIn;
            _core.UserService.UserLoggedOut -= UserService_UserLoggedOut;
        }


        private Task UserService_UserLoggedOut(IUser user)
        {
            Update();
            return Task.CompletedTask;
        }

        private Task UserService_UserLoggedIn(IUser user)
        {
            Update();
            return Task.CompletedTask;
        }

        void Update()
        {
            UsernameLoggedOutVisible = _core.UserService.User.Id == null;
            User = _core.UserService.User;
        }

        public ICommand MainAccountButtonClickCommand { get; }
    }
}
