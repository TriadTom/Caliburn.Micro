using System.Diagnostics;

namespace Caliburn.Micro {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public class EventAggregator : IEventAggregator {
        readonly List<Handler> handlers = new List<Handler>();

        /// <summary>
        /// Processing of handler results on publication thread.
        /// </summary>
        public static Action<object, object> HandlerResultProcessing = (target, result) => { };

        /// <summary>
        /// Searches the subscribed handlers to check if we have a handler for
        /// the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <returns>True if any handler is found, false if not.</returns>
        public bool HandlerExistsFor(Type messageType, string filter = null) {
            return handlers.Any(handler => handler.Handles(messageType, filter) & !handler.IsDead);
        }

        /// <summary>
        /// Returns the number of supplied handlers for
        /// the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <returns>Returns the count of handlers for that message type.</returns>
        public int HandlerCountFor(Type messageType, string filter = null) {
            lock (handlers) {
                return handlers.Count(handler => handler.Handles(messageType, filter) & !handler.IsDead);
            }
        }

        /// <summary>
        /// Returns the active filters of supplied messageType.
        /// </summary>
        /// <param name="messageType">The message type to check filters</param>
        /// <returns>Returns the active filters for the given message type.</returns>
        public List<string> ActiveFiltersForType(Type messageType) {
            lock (handlers) {
                return handlers.Where(handler => handler.ContainsFilterdOfType(messageType) & !handler.IsDead).Select(x => x.GetFilterForType(messageType)).ToList();
            }
        }

        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />
        /// </summary>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        /// <param name="filter">filter for all message types</param>
        public virtual void Subscribe(object subscriber, string filter = null) {
            if (subscriber == null) {
                throw new ArgumentNullException("subscriber");
            }
            //Debug.WriteLine("Subscriber Added: " + subscriber.GetType());
            lock (handlers) {
                if (handlers.Any(x => x.Matches(subscriber))) {
                    return;
                }
                var handle = new Handler(subscriber, this);
                handlers.Add(handle);
                foreach (Type t in handle.SupportedMessageTypes) {
                    this.PublishOnCurrentThread(new MessageAdded(t, filter));
                }
            }
        }

