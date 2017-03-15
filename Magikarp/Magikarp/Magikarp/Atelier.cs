using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Diagnostics;

namespace AtelierXNA
{
    enum �tats { PAGE_TITRE, JEU3D, COMBAT, FIN}
    public class Atelier : Microsoft.Xna.Framework.Game
    {
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        GraphicsDeviceManager P�riph�riqueGraphique { get; set; }

        Cam�ra Cam�raJeu { get; set; }
        InputManager GestionInput { get; set; }
         �tats �tatJeu { get; set; }
        

        public Atelier()
        {
            P�riph�riqueGraphique = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            P�riph�riqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = false;
            P�riph�riqueGraphique.PreferredBackBufferWidth = 800;
            P�riph�riqueGraphique.PreferredBackBufferHeight = 480;
            P�riph�riqueGraphique.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            GestionInput = new InputManager(this);
            Components.Add(GestionInput);

            Services.AddService(typeof(Random), new Random());
            Services.AddService(typeof(�tats), �tatJeu);
            Services.AddService(typeof(RessourcesManager<SpriteFont>), new RessourcesManager<SpriteFont>(this, "Fonts"));
            Services.AddService(typeof(RessourcesManager<SoundEffect>), new RessourcesManager<SoundEffect>(this, "Sounds"));
            Services.AddService(typeof(RessourcesManager<Song>), new RessourcesManager<Song>(this, "Songs"));
            Services.AddService(typeof(RessourcesManager<Texture2D>), new RessourcesManager<Texture2D>(this, "Textures"));
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Cam�ra), Cam�raJeu);
            Services.AddService(typeof(SpriteBatch), new SpriteBatch(GraphicsDevice));
            Services.AddService(typeof(AccessBaseDeDonn�e), new AccessBaseDeDonn�e());
            base.Initialize();
        }
        protected override void Update(GameTime gameTime)
        {
            G�rerTransition();
            G�rer�tat();

            NettoyerListeComponents();
            G�rerClavier();
            base.Update(gameTime);
        }
        private void G�rerTransition()
        {
            switch (�tatJeu)
            {
                case �tats.PAGE_TITRE:
                    G�rerTransitionINITIALISATION();
                    break;
                case �tats.JEU3D:
                    G�rerTransitionJEU();
                    break;
            }
        }
        private void G�rer�tat()
        {
            switch (�tatJeu)
            {
                case �tats.PAGE_TITRE:
                    break;
                case �tats.JEU3D:
                    InitialiserParcours();
                    InitialiserCam�ra();
                    V�rifierCollision();
                    break;
            }
        }
        private void G�rerTransitionJEU()
        {
            Cam�raJeu = new Cam�raSubjective(this, Vector3.Zero, Vector3.Zero, Vector3.Up, INTERVALLE_MAJ_STANDARD);
            Components.Add(Cam�raJeu);
            Components.Add(new Arri�rePlanSpatial(this, "Ciel�toil�", INTERVALLE_MAJ_STANDARD));
            Components.Add(new Afficheur3D(this));
            Components.Add(new Jeu(this));
            Components.Add(new AfficheurFPS(this, "Arial", Color.Red, INTERVALLE_CALCUL_FPS));
        }

        void NettoyerListeComponents()
        {
            for (int i = Components.Count - 1; i >= 0; --i)
            {
                if (Components[i] is IDestructible && ((IDestructible)Components[i]).�D�truire)
                {
                    Components.RemoveAt(i);
                }
            }
        }

        private void G�rerClavier()
        {
            if (GestionInput.EstEnfonc�e(Keys.Escape))
            {
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}
