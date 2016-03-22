using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWPF.ViewModel
{
    public class StartGpssWindowVm : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private int _ModelingTime;
        private int _ObservationTime;
        Dictionary<string, List<string>> propErrors = new Dictionary<string, List<string>>();
        #region Implement INotyfyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public int ModelingTime
        {
            get { return _ModelingTime; }
            set
            {
                if (_ModelingTime != value)
                {
                    _ModelingTime = value;
                    OnPropertyChanged("ModelingTime");
                }
            }
        }

        public int ObservationTime
        {
            get { return _ObservationTime; }
            set
            {
                if (_ObservationTime != value)
                {
                    _ObservationTime = value;
                    OnPropertyChanged("ObservationTime");
                }
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            var errors = new List<string>();
            if (propertyName != null)
            {
                propErrors.TryGetValue(propertyName, out errors);
                return errors;
            }
            else
                return null;
        }

        public bool HasErrors
        {
            get
            {
                try
                {
                    var propErrorsCount = propErrors.Values.FirstOrDefault(r => r.Count > 0);
                    if (propErrorsCount != null)
                        return true;
                    else
                        return false;
                }
                catch { }
                return true;
            }
        }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}
