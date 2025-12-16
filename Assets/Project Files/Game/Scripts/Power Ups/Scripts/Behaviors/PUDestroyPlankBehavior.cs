using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class PUDestroyPlankBehavior : PUBehavior
    {
        [SerializeField] Particle particle;
        [SerializeField] bool usePlankShape;
        [SerializeField] bool usePlankColor;

        public override void Init()
        {
            ParticlesController.RegisterParticle(particle);
        }

        public override bool Activate()
        {
            return true;
        }

        public override void OnSelected()
        {
            base.OnSelected();

            if (ScrewBehavior.SelectedScrew != null) ScrewBehavior.SelectedScrew.Deselect();
        }

        public override bool ApplyToElement(IClickableObject clickableObject, Vector3 clickPosition)
        {
            if(clickableObject is PlankBehavior)
            {
                PlankBehavior plankBehavior = (PlankBehavior)clickableObject;
                if(plankBehavior != null)
                {
                    ParticleCase particleCase = particle.Play().SetPosition(plankBehavior.transform.position);
                    particleCase.ParticleSystem.Stop();
                    particleCase.ParticleSystem.transform.localEulerAngles = plankBehavior.transform.localEulerAngles;

                    Sprite plankSprite = plankBehavior.SpriteRenderer.sprite;

                    if (usePlankColor)
                    {
                        ParticleSystem.MainModule main = particleCase.ParticleSystem.main;
                        main.startColor = plankBehavior.Color;
                    }

                    particleCase.ApplyToParticles((ParticleSystem particleSystem) =>
                    {
                        if(usePlankShape)
                        {
                            ParticleSystem.ShapeModule shapeModule = particleSystem.shape;
                            shapeModule.sprite = plankSprite;
                            shapeModule.rotation = new Vector3(0, 180, 0);
                        }
                    });

                    particleCase.ParticleSystem.Play();

                    plankBehavior.DestroyPlank();

                    return true;
                }
            }

            return false;
        }

        public override bool IsSelectable() => true;
    }
}
