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
    class FryEnemy : MeleeEnemy
    {
        public FryEnemy(Vector2 newPos) : base(newPos)
        {
            //speed.X = 3;
            scoreAward = 20;

            idleAnim = "Enemy/fryidleright";
            idleLeftAnim = "Enemy/fryidleleft";
            runAnim = "Enemy/fryrunright";
            runLeftAnim = "Enemy/fryrunleft";
            attackAnim = "Enemy/fryattackright";
            attackLeftAnim = "Enemy/fryattackleft";
            hurtLeft = "Enemy/fryhurtleft";
            hurtRight = "Enemy/fryhurtright";
        }
    }
}