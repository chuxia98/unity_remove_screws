using UnityEngine;

namespace Watermelon
{

    public class PURemoveScrewBehavior : PUBehavior
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

        public override bool ApplyToElement(IClickableObject clickableObject, Vector3 clickPosition)
        {
            if (clickableObject is ScrewBehavior)
            {
                ScrewBehavior screwBehavior = (ScrewBehavior)clickableObject;
                if(screwBehavior != null)
                {
                    particle.Play().SetPosition(screwBehavior.transform.position);

                    screwBehavior.UnscrewAndDiscard();

                    return true;
                }
            }

            return false;
        }

        public override void OnSelected()
        {
            base.OnSelected();

            if (ScrewBehavior.SelectedScrew != null) ScrewBehavior.SelectedScrew.Deselect();

            foreach (ScrewBehavior screw in LevelController.StageLoader.Screws)
            {
                screw.Highlight();
            }
        }

        public override void OnDeselected()
        {
            base.OnDeselected();

            foreach (ScrewBehavior screw in LevelController.StageLoader.Screws)
            {
                screw.Unhighlight();
            }
        }

        public override bool IsSelectable() => true;
    }
}
