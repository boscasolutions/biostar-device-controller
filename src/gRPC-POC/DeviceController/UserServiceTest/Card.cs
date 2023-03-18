using Gsdk.User;
using System;

namespace example
{
    class CardTest
    {
        private CardSvc cardSvc;
        private UserSvc userSvc;

        public CardTest(CardSvc cardSvc, UserSvc userSvc)
        {
            this.cardSvc = cardSvc;
            this.userSvc = userSvc;
        }

        public void Test(uint deviceID, string userID)
        {
            Console.WriteLine(Environment.NewLine + "===== Card Test =====" + Environment.NewLine);

            Console.WriteLine(">> Place a unregistered card on the device...");

            var cardData = cardSvc.Scan(deviceID);

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

