using UnityEngine;
using UnityEditor;
namespace SoundKraft
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // cast the attribute to make life easier
            MinMaxAttribute minMax = attribute as MinMaxAttribute;



            float minValue = 0f, maxValue = 0f, minLimit = 0f, maxLimit = 0f; // the currently set minimum and maximum value
            Vector2 vec = Vector2.zero;
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                minValue = property.vector2Value.x; // the currently set minimum and maximum value
                maxValue = property.vector2Value.y;
            }
            else if (property.FindPropertyRelative("MinMax") != null)
            {
                Vector2 floatMinMax = property.FindPropertyRelative("MinMax").vector2Value;
                minValue = floatMinMax.x;
                maxValue = floatMinMax.y;


            }
            else
            {
                Debug.LogError("No right stuff");
                return;
            }
            minLimit = minMax.Min;
            maxLimit = minMax.Max;
            vec.x = minValue;
            vec.y = maxValue;

            // if we are flagged to draw in a special mode, lets modify the drawing rectangle to draw only one line at a time
            if (minMax.ShowDebugValues || minMax.ShowEditRange)
            {
                position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            }

            // pull out a bunch of helpful min/max values....


            // and ask unity to draw them all nice for us!
            EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);

            // save the results into the property!

            if (property.propertyType == SerializedPropertyType.Vector2)
                property.vector2Value = vec;
            else if (property.FindPropertyRelative("MinMax") != null)
                property.FindPropertyRelative("MinMax").vector2Value = vec;

            // Do we have a special mode flagged? time to draw lines!
            if (minMax.ShowDebugValues || minMax.ShowEditRange)
            {
                bool isEditable = false;
                if (minMax.ShowEditRange)
                {
                    isEditable = true;
                }

                if (!isEditable)
                    GUI.enabled = false; // if were just in debug mode and not edit mode, make sure all the UI is read only!

                // move the draw rect on by one line
                position.y += EditorGUIUtility.singleLineHeight;

                Vector2 val = new Vector2(minValue, maxValue); // shove the values and limits into a vector4 and draw them all at once
                val = EditorGUI.Vector2Field(position, "Min/Max", val);

                GUI.enabled = false; // the range part is always read only
                position.y += EditorGUIUtility.singleLineHeight;
                //    EditorGUI.FloatField(position, "Selected Range", maxValue - minValue);
                GUI.enabled = true; // remember to make the UI editable again!

                if (isEditable)
                {

                    if (property.propertyType == SerializedPropertyType.Vector2)
                        property.vector2Value = new Vector2(val.x, val.y);
                    else if (property.FindPropertyRelative("MinMax") != null)
                        property.FindPropertyRelative("MinMax").vector2Value = new Vector2(val.x, val.y);
                }
            }

        }

        // this method lets unity know how big to draw the property. We need to override this because it could end up meing more than one line big
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            MinMaxAttribute minMax = attribute as MinMaxAttribute;

            // by default just return the standard line height
            float size = EditorGUIUtility.singleLineHeight;

            // if we have a special mode, add two extra lines!
            if (minMax.ShowEditRange || minMax.ShowDebugValues)
            {
                size += EditorGUIUtility.singleLineHeight * 2;
            }

            return size;
        }
    }
}