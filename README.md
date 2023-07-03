# biostar-device-controller

net stop hns 
 net start hns   
 .\device_gateway.exe

Net2 integration setup
 https://support.supremainc.com/en/support/solutions/articles/24000057458-suprema-integration-with-paxton-net2

Default device config:
=========================================================================================
>>> Original Wiegand Config

Wiegand Config: { "outPulseWidth": 40, "outPulseInterval": 10000, "formats": [ { "length": 26, "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] } ], "useWiegandUserID": "WIEGAND_OUT_CARD_ID" }

Format 0
Format ID: 0, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0

===== Wiegand Config Test

=========================================================================================

Config that works:

>>> Original Wiegand Config

Wiegand Config: { "mode": "WIEGAND_OUT_ONLY", "outPulseWidth": 40, "outPulseInterval": 2000, "formats": [ { "length": 26, "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] } ], "CSNFormat": { "formatID": 1, "length": 26, "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] }, "useWiegandUserID": "WIEGAND_OUT_CARD_ID" }

Format 0
Format ID: 0, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
CSN Format
Format ID: 1, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0

===== Wiegand Config Test

We need to add a token to Net2 and the suprema device
It's a card token on Net2 
and a card id on the suprema device

Token on Net2:

  {
    "id": 1,
    "tokenType": "FingerprintVerificationCard",
    "tokenValue": "7907081",
    "isLost": false
  }

Test User: { "hdr": { "ID": "1688135759" }, "setting": { "faceAuthExtMode": 11, "fingerAuthExtMode": 16, "cardAuthExtMode": 21, "IDAuthExtMode": 255 } }

7907080
  {
    "id": 1688135759,
    "tokenType": "FingerprintVerificationCard",
    "tokenValue": "7907080",
    "isLost": false
  }
