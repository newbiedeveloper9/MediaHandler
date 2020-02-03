using System;
using System.Windows.Media;
using Caliburn.Micro;

namespace MediaHandler.Models
{
    public class MessageModel : PropertyChangedBase
    {
        private BindableCollection<SimpleMessage> _simpleMessageCollection
            = new BindableCollection<SimpleMessage>();
        public BindableCollection<SimpleMessage> SimpleMessageCollection
        {
            get => _simpleMessageCollection;
            set
            {
                if (_simpleMessageCollection == value) return;
                _simpleMessageCollection = value;
                NotifyOfPropertyChange(() => SimpleMessageCollection);
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