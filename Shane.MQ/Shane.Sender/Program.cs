using RabbitMQ.Client;
using Shane.Common;
using System;
using System.Text;

namespace Shane.Sender
{
    class Program
    {
        static ConnectionHandler connectionHandler = new ConnectionHandler();


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Shane.Sender - used to post messages to RabbitMQ");
            connectionHandler.InitiateSenderConnection();
            loopInput();
        }

        private static void loopInput()
        {
            //Show message to input name or press exit
            Console.WriteLine("\r\n======================================================");
            Console.WriteLine("Please enter your name, or input 'exit' to shutdown...");
            Console.WriteLine("======================================================\r\n");
            var inputtedName = Console.ReadLine().Trim();
            if (String.IsNullOrEmpty(inputtedName))
            {
                Console.WriteLine("No name detected...");
                loopInput();
            }
            if(inputtedName.ToUpper() == "EXIT")
            {

                connectionHandler.ShutDown();

            }
            var sendMessage = $"Hello my name is, {inputtedName}";
            connectionHandler.PublishMessage(sendMessage);
            loopInput();
        }

        ~Program()
        {
            connectionHandler.CloseConnection();
        }
    }
}
