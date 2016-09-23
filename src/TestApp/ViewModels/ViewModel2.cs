using System.Diagnostics;
using Caliburn.Micro;
using TestApp.Messages;

namespace TestApp.ViewModels
{
    public class ViewModel2 : ViewModelBase, IHandle<Message2>
    {
        public ViewModel2(IEventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public void Handle(Message2 message)
        {
            Debug.WriteLine(message.GetType() + " Received");
        }
    }
}