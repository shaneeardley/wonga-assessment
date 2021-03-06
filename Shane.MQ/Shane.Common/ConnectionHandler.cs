﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Shane.Common
{
    public class ConnectionHandler
    {
        #region class members
        public static IConnection connection;
        public static IModel channel;
        public static string queueName = "shane";
        static EventingBasicConsumer consumer;
        public event EventHandler<BasicDeliverEventArgs> Message_Received;
        #endregion

        #region public methods
        public void PublishMessage(string sendMessage)
        {
            try
            {
                Console.WriteLine($"Sending message '{sendMessage}'");
                channel.BasicPublish("", queueName, null, Encoding.UTF8.GetBytes(sendMessage));
                Console.WriteLine("Message sent successfully! ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to send a message\r\nError:\r\n{ex.ToString()}");
                ShutDown();
            }
        }
        public void ShutDown()
        {
            Console.WriteLine("Shutting down, please press enter...");
            Console.ReadLine();
            CloseConnection();
            Environment.Exit(0);
        }

        public void CloseConnection()
        {
            if (channel != null) channel.Dispose();
            channel = null;
            if (connection != null) connection.Dispose();
            connection = null;
            if (consumer != null)
            {
                consumer.Received -= Message_Received;
                consumer = null;
            }
        }

        public void InitiateSenderConnection()
        {
            this.initiateConnection(false);
        }

        public void InitiateReceiverConnection()
        {
            this.initiateConnection(true);
        }

        public void CreateConnection(string hostName)
        {
            createConnection(hostName);
        }

        public void ConsumeQueue(string consumeQueueName)
        {
            consumeQueue(consumeQueueName);
        }
        public void InitiateQueue(string newQueueName)
        {
            initiateQueue(newQueueName);
        }
        #endregion

        #region private methods

        private void createConnection(string hostName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        private void initiateQueue(string newQueueName)
        {
            channel.QueueDeclare(newQueueName, false, false, false, null);
        }

        private void consumeQueue(string consumeQueueName)
        {
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += Message_Received;
            channel.BasicConsume(consumeQueueName, true, consumer);

        }

        private void initiateConnection(bool isReceiverConnection)
        {
            Console.WriteLine("Connecting to Rabbit MQ Server");
            Console.WriteLine("Default Connection: 'localhost'.");
            Console.WriteLine("Press enter to continue, or input a different hostname and press enter");
            var hostName = Console.ReadLine();

            if (string.IsNullOrEmpty(hostName)) hostName = "localhost";
            try
            {
                createConnection(hostName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to RabbitMQ server {hostName}\r\nException Details:\r\n{ex.ToString()}");
                ShutDown();
            }

            Console.WriteLine($"Successfully connected to RabbitMQ server with hostname: {hostName}");
            Console.WriteLine($"Default Queue: '{queueName}'.");
            Console.WriteLine($"Press enter to continue, or input a different queue name and press enter");
            var inputQueueName = Console.ReadLine();
            if (!string.IsNullOrEmpty(inputQueueName))
                queueName = inputQueueName;
            Console.WriteLine($"Successfully connected to queue with name {queueName}");
            initiateQueue(queueName);
            if (isReceiverConnection)
            {
                consumeQueue(queueName);
            }
        }

        #endregion
    }
}
