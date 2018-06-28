using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class LivingCreature
    {
        public int CurrentHealth { get; set; }
        public int MaximumHealth { get; set; }

        public LivingCreature(int currentHealth, int maximumHealth)
        {
            CurrentHealth = currentHealth;
            MaximumHealth = maximumHealth;
        }
    }
}
