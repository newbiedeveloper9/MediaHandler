using System;
using System.Collections.Generic;
using System.Windows.Media;
using Caliburn.Micro;

namespace MediaHandler.Models
{
    public class MessageModel : PropertyChangedBase
    {
        private BindableCollection<string> _texts = new BindableCollection<string>();
        public BindableCollection<string> Texts
        {
            get => _texts;
            set
            {
                if (_texts == value) return;
                _texts = value;
                NotifyOfPropertyChange(() => Texts);
            }
        }

        private DateTime _time;
        public DateTime Time
        {
            get => _time;
            set
            {
                if (_time == value) return;
                _time = value;
                NotifyOfPropertyChange(() => Time);
            }
        }

        private SolidColorBrush _color;
        public SolidColorBrush Color
        {
            get => _color;
            set
            {
                if (_color == value) return;
                _color = value;
                NotifyOfPropertyChange(() => Color);
            }
        }

        private bool _isOwnMessage;
        public bool IsOwnMessage
        {
            get => _isOwnMessage;
            set
            {
                if (_isOwnMessage == value) return;
                _isOwnMessage = value;
                NotifyOfPropertyChange(() => IsOwnMessage);
            }
        }
    }
}