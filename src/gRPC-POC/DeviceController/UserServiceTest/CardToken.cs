using Google.Protobuf;
using Gsdk.Card;
using Gsdk.User;
using System;
using System.Collections;
using Type = Gsdk.Card.Type;

namespace example
{
    class CardTokenTest
    {
        /*You would first need a new CardData object, 
         * this can be got from the CardData variable under card.proto
         * Once youve got this, you need to set the type (1=CSN,2=Secure,3=Access,4=Mobile,5=WiegandMobile,6=QR,7=SecureQR).
         * You then need a new CSNCardData object, under the CSNCardData object in card.proto (although, i believe it already creates this under the new CardData object by default)
         * You then need to set the type of this CSNCardData (We include it twice for some reason)
         * the size (Should always be 32 for a CSN)
         * then the actual data
         * Any of the objects included under any of the services can just be created by themselves, 
         * you then just populate the data as needed, then assign this to a user (For a new user, you'd create a new UserInfo object for example)
        */

        private CardSvc cardSvc;
        private UserSvc userSvc;

        public CardTokenTest(CardSvc cardSvc, UserSvc userSvc)
        {
            this.cardSvc = cardSvc;
            this.userSvc = userSvc;
        }

        public void Test(uint deviceID, string userID)
        {
            Console.WriteLine(Environment.NewLine + "===== Card Token Test =====" + Environment.NewLine);

            Console.WriteLine(">> Place a unregistered card on the device...");

            var cardData = new CardData { CSNCardData = null, SmartCardData = null };
            cardData.Type = Type.CardTypeCsn;
            cardData.CSNCardData = new CSNCardData();
            // 7907089
            var byteArray = ByteString.CopyFrom(new byte[]
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 9, 0, 7, 0, 8, 9});
            cardData.CSNCardData.Data = byteArray;

            if (cardData.CSNCardData == null)
            {
                Console.WriteLine("!! The card is a smart card. For this test, you have to use a CSN card. Skip the card test.");
                return;
            }

            var userCard = new UserCard { UserID = userID };
            userCard.Cards.Add(cardData.CSNCardData);
            userSvc.SetCard(deviceID, new UserCard[] { userCard });

            KeyInput.PressEnter(">> Try to authenticate the enrolled card. And, press ENTER to end the test." + Environment.NewLine);
        }
    }
}

