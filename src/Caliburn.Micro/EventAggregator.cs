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
        public bool HandlerExistsFor(Type messageType) {
            return handlers.Any(handler => handler.Handles(messageType) & !handler.IsDead);
        }

        public int HandlerCountFor(Type messageType)
        {
            return handlers.Where(handler => handler.Handles(messageType) & !handler.IsDead).Count();
        }

        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />
        /// </summary>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        public virtual void Subscribe(object subscriber) {
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
                foreach (Type t in handle.Types) {
                    this.PublishOnCurrentThread(new MessageAdded(t));
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
                    //Debug.WriteLine("Subscriber Removed: "+subscriber.GetType());
                    //found.RemoveEvents();
                    handlers.Remove(found);
                }
            }
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name = "marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        public virtual void Publish(object message, Action<System.Action> marshal) {
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
                    .Where(handler => !handler.Handle(messageType, message))
                    .ToList();

                if(dead.Any()) {
                    lock(handlers) {
                        dead.Apply(x => handlers.Remove(x));
                    }
                }
            });
        }

        public class MessageRemoved {
            public Type Type { get; }
            public MessageRemoved(Type type) {
                Type = type;
            }
        }

        public class MessageAdded
        {
            public Type Type { get; }
            public MessageAdded(Type type)
            {
                Type = type;
            }
        }

        class Handler {
            private IEventAggregator _eventAggregator;
            readonly WeakReference reference;
            readonly Dictionary<Type, MethodInfo> supportedHandlers = new Dictionary<Type, MethodInfo>();
            public List<Type> Types => supportedHandlers.Keys.ToList();
            public bool IsDead => reference.Target == null;

            ~Handler()
            {
                foreach (var h in supportedHandlers)
                {
                    //Debug.WriteLine(" Dispose Removed Type: " + h.Key.Name);
                    _eventAggregator.PublishOnCurrentThread(new MessageRemoved(h.Key));
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
                        supportedHandlers[type] = method;
                    }
                }
            }
            
            public bool Matches(object instance) {
                return reference.Target == instance;
            }

            public bool Handle(Type messageType, object message) {
                var target = reference.Target;
                if (target == null) {
                    return false;
                }

                foreach(var pair in supportedHandlers) {
                    if(pair.Key.IsAssignableFrom(messageType)) {
                        var result = pair.Value.Invoke(target, new[] { message });
                        if (result != null) {
                            HandlerResultProcessing(target, result);
                        }
                    }
                }
                
                return true;
            }

            public bool Handles(Type messageType) {
                return supportedHandlers.Any(pair => pair.Key.IsAssignableFrom(messageType));
            }
        }
    }
}
