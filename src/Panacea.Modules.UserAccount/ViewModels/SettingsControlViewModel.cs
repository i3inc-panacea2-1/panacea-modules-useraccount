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

        public SettingsControlViewModel(IUserService service)
        {
            _service = service;
            User = _service.User;
        }

        public override void Activate()
        {
            _service.UserLoggedIn += _service_UserLoggedIn;
            _service.UserLoggedOut += _service_UserLoggedOut;
        }

        public override void Deactivate()
        {
            _service.UserLoggedIn -= _service_UserLoggedIn;
            _service.UserLoggedOut -= _service_UserLoggedOut;
        }

        private Task _service_UserLoggedOut(IUser user)
        {
            User = _service.User;
            return Task.CompletedTask;
        }

        private Task _service_UserLoggedIn(IUser user)
        {
            User = _service.User;
            return Task.CompletedTask;
        }
    }
}
