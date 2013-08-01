using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using CosmosCombat.AI;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Reflection;

namespace CosmosCombat
{
    public enum SelectionState
    {
        Selection,Target
    }
    public enum GameState
    {
        Loading,MainMenu,Game,Pause,LevelSelection,Options,Credits,AfterGame,Tutorial
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameBase : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D Fleet;
        Texture2D BG;
        Texture2D MainMenuBorders;
        Texture2D Logo;
        Texture2D BlackBoarder;
        Texture2D[] Planet;

        Texture2D MuteChecked;
        Texture2D MuteUnchecked;

        Texture2D MuteEffectsChecked;
        Texture2D MuteEffectsUnchecked;

        Texture2D Victory;
        Texture2D Defeat;
        
        bool Won;

        Texture2D Shadow;
        Texture2D Silhoute;
        Texture2D Tutorial;
        int CurrentTutorial=1;

        SpriteFont GameFont;

        Song Soundtrack;

        GameManger Manager;
        GameState GameState;
        MenuManager Menu;
        MenuManager PauseMenu;
        MenuManager LevelSelection;
        MenuManager OptionsMenu;

        List<Planet> Selected = new List<Planet>();
        SelectionState SelectionState;

        AI.AI Ai;
        Random rnd = new Random();
        bool SoundTracksEnabled;
        int PlanetsDiscovered=1;
        int CurrentPlanet = 1;


        public GameBase()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.IsFullScreen = true;
            graphics.PreferMultiSampling = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            // Frame rate is 30 fps by default for Windows Phone.
            //TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Manager = new GameManger();
            Menu = new MenuManager();
            PauseMenu = new MenuManager();

            SelectionState = SelectionState.Selection;
            GameState = GameState.MainMenu;
            Ai = new EasyAI();
            SoundTracksEnabled = MediaPlayer.GameHasControl;

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

            GameFont = Content.Load<SpriteFont>("GameFont");
            Fleet = Content.Load<Texture2D>("plane_white");
            BG = Content.Load<Texture2D>(String.Format("Backgrounds\\bg{0}",rnd.Next(1,7)));
            MainMenuBorders = Content.Load<Texture2D>("Menues\\menu");
            Planet = new Texture2D[3];
            for (int i = 1; i <= 3; i++)
            {
                Planet[i-1] = Content.Load<Texture2D>(String.Format("Planets\\new_p{0}",i));
            }
            Logo = Content.Load<Texture2D>("logo");
            Tutorial = Content.Load<Texture2D>("Tutorials\\tutorial1");
            Shadow = Content.Load<Texture2D>("Planets\\shadow");
            MuteUnchecked = Content.Load<Texture2D>("Menues\\mute_music_unchecked");
            //MuteEffectsUnchecked = Content.Load<Texture2D>("Menues\\mute_effects_unchecked");
            MuteChecked = Content.Load<Texture2D>("Menues\\Click\\mute_music_checked");
            //MuteEffectsChecked = Content.Load<Texture2D>("Menues\\Click\\mute_effects_checked");
            Silhoute = Content.Load<Texture2D>("selected3");

            Defeat = Content.Load<Texture2D>("defeat");
            Victory = Content.Load<Texture2D>("victory");


            Color[] Black = new Color[800 * (Logo.Height+20)];
            for (int i = 0; i < Black.Length; i++)
            {
                Black[i] = Color.Black;
            }
            BlackBoarder = new Texture2D(GraphicsDevice, 800, Logo.Height + 20);
            BlackBoarder.SetData(Black);

