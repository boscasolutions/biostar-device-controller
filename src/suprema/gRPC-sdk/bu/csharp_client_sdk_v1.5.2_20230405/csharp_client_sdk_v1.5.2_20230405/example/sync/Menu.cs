using System;
using Gsdk.Connect;

namespace example
{
  class TestMenu {
    private Menu menu;
    private DeviceMgr deviceMgr;
    private EventMgr eventMgr;
    private UserMgr userMgr;
    private TestConfig testConfig;

    private const string DEFAULT_USER_ID = "1234";
    private const int MAX_NEW_LOG = 16;

    public TestMenu(DeviceMgr deviceMgr, EventMgr eventMgr, UserMgr userMgr, TestConfig config) {
      this.deviceMgr = deviceMgr;
      this.eventMgr = eventMgr;
      this.userMgr = userMgr;
      this.testConfig = config;

      MenuItem[] items = new MenuItem[6];

      items[0] = new MenuItem("1", "Show test devices", ShowTestDevice, false);
      items[1] = new MenuItem("2", "Show new events", ShowNewEvent, false);
      items[2] = new MenuItem("3", "Show new users", ShowNewUser, false);
      items[3] = new MenuItem("4", "Enroll a user", EnrollUser, false);
      items[4] = new MenuItem("5", "Delete a user", DeleteUser, false);
      items[5] = new MenuItem("q", "Quit", null, true);      

      menu = new Menu(items);
    }

    public void Show() {
      menu.Show("Test Menu");
    }

    public void ShowTestDevice() {
      Console.WriteLine("***** Test Configuration:" + Environment.NewLine + "{0}" + Environment.NewLine, testConfig.GetConfigData());
      Console.WriteLine("***** Connected Devices: {0}", string.Join(", ", deviceMgr.GetConnectedDevices(true)));
    }

    public void ShowNewEvent() {
      var deviceIDs = deviceMgr.GetConnectedDevices(false);

      foreach(uint devID in deviceIDs) {
        var devInfo = testConfig.GetDeviceInfo(devID);
        if(devInfo == null) {
          Console.WriteLine("Device {0} is not in the configuration file", devID);
          continue;
        }

        Console.WriteLine("Read new event logs from device {0}...", devID);
        var eventLogs = eventMgr.ReadNewLog(devInfo, EventMgr.MAX_NUM_OF_LOG);

        Console.WriteLine("Read {0} event logs", eventLogs.Count);

        int numOfLog = eventLogs.Count;
        if(numOfLog > MAX_NEW_LOG) {
          numOfLog = MAX_NEW_LOG;
        }
    
        if(numOfLog > 0) {
          Console.WriteLine("Show the last {0} events...", numOfLog);
          for(int i = 0; i < numOfLog; i++) {
            eventMgr.PrintEvent(eventLogs[numOfLog - i - 1]);
          }
        }        
      }
    }

    public void ShowNewUser() {
      var deviceIDs = deviceMgr.GetConnectedDevices(false);

      foreach(uint devID in deviceIDs) {      
        Console.WriteLine("Read new users from device {0}...", devID);

        var userInfos = userMgr.GetNewUser(devID);
        if(userInfos != null) {
          Console.WriteLine("New users: {0}", userInfos);        
        }
      }
    }

    public string GetUserID() {
      InputItem[] items = new InputItem[1];
      items[0] = new InputItem{ text = "Enter the user ID", defaultVal = DEFAULT_USER_ID };

      var userInputs = Menu.GetUserInput(items);

      return userInputs[0];
    }

    public void EnrollUser() {
      userMgr.EnrollUser(GetUserID());
    }

    public void DeleteUser() {
      userMgr.DeleteUser(GetUserID());
    }

  }
}


