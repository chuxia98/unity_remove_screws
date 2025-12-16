using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class PlankHoleBehavior : HoleBehavior
    {
        [SerializeField] SpriteRenderer outlineSprite;
        [SerializeField] SpriteMask mask;
        [SerializeField] ScrewJointBehavior jointBehavior;

        private PlankBehavior plank;

        private void Awake()
        {
            jointBehavior.Init();
            jointBehavior.DisableSimulation();
        }

        public void Init(PlankBehavior plank)
        {
            this.plank = plank;

            outlineSprite.color = plank.Color;
            outlineSprite.sortingOrder = plank.SortingOrder + 1;
            mask.frontSortingOrder = plank.SortingOrder;
            mask.backSortingOrder = plank.SortingOrder - 1;

            jointBehavior.SetConnectedBody(plank.Rigidbody);

            DisableJoint();
        }

        public void ActivateJoint(Collider2D collider, Vector3 screwPos)
        {
            if (plank != null)
            {
                plank.IgnoreCollider(collider);
                plank.OnHoleAboutToBeSnapped(this, screwPos.SetZ(transform.position.z));
            }
            if (jointBehavior != null) jointBehavior.EnableSimulation();

            IsActive = true;
        }

        public void DisableJoint(Collider2D collider = null, bool tellPlank = false)
        {
            if (jointBehavior != null) jointBehavior.DisableSimulation();
            if (plank != null && collider != null) plank.StopIgnoringCollider(collider);

            IsActive = false;

            if (plank != null && tellPlank)
            {
                plank.OnHoleUnscrewed();
            }
        }

        public override void Discard()
        {
            DisableJoint();

            transform.SetParent(null);

            gameObject.SetActive(false);
        }
    }
}
