using EasyStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RTSEngine;

namespace GameStateManagement
{
    class PressStartScreen : MenuScreen
    {
        IAsyncSaveDevice saveDevice;

        public PressStartScreen()
            : base("")
        {
            MenuEntry startMenuEntry = new MenuEntry("Press Start to continue");
            startMenuEntry.Selected += StartMenuEntrySelected;
            MenuEntries.Add(startMenuEntry);
        }

        void StartMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            PromptMe();
        }

        private void PromptMe()
        {
            // we can set our supported languages explicitly or we can allow the
            // game to support all the languages. the first language given will
            // be the default if the current language is not one of the supported
            // languages. this only affects the text found in message boxes shown
            // by EasyStorage and does not have any affect on the rest of the game.
            EasyStorageSettings.SetSupportedLanguages(Language.French, Language.Spanish);

            // on Windows Phone we use a save device that uses IsolatedStorage
            // on Windows and Xbox 360, we use a save device that gets a 
            //shared StorageDevice to handle our file IO.
#if WINDOWS_PHONE
            saveDevice = new IsolatedStorageSaveDevice();
            Global.SaveDevice = saveDevice;

            // we use the tap gesture for input on the phone
            TouchPanel.EnabledGestures = GestureType.Tap;
#else
            // create and add our SaveDevice
            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
            ScreenManager.Game.Components.Add(sharedSaveDevice);

            // make sure we hold on to the device
            saveDevice = sharedSaveDevice;

            // hook two event handlers to force the user to choose a new device if they cancel the
            // device selector or if they disconnect the storage device after selecting it
            sharedSaveDevice.DeviceSelectorCanceled +=
                (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceDisconnected +=
                (s, e) => e.Response = SaveDeviceEventResponse.Force;

            // prompt for a device on the first Update we can
            sharedSaveDevice.PromptForDevice();

            sharedSaveDevice.DeviceSelected += (s, e) =>
            {
                //Save our save device to the global counterpart, so we can access it
                //anywhere we want to save/load
                GameSettings.SaveDevice = (SaveDevice)s;

                //Once they select a storage device, we can load the main menu.
                //You'll notice I hard coded PlayerIndex.One here. You'll need to 
                //change that if you plan on releasing your game. I linked to an
                //example on how to do that but here's the link if you need it.
                //http://blog.nickgravelyn.com/2009/03/basic-handling-of-multiple-controllers/
                ScreenManager.AddScreen(new MainMenuScreen(), PlayerIndex.One);
            };
#endif

#if XBOX
            // add the GamerServicesComponent
            ScreenManager.Game.Components.Add(
                new Microsoft.Xna.Framework.GamerServices.GamerServicesComponent(ScreenManager.Game));
#endif
        }
    }
}
