using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreLib;
using CoreLib.Module;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NaughtyAttributes.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UnityEngine.Object), true)]
	public class NaughtyInspector : UnityEditor.Editor
	{
		private static Dictionary<Object, Ref<bool>> s_NoteVisible = new Dictionary<Object, Ref<bool>>();

		private List<SerializedProperty>      _serializedProperties = new List<SerializedProperty>();
		private IEnumerable<FieldInfo>        _nonSerializedFields;
		private FieldInfo[]                   _staticInjectorFields;
		private IEnumerable<PropertyInfo>     _nativeProperties;
		private IEnumerable<MethodInfo>       _methods;
		private Dictionary<string, SavedBool> _foldouts = new Dictionary<string, SavedBool>();
		private PropertyInfo                  _note;
		private Ref<bool>                     _noteVisible;

		private static MethodInfoComparer s_MethodInfoComparer = new MethodInfoComparer();
		private        StaticInjector     _staticInjector;

		// =======================================================================
		public class MethodInfoComparer : IEqualityComparer<MethodInfo>
		{
			public bool Equals(MethodInfo x, MethodInfo y) => x.Name == y.Name;
			public int GetHashCode(MethodInfo obj) => obj.Name.GetHashCode();
		}
		
		// =======================================================================
		protected virtual void OnEnable()
		{
			_staticInjector = Core.Instance?.GetModule<StaticInjector>();
			
			_nonSerializedFields = ReflectionUtility.GetAllFields(
				target, f => f.GetCustomAttributes(typeof(ShowNonSerializedFieldAttribute), true).Length > 0);
			
			_staticInjectorFields = ReflectionUtility.GetAllFields(
				target, f => f.GetCustomAttributes(typeof(StaticInjectionAttribute), true).Length > 0).ToArray();

			_nativeProperties = ReflectionUtility.GetAllProperties(
				target, p => p.GetCustomAttributes(typeof(ShowNativePropertyAttribute), true).Length > 0);

			_methods = ReflectionUtility.GetAllMethods(
				target, m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0)
										.Distinct(s_MethodInfoComparer);
			
			_note = ReflectionUtility.GetAllProperties(target, info => info.Name == "Note" && info.PropertyType == typeof(string)).FirstOrDefault();
			if (s_NoteVisible.TryGetValue(target, out _noteVisible) == false)
			{
				_noteVisible = new Ref<bool>(false);
				s_NoteVisible.Add(target, _noteVisible);
			}
		}

		protected virtual void OnDisable()
		{
			ReorderableListPropertyDrawer.Instance.ClearCache();
		}

		public override void OnInspectorGUI()
		{
			GetSerializedProperties(ref _serializedProperties);

			var anyNaughtyAttribute = _serializedProperties.Any(p => PropertyUtility.GetAttribute<INaughtyAttribute>(p) != null) || (_staticInjectorFields?.Any() == true);
			if (!anyNaughtyAttribute)
			{
				DrawDefaultInspector();
			}
			else
			{
				DrawSerializedProperties();
			}

			DrawNonSerializedFields();
			DrawNativeProperties();
			DrawNote();
			DrawButtons();
		}

		private void DrawNote()
		{
			if (_note == null)
				return;
			
			var text = (string)_note.GetValue(serializedObject.targetObject);
			if (text.IsNullOrEmpty())
				return;
			
			EditorGUILayout.BeginVertical(GUI.skin.box);
			_noteVisible.Value = EditorGUILayout.Foldout(_noteVisible, "Note", true);
			if (_noteVisible)
			{
				//using (var disabled = new EditorGUI.DisabledScope(disabled: true))
					EditorGUILayout.LabelField(text, GUI.skin.label, GUILayout.Height(ResizableTextAreaPropertyDrawer.GetNumberOfLines(text) * EditorGUIUtility.singleLineHeight));
			}
			EditorGUILayout.EndVertical();
		}

		private void DrawStaticFields()
		{
			if (_staticInjectorFields.Any() == false)
				return;
			
			var type     = target.GetType();
			var assembly = Assembly.GetAssembly(type);

			foreach (var field in _staticInjectorFields)
			{
				var injector = _staticInjector.GetInjector(assembly.FullName, type.FullName, field.Name);
				if (injector == null)
					continue;

				EditorGUI.BeginChangeCheck();
				var fieldName = ObjectNames.NicifyVariableName(field.Name);
				if (field.FieldType == typeof(int))
					injector._int = EditorGUILayout.IntField(fieldName, injector._int);
				else if (field.FieldType == typeof(float))
					injector._float = EditorGUILayout.FloatField(fieldName, injector._float);
				else if (field.FieldType == typeof(string))
					injector._string = EditorGUILayout.TextField(fieldName, injector._string);
				else if (field.FieldType.Implements<Object>())
					injector._obj = EditorGUILayout.ObjectField(new GUIContent(fieldName), injector._obj, field.FieldType, false);
				else
					throw new ArgumentOutOfRangeException();

				if (EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(injector);
					injector.Invoke();
				}
			}
		}

		protected void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
		{
			outSerializedProperties.Clear();
			using (var iterator = serializedObject.GetIterator())
			{
				if (iterator.NextVisible(true))
				{
					do
					{
						outSerializedProperties.Add(serializedObject.FindProperty(iterator.name));
					}
					while (iterator.NextVisible(false));
				}
			}
		}

		protected void DrawSerializedProperties()
		{
			serializedObject.Update();

			// Draw non-grouped serialized properties
			foreach (var property in GetNonGroupedProperties(_serializedProperties))
			{
				if (property.name.Equals("m_Script", System.StringComparison.Ordinal))
				{
					using (new EditorGUI.DisabledScope(disabled: true))
					{
						EditorGUILayout.PropertyField(property);
					}
					// draw statics right after script
					DrawStaticFields();
				}
				else
				{
					NaughtyEditorGUI.PropertyField_Layout(property, includeChildren: true);
				}
			}

			// Draw grouped serialized properties
			foreach (var group in GetGroupedProperties(_serializedProperties))
			{
				IEnumerable<SerializedProperty> visibleProperties = group.Where(p => PropertyUtility.IsVisible(p));
				if (!visibleProperties.Any())
				{
					continue;
				}

				NaughtyEditorGUI.BeginBoxGroup_Layout(group.Key);
				foreach (var property in visibleProperties)
				{
					NaughtyEditorGUI.PropertyField_Layout(property, includeChildren: true);
				}

				NaughtyEditorGUI.EndBoxGroup_Layout();
			}

			// Draw foldout serialized properties
			foreach (var group in GetFoldoutProperties(_serializedProperties))
			{
				IEnumerable<SerializedProperty> visibleProperties = group.Where(p => PropertyUtility.IsVisible(p));
				if (!visibleProperties.Any())
				{
					continue;
				}

				if (!_foldouts.ContainsKey(group.Key))
				{
					_foldouts[group.Key] = new SavedBool($"{target.GetInstanceID()}.{group.Key}", false);
				}

				_foldouts[group.Key].Value = EditorGUILayout.Foldout(_foldouts[group.Key].Value, group.Key, true);
				if (_foldouts[group.Key].Value)
				{
					foreach (var property in visibleProperties)
					{
						NaughtyEditorGUI.PropertyField_Layout(property, true);
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawNonSerializedFields(bool drawHeader = false)
		{
			if (_nonSerializedFields.Any())
			{
				if (drawHeader)
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Non-Serialized Fields", GetHeaderGUIStyle());
					NaughtyEditorGUI.HorizontalLine(
						EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
				}

				foreach (var field in _nonSerializedFields)
				{
					NaughtyEditorGUI.NonSerializedField_Layout(serializedObject.targetObject, field);
				}
			}
		}

		protected void DrawNativeProperties(bool drawHeader = false)
		{
			if (_nativeProperties.Any())
			{
				if (drawHeader)
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Native Properties", GetHeaderGUIStyle());
					NaughtyEditorGUI.HorizontalLine(
						EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
				}

				foreach (var property in _nativeProperties)
				{
					NaughtyEditorGUI.NativeProperty_Layout(serializedObject.targetObject, property);
				}
			}
		}

		protected void DrawButtons(bool drawHeader = false)
		{
			if (_methods.Any())
			{
				if (drawHeader)
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Buttons", GetHeaderGUIStyle());
					NaughtyEditorGUI.HorizontalLine(
						EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
				}

				foreach (var method in _methods)
				{
					NaughtyEditorGUI.Button(serializedObject.targetObject, method);
				}
			}
		}

		private static IEnumerable<SerializedProperty> GetNonGroupedProperties(IEnumerable<SerializedProperty> properties)
		{
			return properties.Where(p => PropertyUtility.GetAttribute<IGroupAttribute>(p) == null);
		}

		private static IEnumerable<IGrouping<string, SerializedProperty>> GetGroupedProperties(IEnumerable<SerializedProperty> properties)
		{
			return properties
				.Where(p => PropertyUtility.GetAttribute<BoxGroupAttribute>(p) != null)
				.GroupBy(p => PropertyUtility.GetAttribute<BoxGroupAttribute>(p).Name);
		}

		private static IEnumerable<IGrouping<string, SerializedProperty>> GetFoldoutProperties(IEnumerable<SerializedProperty> properties)
		{
			return properties
				.Where(p => PropertyUtility.GetAttribute<FoldoutAttribute>(p) != null)
				.GroupBy(p => PropertyUtility.GetAttribute<FoldoutAttribute>(p).Name);
		}

		private static GUIStyle GetHeaderGUIStyle()
		{
			GUIStyle style = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.UpperCenter;

			return style;
		}
	}
}
