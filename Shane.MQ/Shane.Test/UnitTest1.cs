using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shane.Common;
using System.Text;
using System.Threading;

namespace Shane.Test
{
    [TestClass]
    public class UnitTest1
    {
        private string newMessage;
        [TestMethod]
        public void CanConnectToRabbitMQ()
        {
            ConnectionHandler connectionHandler = new ConnectionHandler();
            connectionHandler.CreateConnection("localhost");
            connectionHandler.InitiateQueue("shane");
            //No exception thrown
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void CanPostToMQ()
        {
            ConnectionHandler connectionHandler = new ConnectionHandler();
            connectionHandler.CreateConnection("localhost");
            connectionHandler.InitiateQueue("shane");
            connectionHandler.PublishMessage("Test");
            //No exception thrown
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void CanReadFromMQ() // Will only pass if CanPostToMQ has run, or  a message exists on the queue
        {
            ConnectionHandler connectionHandler = new ConnectionHandler();
            newMessage = null;
            connectionHandler.Message_Received += ConnectionHandler_Message_Received;
            connectionHandler.CreateConnection("localhost");
            connectionHandler.InitiateQueue("shane");
            connectionHandler.ConsumeQueue("shane");
            Thread.Sleep(100);
            Assert.IsNotNull(newMessage);
        }

        [TestMethod]
        public void CanConnectAndSendAndReceive()
        {

            ConnectionHandler connectionHandler = new ConnectionHandler();
            newMessage = null;
            connectionHandler.Message_Received += ConnectionHandler_Message_Received;
            connectionHandler.CreateConnection("localhost");
            connectionHandler.InitiateQueue("shane");
            connectionHandler.ConsumeQueue("shane");
            connectionHandler.PublishMessage("Test");
            Thread.Sleep(100);
            Assert.IsNotNull(newMessage);
        }

        private void ConnectionHandler_Message_Received(object sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            newMessage = Encoding.UTF8.GetString(e.Body);
        }
    }
}
