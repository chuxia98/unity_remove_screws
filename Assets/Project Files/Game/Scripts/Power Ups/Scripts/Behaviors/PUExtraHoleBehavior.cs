using UnityEngine;

namespace Watermelon
{
    public class PUExtraHoleBehavior : PUBehavior
    {
        [SerializeField] Particle particle;

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
            if (clickableObject is BaseBehavior)
            {
                BaseBehavior baseBehavior = (BaseBehavior)clickableObject;
                if(baseBehavior != null)
                {
                    Collider2D[] colliders2D = Physics2D.OverlapCircleAll(clickPosition, 0.4f);
                    bool pointIsAllowed = true;
                    for(int i = 0; i < colliders2D.Length; i++)
                    {
                        if (colliders2D[i] != baseBehavior.BoxCollider)
                        {
                            pointIsAllowed = false;

                            break;
                        }
                    }

                    if(pointIsAllowed)
                    {
                        particle.Play().SetPosition(clickPosition.SetZ(0.9f));

                        LevelController.PlaceAdditionalBaseHole(clickPosition);

                        return true;
                    }
                }
            }

            return false;
        }

        public override bool IsSelectable() => true;
    }
}
