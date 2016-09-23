using System.Diagnostics;
using Caliburn.Micro;
using TestApp.Messages;

namespace TestApp.ViewModels
{
    public class ViewModel1 : ViewModelBase, IHandle<Message1>, IHandle<Message3>
    {
        public ViewModel1(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            
        }
        
        public void Handle(Message1 message)
        {
            Debug.WriteLine(message.GetType() + " Received");
        }

        public void Handle(Message3 message)
        {
            Debug.WriteLine(message.GetType() + " Received");
        }
    }
    
}