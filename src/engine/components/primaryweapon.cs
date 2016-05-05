﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab5.Engine.Components {
    public class Primary_Weapon : Weapon {
        public Primary_Weapon() {
            this.damage = 28;
            this.sound = "LaserBlaster";
            this.fire_rate = 0.1f;
            this.energy_cost = 20.0f;
            this.vib_left = 0.0f;
            this.vib_right = 0.27f;
        }
    }
}
