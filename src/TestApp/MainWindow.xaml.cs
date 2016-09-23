using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using SimpleInjector;
using TestApp.Messages;
using TestApp.ViewModels;
using SimpleInjector;
using TestApp.UI;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IHandle<EventAggregator.MessageAdded>, IHandle<EventAggregator.MessageRemoved> {
        private IEventAggregator _eventAgrigator;
        readonly List<object> _objectList = new List<object>();
        private Container IoCContainer;
        public MainWindow()
        {
            InitializeComponent();
            IoCContainer = new Container();
        }
        
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            IoCContainer.Register<IEventAggregator, EventAggregator>(Lifestyle.Singleton);
            _eventAgrigator = (IEventAggregator)IoCContainer.GetInstance(typeof(IEventAggregator));
            _eventAgrigator.Subscribe(this);
        }

        private void Garbage_OnClick(object sender, RoutedEventArgs e) {
            GC.Collect();
        }

        private void Display_OnClick(object sender, RoutedEventArgs e) {
            Debug.WriteLine("Count for Message1: " + _eventAgrigator.HandlerCountFor(typeof(Message1)));
            Debug.WriteLine("Count for Message2: " + _eventAgrigator.HandlerCountFor(typeof(Message2)));
            Debug.WriteLine("Count for Message3: " + _eventAgrigator.HandlerCountFor(typeof(Message3)));
            Debug.WriteLine("Count for Message4: " + _eventAgrigator.HandlerCountFor(typeof(Message4)));
            Debug.WriteLine("Count for Message5: " + _eventAgrigator.HandlerCountFor(typeof(Message5)));
        }

        private void Remove_OnClick(object sender, RoutedEventArgs e) {
            Button btn = (Button) sender;
            var nmb = int.Parse(Regex.Replace(btn.Name, "[^0-9.]", ""));
            object ob = null;
            switch (nmb) {
                case 1:
                    ob = _objectList.OfType<ViewModel1>().FirstOrDefault();
                    break;
                case 2:
                    ob = _objectList.OfType<ViewModel2>().FirstOrDefault();
                    break;
                case 3:
                    ob = _objectList.OfType<ViewModel3>().FirstOrDefault();
                    break;
                case 4:
                    ob = _objectList.OfType<ViewModel4>().FirstOrDefault();
                    break;
                case 5:
                    ob = _objectList.OfType<ViewModel5>().FirstOrDefault();
                    break;
            }
            if (ob != null)
                _objectList.Remove(ob);
        }

        private void Message_OnClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            var nmb = int.Parse(Regex.Replace(btn.Name, "[^0-9.]", ""));
            object ob = null;
            switch (nmb)
            {
                case 1:
                    ob = new Message1();
                    break;
                case 2:
                    ob = new Message2();
                    break;
                case 3:
                    ob = new Message3();
                    break;
                case 4:
                    ob = new Message4();
                    break;
                case 5:
                    ob = new Message5();
                    break;
            }
            if (ob != null)
                _eventAgrigator.PublishOnCurrentThread(ob);
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            var nmb = int.Parse(Regex.Replace(btn.Name, "[^0-9.]", ""));
            object ob = null;
            switch (nmb)
            {
                case 1:
                    ob = new ViewModel1(_eventAgrigator);
                    break;
                case 2:
                    ob = new ViewModel2(_eventAgrigator);
                    break;
                case 3:
                    ob = new ViewModel3(_eventAgrigator);
                    break;
                case 4:
                    ob = new ViewModel4(_eventAgrigator);
                    break;
                case 5:
                    ob = new ViewModel5(_eventAgrigator);
                    break;
            }
            if (ob != null)
                _objectList.Add(ob);
        }

        public void Handle(EventAggregator.MessageAdded message) {
            Debug.WriteLine("MainWindow MessageAdded: "+message.Type.Name+ " Remaining: " + _eventAgrigator.HandlerCountFor(message.Type));
        }

        public void Handle(EventAggregator.MessageRemoved message)
        {
            Debug.WriteLine("MainWindow MessageRemoved: " + message.Type.Name + " Remaining: " + _eventAgrigator.HandlerCountFor(message.Type));
        }
    }
}
