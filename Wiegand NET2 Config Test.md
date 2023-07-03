>>> Wiegand Config with NET2 26bit Format

Wiegand Config: { "mode": "WIEGAND_OUT_ONLY", "outPulseWidth": 40, "outPulseInterval": 2000, "formats": [ { "length": 26, "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] } ], "CSNFormat": { "length": 26, "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] }, "useWiegandUserID": "WIEGAND_OUT_CARD_ID" }

Format 0
Format ID: 0, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
CSN Format
Format ID: 0, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0
Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0