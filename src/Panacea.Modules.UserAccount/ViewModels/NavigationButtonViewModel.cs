using Panacea.Controls;
using Panacea.Core;
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
    [View(typeof(NavigationButton))]
    class NavigationButtonViewModel:ViewModelBase
    {
        public NavigationButtonViewModel(IUserService service, IUserAccountManager manager)
        {
            _service = service;
            ClickCommand = new RelayCommand(async args =>
            {
                if(_service.User.Id == null)
                {
                    await manager.LoginAsync();
                }
                else
                {
                    await manager.LogoutAsync();
                }
            });
        }

        public override void Activate()
        {
            _service.UserLoggedIn += _service_UserLoggedIn;
            _service.UserLoggedOut += _service_UserLoggedOut;
            SignedIn = _service.User.Id != null;
        }

        public override void Deactivate()
        {
            _service.UserLoggedIn -= _service_UserLoggedIn;
            _service.UserLoggedOut -= _service_UserLoggedOut;
        }

        private Task _service_UserLoggedOut(IUser user)
        {
            SignedIn = false;
            return Task.CompletedTask;
        }

        private Task _service_UserLoggedIn(IUser user)
        {
            SignedIn = true;
            return Task.CompletedTask;
        }

        bool _signedIn;
        private readonly IUserService _service;

        public bool SignedIn {
            get => _signedIn;
            set
            {
                _signedIn = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ClickCommand { get; }
    }
}
