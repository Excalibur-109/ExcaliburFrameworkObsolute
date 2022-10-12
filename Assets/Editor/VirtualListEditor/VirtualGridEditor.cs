using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace Excalibur
{
	[CustomEditor(typeof(VirtualGrid))]
	public class VirtualGridEditor : Editor
    {
        SerializedProperty m_IsVirtual;
        SerializedProperty m_Tumble;
        SerializedProperty m_Prefab;
        SerializedProperty m_RolAndColumn;
        SerializedProperty m_AutoSelect;
        SerializedProperty m_UseMouseWheel;
        SerializedProperty m_PreButton;
        SerializedProperty m_NextButton;
        SerializedProperty m_PageText;
        SerializedProperty m_PageTextTMP;
        SerializedProperty m_ShowPageCount;
        SerializedProperty m_PageScrollEnable;
        SerializedProperty m_AutoScroll;
        SerializedProperty m_AutoScrollInterval;
        SerializedProperty m_AutoScrollSpeed;

        VirtualGrid m_TargetGrid;
        ScrollRect m_ScrollRect;

        LayoutGroup m_LayoutGroup;
        RectOffset m_Offset;
        ContentSizeFitter m_ContentFitter;
        static Vector2 m_Spacing;
        static Vector2 m_GridCellSize;

        GUIContent gridGroupBtnTip;
        GUIContent horizontalGroupBtnTip;
        GUIContent verticalGroupBtnTip;
        GUIContent msgtip;

		private void OnEnable()
        {
            m_TargetGrid = target as VirtualGrid;
            m_IsVirtual = serializedObject.FindProperty("m_IsVirtual");
            m_Tumble = serializedObject.FindProperty("m_Tumble");
            m_Prefab = serializedObject.FindProperty("m_Prefab");
            m_RolAndColumn = serializedObject.FindProperty("m_RowAndColumn");
            m_AutoSelect = serializedObject.FindProperty("m_AutoSelect");
            m_UseMouseWheel = serializedObject.FindProperty("m_UseMouseWheel");
            m_PreButton = serializedObject.FindProperty("m_PreButton");
            m_NextButton = serializedObject.FindProperty("m_NextButton");
            m_PageText = serializedObject.FindProperty("m_PageText");
            m_PageTextTMP = serializedObject.FindProperty("m_PageTextTMP");
            m_ShowPageCount = serializedObject.FindProperty("m_ShowPageCount");
            m_PageScrollEnable = serializedObject.FindProperty("m_PageScrollEnable");
            m_AutoScroll = serializedObject.FindProperty("m_AutoScroll");
            m_AutoScrollInterval = serializedObject.FindProperty("m_AutoScrollInterval");
            m_AutoScrollSpeed = serializedObject.FindProperty("m_AutoScrollSpeed");

            m_ScrollRect = m_TargetGrid.transform.parent.parent.GetComponent<ScrollRect>();

            if (!EditorApplication.isPlaying)
            {
                RectTransform rect = m_TargetGrid.transform as RectTransform;
                RectTransform parentRect = m_TargetGrid.transform.parent as RectTransform;
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRect.rect.width);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentRect.rect.height);
                rect.anchoredPosition = Vector2.zero;
            }

            gridGroupBtnTip = new GUIContent("Grid");
            horizontalGroupBtnTip = new GUIContent("Horizontal");
            verticalGroupBtnTip = new GUIContent("Vertical");
            msgtip = new GUIContent("1.Tumble��ʾ����������ˮƽ����ֱ��ˮƽ��ҳ����ֱ��ҳ\n" +
                                    "2.SlotPrefab���Բ����뵽���ӣ��������ӽڵ�һ��Ҫ��\n" +
                                    "3.��ѡAuto Select���Զ�ѡ���һ��Item���������¼�\n");
                                    //"4.X��Y��ʾ��ViewPort��Ұ�ڵ��С��е�����\n" +
                                    //"5.��ѡUseMouseWheel������ʹ�ù��ֹ����б���ScrollSensitivity���Ϊ50");
        }

		public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // EditorGUILayout.PropertyField(m_IsVirtual);
            m_IsVirtual.boolValue = true;
            EditorGUILayout.PropertyField(m_Tumble);
            EditorGUILayout.PropertyField(m_Prefab);
            EditorGUILayout.PropertyField(m_AutoSelect);
            EditorGUILayout.PropertyField(m_UseMouseWheel);
            if (m_IsVirtual.boolValue)
            {
                EditorGUILayout.PropertyField(m_RolAndColumn);
            }
            switch ((Tumble)m_Tumble.enumValueIndex)
            {
                case Tumble.PageTurning_Horizontal:
                case Tumble.PageTurning_Vertical:
                    EditorGUILayout.PropertyField(m_PreButton);
                    EditorGUILayout.PropertyField(m_NextButton);
                    EditorGUILayout.PropertyField(m_PageText);
                    EditorGUILayout.PropertyField(m_PageTextTMP);
                    EditorGUILayout.PropertyField(m_ShowPageCount);
                    if (!EditorApplication.isPlaying && m_IsVirtual.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_PageScrollEnable);
                    }
                    EditorGUILayout.PropertyField(m_AutoScroll);
                    if (m_AutoScroll.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_AutoScrollInterval);
                    }
                    if (m_PageScrollEnable.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_AutoScrollSpeed);
                    }
                    break;
            }

            if (m_LayoutGroup == null)
                m_LayoutGroup = m_TargetGrid.GetComponent<LayoutGroup>();
            if (m_LayoutGroup != null)
            {
                m_Offset = m_LayoutGroup.padding;
                if (m_LayoutGroup is GridLayoutGroup grid)
                {
                    m_Spacing = grid.spacing;
                    m_GridCellSize = grid.cellSize;
                    if (m_Tumble.enumValueIndex == (int)Tumble.Tumble_Horizontal ||
                        m_Tumble.enumValueIndex == (int)Tumble.PageTurning_Horizontal)
                    {
                        grid.startAxis = GridLayoutGroup.Axis.Vertical;
                    }
                    if (m_Tumble.enumValueIndex == (int)Tumble.Tumble_Vertical ||
                        m_Tumble.enumValueIndex == (int)Tumble.PageTurning_Vertical)
                    {
                        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
                    }
                }
                else if (m_LayoutGroup is HorizontalLayoutGroup horizontal)
                {
                    m_Spacing[1] = horizontal.spacing;
                    m_RolAndColumn.vector2IntValue = new Vector2Int(1, m_RolAndColumn.vector2IntValue[1]);
                }
                else if (m_LayoutGroup is VerticalLayoutGroup vertical)
                {
                    m_Spacing[0] = vertical.spacing;
                    m_RolAndColumn.vector2IntValue = new Vector2Int(m_RolAndColumn.vector2IntValue[0], 1);
                }
            }

            if (!EditorApplication.isPlaying)
            {
                GUILayout.Space(5);

                GUILayout.Label("LayoutGroup:");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(gridGroupBtnTip))
                {
                    if (m_LayoutGroup is GridLayoutGroup)
                        return;
                    DestroyImmediate(m_LayoutGroup);
                    m_LayoutGroup = m_TargetGrid.gameObject.AddComponent<GridLayoutGroup>();
                    m_LayoutGroup.padding = m_Offset;
                    (m_LayoutGroup as GridLayoutGroup).spacing = m_Spacing;
                    (m_LayoutGroup as GridLayoutGroup).cellSize = m_GridCellSize;
                }
                if (GUILayout.Button(horizontalGroupBtnTip))
                {
                    if (m_LayoutGroup is HorizontalLayoutGroup)
                        return;
                    DestroyImmediate(m_LayoutGroup);
                    m_LayoutGroup = m_TargetGrid.gameObject.AddComponent<HorizontalLayoutGroup>();
                    m_LayoutGroup.padding = m_Offset;
                    (m_LayoutGroup as HorizontalLayoutGroup).spacing = m_Spacing[1];
                }
                if (GUILayout.Button(verticalGroupBtnTip))
                {
                    if (m_LayoutGroup is VerticalLayoutGroup)
                        return;
                    DestroyImmediate(m_LayoutGroup);
                    m_LayoutGroup = m_TargetGrid.gameObject.AddComponent<VerticalLayoutGroup>();
                    m_LayoutGroup.padding = m_Offset;
                    (m_LayoutGroup as VerticalLayoutGroup).spacing = m_Spacing[0];
                }
                EditorGUILayout.EndHorizontal();
            }

            if (m_ScrollRect == null)
            {
                m_ScrollRect = m_TargetGrid.transform.parent.parent.gameObject.AddComponent<ScrollRect>();
            }
            switch ((Tumble)m_Tumble.enumValueIndex)
            {
                case Tumble.Tumble_Horizontal:
                case Tumble.PageTurning_Horizontal:
                    m_ScrollRect.horizontal = true;
                    m_ScrollRect.vertical = false;
                    break;
                case Tumble.Tumble_Vertical:
                case Tumble.PageTurning_Vertical:
                    m_ScrollRect.horizontal = false;
                    m_ScrollRect.vertical = true;
                    break;
            }
            m_ScrollRect.movementType = ScrollRect.MovementType.Elastic;
            m_ScrollRect.elasticity = 0.1f;
            if (m_ScrollRect.scrollSensitivity < 50f)
                m_ScrollRect.scrollSensitivity = 50f;

            if ((Tumble)m_Tumble.enumValueIndex == Tumble.Tumble_Horizontal ||
                (Tumble)m_Tumble.enumValueIndex == Tumble.Tumble_Vertical)
            {
                m_ScrollRect.inertia = true;
                m_ScrollRect.enabled = true;
                if (!m_IsVirtual.boolValue)
                {
                    m_PageScrollEnable.boolValue = true;
                }
            }
            else
            {
                m_ScrollRect.inertia = false;
                m_ScrollRect.enabled = m_PageScrollEnable.boolValue;
            }
            if (!m_UseMouseWheel.boolValue)
                m_ScrollRect.scrollSensitivity = 0f;

            m_ContentFitter = m_TargetGrid.GetComponent<ContentSizeFitter>();
            if (m_IsVirtual.boolValue)
            {
                DestroyImmediate(m_ContentFitter);
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.HelpBox(msgtip);
        }
	}
}