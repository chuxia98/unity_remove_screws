#pragma warning disable 649

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using Watermelon;
using System.Text;
using System.Collections.Generic;

using System.Linq;

namespace Watermelon
{
    public class LevelEditorWindow : LevelEditorBase
    {

        //Path variables need to be changed ----------------------------------------
        private const string GAME_SCENE_PATH = "Assets/Project Files/Game/Scenes/Game.unity";
        private const string EDITOR_SCENE_PATH = "Assets/Project Files/Game/Scenes/Level Editor.unity";
        private static string EDITOR_SCENE_NAME = "Level Editor";

        //Window configuration
        private const string TITLE = "Level Editor";
        private const float WINDOW_MIN_WIDTH = 600;
        private const float WINDOW_MIN_HEIGHT = 560;

        //Level database fields
        private const string LEVELS_PROPERTY_NAME = "levels";
        private SerializedProperty levelsSerializedProperty;

        //EnumObjectsList 
        private const string TYPE_PROPERTY_PATH = "type";
        private const string PREFAB_PROPERTY_PATH = "prefab";
        private const string USE_IN_QUICK_MODE_PROPERTY_PATH = "useInQuickMode";
        private const string QUICK_MODE_SIZE_PROPERTY_PATH = "quickModeSize";

        //sidebar
        private LevelsHandler levelsHandler;
        private GUIStyle labelCenteredStyle;
        private LevelRepresentation selectedLevelRepresentation;
        private const int SIDEBAR_WIDTH = 240;
        private const string OPEN_GAME_SCENE_LABEL = "Open \"Game\" scene";

        private const string OPEN_GAME_SCENE_WARNING = "Please make sure you saved changes before swiching scene. Are you ready to proceed?";
        private const string REMOVE_SELECTION = "Remove selection";



        //General
        private const string YES = "Yes";
        private const string CANCEL = "Cancel";
        private const string WARNING_TITLE = "Warning";

        //PlayerPrefs
        private const string PREFS_LEVEL = "editor_level_index";
        private const string PREFS_WIDTH = "editor_sidebar_width";

        //rest of levels tab
        private const string ITEMS_LABEL = "Spawn items:";
        private const string FILE = "file:";
        private const string ITEM_ASSIGNED = "This buttton spawns item.";
        private const string TEST_LEVEL = "Test level";

        private const float ITEMS_BUTTON_MAX_WIDTH = 120;
        private const float ITEMS_BUTTON_SPACE = 8;
        private const float ITEMS_BUTTON_WIDTH = 80;
        private const float ITEMS_BUTTON_HEIGHT = 80;
        private const string RENAME_LEVELS = "Rename Levels";
        private const string PLAY_MODE_MENUITEM_PATH = "Edit/Play";
        private bool prefabAssigned;
        private GUIContent itemContent;
        private SerializedProperty currentLevelItemProperty;
        private Rect itemsListWidthRect;
        private Vector2 levelItemsScrollVector;
        private float itemPosX;
        private float itemPosY;
        private Rect itemsRect;
        private Rect itemRect;
        private int itemsPerRow;
        private int rowCount;
        private int currentSideBarWidth;
        private Rect separatorRect;
        private bool separatorIsDragged;
        private bool lastActiveLevelOpened;
        private float currentItemListWidth;

        //new stuff

        //PlanksSkinData
        private const string SKIN_ID_PROPERTY_NAME = "id";
        private const string SKIN_PLANKS_PROPERTY_NAME = "planks";
        private const string SKIN_PLANK_HOLE_PROPERTY_NAME = "plankHolePrefab";
        private const string SKIN_BASE_HOLE_PROPERTY_NAME = "baseHolePrefab";
        private const string SKIN_SCREW_PREFAB_PROPERTY_NAME = "screwPrefab";
        private const string SKIN_BACK_PREFAB_PROPERTY_NAME = "backPrefab";
        private const string SKIN_LAYER_COLORS_PROPERTY_NAME = "layerColors";
        private SerializedProperty skinIdSerializedProperty;
        private SerializedProperty skinPlanksSerializedProperty;
        private SerializedProperty skinPlankHolePrefabSerializedProperty;
        private SerializedProperty skinBaseHolePrefabSerializedProperty;
        private SerializedProperty skinScrewPrefabSerializedProperty;
        private SerializedProperty skinBackPrefabSerializedProperty;
        private SerializedProperty skinLayerColorsSerializedProperty;

        //PlankLevelData
        private const string PLANK_TYPE_PROPERTY_PATH = "plankType";
        private const string PLANK_LAYER_PROPERTY_PATH = "plankLayer";
        private const string POSITION_PROPERTY_PATH = "position";
        private const string ROTATION_PROPERTY_PATH = "rotation";
        private const string SCALE_PROPERTY_PATH = "scale";
        private const string SCREWS_POSITION_PROPERTY_PATH = "screwsPositions";

        //
        private const string SKINS_PROPERTY_NAME = "skins";
        private const string DEBUG_SKIN_ID_PROPERTY_NAME = "debugSkinID";
        private const string PREVIEW_SPRITE_PROPERTY_NAME = "previewSprite";
        private const string PLANKS_SKIN_DATA_PROPERTY_NAME = "planksSkinData";
        private SerializedProperty plankSkinsSerializedProperty;
        private SerializedProperty plankDebugSkinIDProperty;
        private SerializedProperty plankPreviewSpriteProperty;
        private SerializedProperty planksSkinDataProperty;


        //HoleData
        private const string HAS_SCREW_PROPERTY_PATH = "hasScrew";

        protected override string LEVELS_DATABASE_FOLDER_PATH => "Assets/Project Files/Data/Level System";

