
>>> Original Wiegand Config

Wiegand Config: { "outPulseWidth": 40, "outPulseInterval": 10000, "formats": [ { "length": 26, "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] } ], "useWiegandUserID": "WIEGAND_OUT_CARD_ID" }

Format 0
Format ID: 0, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0

===== Wiegand Config Test

>>> Wiegand Config with Standard 26bit Format

Wiegand Config: { "outPulseWidth": 40, "outPulseInterval": 10000, "formats": [ { "length": 26, "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] } ], "useWiegandUserID": "WIEGAND_OUT_CARD_ID" }

Format 0
Format ID: 0, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0

>>> Wiegand Config with HID 37bit Format

Wiegand Config: { "outPulseWidth": 40, "outPulseInterval": 10000, "formats": [ { "length": 37, "IDFields": [ "AAEBAQEBAQEBAQEBAQEBAQEAAAAAAAAAAAAAAAAAAAAAAAAAAA==", "AAAAAAAAAAAAAAAAAAAAAAABAQEBAQEBAQEBAQEBAQEBAQEBAA==" ], "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAAAAAAAAA==" }, { "parityPos": 36, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQEBAA==" } ] } ], "useWiegandUserID": "WIEGAND_OUT_CARD_ID" }

Format 0
Format ID: 0, Length: 37
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 36, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0




>>> Original Wiegand Config

Wiegand Config: { "mode": "WIEGAND_OUT_ONLY", "outPulseWidth": 40, "outPulseInterval": 10000, "formats": [ { "length": 26, "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] } ], "useWiegandUserID": "WIEGAND_OUT_CARD_ID" }

Format 0
Format ID: 0, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0

===== Wiegand Config Test

>>> Wiegand Config with Standard 26bit Format

Wiegand Config: { "mode": "WIEGAND_OUT_ONLY", "outPulseWidth": 50, "outPulseInterval": 1000, "formats": [ { "length": 26, "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] } ], "useWiegandUserID": "WIEGAND_OUT_CARD_ID" }

Format 0
Format ID: 0, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0