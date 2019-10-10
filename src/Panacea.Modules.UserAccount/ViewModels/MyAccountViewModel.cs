using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.Billing;
using Panacea.Modularity.UiManager;
using Panacea.Modularity.UserAccount;
using Panacea.Modules.UserAccount.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(MyAccountPage))]
    class MyAccountViewModel : ViewModelBase
    {
        private readonly PanaceaServices _core;

        public MyAccountViewModel(PanaceaServices core, IUserAccountManager manager)
        {
            _core = core;
            SignoutCommand = new RelayCommand(async arg =>
            {
                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    ui.GoHome();
                }
                await manager.LogoutAsync();
            });
            BuyServiceCommand = new RelayCommand(arg =>
            {
                if (_core.TryGetBilling(out IBillingManager billing))
                {
                    billing.NavigateToBuyServiceWizard();
                }
            });
            ChangeInfoCommand = new RelayCommand(arg =>
            {

                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    ui.Navigate(new UpdateAccountViewModel(_core));
                }
                else
                {
                    _core.Logger.Error(this, "ui manager not loaded");
                }
            });
            ChangeCredentialsCommand = new RelayCommand(arg =>
            {

                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    ui.Navigate(new UpdateCredentialsViewModel(_core));
                }
                else
                {
                    _core.Logger.Error(this, "ui manager not loaded");
                }
            });
            ResetPasswordCommand = new RelayCommand(async arg =>
            {

                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    await ui.ShowPopup<object>(new PasswordResetViewModel(_core));
                }
                else
                {
                    _core.Logger.Error(this, "ui manager not loaded");
                }
            });
        }

        public RelayCommand SignoutCommand { get; }
        public RelayCommand BuyServiceCommand { get; }
        public RelayCommand ChangeInfoCommand { get; }
        public RelayCommand ChangeCredentialsCommand { get; }
        public RelayCommand ResetPasswordCommand { get; }
        IBillingSettings _settings;
        public IBillingSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        List<Ledger> _ledgers;
        public List<Ledger> Ledgers
        {
            get => _ledgers;
            set
            {
                _ledgers = value;
                OnPropertyChanged();
            }
        }

        List<Service> _services;
        public List<Service> Services
        {
            get => _services;
            set
            {
                _services = value;
                OnPropertyChanged();
            }
        }


        ObservableCollection<Service> _stackedServices;
        public ObservableCollection<Service> StackedServices
        {
            get => _stackedServices;
            set
            {
                _stackedServices = value;
                OnPropertyChanged();
            }
        }

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

        public bool HasServices { get; set; }

        public override async void Activate()
        {
            if (_core.UserService.User.Id == null)
            {
                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    ui.GoBack();
                }
                return;
            }
            User = _core.UserService.User;
            await Update();
        }

        async Task Update()
        {
            Services = null;
            Ledgers = null;
            StackedServices = new ObservableCollection<Service>();
            if (_core.TryGetBilling(out IBillingManager billing))
            {
                var sett = await billing.GetSettingsAsync();
                Settings = sett;
                Services = await billing.GetActiveUserServicesAsync();

                var plugins = Services.GroupBy(s => s.Plugin);

                foreach (var pluginGroup in plugins)
                {
                    var sameType = pluginGroup.GroupBy(s => s.ServiceType);
                    foreach (var group in sameType)
                    {
                        StackedServices.Add(new Service()
                        {
                            Plugin = group.First().Plugin,
                            ExpirationDate = group.OrderByDescending(g => g.ExpirationDate).First().ExpirationDate
                        });
                    }
                }
                HasServices = StackedServices.Count > 0;
                OnPropertyChanged(nameof(HasServices));

                Ledgers = await billing.GetUserPurchaseHistoryAsync();
            }
        }
    }
}
