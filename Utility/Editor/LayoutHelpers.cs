using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VNTags.Utility
{
    public static class LayoutHelpers
    {
        /// <summary>
        ///     Renders a generic Unity Editor GUI popup (dropdown) control.
        ///     This function handles the display of the popup, updates a reference index based on user selection,
        ///     fetches a corresponding target value using a provided fetcher function, and executes a
        ///     post-selection action if the selection changes.
        /// </summary>
        /// <typeparam name="T">The type of the target object that the popup selection will represent.</typeparam>
        /// <param name="label">The label to display next to the popup.</param>
        /// <param name="options">An array of <see cref="GUIContent" /> representing the selectable items in the popup.</param>
        /// <param name="index">A reference to an integer that stores and updates the currently selected index in the popup.</param>
        /// <param name="fetcher">
        ///     A delegate (<see cref="Func{T, TResult}" />) that takes the selected index and returns an object of type
        ///     <typeparamref name="T" />.
        ///     This is used to retrieve the actual value associated with the selected option.
        /// </param>
        /// <param name="target">
        ///     A reference to the variable of type <typeparamref name="T" /> that will be updated with the value
        ///     fetched by the <paramref name="fetcher" /> function.
        /// </param>
        /// <param name="postPopupAction">
        ///     An <see cref="Action" /> to be executed after the popup selection changes and the
        ///     <paramref name="target" /> is updated.
        /// </param>
        public static void RenderPopup<T>(string label, GUIContent[] options, ref int index, Func<int, T> fetcher, ref T target, Action postPopupAction)
        {
            EditorGUILayout.PrefixLabel(label);

            int lastIndex = index;
            index = EditorGUILayout.Popup(lastIndex, options);

            if (lastIndex == index)
            {
                return;
            }

            T temp = fetcher(index);
            if (temp != null)
            {
                target = temp;
            }
            else if (index != 0)
            {
                Debug.LogError("VNTagEditor: RenderPopup: Option has no value, '" + options[index] + "' with index " + index);
            }
            else
            {
                target = default;
            }

            postPopupAction();
        }

        /// <summary>
        ///     Renders a generic Unity Editor GUI popup (dropdown) control.
        ///     This function handles the display of the popup, updates a reference index based on user selection,
        ///     fetches a corresponding target value using a provided fetcher function, and executes a
        ///     post-selection action if the selection changes.
        /// </summary>
        /// <typeparam name="T">The type of the target object that the popup selection will represent.</typeparam>
        /// <param name="label">The label to display next to the popup.</param>
        /// <param name="options">An array of <see cref="GUIContent" /> representing the selectable items in the popup.</param>
        /// <param name="current">A reference to a string that stores and updates the currently selected string in the popup.</param>
        /// <param name="fetcher">
        ///     A delegate (<see cref="Func{T, TResult}" />) that takes the selected string and returns an object of type
        ///     <typeparamref name="T" />.
        ///     This is used to retrieve the actual value associated with the selected option.
        /// </param>
        /// <param name="target">
        ///     A reference to the variable of type <typeparamref name="T" /> that will be updated with the value
        ///     fetched by the <paramref name="fetcher" /> function.
        /// </param>
        /// <param name="postPopupAction">
        ///     An <see cref="Action" /> to be executed after the popup selection changes and the
        ///     <paramref name="target" /> is updated.
        /// </param>
        public static void RenderPopup<T>(string label, GUIContent[] options, ref string current, Func<string, T> fetcher, ref T target, Action postPopupAction)
        {
            EditorGUILayout.PrefixLabel(label);

            string cc        = current;
            int    lastIndex = Array.FindIndex(options, content => content.text.Equals(cc, StringComparison.OrdinalIgnoreCase));
            if (lastIndex == -1)
            {
                lastIndex = 0; // Default to the first item if current string is not found
            }

            int newIndex = EditorGUILayout.Popup(lastIndex, options);

            // if (lastIndex == newIndex)
            // {
            //     return;
            // }

            if (options.Length <= 0)
            {
                return;
            }

            current = options[newIndex].text;

            T temp = fetcher(current);
            if (temp != null)
            {
                target = temp;
            }
            else if (newIndex != 0) // If the selected option is not the "none" or default option (usually at index 0)
            {
                // Debug.LogError("VNTagEditor: RenderPopup: Option has no value, '" + options[newIndex].text + "' with index " + newIndex);
                target = default;
            }
            else
            {
                target = default;
            }

            postPopupAction();
        }

        /// <summary>
        ///     Renders a Unity Editor GUI multi-select popup (mask field) control.
        ///     This function handles the display of the mask field, updates a reference bitmask based on user selection,
        ///     fetches a corresponding array of target values using a provided fetcher function, and executes a
        ///     post-selection action if the selection changes.
        /// </summary>
        /// <typeparam name="T">The type of the target objects that the multi-select popup selection will represent.</typeparam>
        /// <param name="label">The label to display next to the multi-select popup.</param>
        /// <param name="options">
        ///     An array of <see cref="GUIContent" /> representing the selectable items in the popup.
        ///     The <see cref="GUIContent.text" /> property will be used for the options displayed in the mask field.
        /// </param>
        /// <param name="currentMask">
        ///     A reference to an integer that stores and updates the current bitmask representing the
        ///     selected options.
        /// </param>
        /// <param name="MaskToValueFetcher">
        ///     A delegate (<see cref="Func{T, TResult}" />) that takes the updated bitmask and returns an array of objects of type
        ///     <typeparamref name="T" />
        ///     corresponding to the selected options.
        /// </param>
        /// <param name="target">
        ///     A reference to the variable of type <typeparamref name="T" />[] that will be updated with the
        ///     values fetched by the <paramref name="MaskToValueFetcher" /> function.
        /// </param>
        /// <param name="postPopupAction">
        ///     An <see cref="Action" /> to be executed after the popup selection changes and the
        ///     <paramref name="target" /> is updated.
        /// </param>
        public static void RenderMaskMultiSelectPopup<T>
            (string label, GUIContent[] options, ref int currentMask, Func<int, T[]> MaskToValueFetcher, ref T[] target, Action postPopupAction)
        {
            EditorGUILayout.PrefixLabel(label);

            string[] sOptions = options.Select(t => t.text).ToArray();

            int newMask = EditorGUILayout.MaskField(currentMask, sOptions);

            if (currentMask == newMask)
            {
                return;
            }

            currentMask = newMask;

            var temp = MaskToValueFetcher(newMask);
            if (temp != null)
            {
                target = temp;
            }
            else if (currentMask > 0)
            {
                Debug.LogError("VNTagEditor: RenderPopup: Option has no value with mask " + newMask);
            }
            else
            {
                target = default;
            }

            postPopupAction();
        }

        /// <summary>
        ///     Converts an integer bitmask (from EditorGUILayout.MaskField) into a string array of selected option names.
        /// </summary>
        /// <param name="mask">The integer mask representing the selected options.</param>
        /// <param name="options">The array of all possible option names (strings).</param>
        /// <returns>A string array containing the names of the selected options.</returns>
        public static string[] MaskToStringArray(int mask, string[] options)
        {
            var selectedNames = new List<string>();
            for (int i = 0; i < options.Length; i++)
            {
                if ((mask & (1 << i)) != 0) // Check if the i-th bit is set
                {
                    selectedNames.Add(options[i]);
                }
            }

            return selectedNames.ToArray();
        }

        /// <summary>
        ///     Converts an integer bitmask (from EditorGUILayout.MaskField) into an array of selected generic objects.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the options array.</typeparam>
        /// <param name="mask">The integer mask representing the selected options.</param>
        /// <param name="options">
        ///     The array of all possible <typeparamref name="T" /> objects, corresponding to the order used when
        ///     creating the mask.
        /// </param>
        /// <returns>An array of <typeparamref name="T" /> containing the objects corresponding to the selected bits in the mask.</returns>
        public static T[] MaskToValueArray<T>(int mask, T[] options)
        {
            var selected = new List<T>();
            for (int i = 0; i < options.Length; i++)
            {
                if ((mask & (1 << i)) != 0) // Check if the i-th bit is set
                {
                    selected.Add(options[i]);
                }
            }

            return selected.ToArray();
        }

        /// <summary>
        ///     Converts a string array of selected option names into an integer bitmask suitable for EditorGUILayout.MaskField.
        /// </summary>
        /// <param name="selectedNames">A string array containing the names of the options that are selected.</param>
        /// <param name="allOptions">The array of all possible option names (strings), in the same order as used for the MaskField.</param>
        /// <returns>An integer bitmask representing the selected options.</returns>
        public static int StringArrayToMask(string[] selectedNames, string[] allOptions)
        {
            int mask = 0;
            foreach (string selectedName in selectedNames)
            {
                int index = Array.IndexOf(allOptions, selectedName);
                if (index >= 0)
                {
                    mask |= 1 << index; // Set the bit corresponding to the option's index
                }
            }

            return mask;
        }

        /// <summary>
        ///     Converts an array of selected generic objects into an integer bitmask suitable for EditorGUILayout.MaskField.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the arrays.</typeparam>
        /// <param name="selected">An array containing the <typeparamref name="T" /> values of the options that are selected.</param>
        /// <param name="allOptions">
        ///     The array of all possible <typeparamref name="T" /> objects, in the same order as used for the
        ///     MaskField.
        /// </param>
        /// <returns>An integer bitmask representing the selected options.</returns>
        public static int ValueArrayToMask<T>(T[] selected, T[] allOptions)
        {
            int mask = 0;
            foreach (T select in selected)
            {
                int index = Array.IndexOf(allOptions, select);
                if (index >= 0)
                {
                    mask |= 1 << index; // Set the bit corresponding to the option's index
                }
            }

            return mask;
        }
    }
}