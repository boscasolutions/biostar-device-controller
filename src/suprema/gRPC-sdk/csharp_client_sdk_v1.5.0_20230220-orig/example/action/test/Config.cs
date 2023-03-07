using System;
using Gsdk.Action;
using Google.Protobuf;

namespace example
{
	class ConfigTest
	{
    private const int BS2_EVENT_VERIFY_FAIL = 0x1100;
    private const int BS2_EVENT_IDENTIFY_FAIL = 0x1400;

    private const int BS2_SUB_EVENT_CREDENTIAL_CARD = 0x02;
    private const int BS2_SUB_EVENT_CREDENTIAL_FINGER = 0x04;

		private ActionSvc actionSvc;

		public ConfigTest(ActionSvc svc) {
			actionSvc = svc;
		}

		public void Test(uint deviceID) {    
      // Backup the original configuration
      var origConfig = actionSvc.GetConfig(deviceID);
      Console.WriteLine(Environment.NewLine + "Original Config: {0}" + Environment.NewLine, origConfig);    

      TestEventTrigger(deviceID);

      // Restore the original configuration   
      actionSvc.SetConfig(deviceID, origConfig); 
    }

    void TestEventTrigger(uint deviceID) {
      var cardFailTrigger = new Trigger{ DeviceID = deviceID, Type = TriggerType.TriggerEvent, Event = new EventTrigger{ EventCode = BS2_EVENT_VERIFY_FAIL | BS2_SUB_EVENT_CREDENTIAL_CARD }};
      var fingerFailTrigger = new Trigger{ DeviceID = deviceID, Type = TriggerType.TriggerEvent, Event = new EventTrigger{ EventCode = BS2_EVENT_IDENTIFY_FAIL | BS2_SUB_EVENT_CREDENTIAL_FINGER }};

      var failAction = new Gsdk.Action.Action{ DeviceID = deviceID, Type = ActionType.ActionRelay, Relay = new RelayAction{ RelayIndex = 0, Signal = new Signal{ Count = 3, OnDuration = 500, OffDuration = 500}}};

      var config = new TriggerActionConfig();
      config.TriggerActions.Add( new TriggerActionConfig.Types.TriggerAction{ Trigger = cardFailTrigger, Action = failAction });
      config.TriggerActions.Add( new TriggerActionConfig.Types.TriggerAction{ Trigger = fingerFailTrigger, Action = failAction });

      actionSvc.SetConfig(deviceID, config);

      var newConfig = actionSvc.GetConfig(deviceID);
      Console.WriteLine("Test Config: {0}" + Environment.NewLine, newConfig);    

      Console.WriteLine(">> Try to authenticate a unregistered card or finger. It should trigger a relay signal.");    
      KeyInput.PressEnter(">> Press ENTER to finish the test." + Environment.NewLine);
    }
	}
}

