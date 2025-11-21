using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using NeonSurvivors.Core;

namespace NeonSurvivors.Editor
{
    /// <summary>
    /// Unity Editor tool to automatically setup the game scene
    /// Eliminates all manual Unity Editor work
    /// </summary>
    public class GameAutoSetup : EditorWindow
    {
        [MenuItem("Neon Survivors/Auto Setup Game Scene")]
        public static void ShowWindow()
        {
            GetWindow<GameAutoSetup>("Auto Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Neon Survivors Auto Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "This will automatically setup the game scene with all required components.\n\n" +
                "- Create GameSetup GameObject\n" +
                "- Configure tags and layers\n" +
                "- Setup camera (if needed)\n" +
                "- Create UI Document\n" +
                "- Configure project settings",
                MessageType.Info
            );

            GUILayout.Space(10);

            if (GUILayout.Button("Setup Current Scene", GUILayout.Height(40)))
            {
                SetupScene();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Setup Project Settings Only", GUILayout.Height(30)))
            {
                SetupProjectSettings();
            }

            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "After setup, just press Play to start the game!",
                MessageType.Warning
            );
        }

        static void SetupScene()
        {
            Debug.Log("===== Starting Neon Survivors Auto Setup =====");

            // 1. Setup tags and layers
            SetupTagsAndLayers();

            // 2. Create GameSetup GameObject if it doesn't exist
            CreateGameSetup();

            // 3. Setup camera
            SetupCamera();

            // 4. Create UI Document
            SetupUI();

            // 5. Setup project settings
            SetupProjectSettings();

            // 6. Mark scene as dirty
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("===== Auto Setup Complete! Press Play to start =====");
            EditorUtility.DisplayDialog(
                "Setup Complete!",
                "Scene is ready! Press Play to start the game.\n\n" +
                "The game will automatically create all managers, player, enemies, and UI.",
                "OK"
            );
        }

        static void SetupTagsAndLayers()
        {
            Debug.Log("Setting up tags and layers...");

            // Add tags
            AddTag("Player");
            AddTag("Enemy");
            AddTag("Projectile");
            AddTag("Boundary");
            AddTag("Pickup");

            Debug.Log("Tags configured");
        }

        static void AddTag(string tagName)
        {
            // Get tag manager
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
            );

            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // Check if tag already exists
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tagName))
                {
                    found = true;
                    break;
                }
            }

            // Add tag if not found
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = tagName;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"Added tag: {tagName}");
            }
            else
            {
                Debug.Log($"Tag already exists: {tagName}");
            }
        }

        static void CreateGameSetup()
        {
            Debug.Log("Creating GameSetup GameObject...");

            // Check if GameSetup already exists
            GameSetup existingSetup = FindObjectOfType<GameSetup>();
            if (existingSetup != null)
            {
                Debug.Log("GameSetup already exists in scene");
                return;
            }

            // Create new GameObject
            GameObject gameSetupObj = new GameObject("=== GAME SETUP ===");
            GameSetup setup = gameSetupObj.AddComponent<GameSetup>();

            // Position at origin
            gameSetupObj.transform.position = Vector3.zero;

            Debug.Log("GameSetup created successfully");
        }

        static void SetupCamera()
        {
            Debug.Log("Setting up camera...");

            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                // Create new camera
                GameObject camObj = new GameObject("Main Camera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
                Debug.Log("Created new Main Camera");
            }

            // Configure camera for top-down view
            mainCam.transform.position = new Vector3(0, 15, 0);
            mainCam.transform.rotation = Quaternion.Euler(90, 0, 0);
            mainCam.orthographic = true;
            mainCam.orthographicSize = 10f;
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = new Color(0.05f, 0f, 0.1f); // Dark purple

            Debug.Log("Camera configured");
        }

        static void SetupUI()
        {
            Debug.Log("Setting up UI...");

            // UI will be created at runtime by UIManager
            // No manual setup needed!

            Debug.Log("UI setup complete (runtime generation)");
        }

        static void SetupProjectSettings()
        {
            Debug.Log("Configuring project settings...");

            // Target frame rate
            Application.targetFrameRate = 60;

            // Quality settings
            QualitySettings.vSyncCount = 0;

            // Physics settings
            Physics.defaultSolverIterations = 6;
            Physics.defaultSolverVelocityIterations = 1;

            Debug.Log("Project settings configured");
        }

        [MenuItem("Neon Survivors/Quick Play Setup")]
        public static void QuickPlaySetup()
        {
            SetupScene();
            EditorApplication.isPlaying = true;
        }
    }
}
