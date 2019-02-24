using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Shane.Receiver
{
    class Program
    {
        static IConnection connection;
        static IModel channel;
        static string queueName = "shane";
        static EventingBasicConsumer consumer;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Shane.Receiver - used to pull messages from RabbitMQ");
            initiateConnection();

            loopPolling();
        }

        private static void loopPolling()
        {
            Console.WriteLine("Waiting for messages, input 'exit' to shutdown");
            var input = Console.ReadLine().Trim();
            if (input.ToUpper() == "EXIT")
                shutDown();
            loopPolling();
        }

        #region connection handling
        private static void initiateConnection()
        {
            // ToDo - default to localhost, ask user
            Console.WriteLine("Connecting to Rabbit MQ Server");
            Console.WriteLine("Default Connection: 'localhost'.");
            Console.WriteLine("Press enter to continue, or input a different hostname and press enter");
            var hostName = Console.ReadLine();
            if (string.IsNullOrEmpty(hostName)) hostName = "localhost";
            var factory = new ConnectionFactory() { HostName = hostName };
            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to RabbitMQ server {hostName}\r\nException Details:\r\n{ex.ToString()}");
                shutDown();
            }
            Console.WriteLine($"Successfully connected to RabbitMQ server with hostname: {hostName}");
            Console.WriteLine($"Press enter to continue with the default queue {queueName}, or input a different queue name and press enter");
            var inputQueueName = Console.ReadLine();
            if (!string.IsNullOrEmpty(inputQueueName))
                queueName = inputQueueName;
            channel.QueueDeclare(queueName, false, false, false, null);
            Console.WriteLine($"Successfully connected to queue with name {queueName}");
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(queueName, true, consumer);
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            //Check if message matches format, display "invalid message, discarded";
            var message = Encoding.UTF8.GetString(e.Body); // Regex string;
            if(message.StartsWith("Hello my name is,"))
            {
                var name = message.Substring(18);
                Console.WriteLine($"Message Recieved from {name}. \r\nMessage:\r\n{Encoding.UTF8.GetString(e.Body)}");
                Console.WriteLine($"Writing response now.\r\n Response:\r\nHello {name}, I am your father!\r\n");
            }
            else
            {
                Console.WriteLine("Invalid message received, discarding.");
            }
        }

        private static void shutDown()
        {
            Console.WriteLine("Shutting down, please press enter...");
            Console.ReadLine();
            closeConnection();
            Environment.Exit(0);
        }

        private static void closeConnection()
        {
            if (channel != null) channel.Dispose();
            channel = null;
            if (connection != null) connection.Dispose();
            connection = null;
            if (consumer != null)
            {
                consumer.Received -= Consumer_Received;
                consumer = null;
            }
        }
        #endregion
    }
}
