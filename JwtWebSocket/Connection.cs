using JWT;
using JWT.Algorithms;
using WebSocketSharp;

namespace JwtWebSocket
{
    public class Connection
    {
        private WebSocket client; // = new WebSocket(*path*);

        private IJwtAlgorithm algorithm;
        private IJsonSerializer serializer; // to be JwtWebSocket.JsonParser
        private IBase64UrlEncoder urlEncoder;
        private IJwtEncoder _encoder; // = new JwtEncoder(algorithm, serializer, urlEncoder);

        private IDateTimeProvider provider;
        private IJwtValidator validator; // = new JwtValidator(serializer, provider);
        private IJwtDecoder decoder; // = new JwtDecoder(serializer, validator, urlEncoder);
    }
}