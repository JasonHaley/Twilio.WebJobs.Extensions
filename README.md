# Twilio.WebJobs.Extensions
Utility Azure Web Jobs extensions for using Twilio sms

#Twilio
A binding that will send an SMS message

Example of sending a message from an order objects sent via a queue
```
public static void ProcessOrder_Declarative(
    [QueueTrigger(@"samples-orders")] Order order,
    [Twilio(
                To = "{CustomerCell}",
                Body = "{CustomerName}, we've received your order ({OrderId}) and have begun processing it!")]
            Message message)
{
    // You can set additional message properties here
}
```
