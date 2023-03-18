using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.Card;

namespace example
{
    public class CardSvc
    {
        private Card.CardClient cardClient;

        public CardSvc(Channel channel)
        {
            cardClient = new Card.CardClient(channel);
        }

        public CardData Scan(uint deviceID)
        {
            var request = new ScanRequest { DeviceID = deviceID };
            var response = cardClient.Scan(request);

            return response.CardData;
        }

        public RepeatedField<BlacklistItem> GetBlacklist(uint deviceID)
        {
            var request = new GetBlacklistRequest { DeviceID = deviceID };
            var response = cardClient.GetBlacklist(request);

            return response.Blacklist;
        }

        public void AddBlacklist(uint deviceID, BlacklistItem[] cardInfos)
        {
            var request = new AddBlacklistRequest { DeviceID = deviceID };
            request.CardInfos.AddRange(cardInfos);

            cardClient.AddBlacklist(request);
        }

        public void DeleteBlacklist(uint deviceID, BlacklistItem[] cardInfos)
        {
            var request = new DeleteBlacklistRequest { DeviceID = deviceID };
            request.CardInfos.AddRange(cardInfos);

            cardClient.DeleteBlacklist(request);
        }

        public CardConfig GetCardConfig(uint deviceID)
        {
            var request = new GetConfigRequest { DeviceID = deviceID };
            var response = cardClient.GetConfig(request);

            return response.Config;
        }

        public void SetCardConfig(uint deviceID, CardConfig config)
        {
            var request = new SetConfigRequest { DeviceID = deviceID, Config = config };
            var response = cardClient.SetConfig(request);
        }

        public QRConfig GetQRConfig(uint deviceID)
        {
            var request = new GetQRConfigRequest { DeviceID = deviceID };
            var response = cardClient.GetQRConfig(request);

            return response.Config;
        }

        public void SetQRConfig(uint deviceID, QRConfig config)
        {
            var request = new SetQRConfigRequest { DeviceID = deviceID, Config = config };
            var response = cardClient.SetQRConfig(request);
        }
    }
}