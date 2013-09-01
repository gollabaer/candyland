﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinnedModel;

namespace Candyland
{
    /// <summary>
    /// this NPC can give advice when addressed by the player or get the candyhelper
    /// </summary>
    class BonbonFairy : GameObject
    {
        // Message the fairy has for the player
        String m_text;

        bool isTeleportFairy = false;

        public BonbonFairy(String id, Vector3 pos, UpdateInfo updateInfo, bool visible, String message)
        {
            pos = pos + new Vector3(0.0f, 1.0f, 0.0f);
            base.init(id, pos, updateInfo, visible);
            if (message.Equals("teleport"))
                isTeleportFairy = true;
            else
                m_text = message;
        }

        public override void load(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            if (isTeleportFairy)
                this.m_texture = content.Load<Texture2D>("NPCs/Fee/bonbon_rot");
            else
                this.m_texture = content.Load<Texture2D>("NPCs/Fee/bonbon_blau");
            this.m_original_texture = this.m_texture;
            this.effect = content.Load<Effect>("Shaders/Shader");
            this.m_model = content.Load<Model>("NPCs/Fee/bonbon");
            this.m_original_model = this.m_model;
            // Bounding box is bigger than the model, so that the player can interact, when standing a bit away
            m_boundingBox = new BoundingBox(this.m_position - new Vector3(1,1,1), this.m_position + new Vector3(1,1,1));
            minOld = m_boundingBox.Min;
            maxOld = m_boundingBox.Max;
            base.load(content);

            //AnimationClip clip = m_skinningData.AnimationClips["ArmatureAction"];

            //animationPlayer.StartClip(clip);
        }

        public override void collide(GameObject obj)
        {
            ;
        }
        public override void hasCollidedWith(GameObject obj)
        {
            if(obj.GetType() == typeof(CandyGuy))
            {               
                if (m_updateInfo.m_screenManager.Input.Equals(InputState.Continue))
                {
                    // ask to teleport the helper
                    if (isTeleportFairy)
                    {
                        // only for demonstration, replace with proper dialog and check if teleport really is desired before actually porting
                        m_updateInfo.m_screenManager.ActivateNewScreen(new DialogListeningScreen(m_text, "Images/DialogImages/BonbonFairyRed"));
                        CandyGuy guy = (CandyGuy)obj;
                        CandyHelper helper = guy.getCandyHelper();
                        m_updateInfo.m_screenManager.ActivateNewScreen(new GetHelperQuestion(helper, m_updateInfo, this.m_position));
                    }
                    // show fairy message
                    else
                        m_updateInfo.m_screenManager.ActivateNewScreen(new DialogListeningScreen(m_text, "Images/DialogImages/BonbonFairy"));
                }
            }
            if (obj.GetType() == typeof(CandyHelper))
            {
                if (m_updateInfo.m_screenManager.Input.Equals(InputState.Continue))
                {
                    // greet helper
                    if (isTeleportFairy)
                    {
                        m_updateInfo.m_screenManager.ActivateNewScreen(new DialogListeningScreen("Hallo, Süßer!", "Images/DialogImages/BonbonFairy"));
                    }
                    // show fairy message
                    else
                        m_updateInfo.m_screenManager.ActivateNewScreen(new DialogListeningScreen(m_text, "Images/DialogImages/BonbonFairyBlue"));
                }
            }
        }

        public override void isNotCollidingWith(GameObject obj)
        {
            ;
        }

        public override void update()
        {
        //animationPlayer.Update(m_updateInfo.gameTime.ElapsedGameTime, true, Matrix.Identity);
        }
    }
}
