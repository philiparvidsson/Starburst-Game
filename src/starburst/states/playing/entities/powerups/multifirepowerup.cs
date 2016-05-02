namespace Fab5.Engine {

using Fab5.Engine.Components;
using Fab5.Engine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using System;

public class Multifire_Powerup : Powerup_Impl {
    private Int64 effect_id;

    private static System.Random rand = new System.Random();

    public string name {
        get { return "multifire"; }
    }

    public void end() {
        Fab5_Game.inst().destroy_entity(effect_id);
    }

    public static Component[] create_components() {
        Position pos;
        Velocity vel;

        var powerup = new Component[] {
            new Bounding_Circle { radius = 14.0f },
      pos = new Position        { x = -1500.0f, y = 1600.0f },
            new Sprite          { texture = Fab5_Game.inst().get_content<Texture2D>("turbo_powerup") },
            new Powerup         { impl = new Multifire_Powerup() },
      vel = new Velocity        { x = 0.0f, y = 0.0f },

            new Particle_Emitter {
                emit_fn = () => {
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 13.0f * (float)rand.NextDouble();
                    var speed  = 90.0f * (float)(0.05f+rand.NextDouble());

                    return new Component [] {
                        new Mass     { drag_coeff = 1.9f },
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },

                        new Velocity { x = vel.x * 0.5f + (float)Math.Cos(theta2) * speed,
                                       y = vel.y * 0.5f + (float)Math.Sin(theta2) * speed },

                        new Sprite { blend_mode  = Sprite.BM_ADD,
                                     color       = new Color(1.0f, 0.5f, 0.1f, 1.0f),
                                     layer_depth = 0.3f,
                                     scale       = 0.8f + (float)rand.NextDouble() * 0.7f,
                                     texture     = Fab5_Game.inst().get_content<Texture2D>("particle") },

                        new TTL { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max),
                                  max_time = 0.55f + (float)Math.Pow((rand.NextDouble() * 1.2f), 2.0f) }
                    };
                },

                interval               = 0.07f,
                num_particles_per_emit = 3
            }
        };

        return powerup;
    }

    private void do_pickup_effect(Entity powerup) {
        var pos = powerup.get_component<Position>();
        var vel = powerup.get_component<Velocity>();

        Fab5_Game.inst().create_entity(new Component [] {
            new Particle_Emitter {
                emit_fn = () => {
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 13.0f * (float)rand.NextDouble();
                    var speed  = 400.0f * (float)(0.05f+rand.NextDouble());

                    return new Component [] {
                        new Mass     { drag_coeff = 1.9f },
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },

                        new Velocity { x = vel.x * 0.5f + (float)Math.Cos(theta2) * speed,
                                       y = vel.y * 0.5f + (float)Math.Sin(theta2) * speed },

                        new Sprite { blend_mode  = Sprite.BM_ADD,
                                     color       = new Color(1.0f, 0.5f, 0.1f, 1.0f),
                                     layer_depth = 0.9f,
                                     scale       = 0.8f + (float)rand.NextDouble() * 0.7f,
                                     texture     = Fab5_Game.inst().get_content<Texture2D>("particle") },

                        new TTL { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max),
                                  max_time = 0.25f + (float)Math.Pow((rand.NextDouble() * 0.9f), 2.0f) }
                    };
                },

                interval               = 0.025f,
                num_particles_per_emit = 90
            },

            new TTL { max_time = 0.1f }

        });
    }

    private void do_persistent_effect(Entity holder) {
        var pos = holder.get_component<Position>();
        var vel = holder.get_component<Velocity>();

        effect_id = Fab5_Game.inst().create_entity(new Component [] {
            new Particle_Emitter {
                emit_fn = () => {
                    var theta1 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var theta2 = 2.0f*3.1415f*(float)rand.NextDouble();
                    var radius = 13.0f * (float)rand.NextDouble();
                    var speed  = 200.0f * (float)(0.05f+rand.NextDouble());

                    return new Component [] {
                        new Mass     { drag_coeff = 1.9f },
                        new Position { x = pos.x + (float)Math.Cos(theta1) * radius,
                                       y = pos.y + (float)Math.Sin(theta1) * radius },

                        new Velocity { x = vel.x * 0.5f + (float)Math.Cos(theta2) * speed,
                                       y = vel.y * 0.5f + (float)Math.Sin(theta2) * speed },

                        new Sprite { blend_mode  = Sprite.BM_ADD,
                                     color       = new Color(1.0f, 0.5f, 0.1f, 1.0f),
                                     layer_depth = 0.3f,
                                     scale       = 0.8f + (float)rand.NextDouble() * 0.7f,
                                     texture     = Fab5_Game.inst().get_content<Texture2D>("particle") },

                        new TTL { alpha_fn = (x, max) => 1.0f - (x/max)*(x/max),
                                  max_time = 0.25f + (float)Math.Pow((rand.NextDouble() * 0.9f), 2.0f) }
                    };
                },

                interval               = 0.05f,
                num_particles_per_emit = 7
            }
        }).id;
    }

    public void on_begin(Entity holder, Entity powerup) {
        do_pickup_effect(powerup);
        do_persistent_effect(holder);
    }

}

}