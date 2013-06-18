﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Candyland
{
    class ObstacleForSwitch : Obstacle
    {
        public ObstacleForSwitch(String id, Vector3 pos, UpdateInfo updateInfo)
        {
            initialize(id, pos, updateInfo);
        }

        #region initialization

        protected override void initialize(string id, Vector3 pos, UpdateInfo updateInfo)
        {
            base.initialize(id, pos, updateInfo);
        }

        public override void load(ContentManager content)
        {
            this.m_texture = content.Load<Texture2D>("blocktextur");
            this.m_original_texture = this.m_texture;
            this.effect = content.Load<Effect>("Toon");
            this.m_model = content.Load<Model>("blockmovable");
            this.m_original_model = this.m_model;

            this.calculateBoundingBox();
            minOld = m_boundingBox.Min;
            maxOld = m_boundingBox.Max;
        }

        #endregion

        public override void update()
        {
            // let the Object fall, if no collision with lower Objects
            if (!isDestroyed)
            {
                fall();
                isonground = false;
            }
        }

        #region collision

        // nothing to do here so far

        #endregion

        #region collision related

        public override void isNotCollidingWith(GameObject obj)
        {
        }

        public override void hasCollidedWith(GameObject obj)
        {
        }

        #endregion
    }
}