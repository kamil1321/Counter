using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CounterKB.Models
{
    public class Counter : INotifyPropertyChanged
    {
        public string Filename { get; set; } 
        public string Name { get; set; }

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
