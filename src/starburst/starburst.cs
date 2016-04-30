namespace Fab5.Starburst {

using Fab5.Engine;
using Fab5.Engine.Components;
using Fab5.Engine.Core;
using Fab5.Engine.Subsystems;

using Fab5.Starburst.States;

using Microsoft.Xna.Framework.Graphics;

// Starburst game implementation.
public class Starburst : Fab5_Game {
    protected override void init() {
        GraphicsMgr.PreferredBackBufferWidth = 1280;
        GraphicsMgr.PreferredBackBufferHeight = 768;
        GraphicsMgr.PreferMultiSampling = true;

        GraphicsMgr.GraphicsDevice.RasterizerState = new RasterizerState { MultiSampleAntiAlias = true };

        GraphicsMgr.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;

        GraphicsMgr.ApplyChanges();

        enter_state(new Main_Menu_State());

    }

    protected override void cleanup() {
    }

    protected override void update(float t, float dt) {
    }

    protected override void draw(float t, float dt) {
    }

    static void Main() {
        using (var game = new Starburst()) {
            game.run();
        }
    }
}

}
