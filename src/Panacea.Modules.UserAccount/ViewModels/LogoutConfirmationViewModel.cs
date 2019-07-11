using Panacea.Controls;
using Panacea.Modularity.UiManager;
using Panacea.Modules.UserAccount.Views;
using Panacea.Mvvm;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(LogoutConfirmation))]
    public class LogoutConfirmationViewModel : PopupViewModelBase<bool>
    {
        public RelayCommand OkCommand { get; set; }
        public RelayCommand CancelCommand{ get; set; }

        public LogoutConfirmationViewModel()
        {
            OkCommand = new RelayCommand(args => {
                SetResult(true);
                return;
            });
            CancelCommand = new RelayCommand(args => {
                SetResult(false);
                return;
            });
        }

    }
}
