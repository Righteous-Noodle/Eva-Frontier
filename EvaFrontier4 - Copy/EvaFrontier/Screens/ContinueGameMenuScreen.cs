using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Components;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameStateManagement
{
    class ContinueGameMenuScreen: MenuScreen
    {
 /*       #region Fields
        ISaveDevice saveDevice;
        //ContentManager content;
        //Texture2D background1;
        MenuEntry1 slot1;
        MenuEntry1 slot2;
        MenuEntry1 slot3;
        MenuEntry1 slot4;
        MenuEntry1 slot5;

        private SaveGameDescription loadedSaveGameDescription;
        private SaveGameDescription savedSaveGameDescription;

        public string StorageContainerName = "Eva Frontier";

        private readonly XmlSerializer serializer = new XmlSerializer(typeof(SaveGameDescription));
        #endregion
*/
        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public ContinueGameMenuScreen() : base("Continue Game")
        {
            IsPopup = true;

            MenuEntry backMenuEntry = new MenuEntry("Back");
            backMenuEntry.Selected += OnCancel;

/*            slot1 = new MenuEntry1("Slot 1");

            slot2 = new MenuEntry1("Slot 2");

            slot3 = new MenuEntry1("Slot 3");

            slot4 = new MenuEntry1("Slot 4");

            slot5 = new MenuEntry1("Slot 5");

            SetMenuText();

            slot1.Selected += Slot1Selected;
            slot2.Selected += Slot2Selected;
            slot3.Selected += Slot3Selected;
            slot4.Selected += Slot4Selected;
            slot5.Selected += Slot5Selected;

            MenuEntries.Add(slot1);
            MenuEntries.Add(slot2);
            MenuEntries.Add(slot3);
            MenuEntries.Add(slot4);
            MenuEntries.Add(slot5);  */
            MenuEntries.Add(backMenuEntry);
        }
        #endregion
/*
        #region Load Content
        public override void LoadContent()
        {
            //if (content == null) content = new ContentManager(ScreenManager.Game.Services, "Content");
            //background1 = content.Load<Texture2D>(@"Textures\\background1");
            CenterTitle();
            LineSpacing = ScreenManager.Font.LineSpacing * 1.25f;

            LoadStorageDevice();
        }
        #endregion

        #region Handle Input
        void Slot1Selected(object sender, PlayerIndexEventArgs e)
        {
            SetMenuText();
        }
        void Slot2Selected(object sender, PlayerIndexEventArgs e)
        {
            SetMenuText();
        }
        void Slot3Selected(object sender, PlayerIndexEventArgs e)
        {
            SetMenuText();
        }
        void Slot4Selected(object sender, PlayerIndexEventArgs e)
        {
            SetMenuText();
        }
        void Slot5Selected(object sender, PlayerIndexEventArgs e)
        {
            SetMenuText();
        }
        #endregion
*/
        #region Drawing
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 4 / 5);
            //Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            //Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            //byte fade = TransitionAlpha;
            //Color transitionColor = new Color(fade, fade, fade);
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            //spriteBatch.Draw(background1, fullscreen, transitionColor);               
            //spriteBatch.End();

            Vector2 position = new Vector2(100, 250);
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.DrawString(ScreenManager.Font, " ",
                position, Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "Sorry, Continue Game Option is",
                position + new Vector2(0f, ScreenManager.Font.LineSpacing * 2), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "not yet implemented..",
                position + new Vector2(0f, ScreenManager.Font.LineSpacing * 4), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
        #endregion
/*
        void SetMenuText()
        {
            if (Global.SaveGameDescriptions.Count > 1)
            {
                SaveGameDescription save = Global.SaveGameDescriptions[0];
                slot1.Text = string.Format("Slot 1 | Name: {0} Class: {1} Level: {2} Region: {3}\r\n       | {4}", save.PlayerName, save.PlayerClass, save.PlayerLevel, save.Region, save.Description);
            }
            if (Global.SaveGameDescriptions.Count > 2)
            {
                SaveGameDescription save = Global.SaveGameDescriptions[1];
                slot2.Text = string.Format("Slot 2 | Name: {0} Class: {1} Level: {2} Region: {3}\r\n       | {4}", save.PlayerName, save.PlayerClass, save.PlayerLevel, save.Region, save.Description);
            }
            if (Global.SaveGameDescriptions.Count > 3)
            {
                SaveGameDescription save = Global.SaveGameDescriptions[2];
                slot3.Text = string.Format("Slot 3 | Name: {0} Class: {1} Level: {2} Region: {3}\r\n       | {4}", save.PlayerName, save.PlayerClass, save.PlayerLevel, save.Region, save.Description);
            }
            if (Global.SaveGameDescriptions.Count > 4)
            {
                SaveGameDescription save = Global.SaveGameDescriptions[3];
                slot4.Text = string.Format("Slot 4 | Name: {0} Class: {1} Level: {2} Region: {3}\r\n       | {4}", save.PlayerName, save.PlayerClass, save.PlayerLevel, save.Region, save.Description);
            }
            if (Global.SaveGameDescriptions.Count > 1)
            {
                SaveGameDescription save = Global.SaveGameDescriptions[4];
                slot5.Text = string.Format("Slot 5 | Name: {0} Class: {1} Level: {2} Region: {3}\r\n       | {4}", save.PlayerName, save.PlayerClass, save.PlayerLevel, save.Region, save.Description);
            }   
        }
        void LoadStorageDevice()
            {
#if WINDOWS
            saveDevice = new PCSaveDevice(StorageContainerName);
            ReadSaves();
#else
            // add the GamerServicesComponent
            Components.Add(new GamerServicesComponent(this));

            // create and add our SaveDevice
            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice(StorageContainerName);
            Components.Add(sharedSaveDevice);

            // hook an event for when the device is selected to run our test
            sharedSaveDevice.DeviceSelected += (s, e) => ReadSaves();

            // hook two event handlers to force the user to choose a new device if they cancel the

            // device selector or if they disconnect the storage device after selecting it
            sharedSaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            // prompt for a device on the first Update we can
            sharedSaveDevice.PromptForDevice();

            // make sure we hold on to the device
            saveDevice = sharedSaveDevice;
#endif
            SetMenuText();
        }
        
        private void ReadSaves()
        {
            Global.SaveGameDescriptions.Clear();
            for (int i = 1; i <= 5; i++)
            {
                string filename = String.Format("SaveSlot{0}-Description.xml", i);

                if (!saveDevice.FileExists(StorageContainerName, filename))
                {
                    Trace.WriteLine(String.Format("Failed to find file {0}.", filename));
                    savedSaveGameDescription = new SaveGameDescription();
                    if (!saveDevice.Save(StorageContainerName, filename, SerializeSave))
                    {
                        Trace.WriteLine(string.Format("Failed to save file {0}.", filename));
                    } 
                }
                else
                {
                    if (!saveDevice.Load(StorageContainerName, filename, DeserializeSave))
                    { 
                        Trace.WriteLine(String.Format("Failed to load file {0}.", filename));
                    }
                    Global.SaveGameDescriptions.Add(loadedSaveGameDescription);
                } 
            }
        }
        
        private void DeserializeSave(Stream stream)
        {
            loadedSaveGameDescription = serializer.Deserialize(stream) as SaveGameDescription;
            Trace.WriteLine("Save Game Description Loaded: " + loadedSaveGameDescription);
        }

        private void SerializeSave(Stream stream)
        {
            Trace.WriteLine("Game Description Saved: " + savedSaveGameDescription);
            serializer.Serialize(stream, savedSaveGameDescription);
        }
  */
    }
}
