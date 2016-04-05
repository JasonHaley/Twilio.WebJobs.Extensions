
using Microsoft.Azure.WebJobs;
using SmsSample.Models;
using Twilio;
using Twilio.WebJobs.Extensions;

namespace SmsSample
{
    public static class SmsFunctions
    {
        public static void ProcessOrder_Declarative(
            [QueueTrigger(@"samples-orders")] Order order,
            [Twilio(
                        To = "{CustomerCell}",
                        Body = "{CustomerName}, we've received your order ({OrderId}) and have begun processing it!")]
                    Message message)
        {
            // You can set additional message properties here
        }

        public static void ProcessOrder_Imperative(
            [QueueTrigger(@"samples-orders")] Order order,
            [Twilio] ref Message message)
        {
            if (string.IsNullOrEmpty(order.CustomerCell))
            {
                // if you set the message to null before the function completes,
                // the message will NOT be sent
                message = null;
            }
            else
            {
                message.To = order.CustomerCell;
                message.Body = string.Format("{0}, we've received your order ({1}) and have begun processing it!", order.CustomerName, order.OrderId);
            }
        }

    }
}
