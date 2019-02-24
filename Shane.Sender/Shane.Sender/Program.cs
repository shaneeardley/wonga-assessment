using RabbitMQ.Client;
using System;
using System.Text;

namespace Shane.Sender
{
    class Program
    {
        static IConnection connection;
        static IModel channel;
        static string queueName = "shane";


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Shane.Sender - used to post messages to RabbitMQ");
            initiateConnection();
            loopInput();

        }

        private static void loopInput()
        {
            //Show message to input name or press exit
            Console.WriteLine("Please enter your name, or input 'exit' to shutdown");
            var inputtedName = Console.ReadLine().Trim();
            if (String.IsNullOrEmpty(inputtedName))
            {
                Console.WriteLine("No name detected...");
                loopInput();
            }
            if(inputtedName.ToUpper() == "EXIT")
            {
              
                shutDown();

            }
            var sendMessage = $"Hello my name is, {inputtedName}";
            Console.WriteLine($"Sending message '{sendMessage}'");
            channel.BasicPublish("", queueName, null, Encoding.UTF8.GetBytes(sendMessage));
            Console.WriteLine("Message sent successfully! ");
            loopInput();
        }

        ~Program()
        {
            closeConnection();
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
        }
        #endregion



    }
}
