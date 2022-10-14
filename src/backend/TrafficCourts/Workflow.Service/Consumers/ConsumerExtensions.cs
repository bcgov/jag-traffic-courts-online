using MassTransit;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using TrafficCourts.Common;

namespace TrafficCourts.Workflow.Service.Consumers
{
    public static class ConsumerExtensions
    {
        /// <summary>
        /// Begins a message consume context scope. Adds common consume context properties: Consumer, MessageType and MessageId.
        /// </summary>
        /// <returns>An <see cref="System.IDisposable"/> that ends the logical operation scope on dispose.</returns>
        public static IDisposable BeginScope<TConsumer, TMessage>(this ILogger<TConsumer> logger, ConsumeContext<TMessage> context) 
            where TConsumer : IConsumer<TMessage>
            where TMessage : class
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(context);

            var state = new Dictionary<string, object>();
            AddProperties<TConsumer, TMessage>(context, state);
            return logger.BeginScope(state);
        }

        /// <summary>
        /// Begins a message consume context scope. Adds common consume context properties: Consumer, MessageType and MessageId.
        /// </summary>
        /// <returns>An <see cref="System.IDisposable"/> that ends the logical operation scope on dispose.</returns>
        public static IDisposable BeginScope<TConsumer, TMessage>(this ILogger<TConsumer> logger, ConsumeContext<TMessage> context, 
            Expression<Func<TMessage,object>> property)
            where TConsumer : IConsumer<TMessage>
            where TMessage : class
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(property);

            var state = new Dictionary<string, object>();

            AddProperties<TConsumer, TMessage>(context, state);
            AddProperty(context.Message, property, state);

            return logger.BeginScope(state);
        }

        /// <summary>
        /// Begins a message consume context scope. Adds common consume context properties: Consumer, MessageType and MessageId.
        /// </summary>
        /// <returns>An <see cref="System.IDisposable"/> that ends the logical operation scope on dispose.</returns>
        public static IDisposable BeginScope<TConsumer, TMessage>(this ILogger<TConsumer> logger, ConsumeContext<TMessage> context,
            Expression<Func<TMessage, object>> property1,
            Expression<Func<TMessage, object>> property2)
            where TConsumer : IConsumer<TMessage>
            where TMessage : class
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(property1);
            ArgumentNullException.ThrowIfNull(property2);

            var state = new Dictionary<string, object>();

            AddProperties<TConsumer, TMessage>(context, state);
            AddProperty(context.Message, property1, state);
            AddProperty(context.Message, property2, state);

            return logger.BeginScope(state);
        }

        /// <summary>
        /// Begins a message consume context scope. Adds common consume context properties: Consumer, MessageType and MessageId.
        /// </summary>
        /// <returns>An <see cref="System.IDisposable"/> that ends the logical operation scope on dispose.</returns>
        public static IDisposable BeginScope<TConsumer, TMessage>(this ILogger<TConsumer> logger, ConsumeContext<TMessage> context,
            params Expression<Func<TMessage, object>>[] properties)
            where TConsumer : IConsumer<TMessage>
            where TMessage : class
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(properties);

            var state = new Dictionary<string, object>();

            AddProperties<TConsumer, TMessage>(context, state);
            foreach (var property in properties)
            {
                AddProperty(context.Message, property, state);
            }

            return logger.BeginScope(state);
        }

        private static void AddProperty<TMessage>(TMessage message, Expression<Func<TMessage, object>> property, Dictionary<string, object> state)
            where TMessage : class
        {
            Debug.Assert(message is not null);
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            PropertyInfo propertyInfo = property.GetPropertyInfo();
            object? value = propertyInfo.GetValue(message);
            AddNotNullProperty(propertyInfo.Name, value, state);
        }

        /// <summary>
        /// Adds common message properties to the state.
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="context"></param>
        /// <param name="state"></param>
        private static void AddProperties<TConsumer, TMessage>(ConsumeContext<TMessage> context, Dictionary<string, object> state)
            where TConsumer : IConsumer<TMessage>
            where TMessage : class
        {
            Debug.Assert(context is not null);
            Debug.Assert(state is not null);

            AddNotNullProperty("Consumer", typeof(TConsumer).FullName, state);
            AddNotNullProperty("MessageType", typeof(TMessage).FullName, state);
            AddNotNullProperty("MessageId", context.MessageId, state);
        }

        /// <summary>
        /// Adds property if the value is not null.
        /// </summary>
        private static void AddNotNullProperty(string name, object? value, Dictionary<string, object> state)
        {
            Debug.Assert(name is not null);
            Debug.Assert(state is not null);

            if (value is not null)
            {
                state[name] = value;
            }
        }

        /// <summary>
        /// Adds property if the value is not null.
        /// </summary>
        private static void AddNotNullProperty(string name, Guid? value, Dictionary<string, object> state)
        {
            Debug.Assert(name is not null);
            Debug.Assert(state is not null);

            if (value is not null)
            {
                state[name] = value;
            }
        }
    }
}
