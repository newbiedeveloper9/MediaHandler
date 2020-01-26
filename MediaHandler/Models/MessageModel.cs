using System;
using System.Windows.Media;
using Caliburn.Micro;

namespace MediaHandler.Models
{
    public class MessageModel : PropertyChangedBase
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                if (_id == value) return;
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                NotifyOfPropertyChange(() => Text);
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

        private bool _separator;
        public bool Separator
        {
            get => _separator;
            set
            {
                if (_separator == value) return;
                _separator = value;
                NotifyOfPropertyChange(() => Separator);
            }
        }
    }
}