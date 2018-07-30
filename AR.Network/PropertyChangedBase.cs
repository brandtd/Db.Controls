using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AR.Drone
{
    /// <summary>
    ///     Provides method for firing property changed event with correct property name.
    /// </summary>
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        /// <inheritdoc cref="INotifyPropertyChanged" />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Fires property changed event, automatically figuring out the property name if called
        ///     from within the setter of a property.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        ///     Sets the property and fires <see cref="PropertyChanged" /> event if new value is not
        ///     equal to current value.
        /// </summary>
        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string name = "") where T : IEquatable<T>
        {
            if (!property.Equals(value))
            {
                property = value;
                OnPropertyChanged(name);
            }
        }
    }
}