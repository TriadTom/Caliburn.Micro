using System.Collections.Generic;

namespace Caliburn.Micro {
    using System;

    /// <summary>
    ///   Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public interface IEventAggregator {
        /// <summary>
        /// Searches the subscribed handlers to check if we have a handler for
        /// the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <param name="filter">The filter type to check with</param>
        /// <returns>True if any handler is found, false if not.</returns>
        bool HandlerExistsFor(Type messageType, string filter = null);
        
        /// <summary>
        /// Number of subscribed handlers for
        /// the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <param name="filter">The filter type to check with</param>
        /// <returns>The number of handlers</returns>
        int HandlerCountFor(Type messageType, string filter = null);

        /// <summary>
        /// Returns the active filters of supplied messageType.
        /// </summary>
        /// <param name="messageType">The message type to check filters</param>
        /// <returns>Returns the active filters for the given message type.</returns>
        List<string> ActiveFiltersForType(Type messageType);

        /// <summary>
        ///   Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />
        /// </summary>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        void Subscribe(object subscriber, string filter);

        /// <summary>
        ///   Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "subscriber">The instance to unsubscribe.</param>
        void Unsubscribe(object subscriber);

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name = "marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        /// <param name="filter">The filter instance</param>
        void Publish(object message, Action<System.Action> marshal, string filter = null);

        /// <summary>
        /// Set filter so it only receives messages for that particular filter type.
        /// </summary>
        /// <param name="subscriber">object subscirbed to messages.</param>
        /// <param name="messageType">message type to add filter to.</param>
        /// <param name="filter">filter object used for filtering messages.</param>
        void SetFilter(object subscriber, Type messageType, string filter);
    }
}