        private List<GameObject> levelItemsList;
        private List<int> levelItemsEnumValues;
        private bool sceneVariablesSet;
        private int tempStageTabIndex;

        protected override WindowConfiguration SetUpWindowConfiguration(WindowConfiguration.Builder builder)
        {
            builder.KeepWindowOpenOnScriptReload(true);
            builder.SetWindowMinSize(new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HEIGHT));
            return builder.Build();
        }

        public override bool WindowClosedInPlaymode()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (window != null)
                {
                    window.Close();
                    OpenScene(GAME_SCENE_PATH);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        protected override Type GetLevelsDatabaseType()
        {
            return typeof(LevelDatabase);
        }

        public override Type GetLevelType()
        {
            return typeof(LevelData);
        }

        protected override void ReadLevelDatabaseFields()
        {
            levelsSerializedProperty = levelsDatabaseSerializedObject.FindProperty(LEVELS_PROPERTY_NAME);

            ParsePlankSkinData();
        }

        private void ParsePlankSkinData()
        {
            PlanksSkinsDatabase skinsDatabase = EditorUtils.GetAsset<PlanksSkinsDatabase>();

            if (skinsDatabase == null)
            {
                Debug.LogError("SkinsDatabase not found in the project.");
            }

            SerializedObject skinDatabaseSerializedObject = new SerializedObject(skinsDatabase);

            plankSkinsSerializedProperty = skinDatabaseSerializedObject.FindProperty(SKINS_PROPERTY_NAME);
            plankDebugSkinIDProperty = skinDatabaseSerializedObject.FindProperty(DEBUG_SKIN_ID_PROPERTY_NAME);
            SerializedProperty skinProperty;

            if (plankSkinsSerializedProperty.arraySize == 0)
            {
                Debug.LogError("Skin database is empty. Level editor needs at least 1 skin to work.");
            }

            //loading first skin
            skinProperty = plankSkinsSerializedProperty.GetArrayElementAtIndex(0);
            skinIdSerializedProperty = skinProperty.FindPropertyRelative(SKIN_ID_PROPERTY_NAME);
            plankPreviewSpriteProperty = skinProperty.FindPropertyRelative(PREVIEW_SPRITE_PROPERTY_NAME);
            planksSkinDataProperty = skinProperty.FindPropertyRelative(PLANKS_SKIN_DATA_PROPERTY_NAME);
            SerializedObject skinSerializedObject = new SerializedObject(planksSkinDataProperty.objectReferenceValue);

            skinPlanksSerializedProperty = skinSerializedObject.FindProperty(SKIN_PLANKS_PROPERTY_NAME);
            skinPlankHolePrefabSerializedProperty = skinSerializedObject.FindProperty(SKIN_PLANK_HOLE_PROPERTY_NAME);
            skinBaseHolePrefabSerializedProperty = skinSerializedObject.FindProperty(SKIN_BASE_HOLE_PROPERTY_NAME);
            skinScrewPrefabSerializedProperty = skinSerializedObject.FindProperty(SKIN_SCREW_PREFAB_PROPERTY_NAME);
            skinBackPrefabSerializedProperty = skinSerializedObject.FindProperty(SKIN_BACK_PREFAB_PROPERTY_NAME);
            skinLayerColorsSerializedProperty = skinSerializedObject.FindProperty(SKIN_LAYER_COLORS_PROPERTY_NAME);
        }

        protected override void InitialiseVariables()
        {
            currentSideBarWidth = PlayerPrefs.GetInt(PREFS_WIDTH, SIDEBAR_WIDTH);
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (EditorSceneManager.GetActiveScene().name != EDITOR_SCENE_NAME)
            {
                return;
            }

            if (change != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }

            if (levelsHandler.SelectedLevelIndex == -1)
            {
                OpenScene(GAME_SCENE_PATH);
            }
            else
            {
                SaveLevel();
                SetAsCurrentLevel();
                EditorSceneController.Instance.Unsubscribe();
                OpenScene(GAME_SCENE_PATH);
                sceneVariablesSet = false;
            }
        }

        private void SetUpVariablesInEditorSceneController()
        {
            levelItemsList = new List<GameObject>(); //list of items that can be spawned
            levelItemsEnumValues = new List<int>();
            GameObject temp;
            temp = skinBaseHolePrefabSerializedProperty.objectReferenceValue as GameObject;

            if (temp != null)
            {
                levelItemsList.Add(temp);//adding hole
                levelItemsEnumValues.Add(-1);
            }
            else
            {
                Debug.LogError("Null reference in skin base hole prefab");
            }

            EditorSceneController.Instance.ScrewPrefab = skinScrewPrefabSerializedProperty.objectReferenceValue as GameObject;
            List<PlankData> plankData = new List<PlankData>();
            PlankData element;
            SerializedProperty elementProperty;

            for (int i = 0; i < skinPlanksSerializedProperty.arraySize; i++)
            {
                elementProperty = skinPlanksSerializedProperty.GetArrayElementAtIndex(i);
                temp = elementProperty.FindPropertyRelative(PREFAB_PROPERTY_PATH).objectReferenceValue as GameObject;

                if (temp != null)
                {
                    if (elementProperty.FindPropertyRelative(USE_IN_QUICK_MODE_PROPERTY_PATH).boolValue)
                    {
                        element = new PlankData((PlankType)elementProperty.FindPropertyRelative(TYPE_PROPERTY_PATH).intValue, temp, true, elementProperty.FindPropertyRelative(QUICK_MODE_SIZE_PROPERTY_PATH).floatValue);
                        plankData.Add(element);
                    }

                    levelItemsList.Add(temp);
                    levelItemsEnumValues.Add(elementProperty.FindPropertyRelative(TYPE_PROPERTY_PATH).intValue);
                }
                else
                {
                    Debug.LogError("Null reference in Plank with index:" + i);
                }
            }

            Color[] layerColors = new Color[skinLayerColorsSerializedProperty.arraySize];

            for (int i = 0; i < skinLayerColorsSerializedProperty.arraySize; i++)
            {
                layerColors[i] = skinLayerColorsSerializedProperty.GetArrayElementAtIndex(i).colorValue;
            }

            for (int i = 0; i < levelItemsList.Count; i++) // attempt to load images faster
            {
                AssetPreview.GetAssetPreview(levelItemsList[i]);
                AssetPreview.SetPreviewTextureCacheSize(levelItemsList.Count * 3);
            }

            plankData = plankData.OrderBy(o => o.QuickModeSize).ToList();
            EditorSceneController.Instance.PlankData = plankData;
            EditorSceneController.Instance.SpawnBackground(skinBackPrefabSerializedProperty.objectReferenceValue as GameObject);
            EditorSceneController.Instance.LayerColors = layerColors;
            sceneVariablesSet = true;
        }

        private void OpenLastActiveLevel()
        {
            if (!lastActiveLevelOpened)
            {
                if ((levelsSerializedProperty.arraySize > 0) && PlayerPrefs.HasKey(PREFS_LEVEL))
                {
                    levelsHandler.CustomList.SelectedIndex = Mathf.Clamp(PlayerPrefs.GetInt(PREFS_LEVEL, 0), 0, levelsSerializedProperty.arraySize - 1);
                    levelsHandler.OpenLevel(levelsHandler.SelectedLevelIndex);
                }

                lastActiveLevelOpened = true;
            }
        }

        protected override void Styles()
        {
            levelsHandler = new LevelsHandler(levelsDatabaseSerializedObject, levelsSerializedProperty);
            labelCenteredStyle = new GUIStyle(GUI.skin.label);
            labelCenteredStyle.alignment = TextAnchor.MiddleCenter;
        }

        public override void OpenLevel(UnityEngine.Object levelObject, int index)
        {
            SaveLevelIfPosssible();
            PlayerPrefs.SetInt(PREFS_LEVEL, index);
            PlayerPrefs.Save();
            selectedLevelRepresentation = new LevelRepresentation(levelObject);
            levelsHandler.UpdateCurrentLevelLabel(GetLevelLabel(levelObject, index));
            LoadLevel();
        }

        public override string GetLevelLabel(UnityEngine.Object levelObject, int index)
        {
            LevelRepresentation levelRepresentation = new LevelRepresentation(levelObject);
            return levelRepresentation.GetLevelLabel(index, stringBuilder);
        }

        public override void ClearLevel(UnityEngine.Object levelObject)
        {
            LevelRepresentation levelRepresentation = new LevelRepresentation(levelObject);
            levelRepresentation.Clear();
        }

        protected override void DrawContent()
        {
            if (EditorSceneManager.GetActiveScene().name != EDITOR_SCENE_NAME)
            {
                DrawOpenEditorScene();
                return;
            }

            if (!sceneVariablesSet)
            {
                SetUpVariablesInEditorSceneController();
            }

            DisplayLevelsTab();
        }

        private void DrawOpenEditorScene()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.HelpBox(EDITOR_SCENE_NAME + " scene required for level editor.", MessageType.Error, true);

            if (GUILayout.Button("Open \"" + EDITOR_SCENE_NAME + "\" scene"))
            {
                OpenScene(EDITOR_SCENE_PATH);
                lastActiveLevelOpened = false;
            }

            EditorGUILayout.EndVertical();
        }

