namespace Fab5.Engine.Components {

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*------------------------------------------------
 * CLASSES
 *----------------------------------------------*/

public class Sprite : Component {
    public const int BM_ALPHA = 1;
    public const int BM_ADD = 2;

    public float fps = 15.0f;
    public float frame_timer;
    public int num_frames = 1;

    public int frame_x = 0;
    public int frame_y = 0;
    public int frame_width = 0;
    public int frame_height = 0;

    public int frame_counter;

    public Texture2D texture;

    public float scale_x = 1.0f;
    public float scale_y = 1.0f;

    public float scale {
        set { scale_x = scale_y = value; }
    }

    public Color color = Color.White;

    public int blend_mode = BM_ALPHA;

    public float layer_depth = 0.5f;

}

}
