# What is Stargate

Stargate is a message queue built on .NET Core to help Aiursoft's microservices system communicate in real time.

## Which application is Stargate suitable

Stargate acts as a message queue for handling the issue of pushing all complex events in turn. In the Stargate, the issuer of the event is called the `producer` and the recipient of the event is called the `consumer`.

Many applications can use Stargate to process message pushes, which includes:

* Instant Messaging App
* Battle game that requires real-time response
* Highly immediacy web applications, such as forums
* Cross-device interaction applications

When Stargate is working, you need to create a message channel first, and the consumer connects to the channel. If you need to push the message to the consumer, the producer can use the Restful API.

## Supported protocol

At present, Stargate only supports the WebSocket protocol.

## Message processing

The consumer can only receive messages from the producer after the connection is successful.

Stargate is not responsible for storing messages. Messages are deleted as soon as they are processed by all consumers.

The message period of Stargate is 1 minute. That is to say, after the producer submits the message to the Stargate, the consumer must process the message within 1 minute. Otherwise, the message will be lost.

## Stargate lifecycle

It acts as a broadcast platform, allowing developers to create several new channels for their specific applications. You cannot access each other's channels between different applications. Each channel will have a connection key.

It acts as a push tool itself, and consumers can connect to specific channels using the WebSocket protocol. After the consumer connects to the channel, the producer pushes any information to the channel, and all consumers of that channel receive the message.

The life of the channel itself is 24 hours. In the channel, the server will only push the most primitive event content, without any encoding and case conversion, and will not use JSON or XML.

It is not responsible for checking that the consumer is actually reading the message and is not responsible for receiving any message from the consumer.

## Continue reading

If you plan to use Stargate in your project, please follow the steps below

* [Create a new app in the Aiursoft Developer Center](https://developer.aiursoft.com)
* [Read Stargate API Documentation](./Channel.md)
