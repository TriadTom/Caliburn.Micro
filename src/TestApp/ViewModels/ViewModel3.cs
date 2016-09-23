using System.Diagnostics;
using Caliburn.Micro;
using TestApp.Messages;

namespace TestApp.ViewModels
{
    public class ViewModel3 : ViewModelBase, IHandle<Message1>
    {
        public ViewModel3(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            eventAggregator.SetFilter(this, typeof(Message1), "M3");
        }

        public void Handle(Message1 message)
        {
            Debug.WriteLine(GetType().Name + " Received " + message.GetType().Name);
        }
    }
}