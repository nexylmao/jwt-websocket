# jwt-websocket
This is a library, that incorporates dotnet-jwt, newtonsoft.json and websocket-sharp, to make a simple to use, websocket client that can automatically verify messages, in form of JWT's, or ones sent as stringified Json's. For communication, a class is used, named 'SocketMessage<>'

## Class SocketMessage<>
This class is the type in which messages will be received or emitted.
```
public class SocketMessage<T> : EventArgs
{
    public string tag, message;
    public T data;

    public SocketMessage(string tag, string message, T data)
    {
        this.tag = tag;
        this.message = message;
        this.data = data;
    }
}
```

## How does this work?
### Basics
First I'm gonna show how basic things from WebSocket client are done.

Well, still kinda simple. The first thing that's different, is that when you make a WebSocket client, you also make a JsonWebToken encoder and decoder. So, when you make a *Connection*, you need to provide both a path to the WebSocket server, and a secret for JWT's.
```
Connection connection = new Connection(path, secret);
// string path, secret
```

#### All events are still accessible through Subscribe{*event*}(EventHandler), and Unsubscribe{*event*}(EventHandler)

Intention behind this, was to encourage use of non-anonymous methods to handle events.

```
connection.SubscribeOnOpen(onOpenEvent); // EventHandler onOpenEvent
connection.UnsubscribeOnOpen(onOpenEvent); // EventHandler onOpenEvent
connection.SubscribeOnMessage(onMessageEvent); // EventHandler<MessageEventArgs> onMessageEvent
connection.UnsubscribeOnMessage(onMessageEvent); // EventHandler<MessageEventArgs> onMessageEvent
connection.SubscribeOnEvent(onErrorEvent); // EventHandler<ErrorEventArgs> onMessageEvent
connection.UnsubscribeOnEvent(onErrorEvent); // EventHandler<ErrorEventArgs> onMessageEvent
connection.SubscribeOnClose(onCloseEvent); // EventHandler<CloseEventArgs> onMessageEvent
connection.UnsubscribeOnClose(onCloseEvent); // EventHandler<CloseEventArgs> onMessageEvent
```

#### And to send things to the serve
```
connection.Send(data, jwt); // EventArgs data, bool jwt
```

#### Open/close the connection
```
connection.Start();
connection.Close();
```

## Now, what's new
OnMessageHandler, is a class that integrates into our OnMessage event, it keeps track of events tied to tags. So, you pretty much, sign a tag with the type of the data field of the message, that will show up with the tag.

```
OnMessageHandler messageHandler = new OnMessageHandler();
OnMessageHandler messageHandler = new OnMessageHandler((sender, message) => 
{
     // do stuff with message
});
// object sender, SocketMessage<object> message
```
There are two constructors, one will not handle 'default' case, which is when the tag is not signed, and the second one will.

And to get the Event for a tag, you can either sign it :
```
ET event = messageHandler.SignTag("firstTag", typeof(T));
```
or get one which you already signed :
```
ET event = messageHandler.GetHandler("firstTag");
```

When you have the event, you can simply do :
```
event.Event += (sender, message) => // object sender, SocketMessage<T> message
{
    // do stuff with message
};
```

And when you're done with signing all the tags and events, you attach the messageHandler
```
connection.MessageHandler = messageHandler;
```
