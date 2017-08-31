using Microsoft.AspNet.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CGAutumnResitClient
{
    #region Copy-Pasted From GameHub.cs
    public class Player
    {
        public string PlayerID;
        public int Avatar; //1-4, corresponds to each player sprite
        public int LapNumber; //1-3
        public int CheckpointNumber; //0-3 (Fourth Checkpoint is a new lap, resets to zero)
        public double PosX;
        public double PosY;
        //rotation variable
    }
    #endregion

    public enum gameStates { Main, Waiting, Pause, Login, Register, InGame }; //how many of these I will end up needing, or if I'll need more, remains to be seen

    public class Game1 : Game
    {
        //hold off on menus till the 'game itself' is done
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player localPlayer;
        List<Player> players;
        Player otherPlayerA;
        Player otherPlayerB;
        Player otherPlayerC;
        public gameStates currentState = gameStates.Main;

        //TODO: Don't actually have sprites yet
        Texture2D player1Sprite;
        Texture2D player2Sprite;
        Texture2D player3Sprite;
        Texture2D player4Sprite;
        Texture2D checkpointSprite;
        Texture2D goalSprite;
        
        HubConnection connection;

        static IHubProxy proxy;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            //Player is assigned a playerID, has to be done Server-Side. Request ID, then request list of players. Will need to be able to update the player list
            //since all players might not join simultaneously
            connection = new HubConnection("");
            proxy = connection.CreateHubProxy("GameHub");
            connection.Start().Wait();
            proxy.Invoke<string>("requestID").ContinueWith((callback) =>
            {
                localPlayer.PlayerID = callback.Result;
            }).Wait();

            proxy.Invoke<List<Player>>("getPlayers").ContinueWith((callback) =>
            {
                foreach (Player p in callback.Result)
                {
                    players.Add(p);
                }
            }).Wait();
            

            // TODO: Add your initialization logic here
            localPlayer.CheckpointNumber = 0;
            localPlayer.LapNumber = 0;
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            

            base.Update(gameTime);
        }
        
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            

            base.Draw(gameTime);
        }

        public void CrossCheckpoint()
        {
            localPlayer.CheckpointNumber += 1;
        }

        public void CompleteLap()
        {
            localPlayer.LapNumber += 1;
            localPlayer.CheckpointNumber = 0;
        }

        public void UpdateHub()
        {

        }
    }
}
