using System.Diagnostics;
using Caliburn.Micro;
using TestApp.Messages;

namespace TestApp.ViewModels
{
    public class ViewModel3 : ViewModelBase, IHandle<Message3>
    {
        public ViewModel3(IEventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public void Handle(Message3 message)
        {
            Debug.WriteLine(message.GetType() + " Received");
        }
    }
}