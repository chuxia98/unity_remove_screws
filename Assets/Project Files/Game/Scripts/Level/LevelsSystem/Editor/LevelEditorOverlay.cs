using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using System;

namespace Watermelon
{
    [Overlay(typeof(SceneView), "Level editor overlay", true)]
    public class LevelEditorOverlay : UnityEditor.Overlays.Overlay
    {
        private Button mirrorPosX;
        private Button mirrorPosY;
        private Button mirrorRotX;
        private Button mirrorRotY;
        private Button mirrorRot;
        private Button flipScaleX;
        private Button flipScaleY;
        private bool buttonsEnabled;

        private VisualElement tempUIRoot;
        private List<SavablePlank> savablePlanks;
        private List<SavableHole> savableHoles;
        SavablePlank tempPlank;
        SavableHole tempHole;
        private Label layerNumberLayer;

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();
            
            mirrorPosX = new Button(MirrorPositionX);
            mirrorPosX.text = "Mirror Pos Horiz";
            mirrorPosY = new Button(MirrorPositionY);
            mirrorPosY.text = "Mirror Pos Vert";
            mirrorRotX = new Button(MirrorRotationX);
            mirrorRotX.text = "Mirror Rot Horiz";
            mirrorRotY = new Button(MirrorRotationY);
            mirrorRotY.text = "Mirror Rot Vert";
            mirrorRot = new Button(MirrorRotation);
            mirrorRot.text = "Mirror Rot";
            flipScaleX = new Button(FlipScaleX);
            flipScaleX.text = "Flip scale X";
            flipScaleY = new Button(FlipScaleY);
            flipScaleY.text = "Flip scale Y";

            tempUIRoot = new VisualElement();
            savablePlanks = new List<SavablePlank>();
            savableHoles = new List<SavableHole>();

            root.Add(mirrorPosX);
            root.Add(mirrorPosY);
            root.Add(mirrorRotX);
            root.Add(mirrorRotY);
            root.Add(mirrorRot);
            root.Add(flipScaleX);
            root.Add(flipScaleY);
            root.Add(tempUIRoot);


            
            root.MarkDirtyRepaint();

            Selection.selectionChanged += SelectionChanged;
            return root;
        }



        private void SelectionChanged()
        {
            buttonsEnabled = (Selection.gameObjects.Length > 0);
            mirrorPosX.SetEnabled(buttonsEnabled);
            mirrorPosY.SetEnabled(buttonsEnabled);
            mirrorRotX.SetEnabled(buttonsEnabled);
            mirrorRotY.SetEnabled(buttonsEnabled);
            mirrorRot.SetEnabled(buttonsEnabled);
            flipScaleX.SetEnabled(buttonsEnabled);
            flipScaleY.SetEnabled(buttonsEnabled);

            tempUIRoot.Clear();
            savableHoles.Clear();
            savablePlanks.Clear();

            if (!buttonsEnabled)
            {
                return;
            }

            //collecting data for selection
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                tempHole = Selection.gameObjects[i].GetComponent<SavableHole>();

                if(tempHole != null)
                {
                    savableHoles.Add(tempHole);
                }
                else
                {
                    tempPlank = Selection.gameObjects[i].GetComponent<SavablePlank>();

                    if(tempPlank != null)
                    {
                        savablePlanks.Add(tempPlank);
                    }
                }
            }

