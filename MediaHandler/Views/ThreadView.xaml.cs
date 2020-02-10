using System;
using System.Windows.Controls;
using System.Windows.Input;
using MediaHandler.Input;
using MediaHandler.Services;

namespace MediaHandler.Views
{
    public partial class ThreadView : UserControl
    {
        public ThreadView()
        {
            InitializeComponent();
        }

        private void ChatMessage_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox txtBox)) return;
            e.Handled = TextboxHelper.ShortcutsFixHandled(txtBox, e.Key);
        }
    }
}
