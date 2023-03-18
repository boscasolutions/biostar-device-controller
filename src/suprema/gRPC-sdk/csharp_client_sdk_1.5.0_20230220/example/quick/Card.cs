using System;
using Gsdk.Card;
using Gsdk.Device;
using Google.Protobuf;

namespace example
{
	class CardTest
	{
		private const int NUM_OF_BLACKLIST_ITEM = 2;      
		private const int FIRST_BLACKLISTED_CARD_ID = 100000;
		private const int ISSUE_COUNT = 3;

		private CardSvc cardSvc;

		public CardTest(CardSvc svc) {
			cardSvc = svc;
		}

		public void Test(uint deviceID, CapabilityInfo capabilityInfo) {
			Console.WriteLine(">>> Scan a card...");
			
			var cardData = cardSvc.Scan(deviceID);

			Console.WriteLine("Card data: {0}" + Environment.NewLine, cardData);

			var blacklist = cardSvc.GetBlacklist(deviceID);

			Console.WriteLine("Blacklist: {0}" + Environment.NewLine, blacklist);

			var newCardInfos = new BlacklistItem[NUM_OF_BLACKLIST_ITEM];

			for(int i = 0; i < NUM_OF_BLACKLIST_ITEM; i++) {
				newCardInfos[i] = new BlacklistItem{ CardID = ByteString.CopyFromUtf8(String.Format("{0}", FIRST_BLACKLISTED_CARD_ID + i)), IssueCount = ISSUE_COUNT };
			}

			cardSvc.AddBlacklist(deviceID, newCardInfos);

			blacklist = cardSvc.GetBlacklist(deviceID);
			Console.WriteLine("Blacklist after adding new items: {0}" + Environment.NewLine, blacklist);

			cardSvc.DeleteBlacklist(deviceID, newCardInfos);
			blacklist = cardSvc.GetBlacklist(deviceID);
			Console.WriteLine("Blacklist after deleting new items: {0}" + Environment.NewLine, blacklist);

			var cardConfig = cardSvc.GetCardConfig(deviceID);

			Console.WriteLine("Card config: {0}" + Environment.NewLine, cardConfig);

			cardConfig.DESFireConfig.OperationMode = DESFireOperationMode.OperationApplevelkey;
			cardConfig.Cipher = true;
			cardConfig.SmartCardByteOrder = CardByteOrder.Lsb;

			cardSvc.SetCardConfig(deviceID, cardConfig);

			if (capabilityInfo.QRSupported) {
				var qrConfig = cardSvc.GetQRConfig(deviceID);

				Console.WriteLine("QR config: {0}" + Environment.NewLine, qrConfig);

				qrConfig.BypassData = false;
				qrConfig.TreatAsCSN = true;

				cardSvc.SetQRConfig(deviceID, qrConfig);
			}
		} 
	}
}

