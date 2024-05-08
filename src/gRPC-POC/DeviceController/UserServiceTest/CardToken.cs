using Google.Protobuf;
using Gsdk.Card;
using Gsdk.Device;
using Gsdk.User;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

        public async Task TestAsync(uint deviceID, string userID)
        {
            Console.WriteLine(Environment.NewLine + "===== Card Token Test =====" + Environment.NewLine);

            Console.WriteLine(">> Place a unregistered card on the device...");

            var cardData = new CardData { CSNCardData = null, SmartCardData = null };
            cardData.Type = Type.CardTypeCsn;
            cardData.CSNCardData = new CSNCardData();

            byte[] csnCardData = new byte[32];

            // 7907101
            var cardNumber = Convert.ToUInt64(7907101);
            byte[] csnData = new byte[32];
            
            csnData = BitConverter.GetBytes(cardNumber);
            
            Array.Reverse(csnData);
            
            Array.Copy(csnData, 0, csnCardData, csnCardData.Length - csnData.Length, csnData.Length);

            cardData.CSNCardData.Type = Type.CardTypeCsn;
            cardData.CSNCardData.Size = 32;

            cardData.CSNCardData.Data = ByteString.CopyFrom(csnCardData);

            var userCard = new UserCard { UserID = userID };
            
            userCard.Cards.Add(cardData.CSNCardData);
            
            await userSvc.SetCardAsync(deviceID, new UserCard[] { userCard });

            var newUserInfo = await userSvc.GetUserAsync(deviceID, new string[] { userID });
            
            Console.WriteLine("card data:{0}", newUserInfo.FirstOrDefault().Cards[0].Data.ToString());

            KeyInput.PressEnter(">> Press ENTER to end the test." + Environment.NewLine);
        }

        public async Task GetUserInfoTestAsync(uint deviceID, string userID)
        {
            var newUserInfo = await userSvc.GetUserAsync(deviceID, new string[] { userID });

            Console.WriteLine("card data:{0}", newUserInfo.FirstOrDefault().Cards[0].Data.ToString());

            KeyInput.PressEnter(">> Press ENTER to end the test." + Environment.NewLine);
        }

        private byte[] GetCardNumber(int cardNumber)
        {
            // Card Type for CSN : 1
            // For CSN you will need to enroll the card type as 1 which means CSN card.

            // When enrolling a wiegand card such as HID Prox or iClass, you will need to input the type according
            // to the wiegand format set on the device.
            // For example, if a 26 bit format is stored on the BS2WiegandMultiConfig.formats[0], and if you want to enroll
            // a 26 bit wiegand card, the type should be 0x1A
            // If a 37 bit format is stored on the BS2WiegandMultiConfig.formats[1] the type should be 0x2A
            // It goes on and on with the same pattern.


                //byte[] csnData = new byte[cardNumber.ToString().Length];

                //csnData = BitConverter.GetBytes(cardNumber);
                //Array.Reverse(csnData);

                //Array.Clear(csnCard.data, 0, csnCard.data.Length);
                //Array.Copy(csnData, 0, csnCard.data, csnCard.data.Length - csnData.Length, csnData.Length);

                //byte[] cardData = new byte[structSize];

                //Marshal.StructureToPtr(csnCard, csnObj, true);
                //Marshal.Copy(csnObj, cardData, 0, structSize);



                //Marshal.Copy(cardData, 0, cardObj, structSize);
                //cardObj += structSize;

                //return cardData;

            return null;
        }
    }
}