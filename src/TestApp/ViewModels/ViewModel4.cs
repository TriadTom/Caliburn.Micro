using System.Diagnostics;
using Caliburn.Micro;
using TestApp.Messages;

namespace TestApp.ViewModels
{
    public class ViewModel4 : ViewModelBase, IHandle<Message4>
    {
        public ViewModel4(IEventAggregator eventAggregator) : base(eventAggregator)
        {
           
        }

        public void Handle(Message4 message)
        {
            Debug.WriteLine(message.GetType() + " Received");
        }
    }
}