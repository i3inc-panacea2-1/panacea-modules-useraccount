using Panacea.Modularity.UiManager;
using Panacea.Modules.UserAccount.Views;
using Panacea.Multilinguality;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(RegisterErrorsPopup))]
    class RegisterErrorsViewModel : PopupViewModelBase<object>
    {
        List<TranslatableObject> _errors;
        public List<TranslatableObject> Errors
        {
            get => _errors;
            set
            {
                _errors = value;
                OnPropertyChanged();
            }
        }

    }
}
