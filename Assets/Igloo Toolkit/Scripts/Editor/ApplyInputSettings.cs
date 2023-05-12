using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text;

namespace Igloo
{
    /// <summary>
    /// This is an Editor script to apply the various project settings required by the Igloo System and it's various plugins
    /// A single click of the Apply Project Settings button will cause: 
    /// - Two input axis to be added to the Player->Input Settings
    /// - A shader to be added to the Player->Graphics Settings
    /// - 2 Scoped Registry packages added to the Packages Manifest
    /// - 3 nuget packages added to the Packages Manifest.
    /// A popup will then confirm the actions on completion.
    /// </summary>
    public class ApplyInputSettings : MonoBehaviour
    {
        private static bool _axisXApplied, _axisYApplied, _layerScreenApplied, _layerPlayerApplied, _layerCompositeQuadApplied, _tagOutputCameraApplied, _klakPackagesAdded;

        /// <summary>
        /// Called when the Apply Project Settings button is clicked in the Igloo Menu.
        /// Runs through the various functions within this class, before finally displaying the 
        /// resolution diaglog. 
        /// </summary>
        [MenuItem("Igloo/Apply Igloo Settings")]
        public static void SetupInput()
        {
            _axisXApplied = AddAxis(new InputAxis() { name = "Right Stick X Axis", dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 4 });
            _axisYApplied = AddAxis(new InputAxis() { name = "Right Stick Y Axis", invert = true, dead = 0.2f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = 5 });

            _layerScreenApplied = CreateLayer("IglooScreen");
            _layerPlayerApplied = CreateLayer("IglooPlayer");
            _layerCompositeQuadApplied = CreateLayer("IglooCompositeQuad");
            _tagOutputCameraApplied = CreateTag("WarpBlendOutputCamera");
            _klakPackagesAdded = AddKlakDependencies();
            OnCompleteDialog();
        }

        /// <summary>
        /// Gets the child of the serialised property name
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns>null, or the child property</returns>
        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }

        /// <summary>
        /// Find the Axis name in the list of player->Input axis
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns>True if axis found, false otherwise.</returns>
        private static bool AxisDefined(string axisName)
        {
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                if (axis.hasChildren)
                {
                    axis.Next(true);
                    if (axis.stringValue == axisName) return true;
                }
                else return false;
            }
            return false;
        }

        /// <summary>
        /// Enum type for the Axis system
        /// </summary>
        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        };

        /// <summary>
        /// This adds both Klak Spout and Klak NDI packages to the packages manifest json
        /// Along with the NuGet package manager dependency.
        /// </summary>
        private static bool AddKlakDependencies() {
            string jsonFilePath = Path.Combine(GetProjectPath(), "Packages/manifest.json");
            if (File.Exists(jsonFilePath)) {
                string fileContents = File.ReadAllText(jsonFilePath);
                if (fileContents.Contains("scopedRegistries")) {
                     return false; 
                }
                string result = fileContents.Replace("dependencies", "scopedRegistries\": \n[\n{ \"name\": \"Unity NuGet\",\n \"url\": \"https://unitynuget-registry.azurewebsites.net\",\n \"scopes\": [ \"org.nuget\" ]\n }, \n { \"name\": \"Keijiro\", \n \"url\": \"https://registry.npmjs.com\",\n \"scopes\": [ \"jp.keijiro\" ]\n }\n ],\n \"dependencies");
                string output = result.Replace("com.unity.collab-proxy", "org.nuget.system.memory\": \"4.5.3\",\n    \"jp.keijiro.klak.ndi\": \"2.0.3\", \n    \"jp.keijiro.klak.spout\": \"2.0.3\",\n    \"com.unity.collab-proxy");
                File.WriteAllText(jsonFilePath, output);
                return true;
            } else {
                Debug.LogError("<b>[Igloo]</b> Couldn't find packages manifest. This may indicate a larger issue");
                return false;
            }
        }

        /// <summary>
        /// Creates a popup dialog to inform the player what has happened
        /// </summary>
        private static void OnCompleteDialog() {
            EditorUtility.DisplayDialog("Igloo Settings Applied", 
                $"The following Igloo settings have been applied:\n - RightStick X Axis added: {_axisXApplied} \n - RightStick Y Axis added: {_axisYApplied} \n" + 
                $" - Layer IglooScreen added: {_layerScreenApplied} \n - Layer IglooPlayer added: {_layerPlayerApplied} \n - Layer Composite Quad added: {_layerCompositeQuadApplied} \n" +
                $" - Igloo Warp Blend Tag added: {_tagOutputCameraApplied} \n - Spout and NDI packages added: {_klakPackagesAdded}", 
                "Okay");
        }

        /// <summary>
        /// The Input Axis Structure
        /// </summary>
        public class InputAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap = false;
            public bool invert = false;

            public AxisType type;

            public int axis;
            public int joyNum;
        }

        /// <summary>
        /// Adds the given Input Axis to the ProjectSettings->Input system
        /// </summary>
        /// <param name="axis">The Input Axis to add</param>
        private static bool AddAxis(InputAxis axis)
        {
            if (AxisDefined(axis.name)) return false;

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
            GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
            GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

            serializedObject.ApplyModifiedProperties();
            return true;
        }

        /// <summary>
        /// Creates a new object layer, based on the new layer name given. 
        /// </summary>
        /// <param name="name">New Layer name</param>
        public static bool CreateLayer(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "New layer name string is either null or empty.");

            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layerProps = tagManager.FindProperty("layers");
            var propCount = layerProps.arraySize;

            SerializedProperty firstEmptyProp = null;

            for (var i = 0; i < propCount; i++)
            {
                var layerProp = layerProps.GetArrayElementAtIndex(i);

                var stringValue = layerProp.stringValue;

                if (stringValue == name) return false;

                if (i < 8 || stringValue != string.Empty) continue;

                if (firstEmptyProp == null)
                    firstEmptyProp = layerProp;
            }

            if (firstEmptyProp == null)
            {
                UnityEngine.Debug.LogError("<b>[Igloo]</b> Maximum limit of " + propCount + " layers exceeded. Layer \"" + name + "\" not created.");
                return false;
            }

            firstEmptyProp.stringValue = name;
            tagManager.ApplyModifiedProperties();
            return true;
        }

        /// <summary>
        /// Creates a new object tag based on the new tag name given.
        /// </summary>
        /// <param name="name">New Tag name</param>
        public static bool CreateTag(string name)
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");
            for(int i = 0; i <tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(name)) return false;
            }
            // if we get to here, we'e not found the tag so make it.
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = name;

            tagManager.ApplyModifiedProperties();
            return true;
        }

        /// <summary>
        /// Gets the complete path of the project
        /// </summary>
        /// <returns>empty string, or the complete project path.</returns>
        public static string GetProjectPath() {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++) {
                if (args[i].Equals("-projectpath", System.StringComparison.InvariantCultureIgnoreCase)) return args[i + 1];
            }
            return string.Empty;
        }
        
    }
}

