namespace Fab5.Engine.Subsystems {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

public class Lifetime_Manager : Subsystem {
    private System.Threading.AutoResetEvent mre = new System.Threading.AutoResetEvent(false);
    public override void draw(float t, float dt) {
        var entities = Fab5_Game.inst().get_entities_fast(typeof (TTL));
        int num_entities = entities.Count;

        List<Entity> entities_to_destroy = new List<Entity>();

        int counter = num_entities;

        for (int i = 0; i < num_entities; i++) {
            System.Threading.ThreadPool.QueueUserWorkItem(o => {
                var entity = (Entity)o;//entities[i];
                var ttl    = entity.get_component<TTL>();

                ttl.time += dt;

                if (ttl.time >= ttl.max_time) {
                    lock (entities_to_destroy) {
                        entities_to_destroy.Add(entity);
                    }
                    if (System.Threading.Interlocked.Decrement(ref counter) == 0) {
                        mre.Set();
                    }
                    return;
                }

                ttl.counter = (ttl.counter+1)&3;

                if (ttl.alpha_fn != null && ttl.counter == 0) {
                    var s = entity.get_component<Sprite>();
                    if (s != null) {
                        var a = ttl.alpha_fn(ttl.time, ttl.max_time) * 255.0f;
                        if (a < 0.0f) a = 0.0f;
                        if (a > 255.0f) a = 255.0f;
                        s.color = new Color(s.color.R, s.color.G, s.color.B, (byte)a);
                    }
                }

                if (System.Threading.Interlocked.Decrement(ref counter) == 0) {
                    mre.Set();
                }
            }, entities[i]);
        }

        mre.WaitOne();

        if (entities_to_destroy != null) {
            foreach (var e in entities_to_destroy) {
                e.destroy();
                var ttl = e.get_component<TTL>();

                if (ttl.destroy_cb != null) {
                    ttl.destroy_cb();
                }
            }
        }

    }
}

}