        /// <summary>
        /// Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "subscriber">The instance to unsubscribe.</param>
        public virtual void Unsubscribe(object subscriber) {
            if (subscriber == null) {
                throw new ArgumentNullException("subscriber");
            }
            lock(handlers) {
                var found = handlers.FirstOrDefault(x => x.Matches(subscriber));

                if (found != null) {
                    handlers.Remove(found);
                }
            }
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name = "marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        /// <param name="filter">The filter instance.</param>
        public virtual void Publish(object message, Action<System.Action> marshal, string filter = null) {
            if (message == null){
                throw new ArgumentNullException("message");
            }
            if (marshal == null) {
                throw new ArgumentNullException("marshal");
            }

            Handler[] toNotify;
            lock (handlers) {
                toNotify = handlers.ToArray();
            }

            marshal(() => {
                var messageType = message.GetType();

                var dead = toNotify
                    .Where(handler => !handler.Handle(messageType, message, filter))
                    .ToList();

                if(dead.Any()) {
                    lock(handlers) {
                        dead.Apply(x => handlers.Remove(x));
                    }
                }
            });
        }

        /// <summary>
        /// Set filter so it only receives messages for that particular filter type.
        /// </summary>
        /// <param name="subscriber">object subscirbed to messages.</param>
        /// <param name="messageType">message type to add filter to.</param>
        /// <param name="filter">filter object used for filtering messages.</param>
        public void SetFilter(object subscriber, Type messageType, string filter) {
            lock (handlers) {
                handlers.FirstOrDefault(x => x.Matches(subscriber))?.SetFilter(messageType, filter);
            }
        }

        /// <summary>
        /// Message Type for when message subscriptions are removed.
        /// </summary>
        public class MessageRemoved {
            public Type Type { get; }
            public string Filter { get; }
            public MessageRemoved(Type type, string filter) {
                Type = type;
                Filter = filter;
            }
        }

        /// <summary>
        /// Message Type for when message subscriptions are added.
        /// </summary>
        public class MessageAdded
        {
            public Type Type { get; }
            public string Filter { get;  }
            public MessageAdded(Type type, string filter)
            {
                Type = type;
                Filter = filter;
            }
        }

        class HandlerMethod {
            public MethodInfo MethodInfo => _methodInfo;
            private readonly MethodInfo _methodInfo;
            public string Filter => _filter;
            private string _filter;

            public HandlerMethod(MethodInfo methodInfo, string filter = null) {
                _methodInfo = methodInfo;
                _filter = filter;
            }

            public void SetFilter(string filter) {
                _filter = filter;
            }
        }

        class Handler {
            private IEventAggregator _eventAggregator;
            readonly WeakReference reference;
            readonly Dictionary<Type, HandlerMethod> supportedHandlers = new Dictionary<Type, HandlerMethod>();
            public List<Type> SupportedMessageTypes => supportedHandlers.Keys.ToList();
            public bool IsDead => reference.Target == null;

            ~Handler()
            {
                foreach (var h in supportedHandlers)
                {
                    _eventAggregator.PublishOnCurrentThread(new MessageRemoved(h.Key, h.Value.Filter));
                }
            }

            public Handler(object handler, IEventAggregator eventAggregator) {
                _eventAggregator = eventAggregator;
                reference = new WeakReference(handler, true);
                var interfaces = handler.GetType().GetInterfaces()
                    .Where(x => typeof(IHandle).IsAssignableFrom(x) && x.IsGenericType());

                foreach(var @interface in interfaces) {
                    var type = @interface.GetGenericArguments()[0];
                    var method = @interface.GetMethod("Handle", new[] { type });
                    //Debug.WriteLine("Added Reference: "+reference.Target.GetType().Name+" Type: "+type.Name);
                    if (method != null) {
                        supportedHandlers[type] = new HandlerMethod(method);
                    }
                }
            }

            public void SetFilter(Type messageType, string filter) {
                var type = supportedHandlers.FirstOrDefault(pair => pair.Key.IsAssignableFrom(messageType));
                if (type.Key != null && type.Value.Filter != filter) {
                    var oldFilter = type.Value.Filter;
                    type.Value.SetFilter(filter);
                    _eventAggregator.PublishOnCurrentThread(new MessageRemoved(type.Key, oldFilter));
                    _eventAggregator.PublishOnCurrentThread(new MessageAdded(type.Key, type.Value.Filter));
                }
            }

            public string GetFilterForType(Type messageType) {
                return supportedHandlers.FirstOrDefault(
                    pair => pair.Key.IsAssignableFrom(messageType) && pair.Value.Filter != null).Value?.Filter;
            }
            
            public bool Matches(object instance) {
                return reference.Target == instance;
            }
            
            public bool Handle(Type messageType, object message, string filter)
            {
                var target = reference.Target;
                if (target == null)
                {
                    return false;
                }

                foreach (var pair in supportedHandlers)
                {
                    if (!pair.Key.IsAssignableFrom(messageType) || (pair.Value.Filter != filter && pair.Value.Filter != null)) continue;
                    var result = pair.Value.MethodInfo.Invoke(target, new[] { message });
                    if (result != null)
                    {
                        HandlerResultProcessing(target, result);
                    }
                }

                return true;
            }

            public bool ContainsFilterdOfType(Type messageType)
            {
                return supportedHandlers.Any(pair => pair.Key.IsAssignableFrom(messageType) && pair.Value.Filter != null);
            }
            public bool Handles(Type messageType, string filter) {
                return supportedHandlers.Any(pair => pair.Key.IsAssignableFrom(messageType) && pair.Value.Filter == filter);
            }
        }
    }
}