            if (SoundTracksEnabled)
            {
                MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(MediaPlayer_MediaStateChanged);
                Soundtrack = Content.Load<Song>(String.Format("Sound\\SoundTrack{0}", rnd.Next(1, 4)));
                MediaPlayer.Play(Soundtrack);
               
            }
            #region Buttons Create
            Button button_continue = new Button(Content.Load<Texture2D>("Menues\\bt_continue"), Content.Load<Texture2D>("Menues\\Click\\bt_continue_red"), new Vector2(400,130));
            button_continue.OnClick += ContinuePress;
            Button button_newgame = new Button(Content.Load<Texture2D>("Menues\\bt_newgame"), Content.Load<Texture2D>("Menues\\Click\\bt_newgame_red"), new Vector2(400, 200));
            button_newgame.OnClick += NewGamePress;
            Button button_options = new Button(Content.Load<Texture2D>("Menues\\bt_options"), Content.Load<Texture2D>("Menues\\Click\\bt_options_red"), new Vector2(400, 270));
            button_options.OnClick = GoToOptionsPress;
            Button button_credits = new Button(Content.Load<Texture2D>("Menues\\bt_credits"), Content.Load<Texture2D>("Menues\\Click\\bt_credits_red"), new Vector2(400, 340));
            button_credits.OnClick = CreditsPress;
            Button button_pause_continue = new Button(Content.Load<Texture2D>("Menues\\bt_continue"), Content.Load<Texture2D>("Menues\\Click\\bt_continue_red"), new Vector2(400, 130));
            button_pause_continue.OnClick += UnpausePress;

            Menu.Buttons.Add(button_continue);
            Menu.Buttons.Add(button_newgame);
            Menu.Buttons.Add(button_options);
            Menu.Buttons.Add(button_credits);

           

            PauseMenu.Buttons.Add(button_pause_continue);

            

            LevelSelection = new MenuManager();
            for (int i = 1; i <= 7; i++)
            {
                string file=String.Format("LevelButtons\\lvl{0}", i);
                Texture2D Texture = Content.Load<Texture2D>(file);
                Button button = new Button(Texture,Texture,new Vector2(i*105f -26.25f,110));
                button.Scale = 0.90f;
                button.Tag = i;
                button.OnClick = LevelSelectionButtonClick;
                LevelSelection.Buttons.Add(button);
            }
            for (int i = 1; i <= 7; i++)
            {
                string file = String.Format("LevelButtons\\lvl{0}", i + 7);
                Texture2D Texture = Content.Load<Texture2D>(file);
                Button button = new Button(Texture, Texture, new Vector2(i * 105f-26.25f, 210));
                button.Scale = 0.90f;
                button.Tag = i+7;
                button.OnClick = LevelSelectionButtonClick;
                LevelSelection.Buttons.Add(button);
            }
            for (int i = 1; i <= 6; i++)
            {
                string file = String.Format("LevelButtons\\lvl{0}", i + 14);
                Texture2D Texture = Content.Load<Texture2D>(file);
                Button button = new Button(Texture, Texture, new Vector2(i * 105f + 26.25f, 310));
                button.Scale = 0.90f;
                button.Tag = i + 14;
                button.OnClick = LevelSelectionButtonClick;
                LevelSelection.Buttons.Add(button);
            }

            string f = String.Format("LevelButtons\\lvl{0}", 21);
            Texture2D t = Content.Load<Texture2D>(f);
            Button b = new Button(t, t, new Vector2(400, 420));
            b.Scale = 1f;
            b.Tag = 21;
            b.OnClick = LevelSelectionButtonClick;
            LevelSelection.Buttons.Add(b);
            LevelSelection.Overlay = Content.Load<Texture2D>("lvl_completed");
            LevelSelection.OverlayOrigin = new Vector2(58, 58);

            

