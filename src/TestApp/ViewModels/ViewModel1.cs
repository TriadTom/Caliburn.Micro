using System.Diagnostics;
using Caliburn.Micro;
using TestApp.Messages;

namespace TestApp.ViewModels
{
    public class ViewModel1 : ViewModelBase, IHandle<Message1>//, IHandle<Message3>
    {
        public ViewModel1(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            eventAggregator.SetFilter(this, typeof(Message1), "M1");
        }
        
        public void Handle(Message1 message)
        {
            Debug.WriteLine(GetType().Name + " Received " + message.GetType().Name);
        }

        public void Handle(Message3 message)
        {
            Debug.WriteLine(GetType().Name + " Received " + message.GetType().Name);
        }
    }
    
}