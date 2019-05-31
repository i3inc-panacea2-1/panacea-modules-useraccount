﻿using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.Billing;
using Panacea.Modularity.UiManager;
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
        }

        public RelayCommand SignoutCommand { get; }
        public RelayCommand BuyServiceCommand { get; }
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

        public override async void Activate()
        {
            User = _core.UserService.User;
            await Update();
        }

        async Task Update()
        {
            Services = null;
            Ledgers = null;
            if (_core.TryGetBilling(out IBillingManager billing))
            {
                var sett = await billing.GetSettingsAsync();
                Settings = sett;
                Services = await billing.GetActiveUserServicesAsync();
                Ledgers = await billing.GetUserPurchaseHistoryAsync();
            }
        }
    }
}
