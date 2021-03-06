namespace Fab5.Engine.Core {

/*
 * This is the core of the game engine, structured in a single, concise file
 * like this because the C# convention of one-class-one-file is retarded.
 */

/*------------------------------------------------
 * USINGS
 *----------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Threading;

    using Fab5.Engine.Components;

    /*------------------------------------------------
     * CLASSES
     *----------------------------------------------*/


// Represents a single game state (for example, some main menu state or in-game
// state). Entities are contained in states.
public abstract class Game_State {

        public float update_t = 0.0f;
        public float draw_t = 0.0f;

    // The subsystems that the game state is using.
    private readonly List<Subsystem> subsystems = new List<Subsystem>();

    // Entities in the game state.
    private readonly Dictionary<Int64, Entity> entities = new Dictionary<Int64, Entity>();
    private readonly Dictionary<Type, List<Entity>> entity_dic = new Dictionary<Type, List<Entity>>();

    private class MsgInfo {
        public string msg;
        public object data;
    }

    private readonly System.Collections.Concurrent.ConcurrentQueue<MsgInfo> messages = new System.Collections.Concurrent.ConcurrentQueue<MsgInfo>();

    // Entity id counter. Static to make sure all entity ids are unique.
    private static Int64 next_entity_id = 1;

    internal void queue_message(string msg, object data) {
        messages.Enqueue(new MsgInfo{msg = msg, data = data});
    }

    internal void dispatch_messages() {
        MsgInfo msg;
        while (messages.TryDequeue(out msg)) {
            foreach (var subsystem in subsystems) {
                //try {
                    subsystem.on_message(msg.msg, msg.data);
                //} catch (Exception e) {
                    //Console.WriteLine("message handler crashed in {0}", subsystem);
                    //Console.WriteLine(e);
                //}

            }

            on_message(msg.msg, msg.data);
        }

    }

    public int get_num_entities() {
        return entities.Count;
    }

    public virtual void on_message(string msg, dynamic data) {
    }

    internal void remove_component(Entity entity, Type type) {
        lock (dummy_lock) {
            List<Entity> a;
            if (!entity_dic.TryGetValue(type, out a)) {
                return;
            }

            a.Remove(entity);
        }
    }

    internal void add_component(Entity entity, Type type) {
        if (!entity_dic.ContainsKey(type)) {
            entity_dic[type] = new List<Entity>();
        }

        entity_dic[type].Add(entity);

        if (type == typeof (Sprite)) {
            resort_sprites = true;
        }
    }

    // Creates an entity from the specified components and assigns an id to it.
    readonly object dummy_lock = new object();
    private bool resort_sprites;
    public Entity create_entity(params Component[] components) {
        var entity = new Entity();

        entity.id = Interlocked.Increment(ref next_entity_id);
        entity.state = this;

        lock (dummy_lock) {
            entity.add_components(components);
            entities[entity.id] = entity;
        }

        return (entity);
    }

    public void remove_entity(Int64 id) {
        lock (dummy_lock) {
            if (!entities.ContainsKey(id)) {
                return;
            }

            var entity = entities[id];

            foreach (var c in entity.components.Values) {
                entity_dic[c.GetType()].Remove(entity);
            }

            entities.Remove(id);
        }
    }

    // Retrieves all entities containing the specified component types. Do not
    // use the .Length-attribute of the returned array to iterate through the
    // results, but rather the num_entities out-parameter.
    static List<Entity> results = new List<Entity>();


    public Entity get_entity(Int64 id) {
        if (entities.ContainsKey(id)) {
            return entities[id];
        }

        return null;
    }

    List<Entity> empty = new List<Entity>();
    public List<Entity> get_entities_fast(Type component_type) {
        List<Entity> e = null;
        entity_dic.TryGetValue(component_type, out e);
        if (e == null) {
            return empty;
        }
        return e;
    }

    /*public List<Entity> get_entities_safe(Type component_type) {
        lock (dummy_lock) {
            List<Entity> e = null;
            entity_dic.TryGetValue(component_type, out e);
            if (e == null) {
                return empty;
            }
            var q = new List<Entity>(e);
            return q;
        }
    }*/

    /*public Entity[] get_entities(out int num_entities,
                                 params Type[] component_types)

    {
        System.Console.WriteLine("do not use this ass function, use get_entities_fast instead");
        //var results = new List<Entity>(1024);

        results.Clear();
        foreach (var entry in entities) {
            var entity = entry.Value;

            bool has_all_component_types = true;
            for (int i = 0; i < component_types.Length; i++) {
                var type = component_types[i];

                if (!entity.has_component(type)) {
                    has_all_component_types = false;
                    break;
                }
            }

            if (has_all_component_types) {
                results.Add(entity);
            }
        }

        num_entities = results.Count;

        return (results.ToArray());
    }*/

    // Adds the specified subsystems to the state.
    public void add_subsystems(params Subsystem[] subsystems) {
        foreach (Subsystem subsystem in subsystems) {
            subsystem.state = this;
            subsystem.init();
        }

        this.subsystems.AddRange(subsystems);
    }

    // @To-do: init cleanup etc should probably be internal protected.
    public virtual void init() {
    }

    public virtual void cleanup() {
        foreach (var subsystem in subsystems) {
            subsystem.cleanup();
        }
    }

    public virtual void update(float t, float dt) {
        foreach (var subsystem in subsystems) {
            subsystem.update(t, dt);
        }
    }

    private Comparer<Entity> sort_on_blend_mode = Comparer<Entity>.Create((e1, e2) => e1.get_component<Sprite>().blend_mode.CompareTo(e2.get_component<Sprite>().blend_mode));
    //private Comparer<Entity> sort_on_layer_depth = Comparer<Entity>.Create((e1, e2) => e1.get_component<Sprite>().layer_depth.CompareTo(e2.get_component<Sprite>().layer_depth));
    //private Comparer<Entity> sort_on_texture = Comparer<Entity>.Create((e1, e2) => e1.get_component<Sprite>().texture.Name.CompareTo(e2.get_component<Sprite>().texture.Name));

    public virtual void draw(float t, float dt) {
        if (resort_sprites) {
            resort_sprites = false;

            var sprites = get_entities_fast(typeof(Sprite));

            //sprites.Sort(sort_on_texture);
            //sprites.Sort(sort_on_blend_mode);
            int n = sprites.Count-1;
            for (int i = 0; i > n; i++) {
                var j = (i+1);
                var a = sprites[i].get_component<Sprite>();
                var b = sprites[j].get_component<Sprite>();
                if (a.blend_mode > b.blend_mode) {
                    var tmp = sprites[i];
                    sprites[i] = sprites[j];
                    sprites[j] = tmp;
                    break;
                }
                /*if (a.layer_depth > b.layer_depth) {
                    var tmp = sprites[i];
                    sprites[i] = sprites[j];
                    sprites[j] = tmp;
                    break;
                }*/
            }
        }

        foreach (var subsystem in subsystems) {
            subsystem.draw(t, dt);
        }
    }

}


}
