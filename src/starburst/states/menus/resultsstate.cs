namespace Fab5.Starburst.States {

    using Fab5.Engine;
    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Fab5.Engine.Subsystems;

    using Fab5.Starburst.States.Playing.Entities;
    using Main_Menu.Entities;
    using Main_Menu.Subsystems;
    using Menus.Subsystems;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;
    using Microsoft.Xna.Framework.Input;

    using System;
    using System.Collections.Generic;
    using Playing;
    public class Results_State : Game_State {
        Texture2D background;
        Texture2D rectBg;
        SpriteFont font;
        private SpriteFont largeFont;
        SpriteBatch sprite_batch;

        private const float BTN_DELAY = .25f;
        float animateInTime = 1.4f;
        float startTime;

        float elapsedTime;
        float delay = .1f; // tid innan f�rsta animation startar
        float inDuration = .4f; // tid f�r animationer
        float outDuration = .4f; // tid f�r animationer
        float outDelay; // tid innan andra animationen
        float displayTime = .1f;
        float animationTime; // total animationstid
        float textOpacity;

        float btnDelay = BTN_DELAY;
        enum options {
            proceed
        };
        
        private Texture2D controller_a_button;
        private Texture2D keyboard_key;
        private Texture2D controller_l_stick;

        public float vol = 0.0f;
        public float fade = 0.0f;
        private bool lowRes;

        String text_ok;
        String text_select;
        Vector2 okSize;
        int controllerBtnSize;
        int heightDiff;
        int yPos;
        private bool started;

        private List<Entity> players;
        private Game_Config gameConfig;
        private Entity soundMgr;

        public Results_State(List<Entity> players, Game_Config config) {
            this.players = players;
            this.gameConfig = config;
        }
        public override void on_message(string msg, dynamic data) {

            if (btnDelay <= 0) {
                if (msg.Equals("fullscreen")) {
                    btnDelay = .5f;
                    Starburst.inst().GraphicsMgr.ToggleFullScreen();
                }
                else if (msg.Equals("select")) {
                    proceed();
                }
                else if (msg.Equals("start")) {
                    proceed();
                }
            }
        }

        private void proceed() {
        }

        public override void init() {
            sprite_batch = new SpriteBatch(Starburst.inst().GraphicsDevice);

            add_subsystems(
                new Menu_Input_Handler(),
                new Sound(),
                new Particle_System(),
                new Background_Renderer(sprite_batch)
            );

            outDelay = delay + inDuration + displayTime;
            animationTime = outDelay + outDuration;

            soundMgr = create_entity(SoundManager.create_backmusic_component());
            soundMgr.get_component<SoundLibrary>().song_index = 1;

            Viewport vp = sprite_batch.GraphicsDevice.Viewport;
            lowRes = (vp.Height < 800 && vp.Width < 1600);

            // load textures
            background = Starburst.inst().get_content<Texture2D>("backdrops/backdrop4");
            rectBg = new Texture2D(Fab5_Game.inst().GraphicsDevice, 1, 1);
            rectBg.SetData(new Color[]{Color.Black},1,1);//Starburst.inst().get_content<Texture2D>("controller_rectangle");
            font = Starburst.inst().get_content<SpriteFont>("sector034");
            largeFont = Starburst.inst().get_content<SpriteFont>("large");
            controller_a_button = Starburst.inst().get_content<Texture2D>("menu/Xbox_A_white");
            keyboard_key = Starburst.inst().get_content<Texture2D>("menu/Key");
            controller_l_stick = Starburst.inst().get_content<Texture2D>("menu/Xbox_L_white");

            text_ok = "Ok";
            text_select = "Select";
            okSize = font.MeasureString(text_ok);
            controllerBtnSize = 50; // ikon f�r knapp
            heightDiff = (int)(controllerBtnSize - okSize.Y);
            yPos = (int)(vp.Height - controllerBtnSize - 15);
            
        }

        public override void update(float t, float dt) {
            base.update(t, dt);
            
            // r�kna upp tid (dt)
            elapsedTime += dt;
            if(btnDelay > 0)
                btnDelay -= dt;

            if (elapsedTime >= animationTime) {
                elapsedTime = 0;
            }

            // fade in
            if (elapsedTime > delay && elapsedTime < outDelay) {
                textOpacity = (float)Easing.QuadEaseInOut(delay, 0, 1, inDuration);
            }
            // fade out
            else if (elapsedTime >= outDelay) {
                textOpacity = 1 - (float)Easing.QuadEaseInOut(outDelay, 0, 1, outDuration);
            }
        }
        public override void draw(float t, float dt) {
            Starburst.inst().GraphicsDevice.Clear(Color.Black);
            base.draw(t, dt);
            Viewport vp = sprite_batch.GraphicsDevice.Viewport;
            var s = new[] { "one", "two", "three", "four" };

            sprite_batch.Begin();

            var text_size = GFX_Util.measure_string("Paused");
            var tx = (vp.Width - text_size.X) * 0.5f;
            var ty = (vp.Height - text_size.Y) * 0.5f;
            

            int rowHeight = 50;
            int vertSpacing = 10;
            int totalScoreHeight = rowHeight * players.Count + 1 + vertSpacing * (players.Count - 1 + 1);
            int startY = (int)(vp.Height * .25f - totalScoreHeight * .5f);

            int horSpacing = 20;
            int nameWidth = 300;

            String killsHeader = "Kills";
            String deathsHeader = "Deaths";
            String scoreHeader = "Score";
            Vector2 killsSize = font.MeasureString(killsHeader);
            Vector2 deathsSize = font.MeasureString(deathsHeader);
            Vector2 scoreSize = font.MeasureString("999999");

            int totalScoreWidth = (int)(nameWidth + horSpacing + killsSize.X + horSpacing + deathsSize.X + horSpacing + scoreSize.X);
            int nameX = (int)(vp.Width * .5f - totalScoreWidth * .5f);
            int killsX = nameX + nameWidth + horSpacing;
            int deathsX = (int)(killsX + killsSize.X + horSpacing);
            int scoreX = (int)(deathsX + deathsSize.X + horSpacing);

            // header row
            // TODO: m�la en rektangel bakom

            //GFX_Util.draw_def_text(sprite_batch, "Player", nameX, startY);
            GFX_Util.draw_def_text(sprite_batch, killsHeader, killsX, startY);
            GFX_Util.draw_def_text(sprite_batch, deathsHeader, deathsX, startY);
            GFX_Util.draw_def_text(sprite_batch, scoreHeader, scoreX, startY);

            startY += rowHeight + vertSpacing;

            if (gameConfig.mode == Game_Config.GM_TEAM_DEATHMATCH) {
                List<Entity> redTeam = new List<Entity>();
                List<Entity> blueTeam = new List<Entity>();
                float redScore = 0, blueScore = 0;
                for (int p = 0; p < players.Count; p++) {
                    Ship_Info player_info = players[p].get_component<Ship_Info>();
                    if (player_info.team == 1)
                        redTeam.Add(players[p]);
                    else
                        blueTeam.Add(players[p]);
                }
                // m�la ut lagruta inkl lag-header
                
                GFX_Util.fill_rect(sprite_batch, new Rectangle(nameX, startY, totalScoreWidth, totalScoreHeight), new Color(1.0f, 0.2f, 0.2f, 0.3f));
                for (int i=0; i < redTeam.Count; i++) {
                    Score player_score = redTeam[i].get_component<Score>();
                    Ship_Info player_info = redTeam[i].get_component<Ship_Info>();
                    redScore += player_score.score;

                    // m�la ut spelarscore
                    int rowY = startY + rowHeight * i + vertSpacing * i;

                    GFX_Util.draw_def_text(sprite_batch, "Player " + s[player_info.pindex - 1], nameX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_kills.ToString(), killsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_deaths.ToString(), deathsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.display_score.ToString(), scoreX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_info.team == 1 ? "Red" : "Blue", scoreX + scoreSize.X + horSpacing, rowY);
                }
                startY += rowHeight*redTeam.Count+vertSpacing*(redTeam.Count-1);
                // m�la ut lagpo�ng

                // m�la ut lagruta inkl lag-header
                GFX_Util.fill_rect(sprite_batch, new Rectangle(), new Color(0.0f, 0.5f, 1.0f, 0.3f));
                for (int i = 0; i < blueTeam.Count; i++) {
                    Score player_score = blueTeam[i].get_component<Score>();
                    Ship_Info player_info = blueTeam[i].get_component<Ship_Info>();
                    blueScore += player_score.score;

                    // m�la ut spelarscore
                    int rowY = startY + rowHeight * i + vertSpacing * i;

                    GFX_Util.draw_def_text(sprite_batch, "Player " + s[player_info.pindex - 1], nameX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_kills.ToString(), killsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_deaths.ToString(), deathsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.display_score.ToString(), scoreX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_info.team == 1 ? "Red" : "Blue", scoreX + scoreSize.X + horSpacing, rowY);

                }
                // m�la ut lagpo�ng

                // skriv ut vem som vann
                String tieText = "Match ended in a tie";
                String winText = (redScore > blueScore ? "Red":"Blue") + " team won!";

                GFX_Util.draw_def_text(sprite_batch, (redScore == blueScore ? tieText : winText), 0, 0);
                GFX_Util.draw_def_text(sprite_batch, "Red team: " + redScore, 0, 30);
                GFX_Util.draw_def_text(sprite_batch, "Blue team: " + blueScore, 0, 60);
                /*
                for (int p = 0; p < players.Count; p++) {
                    Score player_score = players[p].get_component<Score>();
                    Ship_Info player_info = players[p].get_component<Ship_Info>();

                    int rowY = startY + rowHeight * p + vertSpacing * p;

                    GFX_Util.draw_def_text(sprite_batch, "Player " + s[player_info.pindex - 1], nameX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_kills.ToString(), killsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_deaths.ToString(), deathsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.display_score.ToString(), scoreX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_info.team == 1 ? "Red" : "Blue", scoreX + scoreSize.X + horSpacing, rowY);

                    //GFX_Util.fill_rect(sprite_batch, new Rectangle((int)xx - pad_size, rowY, (int)scoretext_size.X + (pad_size * 2), (int)scoretext_size.Y + (pad_size * 2)), Color.AliceBlue * 0.2f);
                }
                */
            }
            else {
                for (int p = 0; p < players.Count; p++) {
                    Score player_score = players[p].get_component<Score>();
                    Ship_Info player_info = players[p].get_component<Ship_Info>();

                    int rowY = startY + rowHeight * p + vertSpacing * p;

                    GFX_Util.draw_def_text(sprite_batch, "Player " + s[player_info.pindex - 1], nameX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_kills.ToString(), killsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.num_deaths.ToString(), deathsX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_score.display_score.ToString(), scoreX, rowY);
                    GFX_Util.draw_def_text(sprite_batch, player_info.team == 1 ? "Red" : "Blue", scoreX + scoreSize.X + horSpacing, rowY);

                    //GFX_Util.fill_rect(sprite_batch, new Rectangle((int)xx - pad_size, rowY, (int)scoretext_size.X + (pad_size * 2), (int)scoretext_size.Y + (pad_size * 2)), Color.AliceBlue * 0.2f);
                }
            }

            sprite_batch.End();
        }
    }

}