            if(savableHoles.Count > 0)
            {
                if(savablePlanks.Count > 0)
                {
                    tempUIRoot.Add(new Label("Different types selected"));
                }
                else
                {
                    HandleHoleMenu();
                }
            }
            else
            {
                if (savablePlanks.Count > 0)
                {
                    HandlePlanksMenu();
                }
            }

        }


        private void HandleHoleMenu()
        {
            Toggle toggle = new Toggle("Has Screw");
            toggle.SetValueWithoutNotify(savableHoles[0].HasScrew);
            toggle.RegisterValueChangedCallback(HoleToggleCallback);
            tempUIRoot.Add(toggle);
        }

        private void HoleToggleCallback(ChangeEvent<bool> evt)
        {
            for (int i = 0; i < savableHoles.Count; i++)
            {
                savableHoles[i].HasScrew = evt.newValue;
            }
        }

        private void HandlePlanksMenu()
        {
            VisualElement horizontalLayout = new VisualElement();
            horizontalLayout.style.flexDirection = FlexDirection.Row;
            Label layerLabel = new Label("Layer");
            layerNumberLayer = new Label((savablePlanks[0].PlankLayer + 1).ToString());
            Button minusButton = new Button(MinusLayerClick);
            Button plusButton = new Button(PlusLayerClick);
            minusButton.text = "-";
            plusButton.text = "+";

            horizontalLayout.Add(layerLabel);
            horizontalLayout.Add(minusButton);
            horizontalLayout.Add(layerNumberLayer);
            horizontalLayout.Add(plusButton);
            
            tempUIRoot.Add(horizontalLayout);
        }

        private void PlusLayerClick()
        {
            for (int i = 0; i < savablePlanks.Count; i++)
            {
                savablePlanks[i].PlankLayer++;
            }

            layerNumberLayer.text = (savablePlanks[0].PlankLayer + 1).ToString();
        }

        private void MinusLayerClick()
        {
            if(savablePlanks[0].PlankLayer == 0)
            {
                return;
            }

            for (int i = 0; i < savablePlanks.Count; i++)
            {
                savablePlanks[i].PlankLayer--;
            }

            layerNumberLayer.text = (savablePlanks[0].PlankLayer + 1).ToString();
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();
            Selection.selectionChanged -= SelectionChanged;
        }

        private void MirrorPositionX()
        {
            Undo.IncrementCurrentGroup();
            
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Undo.RecordObject(Selection.gameObjects[i].transform, "MirrorPositionX");
                Selection.gameObjects[i].transform.position = Selection.gameObjects[i].transform.position.MultX(-1);
            }

            Undo.SetCurrentGroupName("MirrorPositionX");
        }

        private void MirrorPositionY()
        {
            Undo.IncrementCurrentGroup();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Undo.RecordObject(Selection.gameObjects[i].transform, "MirrorPositionY");
                Selection.gameObjects[i].transform.position = Selection.gameObjects[i].transform.position.MultY(-1);
            }

            Undo.SetCurrentGroupName("MirrorPositionY");
        }

        private void MirrorRotationX()
        {
            Undo.IncrementCurrentGroup();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Undo.RecordObject(Selection.gameObjects[i].transform, "MirrorRotationX");
                Selection.gameObjects[i].transform.eulerAngles = Selection.gameObjects[i].transform.eulerAngles.MultZ(-1);
            }

            Undo.SetCurrentGroupName("MirrorRotationX");
        }

        private void MirrorRotationY()
        {
            Undo.IncrementCurrentGroup();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Undo.RecordObject(Selection.gameObjects[i].transform, "MirrorRotationY");
                Selection.gameObjects[i].transform.eulerAngles = Selection.gameObjects[i].transform.eulerAngles.AddToZ(270);
            }

            Undo.SetCurrentGroupName("MirrorRotationY");
        }
        private void MirrorRotation()
        {
            Undo.IncrementCurrentGroup();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Undo.RecordObject(Selection.gameObjects[i].transform, "MirrorRotation");
                Selection.gameObjects[i].transform.eulerAngles = Selection.gameObjects[i].transform.eulerAngles.AddToZ(180);
            }

            Undo.SetCurrentGroupName("MirrorRotation");
        }

        private void FlipScaleX()
        {
            Undo.IncrementCurrentGroup();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Undo.RecordObject(Selection.gameObjects[i].transform, "FlipScaleX");
                Selection.gameObjects[i].transform.localScale = Selection.gameObjects[i].transform.localScale.MultX(-1);
            }

            Undo.SetCurrentGroupName("FlipScaleX");
        }

        private void FlipScaleY()
        {
            Undo.IncrementCurrentGroup();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Undo.RecordObject(Selection.gameObjects[i].transform, "FlipScaleY");
                Selection.gameObjects[i].transform.localScale = Selection.gameObjects[i].transform.localScale.MultY(-1);
            }

            Undo.SetCurrentGroupName("FlipScaleY");
        }

    }
}
