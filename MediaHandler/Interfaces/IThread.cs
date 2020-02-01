using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using MediaHandler.Models;

namespace MediaHandler.Interfaces
{
    public interface IThread
    {
        IFbThreadService FbThreadService { get; set; }
        SolidColorBrush MyColor { get; set; }
        SolidColorBrush FriendColor { get; set; }
        Task Initialize();
    }
}