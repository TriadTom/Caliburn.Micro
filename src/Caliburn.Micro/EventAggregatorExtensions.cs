namespace Caliburn.Micro {
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extensions for <see cref="IEventAggregator"/>.
    /// </summary>
    public static class EventAggregatorExtensions {
        /// <summary>
        /// Publishes a message on the current thread (synchrone).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        /// <param name = "filter">The filter instance.</param>
        public static void PublishOnCurrentThread(this IEventAggregator eventAggregator, object message, string filter = null) {
            eventAggregator.Publish(message, action => action(), filter);
        }
        
        /// <summary>
        /// Publishes a message on a background thread (async).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        /// <param name = "filter">The filter instance.</param>
        public static void PublishOnBackgroundThread(this IEventAggregator eventAggregator, object message, string filter = null)
        {
            eventAggregator.Publish(message, action => Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default), filter);
        }

        /// <summary>
        /// Publishes a message on the UI thread.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        /// <param name = "filter">The filter instance.</param>
        public static void PublishOnUIThread(this IEventAggregator eventAggregator, object message, string filter = null) {
            eventAggregator.Publish(message, Execute.OnUIThread, filter);
        }

        /// <summary>
        /// Publishes a message on the UI thread asynchrone.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        /// <param name = "filter">The filter instance.</param>
        public static void BeginPublishOnUIThread(this IEventAggregator eventAggregator, object message, string filter = null) {
            eventAggregator.Publish(message, Execute.BeginOnUIThread, filter);
        }

        /// <summary>
        /// Publishes a message on the UI thread asynchrone.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="message">The message instance.</param>
        /// <param name = "filter">The filter instance.</param>
        public static Task PublishOnUIThreadAsync(this IEventAggregator eventAggregator, object message, string filter = null) {
            Task task = null;
            eventAggregator.Publish(message, action => task = action.OnUIThreadAsync(), filter);
            return task;
        }
    }
}
