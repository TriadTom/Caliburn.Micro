using System.Diagnostics;
using Caliburn.Micro;
using TestApp.Messages;

namespace TestApp.ViewModels
{
    public class ViewModel5 : ViewModelBase, IHandle<Message5>
    {
        public ViewModel5(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            
        }

        public void Handle(Message5 message)
        {
            Debug.WriteLine(message.GetType() + " Received");
        }
    }
}