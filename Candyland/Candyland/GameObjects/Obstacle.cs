﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Candyland
{
    /// <summary>
    /// Obstacles are Objects in the Game World, placed on the Platforms and block the Players movement.
    /// The basic Obstacle cannot be moved or destroyed by the Player.
    /// </summary>
    class Obstacle : DynamicGameObjects
    {
        public Obstacle()
        {
        }
        public Obstacle(String id, Vector3 pos, UpdateInfo updateInfo)
        {
            this.ID = id;
            this.m_position = pos;
            this.m_original_position = pos;
            this.isActive = false;
            this.original_isActive = false;
            this.m_updateInfo = updateInfo;
        }

        public override void initialize()
        {
        }

        public override void isNotCollidingWith(GameObject obj)
        {

        }

        public override void hasCollidedWith(GameObject obj)
        {
           
        }

        public override void load(ContentManager content)
        {
            this.m_texture = content.Load<Texture2D>("lakritztextur");
            this.m_original_texture = this.m_texture;
            this.effect = content.Load<Effect>("Toon");
            this.m_model = content.Load<Model>("lakritzblock");
            this.m_original_model = this.m_model;

            this.calculateBoundingBox();
            Console.WriteLine("Min " + this.m_boundingBox.Min + " Max " + this.m_boundingBox.Max);
        }

        public override void collide(GameObject obj)
        {
        }

        public override void update()
        {
        }
    }
}