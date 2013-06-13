using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Xml;

namespace Candyland
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // the scene manager, most stuff happens in there
        SceneManager m_sceneManager;

        // Used to get storage device
        Object stateobj;
        StorageDevice storeDevice;
        bool saveRequested;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Used to get storage device (only needed for Xbox?)
            this.Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_sceneManager = new SceneManager( GraphicsDevice, this.graphics);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load all content required by the scene
            m_sceneManager.Load(this.Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive && gameTime.TotalGameTime.Milliseconds % 1 ==0 )
            {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                    || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();

                m_sceneManager.Update(gameTime);
            }

            // Save, when F5 is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                if (!Guide.IsVisible)
                {
                    saveRequested = true;
                    // Reset the device
                    storeDevice = null;
                    stateobj = (Object)"GetDevice for Player One";
                    StorageDevice.BeginShowSelector(
                            PlayerIndex.One, this.GetDevice, stateobj);
                }
            }

            // Load last savegame with F6
            if (Keyboard.GetState().IsKeyDown(Keys.F6))
            {
                if (!Guide.IsVisible)
                {
                    saveRequested = false;
                    // Reset the device
                    storeDevice = null;
                    stateobj = (Object)"GetDevice for Player One";
                    StorageDevice.BeginShowSelector(
                            PlayerIndex.One, this.GetDevice, stateobj);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            m_sceneManager.Draw(gameTime);
            m_sceneManager.Draw2D(spriteBatch);

            base.Draw(gameTime);
        }

        // Get a storage device
        void GetDevice(IAsyncResult result)
        {
            storeDevice = StorageDevice.EndShowSelector(result);
            if (storeDevice != null && storeDevice.IsConnected)
            {
                if (saveRequested)
                {
                    Save(storeDevice);
                }
                else Load(storeDevice);
            }
        }

        // Save to chosen device
        protected void Save(StorageDevice device)
        {
            // Create the data to save.
            SaveGameData data = new SaveGameData();
            data.currentAreaID = m_sceneManager.getUpdateInfo().currentAreaID;
            data.currentLevelID = m_sceneManager.getUpdateInfo().currentLevelID;
            data.chocoChipState = m_sceneManager.getBonusTracker().chocoChipState;
            data.chocoCount = m_sceneManager.getBonusTracker().chocoCount;
            data.chocoTotal = m_sceneManager.getBonusTracker().chocoTotal;

            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer("LastSave", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "savegame.sav";

            // Check to see whether the save exists.
            if (container.FileExists(filename))
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);

            // Convert the object to XML data
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                IntermediateSerializer.Serialize(writer, data, null);
            }

            // Dispose the container, to commit changes.
            container.Dispose();
        }

        // Load from chosen device
        protected void Load(StorageDevice device)
        {
            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer("LastSave", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "savegame.sav";

            // Check to see whether the save exists.
            //if (!container.FileExists(filename))
            //{
            //    // If not, dispose of the container and return.
            //    container.Dispose();
            //    return;
            //}

            // Open the file.
            XmlReaderSettings settings = new XmlReaderSettings();
            SaveGameData data;
            using (XmlReader reader =
                XmlReader.Create(filename, settings))
            {
                data = IntermediateSerializer.
                    Deserialize<SaveGameData>
                    (reader, null);
            }

            // Use saved data to put Game into the last saved state
            m_sceneManager.getUpdateInfo().currentAreaID = data.currentAreaID;
            m_sceneManager.getUpdateInfo().currentLevelID = data.currentLevelID;
            m_sceneManager.getUpdateInfo().reset = true; //everything should be reset, when game is loaded
            m_sceneManager.getBonusTracker().chocoChipState = data.chocoChipState;
            m_sceneManager.getBonusTracker().chocoCount = data.chocoCount;
            m_sceneManager.getBonusTracker().chocoTotal = data.chocoTotal;

            // Dispose the container.
            container.Dispose();
        }
    }
}
