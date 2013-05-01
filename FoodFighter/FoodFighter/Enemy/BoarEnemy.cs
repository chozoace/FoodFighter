using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Timers;

namespace FoodFighter
{
    class BoarEnemy : MeleeEnemy
    {
        int chargeSpeed;
        string angryIdleRight;
        string angryIdleLeft;

        public BoarEnemy(Vector2 newPos)
            : base(newPos)
        {
            scoreAward = 20;

            idleAnim = "Enemy/Boar/idleRight";
            idleLeftAnim = "Enemy/Boar/idleLeft";
            runAnim = "Enemy/Boar/runRight";
            runLeftAnim = "Enemy/Boar/runLeft";
            attackAnim = "Enemy/Boar/attackRight";
            attackLeftAnim = "Enemy/Boar/attackLeft";
            hurtLeft = "Enemy/Boar/hurtLeft";
            hurtRight = "Enemy/Boar/hurtRight";
        }
    }
}
