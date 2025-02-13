﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OwlAndJackalope.UX.Runtime.DetailBinders;
using OwlAndJackalope.UX.Runtime.Modules;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Editor.DetailDrawers
{
    [CustomPropertyDrawer(typeof(DetailTypeAttribute))]
    public class DetailTypeAttributeDrawer : PropertyDrawer
    {
        public class PropertyData
        {
            public ReferenceModule Module;
            public DetailTypeAttribute Attribute;
        }
        protected readonly Dictionary<string, PropertyData> _data = new Dictionary<string, PropertyData>();

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            label = new Regex("\\.data\\[\\d+\\]$").IsMatch(property.propertyPath) ? GUIContent.none : label;
            
            var name = property.FindPropertyRelative("DetailName");
            if (!_data.TryGetValue(property.propertyPath, out var propertyData))
            {
                propertyData = new PropertyData();
                propertyData.Attribute = (DetailTypeAttribute) attribute;
                _data[property.propertyPath] = propertyData;
            }

            var containingObject = (property.serializedObject.targetObject as MonoBehaviour);
            if (containingObject != null)
            {
                if (containingObject is BaseDetailBinder containingBinder && containingBinder.ReferenceModule != null)
                {
                    propertyData.Module = containingBinder.ReferenceModule;
                }
                else
                {
                    propertyData.Module = containingObject.GetComponentInParent<ReferenceModule>();    
                }
            }

            if (propertyData.Module != null)
            {
                var options = GetOptions(propertyData);
                var currentIndex = Math.Max(0, Array.IndexOf(options, name.stringValue));
                var selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, options);
                if (selectedIndex >= 0 && selectedIndex < options.Length)
                {
                    name.stringValue = options[selectedIndex];
                }
            }
            else
            {
                EditorGUI.PropertyField(position, name, label);
            }
        }

        private string[] GetOptions(PropertyData data)
        {
            var details = data.Module.SerializedReference.GetDetails(data.Attribute.AcceptableTypes);
            return details.Select(x => x.Name).ToArray();
        }
    }
}