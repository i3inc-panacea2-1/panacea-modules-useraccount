using Panacea.Core;
using Panacea.Modules.UserAccount.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(SettingsControl))]
    public class SettingsControlViewModel : ViewModelBase
    {
        private readonly IUserService _service;

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

        bool _userSignedIn;
        public bool UserSignedIn
        {
            get => _userSignedIn;
            set
            {
                _userSignedIn = value;
                OnPropertyChanged();
            }
        }

        public SettingsControlViewModel(IUserService service)
        {
            _service = service;
        }

        public override void Activate()
        {
            _service.UserLoggedIn += _service_UserLoggedIn;
            _service.UserLoggedOut += _service_UserLoggedOut;
            User = _service.User;
            UserSignedIn = _service.User.Id != null;
        }

        public override void Deactivate()
        {
            _service.UserLoggedIn -= _service_UserLoggedIn;
            _service.UserLoggedOut -= _service_UserLoggedOut;
        }

        private Task _service_UserLoggedOut(IUser user)
        {
            User = _service.User;
            UserSignedIn = _service.User.Id != null;
            return Task.CompletedTask;
        }

        private Task _service_UserLoggedIn(IUser user)
        {
            User = _service.User;
            UserSignedIn = _service.User.Id != null;
            return Task.CompletedTask;
        }
    }
}
