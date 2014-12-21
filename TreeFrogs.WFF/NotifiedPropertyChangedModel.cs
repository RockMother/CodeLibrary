using System.ComponentModel;
using System.Runtime.CompilerServices;
using TreeFrogs.WFF.Annotations;

namespace TreeFrogs.WFF
{
    public abstract class NotifiedPropertyChangedModel: INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