        private void DisplayLevelsTab()
        {
            OpenLastActiveLevel();
            EditorGUILayout.BeginHorizontal();
            //sidebar 
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(currentSideBarWidth));
            levelsHandler.DisplayReordableList();
            DisplaySidebarButtons();
            EditorGUILayout.EndVertical();

            HandleChangingSideBar();

            //level content
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DisplaySelectedLevel();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void HandleChangingSideBar()
        {
            separatorRect = EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.MinWidth(8), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.AddCursorRect(separatorRect, MouseCursor.ResizeHorizontal);


            if (separatorRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    separatorIsDragged = true;
                    levelsHandler.IgnoreDragEvents = true;
                    Event.current.Use();
                }
            }

            if (separatorIsDragged)
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    separatorIsDragged = false;
                    levelsHandler.IgnoreDragEvents = false;
                    PlayerPrefs.SetInt(PREFS_WIDTH, currentSideBarWidth);
                    PlayerPrefs.Save();
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDrag)
                {
                    currentSideBarWidth = Mathf.RoundToInt(Event.current.delta.x) + currentSideBarWidth;
                    Event.current.Use();
                }
            }
        }

        private void DisplaySidebarButtons()
        {
            if (GUILayout.Button(RENAME_LEVELS, EditorCustomStyles.button))
            {
                SaveLevelIfPosssible();
                levelsHandler.RenameLevels();
            }

            if (GUILayout.Button(OPEN_GAME_SCENE_LABEL, EditorCustomStyles.button))
            {
                if (EditorUtility.DisplayDialog(WARNING_TITLE, OPEN_GAME_SCENE_WARNING, YES, CANCEL))
                {
                    SaveLevelIfPosssible();
                    ClearSelection();
                    EditorSceneController.Instance.Unsubscribe();
                    OpenScene(GAME_SCENE_PATH);
                    sceneVariablesSet = false;
                }
            }

            if (GUILayout.Button(REMOVE_SELECTION, EditorCustomStyles.button))
            {
                ClearSelection();
            }

            levelsHandler.DrawGlobalValidationButton();
        }

        private void ClearSelection()
        {
            SaveLevelIfPosssible();
            selectedLevelRepresentation = null;
            levelsHandler.ClearSelection();
            ClearScene();
        }

        public override void LogErrorsForGlobalValidation(UnityEngine.Object levelObject, int index)
        {
            LevelRepresentation levelRepresentation = new LevelRepresentation(levelObject);
            levelRepresentation.ValidateLevel();

            if (levelRepresentation.invalidStageIndexProperty.intValue != -1)
            {
                Debug.LogError($"Level # {index + 1} failed validation at stage # {levelRepresentation.invalidStageIndexProperty.intValue + 1}.");
            }
        }

        private static void ClearScene()
        {
            EditorSceneController.Instance.Clear();
        }

        private void SetAsCurrentLevel()
        {
            Serializer.Init();
            GlobalSave tempSave = SaveController.GetGlobalSave();
            LevelSave levelSave = tempSave.GetSaveObject<LevelSave>("level");
            levelSave.RealLevelIndex = levelsHandler.SelectedLevelIndex;
            levelSave.MaxReachedLevelIndex = levelsHandler.SelectedLevelIndex;
            levelSave.IsPlayingRandomLevel = false;
            SaveController.SaveCustom(tempSave);
            LevelAutoRun.EnableAutoRun(levelsHandler.SelectedLevelIndex);
        }

        private void DisplaySelectedLevel()
        {
            if (levelsHandler.SelectedLevelIndex == -1)
            {
                return;
            }

            //handle level file field
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(levelsHandler.SelectedLevelProperty, new GUIContent(FILE));

            if (EditorGUI.EndChangeCheck())
            {
                levelsHandler.ReopenLevel();
            }

            if (selectedLevelRepresentation.NullLevel)
            {
                return;
            }

            EditorGUILayout.Space();

            if (selectedLevelRepresentation.invalidStageIndexProperty.intValue != -1)
            {
                EditorGUILayout.HelpBox($"Not enough empty holes in stage # {selectedLevelRepresentation.invalidStageIndexProperty.intValue + 1}. It has {selectedLevelRepresentation.presentHolesCountProperty.intValue} empty holes but requires at least {selectedLevelRepresentation.requiredHolesCountProperty.intValue} to be passable..", MessageType.Error);
            }


            if (GUILayout.Button(TEST_LEVEL, GUILayout.Width(EditorGUIUtility.labelWidth)))
            {
                TestLevel();
            }

            DisplayStageSelection();

            if (EditorSceneController.Instance.IsHierarchyChanged && (selectedLevelRepresentation.selectedStageIndex != -1))
            {
                SaveStage();
                selectedLevelRepresentation.ValidateCurrentStage();
            }

            EditorGUILayout.Space();
        }



        private void TestLevel()
        {
            SaveLevel();
            SetAsCurrentLevel();
            EditorSceneController.Instance.Unsubscribe();
            OpenScene(GAME_SCENE_PATH);
            sceneVariablesSet = false;
            EditorApplication.ExecuteMenuItem(PLAY_MODE_MENUITEM_PATH);
        }

        private void DisplayStageSelection()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Toggle(selectedLevelRepresentation.selectedStageIndex == -1, "Settings", GUI.skin.button))
            {
                SaveStage();
                selectedLevelRepresentation.selectedStageIndex = -1;
                LoadStage();
            }

            tempStageTabIndex = GUILayout.Toolbar(selectedLevelRepresentation.selectedStageIndex, selectedLevelRepresentation.stageTabNames);

            if (GUILayout.Button("+", GUILayout.MaxWidth(24)))
            {
                SaveStage();
                selectedLevelRepresentation.AddStage();
                LoadStage();
            }
            else if (tempStageTabIndex != selectedLevelRepresentation.selectedStageIndex)
            {
                SaveStage();
                selectedLevelRepresentation.OpenStage(tempStageTabIndex);
                LoadStage();
            }


            EditorGUILayout.EndHorizontal();

            if (selectedLevelRepresentation.selectedStageIndex != -1)
            {

                DisplayStageSection();
            }
            else
            {
                DisplaySettingsSection();
            }

            EditorGUILayout.EndVertical();
        }



        private void DisplayStageSection()
        {
            if (GUILayout.Button("Delete stage"))
            {
                if (EditorUtility.DisplayDialog("Warning", "Are you sure that you want to delete stage?", "Yes", "Cancel"))
                {
                    int stageIndex = selectedLevelRepresentation.selectedStageIndex;
                    selectedLevelRepresentation.selectedStageIndex = -1;
                    LoadStage();
                    selectedLevelRepresentation.DeleteStage(stageIndex);
                }
            }

            DisplayStageFields();
            DisplayLayersControlSection();
            DisplayItemsListSection();
            DisplayShortcutInfo();
        }

        private void DisplayStageFields()
        {
            if (selectedLevelRepresentation.ungroupedStageProperties != null)
            {
                foreach (var property in selectedLevelRepresentation.ungroupedStageProperties)
                {
                    EditorGUILayout.PropertyField(property);
                }
            }
        }

        private void DisplaySettingsSection()
        {
            itemsListWidthRect = GUILayoutUtility.GetRect(1, Screen.width, 0, 0, GUILayout.ExpandWidth(true)); //we need this to strech window so it wouldn`t change size

            if (selectedLevelRepresentation.ungroupedProperties != null)
            {
                foreach (SerializedProperty property in selectedLevelRepresentation.ungroupedProperties)
                {
                    EditorGUILayout.PropertyField(property);
                }
            }
        }

        private void DisplayLayersControlSection()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Layer Control", EditorCustomStyles.labelMediumBold);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(EditorSceneController.Instance.CurrentLayerIndex == 0);

            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                EditorSceneController.Instance.CurrentLayerIndex--;
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.LabelField($"{(EditorSceneController.Instance.CurrentLayerIndex + 1)} / {(EditorSceneController.Instance.MaxLayerIndex + 1)}", labelCenteredStyle, GUILayout.Width(60));

            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                EditorSceneController.Instance.CurrentLayerIndex++;

                if (EditorSceneController.Instance.CurrentLayerIndex > EditorSceneController.Instance.MaxLayerIndex)
                {
                    EditorSceneController.Instance.MaxLayerIndex = EditorSceneController.Instance.CurrentLayerIndex;
                }
            }

            if (GUILayout.Button("Reset", GUILayout.Width(100)))
            {
                EditorSceneController.Instance.CurrentLayerIndex = EditorSceneController.Instance.MaxLayerIndex;
            }


            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DisplayItemsListSection()
        {
            EditorGUILayout.LabelField(ITEMS_LABEL);
            itemsListWidthRect = GUILayoutUtility.GetRect(1, Screen.width, 0, 0, GUILayout.ExpandWidth(true));

            if (itemsListWidthRect.width > 1)
            {
                currentItemListWidth = itemsListWidthRect.width;
            }

            levelItemsScrollVector = EditorGUILayout.BeginScrollView(levelItemsScrollVector);

            itemsRect = EditorGUILayout.BeginVertical();
            itemPosX = itemsRect.x;
            itemPosY = itemsRect.y;

            //assigning space
            if (levelItemsList.Count != 0)
            {
                itemsPerRow = Mathf.FloorToInt((currentItemListWidth - 16) / (ITEMS_BUTTON_SPACE + ITEMS_BUTTON_WIDTH)); // 16- space for vertical scroll
                rowCount = Mathf.CeilToInt((levelItemsList.Count * 1f) / itemsPerRow);
                GUILayout.Space(rowCount * (ITEMS_BUTTON_SPACE + ITEMS_BUTTON_HEIGHT));
            }

            for (int i = 0; i < levelItemsList.Count; i++)
            {
                if (AssetPreview.GetAssetPreview(levelItemsList[i]) == null)
                {
                    itemContent = new GUIContent(AssetPreview.GetMiniThumbnail(levelItemsList[i]), ITEM_ASSIGNED);
                }
                else
                {
                    itemContent = new GUIContent(AssetPreview.GetAssetPreview(levelItemsList[i]), ITEM_ASSIGNED);
                }

                //check if need to start new row
                if (itemPosX + ITEMS_BUTTON_SPACE + ITEMS_BUTTON_WIDTH > currentItemListWidth - 16)
                {
                    itemPosX = itemsRect.x;
                    itemPosY = itemPosY + ITEMS_BUTTON_HEIGHT + ITEMS_BUTTON_SPACE;
                }

                itemRect = new Rect(itemPosX, itemPosY, ITEMS_BUTTON_WIDTH, ITEMS_BUTTON_HEIGHT);


                if (GUI.Button(itemRect, itemContent, EditorCustomStyles.button))
                {
                    if (levelItemsEnumValues[i] == -1)
                    {
                        EditorSceneController.Instance.SpawnHole(levelItemsList[i], Vector3.zero, false);
                    }
                    else
                    {
                        PlankType plankType = (PlankType)levelItemsEnumValues[i];
                        PlankLevelData data = new PlankLevelData(plankType, EditorSceneController.Instance.CurrentLayerIndex, Vector3.zero, Vector3.zero, Vector3.one, new List<Vector3>());
                        EditorSceneController.Instance.SpawnPlank(data, levelItemsList[i]);
                    }
                }

                itemPosX += ITEMS_BUTTON_SPACE + ITEMS_BUTTON_WIDTH;
            }


            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DisplayShortcutInfo()
        {
            EditorGUILayout.HelpBox("Shortcuts:\n 1)Hold 'A' key and select two holes to spawn plank. \n 2) Select hole and click 'S' to spawn/despawn screw.", MessageType.Info);
        }

        private void LoadLevel()
        {
            EditorSceneController.Instance.SetUpCamera();
            EditorSceneController.Instance.ShowEditorOverlay();

            if (!selectedLevelRepresentation.NullLevel)
            {
                LoadStage();
            }
            else
            {
                Debug.LogError("Level file missing for level #" + (levelsHandler.SelectedLevelIndex + 1));
                ClearSelection();
            }
        }

        private void LoadStage()
        {
            EditorSceneController.Instance.Clear();

            if (selectedLevelRepresentation.selectedStageIndex != -1)
            {
                LoadHoles();
                LoadPlanks();
                EditorSceneController.Instance.CurrentLayerIndex = EditorSceneController.Instance.MaxLayerIndex;
                Selection.activeGameObject = null;
            }
        }

        private void LoadHoles()
        {
            HoleData data;

            for (int i = 0; i < selectedLevelRepresentation.holePositionsProperty.arraySize; i++)
            {
                data = PropertyToHoleData(selectedLevelRepresentation.holePositionsProperty.GetArrayElementAtIndex(i));
                EditorSceneController.Instance.SpawnHole(skinBaseHolePrefabSerializedProperty.objectReferenceValue as GameObject, data.Position, data.HasScrew);
            }
        }

        private void LoadPlanks()
        {
            PlankLevelData data;

            for (int i = 0; i < selectedLevelRepresentation.planksDataProperty.arraySize; i++)
            {
                data = PropertyToPlankLevelData(selectedLevelRepresentation.planksDataProperty.GetArrayElementAtIndex(i));
                EditorSceneController.Instance.SpawnPlank(data, GetPlankPrefab(data.PlankType));
            }
        }

        private GameObject GetPlankPrefab(PlankType plankType)
        {
            for (int i = 0; i < skinPlanksSerializedProperty.arraySize; i++)
            {
                if (skinPlanksSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative(TYPE_PROPERTY_PATH).intValue == (int)plankType)
                {
                    return skinPlanksSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative(PREFAB_PROPERTY_PATH).objectReferenceValue as GameObject;
                }
            }

            Debug.LogError("GetPlankPrefab null for " + plankType);
            return null;
        }

        private void SaveLevel()
        {
            SaveStage();
            selectedLevelRepresentation.RemoveDuplicates();
            selectedLevelRepresentation.ValidateLevel();
            levelsHandler.UpdateCurrentLevelLabel(selectedLevelRepresentation.GetLevelLabel(levelsHandler.SelectedLevelIndex, stringBuilder));
            AssetDatabase.SaveAssets();
        }

        private void SaveStage()
        {
            if (selectedLevelRepresentation.selectedStageIndex != -1)
            {
                SaveHoles();
                SavePlanks();
                selectedLevelRepresentation.ApplyChanges();
            }
        }

        private void SaveHoles()
        {
            HoleData[] holeData = EditorSceneController.Instance.GetHoleData();
            selectedLevelRepresentation.holePositionsProperty.arraySize = holeData.Length;

            for (int i = 0; i < holeData.Length; i++)
            {
                HoleDataToProperty(holeData[i], i);
            }
        }

        private void SavePlanks()
        {
            PlankLevelData[] plankLevelData = EditorSceneController.Instance.GetPlankData();
            selectedLevelRepresentation.planksDataProperty.arraySize = plankLevelData.Length;

            for (int i = 0; i < plankLevelData.Length; i++)
            {
                PlankLevelDataToProperty(plankLevelData[i], i);
            }
        }

        private void PlankLevelDataToProperty(PlankLevelData data, int index)
        {
            currentLevelItemProperty = selectedLevelRepresentation.planksDataProperty.GetArrayElementAtIndex(index);
            currentLevelItemProperty.FindPropertyRelative(PLANK_TYPE_PROPERTY_PATH).intValue = (int)data.PlankType;
            currentLevelItemProperty.FindPropertyRelative(PLANK_LAYER_PROPERTY_PATH).intValue = data.PlankLayer;
            currentLevelItemProperty.FindPropertyRelative(POSITION_PROPERTY_PATH).vector3Value = data.Position;
            currentLevelItemProperty.FindPropertyRelative(ROTATION_PROPERTY_PATH).vector3Value = data.Rotation;
            currentLevelItemProperty.FindPropertyRelative(SCALE_PROPERTY_PATH).vector3Value = data.Scale;
            SerializedProperty arrayProperty = currentLevelItemProperty.FindPropertyRelative(SCREWS_POSITION_PROPERTY_PATH);
            arrayProperty.arraySize = data.ScrewsPositions.Count;

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                arrayProperty.GetArrayElementAtIndex(i).vector3Value = data.ScrewsPositions[i];
            }
        }

        private PlankLevelData PropertyToPlankLevelData(SerializedProperty property)
        {
            List<Vector3> screwsPositions = new List<Vector3>();

            SerializedProperty arrayProperty = property.FindPropertyRelative(SCREWS_POSITION_PROPERTY_PATH);

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                screwsPositions.Add(arrayProperty.GetArrayElementAtIndex(i).vector3Value);
            }

            return new PlankLevelData(
                (PlankType)property.FindPropertyRelative(PLANK_TYPE_PROPERTY_PATH).intValue,
                property.FindPropertyRelative(PLANK_LAYER_PROPERTY_PATH).intValue,
                property.FindPropertyRelative(POSITION_PROPERTY_PATH).vector3Value,
                property.FindPropertyRelative(ROTATION_PROPERTY_PATH).vector3Value,
                property.FindPropertyRelative(SCALE_PROPERTY_PATH).vector3Value,
                screwsPositions);
        }

        private void HoleDataToProperty(HoleData data, int index)
        {
            currentLevelItemProperty = selectedLevelRepresentation.holePositionsProperty.GetArrayElementAtIndex(index);
            currentLevelItemProperty.FindPropertyRelative(POSITION_PROPERTY_PATH).vector3Value = data.Position;
            currentLevelItemProperty.FindPropertyRelative(HAS_SCREW_PROPERTY_PATH).boolValue = data.HasScrew;
        }

        private HoleData PropertyToHoleData(SerializedProperty property)
        {
            return new HoleData(property.FindPropertyRelative(POSITION_PROPERTY_PATH).vector3Value, property.FindPropertyRelative(HAS_SCREW_PROPERTY_PATH).boolValue);
        }

        private void SaveLevelIfPosssible()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != EDITOR_SCENE_NAME)
            {
                return;
            }

            if (selectedLevelRepresentation == null)
            {
                return;
            }

            if (selectedLevelRepresentation.NullLevel)
            {
                return;
            }

            try
            {
                SaveLevel();
            }
            catch
            {

            }

            levelsHandler.SetLevelLabels();
        }

        private void OnDestroy()
        {
            SaveLevelIfPosssible();

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                try
                {
                    EditorSceneController.Instance.Unsubscribe();
                }
                catch
                {

                }

                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                OpenScene(GAME_SCENE_PATH);
            }
        }

        protected class LevelRepresentation : LevelRepresentationBase
        {
            private const string STAGES_PROPERTY_NAME = "stages";
            private const string INVALID_STAGE_INDEX_PROPERTY_NAME = "invalidStageIndex";
            private const string PRESENT_HOLES_COUNT_PROPERTY_NAME = "presentHolesCount";
            private const string REQUIRED_HOLES_COUNT_PROPERTY_NAME = "requiredHolesCount";
            public SerializedProperty stagesProperty;
            public SerializedProperty invalidStageIndexProperty;
            public SerializedProperty presentHolesCountProperty;
            public SerializedProperty requiredHolesCountProperty;

            public IEnumerable<SerializedProperty> ungroupedProperties;

            //stage
            private const string HOLE_POSITIONS_PROPERTY_NAME = "holePositions";
            private const string PLANKS_DATA_PROPERTY_NAME = "planksData";

            public SerializedProperty holePositionsProperty;
            public SerializedProperty planksDataProperty;
            public SerializedProperty currentStageProperty;

            public IEnumerable<SerializedProperty> ungroupedStageProperties;

            public int selectedStageIndex;
            public string[] stageTabNames;

            //this empty constructor is nessesary
            public LevelRepresentation(UnityEngine.Object levelObject) : base(levelObject)
            {
            }

            protected override void ReadFields()
            {
                stagesProperty = serializedLevelObject.FindProperty(STAGES_PROPERTY_NAME);
                invalidStageIndexProperty = serializedLevelObject.FindProperty(INVALID_STAGE_INDEX_PROPERTY_NAME);
                presentHolesCountProperty = serializedLevelObject.FindProperty(PRESENT_HOLES_COUNT_PROPERTY_NAME);
                requiredHolesCountProperty = serializedLevelObject.FindProperty(REQUIRED_HOLES_COUNT_PROPERTY_NAME);
                ungroupedProperties = serializedLevelObject.GetUngroupProperties();

                if (stagesProperty.arraySize > 0)
                {
                    OpenStage(0);
                    UpdateTabNames();
                }
                else
                {
                    AddStage();
                }
            }

            public void OpenStage(int index)
            {
                selectedStageIndex = index;
                currentStageProperty = stagesProperty.GetArrayElementAtIndex(index);
                holePositionsProperty = currentStageProperty.FindPropertyRelative(HOLE_POSITIONS_PROPERTY_NAME);
                planksDataProperty = currentStageProperty.FindPropertyRelative(PLANKS_DATA_PROPERTY_NAME);

                ungroupedStageProperties = currentStageProperty.GetUngroupProperties();
            }

            public void AddStage()
            {
                stagesProperty.arraySize++;
                UpdateTabNames();
                OpenStage(stagesProperty.arraySize - 1);
                holePositionsProperty.arraySize = 0;
                planksDataProperty.arraySize = 0;
                ApplyChanges();
            }

            public void DeleteStage(int stageIndex)
            {
                stagesProperty.DeleteArrayElementAtIndex(stageIndex);
                UpdateTabNames();
                ungroupedStageProperties = null;
                ApplyChanges();
            }

            public void UpdateTabNames()
            {
                stageTabNames = new string[stagesProperty.arraySize];

                for (int i = 0; i < stageTabNames.Length; i++)
                {
                    stageTabNames[i] = "Stage #" + (i + 1);
                }
            }


            public void RemoveDuplicates()
            {
                Vector3 pos1;
                Vector3 pos2;
                Vector3 rot1;
                Vector3 rot2;


                for (int i = 0; i < planksDataProperty.arraySize - 1; i++)
                {
                    pos1 = planksDataProperty.GetArrayElementAtIndex(i).FindPropertyRelative(POSITION_PROPERTY_PATH).vector3Value;
                    rot1 = planksDataProperty.GetArrayElementAtIndex(i).FindPropertyRelative(ROTATION_PROPERTY_PATH).vector3Value;

                    for (int j = planksDataProperty.arraySize - 1; j > i; j--)
                    {
                        pos2 = planksDataProperty.GetArrayElementAtIndex(j).FindPropertyRelative(POSITION_PROPERTY_PATH).vector3Value;
                        rot2 = planksDataProperty.GetArrayElementAtIndex(j).FindPropertyRelative(ROTATION_PROPERTY_PATH).vector3Value;

                        if (pos1.Equals(pos2) && rot1.Equals(rot2))
                        {
                            Debug.LogWarning($"Removed duplicate plank with position: {pos2} and rotation: {rot2} .");
                            planksDataProperty.DeleteArrayElementAtIndex(j);
                        }
                    }
                }

                for (int i = 0; i < holePositionsProperty.arraySize - 1; i++)
                {
                    pos1 = holePositionsProperty.GetArrayElementAtIndex(i).FindPropertyRelative(POSITION_PROPERTY_PATH).vector3Value;

                    for (int j = holePositionsProperty.arraySize - 1; j > i; j--)
                    {
                        pos2 = holePositionsProperty.GetArrayElementAtIndex(j).FindPropertyRelative(POSITION_PROPERTY_PATH).vector3Value;

                        if (pos1.Equals(pos2))
                        {
                            Debug.LogWarning($"Removed duplicate hole with position: {pos2} .");
                            holePositionsProperty.DeleteArrayElementAtIndex(j);
                        }
                    }
                }

                ApplyChanges();
            }

            public override void Clear()
            {
                if (!NullLevel)
                {
                    stagesProperty.arraySize = 0;
                    ApplyChanges();
                }

            }

            public override string GetLevelLabel(int index, StringBuilder stringBuilder)
            {
                if (NullLevel)
                {
                    return base.GetLevelLabel(index, stringBuilder);
                }
                else
                {

                    stringBuilder.Clear();
                    stringBuilder.Append(NUMBER);
                    stringBuilder.Append(index + 1);
                    stringBuilder.Append(SEPARATOR);

                    if (invalidStageIndexProperty.intValue != -1)
                    {
                        stringBuilder.Append("Invalid stage #");
                        stringBuilder.Append(invalidStageIndexProperty.intValue + 1);
                    }
                    else if (stagesProperty.arraySize != 1)
                    {
                        stringBuilder.Append("stages: ");
                        stringBuilder.Append(stagesProperty.arraySize);
                    }
                    else
                    {
                        stringBuilder.Append("layers: ");

                        int maxLayerIndex = 0;
                        int plankLayer = 0;

                        for (int i = 0; i < planksDataProperty.arraySize; i++)
                        {
                            plankLayer = planksDataProperty.GetArrayElementAtIndex(i).FindPropertyRelative(PLANK_LAYER_PROPERTY_PATH).intValue;

                            if (plankLayer > maxLayerIndex)
                            {
                                maxLayerIndex = plankLayer;
                            }
                        }

                        stringBuilder.Append(maxLayerIndex + 1);
                        stringBuilder.Append(" planks: ");
                        stringBuilder.Append(planksDataProperty.arraySize);
                    }

                    return stringBuilder.ToString();
                }
            }

            public void ValidateCurrentStage()
            {
                if (!((invalidStageIndexProperty.intValue == -1) || (invalidStageIndexProperty.intValue == selectedStageIndex))) //some other stage is invalid
                {
                    return;
                }

                if (planksDataProperty.arraySize == 0)
                {
                    return;
                }


                //loking for plank
                int holeCount = planksDataProperty.GetArrayElementAtIndex(0).FindPropertyRelative(SCREWS_POSITION_PROPERTY_PATH).arraySize;
                int tempHoleCount;

                for (int j = 1; j < planksDataProperty.arraySize; j++)
                {
                    tempHoleCount = planksDataProperty.GetArrayElementAtIndex(j).FindPropertyRelative(SCREWS_POSITION_PROPERTY_PATH).arraySize;

                    if (tempHoleCount > holeCount)
                    {
                        holeCount = tempHoleCount;
                    }
                }


                tempHoleCount = 0;

                for (int j = 0; j < holePositionsProperty.arraySize; j++)
                {
                    if (!holePositionsProperty.GetArrayElementAtIndex(j).FindPropertyRelative(HAS_SCREW_PROPERTY_PATH).boolValue)
                    {
                        tempHoleCount++;
                    }
                }

                if (tempHoleCount < holeCount - 1)
                {
                    invalidStageIndexProperty.intValue = selectedStageIndex;
                    presentHolesCountProperty.intValue = tempHoleCount;
                    requiredHolesCountProperty.intValue = holeCount - 1;
                }
                else
                {
                    invalidStageIndexProperty.intValue = -1;
                }

                ApplyChanges();
            }


            public override void ValidateLevel()
            {
                invalidStageIndexProperty.intValue = -1; // -1 means that level is valid

                for (int i = 0; i < stagesProperty.arraySize; i++)
                {
                    OpenStage(i);

                    if (planksDataProperty.arraySize == 0)
                    {
                        continue;
                    }

                    //loking for plank
                    int holeCount = planksDataProperty.GetArrayElementAtIndex(0).FindPropertyRelative(SCREWS_POSITION_PROPERTY_PATH).arraySize;
                    int tempHoleCount;

                    for (int j = 1; j < planksDataProperty.arraySize; j++)
                    {
                        tempHoleCount = planksDataProperty.GetArrayElementAtIndex(j).FindPropertyRelative(SCREWS_POSITION_PROPERTY_PATH).arraySize;

                        if (tempHoleCount > holeCount)
                        {
                            holeCount = tempHoleCount;
                        }
                    }


                    tempHoleCount = 0;

                    for (int j = 0; j < holePositionsProperty.arraySize; j++)
                    {
                        if (!holePositionsProperty.GetArrayElementAtIndex(j).FindPropertyRelative(HAS_SCREW_PROPERTY_PATH).boolValue)
                        {
                            tempHoleCount++;
                        }
                    }

                    if (tempHoleCount < holeCount - 1)
                    {
                        invalidStageIndexProperty.intValue = i;
                        presentHolesCountProperty.intValue = tempHoleCount;
                        requiredHolesCountProperty.intValue = holeCount - 1;
                        ApplyChanges();
                        return;
                    }

                }

                ApplyChanges();
                OpenStage(selectedStageIndex);
            }

        }
    }
}

// -----------------
// Scene interraction level editor V1.8
// -----------------

// Changelog
// v 1.9
// • Removed max widonw size clamp
// • Fixed bug with items list
// v 1.8
// • Added draggable separator
// • Cached list width
// • Cached last open level
// • Added editor texture to EnumObjectList 
// v 1.7
// • Removed ability to use game scene for editor
// • bug fix
// • using prefabs instances on scene
// v 1.6
// • Replaced "Set as current level" function with playtest level
// • Added autosave
// • Updated object preview
// • Added USE_LEVEL_EDITOR_SCENE bool
// • Small fix for EditorSceneController
// v 1.5
// • Updated Spawner tool
// • Updated list
// v 1.5
// • Added Building tool
// • Updated list
// v 1.4
// • Updated EnumObjectlist
// • Updated object preview
// v 1.4
// • Updated EnumObjectlist
// • Fixed bug with window size
// v 1.3
// • Updated EnumObjectlist
// • Added StartPointHandles script that can be added to gameobjects
// v 1.2
// • Reordered some methods
// v 1.1
// • Added spawner tool
// v 1 basic version works
