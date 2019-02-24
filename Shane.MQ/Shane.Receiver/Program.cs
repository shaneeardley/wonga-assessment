using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shane.Common;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Shane.Receiver
{
    class Program
    {
        static ConnectionHandler connectionHandler = new ConnectionHandler();

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Shane.Receiver - used to pull messages from RabbitMQ");

            connectionHandler.Message_Received += ConnectionHandler_Message_Received;
            connectionHandler.InitiateReceiverConnection();
            

            loopPolling();
        }

        private static void ConnectionHandler_Message_Received(object sender, BasicDeliverEventArgs e)
        {

            //Check if message matches format, display "invalid message, discarded";
            var message = Encoding.UTF8.GetString(e.Body); // Regex string;
            if (message.StartsWith("Hello my name is,"))
            {
                var name = message.Substring(18);
                Console.WriteLine($"Message Recieved from {name}. \r\nMessage:\r\n{Encoding.UTF8.GetString(e.Body)}");
                Console.WriteLine($"Response:\r\nHello {name}, I am your father!\r\n");
                Console.WriteLine("\r\n=====================================================");
                Console.WriteLine("Waiting for new messages. Input 'exit' to shutdown...");
                Console.WriteLine("=====================================================\r\n");
            }
            else
            {
                Console.WriteLine("Invalid message received, discarding.");
            }
        }

        private static void loopPolling()
        {
            Console.WriteLine("\r\n=====================================================");
            Console.WriteLine("Waiting for new messages. Input 'exit' to shutdown...");
            Console.WriteLine("=====================================================\r\n");
            var input = Console.ReadLine().Trim();
            if (input.ToUpper() == "EXIT")
                connectionHandler.ShutDown();
            loopPolling();
        }
        
       
        
    }
}