            OptionsMenu = new MenuManager();
            Button muteMusic = new Button(MuteUnchecked, MuteUnchecked, new Vector2(400, 130));
            muteMusic.OnClick = MuteMusic;
            muteMusic.Tag = 0;
            //Button muteEffects = new Button(MuteEffectsUnchecked, MuteEffectsUnchecked, new Vector2(400, 170));
            //muteEffects.OnClick = MuteEffects;
            //muteEffects.Tag = 0;
            //OptionsMenu.Buttons.Add(muteEffects);
            OptionsMenu.Buttons.Add(muteMusic);
            #endregion
            // TODO: use this.Content to load your game content here
        }

        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (MediaPlayer.State == MediaState.Stopped||MediaPlayer.State==MediaState.Paused)
            {
                Soundtrack = Content.Load<Song>(String.Format("Sound\\SoundTrack{0}",rnd.Next(1,4)));
                MediaPlayer.Play(Soundtrack);
            }
        }

        List<Planet> LoadMap(string map)
        {
            List<Planet> mapPlanets = new List<Planet>();
            XmlReader xmlReader = XmlReader.Create(map);
            xmlReader.ReadStartElement();
            while (xmlReader.Read())
            {
                /*Read planet header*/
                if (xmlReader.LocalName == "Planet" && xmlReader.IsStartElement())
                {
                    /*Read the ID element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("ID");
                    int ID = xmlReader.ReadContentAsInt();

                    /*Read the owner element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Owner");
                    string owner = xmlReader.ReadContentAsString();

                    /*Read the forces element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Forces");
                    int forces = xmlReader.ReadContentAsInt();

                    /*Read the growth element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Growth");
                    int growth = xmlReader.ReadContentAsInt();

                    /*Read the growth cooldown element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("GrowthCooldown");
                    int growthcd = xmlReader.ReadContentAsInt();

                    /*Read the size element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Size");
                    float size = xmlReader.ReadContentAsFloat();

                    /*Read the Position element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Position");
                    Vector2 Position = new Vector2();

                    /*Read the X element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("X");
                    Position.X = xmlReader.ReadContentAsInt();

                    /*Read the Y element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Y");
                    Position.Y = xmlReader.ReadContentAsInt();

                    Planet p = new Planet();
                    p.ID = ID;
                    p.Position = Position;
                    p.Growth = growth;
                    p.GrowthCounter = p.GrowthReset = growthcd;
                    p.Owner=(PlayerType)Enum.Parse(typeof(PlayerType),owner,false);
                    p.Forces.Add(p.Owner, forces);
                    p.PlanetSize = size;
                    p.InStateOfWar = false;
                    if (p.PlanetSize <= 0.45f)
                        p.Texture = Planet[0];
                    else if (p.PlanetSize >= 0.45f && p.PlanetSize <= 0.7f)
                        p.Texture = Planet[2];
                    else
                        p.Texture = Planet[1];
                    mapPlanets.Add(p);
                }

            }

            return mapPlanets;
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                if (GameState == GameState.Game)
                {
                    GameState = GameState.Pause;
                }
                else if (GameState == GameState.AfterGame)
                {
                    GameState = GameState.LevelSelection;
                }
                else if (GameState == GameState.Pause || GameState == GameState.LevelSelection || GameState == GameState.Options || GameState == GameState.Credits||GameState==GameState.Tutorial)
                {
                    GameState = GameState.MainMenu;
                }
                else
                {
                    this.Exit();
                }
            switch (GameState)
            {
                case CosmosCombat.GameState.Game:
                    UpdateGame();
                    break;
                case CosmosCombat.GameState.MainMenu:
                    UpdateMainMenu();
                    break;
                case CosmosCombat.GameState.LevelSelection:
                    UpdateLevelSelection();
                    break;
                case CosmosCombat.GameState.Pause:
                    UpdatePause();
                    break;
                case CosmosCombat.GameState.Options:
                    UpdateOptions();
                    break;
                case CosmosCombat.GameState.AfterGame:
                    UpdateAfterLevel();
                    break;
                case CosmosCombat.GameState.Tutorial:
                    UpdateTutorial();
                    break;
            }
            

            base.Update(gameTime);
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            SaveProgress();
            base.OnExiting(sender, args);
        }
        void LoadProgress()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            bool Exists=file.FileExists("CosmosCombatSave.xml");
            if (Exists)
            {
                IsolatedStorageFileStream stream = file.OpenFile("CosmosCombatSave.xml", System.IO.FileMode.Open);
                XmlReader xmlReader = XmlReader.Create(stream);
                xmlReader.ReadStartElement();
                while (xmlReader.Read())
                {
                    /*Read planet header*/
                    if (xmlReader.LocalName == "SaveData" && xmlReader.IsStartElement())
                    {
                        xmlReader.Read();
                        xmlReader.MoveToElement();
                        xmlReader.ReadStartElement("Discovered");
                        PlanetsDiscovered = xmlReader.ReadContentAsInt();
                    }
                }
                stream.Close();
                return;
            }
            else
            {
                PlanetsDiscovered = 1;

            }
        }
        void SaveProgress()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream= file.OpenFile("CosmosCombatSave.xml", System.IO.FileMode.Create);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(stream, settings);

            writer.WriteStartElement("Save");
            writer.WriteStartElement("SaveData");
            writer.WriteElementString("Discovered", PlanetsDiscovered.ToString());
            writer.WriteEndElement();
            writer.WriteFullEndElement();


            writer.Flush();
            writer.Close();
            stream.Close();
        }

        void ContinuePress(object sender, EventArgs e)
        {
            LoadProgress();
            foreach (Button button in LevelSelection.Buttons)
            {
                if (button.Tag > PlanetsDiscovered)
                    button.Draw = false;
                if (button.Tag < PlanetsDiscovered)
                    button.DrawOverlay = true;
                else
                    button.DrawOverlay = false;
            }
            GameState = GameState.LevelSelection;
        }
        void NewGamePress(object sender, EventArgs e)
        {
            PlanetsDiscovered = 1;
            foreach (Button button in LevelSelection.Buttons)
            {
                if (button.Tag > PlanetsDiscovered)
                    button.Draw = false;
                if (button.Tag < PlanetsDiscovered)
                    button.DrawOverlay = true;
                else
                    button.DrawOverlay = false;
            }
            GameState = GameState.LevelSelection;

        }
        void LevelDone()
        {
            Won = false;
            if (Manager.GetLooser() == PlayerType.AI)
            {
                Won = true;
                if(CurrentPlanet + 1>PlanetsDiscovered)
                PlanetsDiscovered = CurrentPlanet + 1;
                foreach (Button button in LevelSelection.Buttons)
                {
                    if (button.Tag > PlanetsDiscovered)
                        button.Draw = false;
                    else
                        button.Draw = true;
                    if (button.Tag < PlanetsDiscovered)
                        button.DrawOverlay = true;
                }
                
            }
            SaveProgress();
            GameState = GameState.AfterGame;
        }
        void CreditsPress(object sender, EventArgs e)
        {
            GameState = GameState.Credits;
        }

        void GoToOptionsPress(object sender, EventArgs e)
        {
            GameState = GameState.Options;
        }
        void LevelSelectionButtonClick(object sender, EventArgs e)
        {
            BG = Content.Load<Texture2D>(String.Format("Backgrounds\\bg{0}", rnd.Next(1, 7)));
            Manager.State.Planets.Clear();
            Manager.State.Fleets.Clear();
            Selected.Clear();
            Manager.State.AIPlanets.Clear();
            Manager.State.NeutralPlanets.Clear();
            Manager.State.PlayerPlanets.Clear();

            Button b = sender as Button;
            if(b.Tag!=21)
            Manager.State.Planets.AddRange(LoadMap(String.Format("Maps\\map{0}.xml", b.Tag)));
            else
            {
                if(rnd.NextDouble()<0.1)
                    Manager.State.Planets.AddRange(LoadMap(String.Format("Maps\\map{0}.xml", 22)));
                else
                    Manager.State.Planets.AddRange(LoadMap(String.Format("Maps\\map{0}.xml", b.Tag)));
            }
            CurrentPlanet = b.Tag;
            GameState = GameState.Game;

            if (CurrentPlanet <= 7)
            {
                Manager.AIAttackBias = 0.5f;
                Manager.PlayerAttackBias = 1.5f;
                Ai = new EasyAI();
            }
            else if (CurrentPlanet > 7 && CurrentPlanet <= 14)
            {
                Manager.AIAttackBias = 0.75f;
                Manager.PlayerAttackBias = 1.25f;
                Ai = new MediumAI();
            }
            else if (CurrentPlanet > 14 && CurrentPlanet <= 20)
            {
                Manager.AIAttackBias = 1f;
                Manager.PlayerAttackBias = 1;
                Ai = new HardAI();
            }
            else
            {
                Manager.AIAttackBias = 1.25f;
                Manager.PlayerAttackBias = 1f;
                Ai = new InsaneAI();
            }

            if (CurrentPlanet == 1)
                GameState = GameState.Tutorial;
            
        }
        void UnpausePress(object sender, EventArgs e)
        {
            GameState = GameState.Game;
        }

        void MuteMusic(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b.Tag == 0)
            {
                b.Normal = MuteChecked;
                b.Click = MuteChecked;
                b.Tag = 1;

                if (SoundTracksEnabled)
                {
                    MediaPlayer.IsMuted = true;
                }
            }
            else
            {
                b.Normal = MuteUnchecked;
                b.Click = MuteUnchecked;
                b.Tag = 0;


                if (SoundTracksEnabled)
                {
                    MediaPlayer.IsMuted = false;
                }
            }
        }
        void MuteEffects(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b.Tag == 0)
            {
                b.Normal = MuteEffectsChecked;
                b.Click = MuteEffectsChecked;
                b.Tag = 1;
            }
            else
            {
                b.Normal = MuteEffectsUnchecked;
                b.Click = MuteEffectsUnchecked;
                b.Tag = 0;
            }
        }

        protected void UpdateMainMenu()
        {
            Menu.Update(new Rectangle(-200, -200, 0, 0), false);
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Moved)
                {
                    Menu.Update(new Rectangle((int)tl.Position.X,(int)tl.Position.Y,20,20),false);
                }
                else if (tl.State == TouchLocationState.Released)
                {
                    Menu.Update(new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 20, 20), true);
                }
            }
        }
        protected void UpdatePause()
        {
            PauseMenu.Update(new Rectangle(-200, -200, 0, 0), false);
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Moved)
                {
                    PauseMenu.Update(new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 20, 20), false);
                }
                else if (tl.State == TouchLocationState.Released)
                {
                    PauseMenu.Update(new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 20, 20), true);
                }
            }
        }
        protected void UpdateGame()
        {
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Moved)
                {
                    if (SelectionState == SelectionState.Selection)
                    {
                        foreach (Planet planet in Manager.State.Planets)
                        {
                            if (planet.Owner == PlayerType.Player && Vector2.Distance(tl.Position, planet.Position) < planet.PlanetSize * 64)
                            {
                                if (Selected.Contains(planet))
                                    continue;
                                Selected.Add(planet);
                                break;
                            }
                        }
                    }
                }
                else if (tl.State == TouchLocationState.Released)
                {
                    if (SelectionState == SelectionState.Selection)
                    {
                        SelectionState = SelectionState.Target;
                        continue;
                    }
                    foreach (Planet planet in Manager.State.Planets)
                    {
                        if (Vector2.Distance(tl.Position, planet.Position) < planet.PlanetSize * 64 && Selected.Count > 0)
                        {
                            foreach (Planet selected in Selected)
                                //if (selected.Owner == PlayerType.Player)
                                //{
                                    Manager.SendFleet(selected.Forces[PlayerType.Player] / 2, selected, planet);
                                //}
                            break;
                        }

                    }
                    SelectionState = SelectionState.Selection;
                    Selected.Clear();
                }

            }
            Ai.Update(Manager.State, Manager);
            Manager.Update();
            List<Planet> NotOwned = new List<Planet>();
            foreach (Planet p in Selected)
            {
                if (p.Owner != PlayerType.Player)
                    NotOwned.Add(p);
            }
            foreach (Planet p in NotOwned)
            {
                Selected.Remove(p);
            }

            if (Manager.GameEnd())
            {
                LevelDone();
                
            }
        }
        protected void UpdateLevelSelection()
        {
            LevelSelection.Update(new Rectangle(-200, -200, 0, 0), false);
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Moved)
                {
                    LevelSelection.Update(new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 20, 20), false);
                }
                else if (tl.State == TouchLocationState.Released)
                {
                    LevelSelection.Update(new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 20, 20), true);
                }
            }
        }
        protected void UpdateOptions()
        {

            OptionsMenu.Update(new Rectangle(-200, -200, 0, 0), false);
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Moved)
                {
                    OptionsMenu.Update(new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 20, 20), false);
                }
                else if (tl.State == TouchLocationState.Released)
                {
                    OptionsMenu.Update(new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 20, 20), true);
                }
            }
        }
        protected void UpdateAfterLevel()
        {
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Released)
                {
                    GameState = GameState.LevelSelection;
                }
            }
        }
        protected void UpdateTutorial()
        {
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Pressed)
                {
                    CurrentTutorial++;
                    if (CurrentTutorial > 6)
                    {
                        GameState = GameState.Game;
                        CurrentTutorial = 1;
                        Tutorial = Content.Load<Texture2D>(String.Format("Tutorials\\tutorial{0}", CurrentTutorial));
                    }
                    else
                    {
                        Tutorial = Content.Load<Texture2D>(String.Format("Tutorials\\tutorial{0}", CurrentTutorial));
                    }
                }
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend);
            switch (GameState)
            {
                case CosmosCombat.GameState.Game:
                    DrawGame();
                    break;
                case CosmosCombat.GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case CosmosCombat.GameState.Pause:
                    DrawPause();
                    break;
                case CosmosCombat.GameState.LevelSelection:
                    DrawLevelSelection();
                    break;
                case CosmosCombat.GameState.Options:
                    DrawOptions();
                    break;
                case CosmosCombat.GameState.Credits:
                    DrawCredits();
                    break;
                case CosmosCombat.GameState.AfterGame:
                    DrawAfterLevel();
                    break;
                case CosmosCombat.GameState.Tutorial:
                    DrawTutorial();
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawGame()
        {
            spriteBatch.Draw(BG, Vector2.Zero, Color.White);
            foreach (Planet planet in Manager.State.Planets)
            {
                Color DrawColor = Color.White;

                switch (planet.Owner)
                {
                    case PlayerType.Neutral:
                        DrawColor = Color.White;
                        break;
                    case PlayerType.Player:
                        DrawColor = Color.LightGreen;
                        break;
                    case PlayerType.AI:
                        DrawColor = Color.DarkRed;
                        break;
                }

                spriteBatch.Draw(Shadow, planet.Position, null, Color.White, 0f, new Vector2(72, 72), planet.PlanetSize, SpriteEffects.None, 1f);
                spriteBatch.Draw(planet.Texture, planet.Position, null, DrawColor, 0f, new Vector2(62, 62), planet.PlanetSize, SpriteEffects.None, 1f);
                if (!planet.InStateOfWar)
                    spriteBatch.DrawString(GameFont, planet.Forces[planet.Owner].ToString(), planet.Position, DrawColor);
                else
                {
                    spriteBatch.DrawString(GameFont, "WAR!", planet.Position - GameFont.MeasureString("WAR!") / 2, Color.Red);
                    spriteBatch.DrawString(GameFont, planet.Forces[PlayerType.AI].ToString(), planet.Position + new Vector2(62, -62), Color.DarkRed);
                    spriteBatch.DrawString(GameFont, planet.Forces[PlayerType.Player].ToString(), planet.Position - new Vector2(62, -62) - GameFont.MeasureString(planet.Forces[PlayerType.Player].ToString()), Color.LightGreen);
                }

            }

            foreach (Fleet fleet in Manager.State.Fleets)
            {
                Color DrawColor = Color.White;

                switch (fleet.Owner)
                {
                    case PlayerType.Neutral:
                        DrawColor = Color.White;
                        break;
                    case PlayerType.Player:
                        DrawColor = Color.LightGreen;
                        break;
                    case PlayerType.AI:
                        DrawColor = Color.DarkRed;
                        break;
                }

                spriteBatch.Draw(Fleet, fleet.Position, null, DrawColor, fleet.Rotation, new Vector2(32, 32), 0.25f, SpriteEffects.None, 0f);
                foreach (Vector2 p in fleet.Positions)
                {
                    spriteBatch.Draw(Fleet, p + fleet.Position, null, DrawColor, fleet.Rotation, new Vector2(32, 32), 0.25f, SpriteEffects.None, 0f);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            foreach (Planet planet in Manager.State.Planets)
            {
                if (Selected.Contains(planet))
                {
                    spriteBatch.Draw(Silhoute, planet.Position, null, Color.White, 0f, new Vector2(128, 128), planet.PlanetSize, SpriteEffects.None, 0f);
                }
            }
        }
        protected void DrawPause()
        {

            DrawGame();
            spriteBatch.Draw(BlackBoarder, Vector2.Zero, Color.White);
            spriteBatch.Draw(Logo, new Vector2(288.5f, 10), Color.White);
            spriteBatch.Draw(MainMenuBorders, new Vector2(400, 240), null, Color.White, 0f, new Vector2(151, 176.5f), 1f, SpriteEffects.None, 1f);
            PauseMenu.Draw(spriteBatch);
        }
        protected void DrawMainMenu()
        {
            spriteBatch.Draw(BG, Vector2.Zero, Color.White);
            spriteBatch.Draw(BlackBoarder, Vector2.Zero, Color.White);
            spriteBatch.Draw(Logo, new Vector2(288.5f,10), Color.White);
            spriteBatch.Draw(MainMenuBorders, new Vector2(400, 240), null, Color.White, 0f, new Vector2(151,176.5f), 1f, SpriteEffects.None, 1f);
            Menu.Draw(spriteBatch);
        }
        protected void DrawLevelSelection()
        {
            
            spriteBatch.Draw(BG, Vector2.Zero, Color.White);
            spriteBatch.Draw(BlackBoarder, Vector2.Zero, Color.White);
            spriteBatch.Draw(Logo, new Vector2(288.5f, 10), Color.White);
            LevelSelection.Draw(spriteBatch);
        }
        protected void DrawOptions()
        {
            spriteBatch.Draw(BG, Vector2.Zero, Color.White);
            spriteBatch.Draw(BlackBoarder, Vector2.Zero, Color.White);
            spriteBatch.Draw(Logo, new Vector2(288.5f, 10), Color.White);
            spriteBatch.Draw(MainMenuBorders, new Vector2(400, 240), null, Color.White, 0f, new Vector2(151, 176.5f), 1f, SpriteEffects.None, 1f);
            OptionsMenu.Draw(spriteBatch);
        }
        protected void DrawCredits()
        {
            spriteBatch.Draw(BG, Vector2.Zero, Color.White);
            spriteBatch.Draw(BlackBoarder, Vector2.Zero, Color.White);
            spriteBatch.Draw(Logo, new Vector2(288.5f, 10), Color.White);
            string CreditsString="Programming by: Pavlo Malynin\nDesign by: Constantine Malynin\nContact Email: cosmoscombat@gmail.com\nTwitter: @CosmosCombat";
            spriteBatch.DrawString(GameFont, CreditsString, new Vector2(400,150), Color.LightBlue,0f,GameFont.MeasureString(CreditsString)/2,1f,SpriteEffects.None,0f);

            string Version = Assembly.GetExecutingAssembly().FullName.Split('=')[1].Split(',')[0];
            spriteBatch.DrawString(GameFont, Version, new Vector2(800, 480), Color.LightBlue, 0f, GameFont.MeasureString(Version), 1f, SpriteEffects.None, 0f);

        }
        protected void DrawAfterLevel()
        {
            spriteBatch.Draw(BG, Vector2.Zero, Color.White);
            spriteBatch.Draw(BlackBoarder, Vector2.Zero, Color.White);
            spriteBatch.Draw(Logo, new Vector2(288.5f, 10), Color.White);
            if (Won)
            {
                spriteBatch.Draw(Victory, new Vector2(400, 240), null, Color.White, 0f, new Vector2(95, 22), 1f, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(Defeat, new Vector2(400, 240), null, Color.White, 0f, new Vector2(77.5f, 21), 1f, SpriteEffects.None, 0f);
            }
        }
        protected void DrawTutorial()
        {
            spriteBatch.Draw(Tutorial, Vector2.Zero, Color.White);
        }
    }
}
