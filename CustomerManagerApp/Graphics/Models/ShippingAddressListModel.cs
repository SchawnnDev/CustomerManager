using CustomerManagement.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CustomerManagerApp.Graphics.Models
{
    class ShippingAddressListModel : INotifyPropertyChanged
    {
        private ObservableCollection<ShippingAddress> _shippingAddresses;

        public ObservableCollection<ShippingAddress> ShippingAddresses
        {
            get => _shippingAddresses;
            set
            {
                if (Equals(value, _shippingAddresses)) { return; }
                _shippingAddresses = value;
                OnPropertyChanged("ShippingAddresses");
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged implementation
    }
}
