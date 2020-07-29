using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;

namespace J2P
{
	[CustomEditor(typeof (JPhysicsSetting))]
    public class JPhysicsSettingInspector : Editor
    {
		private static string _settingsPath = "Assets/J2P/Resources/JPhysics Settings.asset";

		private JPhysicsSetting _target;

		private SerializedProperty _gravity;

		private bool _showCollisionMasks = true;

		void OnEnable()
		{
			_target = this.target as JPhysicsSetting;
			_gravity = serializedObject.FindProperty( "_gravity" );

			this.InitCollisionLayerMasks();
		}

		private void InitCollisionLayerMasks()
		{
			if( _target.collisionLayerMasks.Count == 0 )
			{
				for( int i = 0; i < 32; i++ )
				{
					var collisionLayerMask = new CollisionLayerMask();
					collisionLayerMask.layer = i;
					_target.collisionLayerMasks.Add( collisionLayerMask );
				}
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField( _gravity );
			//EditorGUILayout.PropertyField( _collisionLayerMasks, true );
			this.DrawCollisionLayerMasks();

			GUILayout.Space( 10.0f );

			if( GUI.changed )
			{
				EditorUtility.SetDirty( _target );
			}

			serializedObject.ApplyModifiedProperties();
		}

		public void DrawCollisionLayerMasks()
		{
			GUILayout.Space( 20 );
			_showCollisionMasks = EditorGUILayout.Foldout( _showCollisionMasks, "Layer Collision Masks" );
			if( _showCollisionMasks )
			{
				Undo.RecordObject( _target, "Change Layer Masks" );
				for( int i = 0; i < _target.collisionLayerMasks.Count; i++ )
				{
					var collisionLayerMask = _target.collisionLayerMasks[i];
					var layerName = LayerMask.LayerToName( i );
					if( !string.IsNullOrEmpty( layerName ) )
					{
						//layer名称区分,并非所有layer都是用来做物理检测的
						//if( layerName.Contains( "R_" ) )
						//{
						//	continue;
						//}
						collisionLayerMask.layerMask = LayerMaskField( layerName, collisionLayerMask.layerMask );
					}
				}
			}
		}

		static List<int> layerNumbers = new List<int>( 32 );
		static List<string> finalLayers = new List<string>( 32 );

		static LayerMask LayerMaskField( string label, LayerMask layerMask )
		{
			var originalLayers = InternalEditorUtility.layers;

			finalLayers.Clear();
			for( int i = 0; i < originalLayers.Length; i++ )
			{
				var layerName = originalLayers[i];
				//layer名称区分,并非所有layer都是用来做物理检测的
				//if( layerName.Contains( "R_" ) )
				//{
				//	continue;
				//}

				finalLayers.Add( layerName );
			}

			layerNumbers.Clear();
			for( int i = 0; i < finalLayers.Count; i++ )
			{
				var layerName = finalLayers[i];
				layerNumbers.Add( LayerMask.NameToLayer( finalLayers[i] ) );
			}

			int maskWithoutEmpty = 0;
			for( int i = 0; i < layerNumbers.Count; i++ )
			{
				if( ( ( 1 << layerNumbers[i] ) & layerMask.value ) > 0 )
				{
					maskWithoutEmpty |= ( 1 << i );
				}
			}

			maskWithoutEmpty = EditorGUILayout.MaskField( label, maskWithoutEmpty, finalLayers.ToArray(), EditorStyles.layerMaskField );

			int mask = 0;
			for( int i = 0; i < layerNumbers.Count; i++ )
			{
				if( ( maskWithoutEmpty & ( 1 << i ) ) > 0 )
				{
					mask |= ( 1 << layerNumbers[i] );
				}
			}
			layerMask.value = mask;

			return layerMask;
		}

		[MenuItem( "J2P/Physics Settings", false, 1 )]
		static void EditPhyscisSettings()
		{
			var settings = AssetDatabase.LoadAssetAtPath<JPhysicsSetting>( _settingsPath );
			if( settings == null )
			{
				settings = ScriptableObject.CreateInstance<JPhysicsSetting>();
				AssetDatabase.CreateAsset( settings, _settingsPath );
				AssetDatabase.SaveAssets();

				EditorUtility.FocusProjectWindow();
			}
			Selection.activeObject = settings;
			//EditorGUIUtility.PingObject( settings );
		}
	}
}
