//-------------------------------------------------------------------------------
// Copyright (c) 2016 Tag of Joy
// E-mail: info@tagofjoy.lt
// To use this, you must have purchased it on the Unity Asset Store (http://u3d.as/n57)
// Sharing or distribution are not permitted
//-------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Depth Effect", 2)]
[RequireComponent(typeof(Text))]
public class DepthEffect : BaseMeshEffect
{
	[SerializeField]
	private Color m_EffectColor = new Color(0f, 0f, 0f, 1f);

	[SerializeField]
	private Vector2 m_EffectDirectionAndDepth = new Vector2(-1f, 1f);

	[SerializeField]
	private Vector2 m_DepthPerspectiveStrength = new Vector2(0, 0);

	[SerializeField]
	private bool m_OnlyInitialCharactersGenerateDepth = true;

	[SerializeField]
	private bool m_UseGraphicAlpha = true;

	private Vector2 m_OverallTextSize = Vector2.zero;
	private Vector2 m_TopLeftPos = Vector2.zero;
	private Vector2 m_BottomRightPos = Vector2.zero;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	protected DepthEffect () { }

	#if UNITY_EDITOR
	protected override void OnValidate()
	{
		effectColor = m_EffectColor;
		effectDirectionAndDepth = m_EffectDirectionAndDepth;
		depthPerspectiveStrength = m_DepthPerspectiveStrength;
		onlyInitialCharactersGenerateDepth = m_OnlyInitialCharactersGenerateDepth;
		useGraphicAlpha = m_UseGraphicAlpha;
		base.OnValidate();
	}
	#endif

	public Color effectColor
	{
		get { return m_EffectColor; }
		set
		{
			m_EffectColor = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public Vector2 effectDirectionAndDepth
	{
		get { return m_EffectDirectionAndDepth; }
		set
		{
			if (m_EffectDirectionAndDepth == value)
			{
				return;
			}

			m_EffectDirectionAndDepth = value;

			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public Vector2 depthPerspectiveStrength
	{
		get { return m_DepthPerspectiveStrength; }
		set
		{
			if (m_DepthPerspectiveStrength == value)
			{
				return;
			}

			m_DepthPerspectiveStrength = value;

			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public bool onlyInitialCharactersGenerateDepth
	{
		get { return m_OnlyInitialCharactersGenerateDepth; }
		set
		{
			m_OnlyInitialCharactersGenerateDepth = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public bool useGraphicAlpha
	{
		get { return m_UseGraphicAlpha; }
		set
		{
			m_UseGraphicAlpha = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y, float factor)
	{
		UIVertex vt;

		for (int i = start; i < end; ++i)
		{
			vt = verts[i];
			verts.Add(vt);

			Vector3 v = vt.position;
			v.x += x * factor;
			if (depthPerspectiveStrength.x != 0)
			{
				v.x -= depthPerspectiveStrength.x * ((v.x - m_TopLeftPos.x) / m_OverallTextSize.x - 0.5f) * factor;
			}

			v.y += y * factor;
			if (depthPerspectiveStrength.y != 0)
			{
				v.y += depthPerspectiveStrength.y * ((m_TopLeftPos.y - v.y) / m_OverallTextSize.y - 0.5f) * factor;
			}

			vt.position = v;
			var newColor = color;
			if (useGraphicAlpha)
			{
				newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
			}
			vt.color = newColor;
			verts[i] = vt;
		}
	}

    public override void ModifyMesh (VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}

		vh.GetUIVertexStream(m_Verts);

		int initialVertexCount = m_Verts.Count;

		Text textComponent = GetComponent<Text>();
		
		int neededVertsRangeStart = 0;
		int neededVertsRangeLength = m_Verts.Count;
		if (m_OnlyInitialCharactersGenerateDepth)
		{
			neededVertsRangeStart = m_Verts.Count - textComponent.cachedTextGenerator.characterCountVisible * 6;
			neededVertsRangeLength = textComponent.cachedTextGenerator.characterCountVisible * 6;
		}
		
		if (neededVertsRangeLength == 0)
		{
			return;
		}

		if ((depthPerspectiveStrength.x != 0) || (depthPerspectiveStrength.y != 0))
		{
			m_TopLeftPos = m_Verts[neededVertsRangeStart].position;
			m_BottomRightPos = m_Verts[neededVertsRangeStart + neededVertsRangeLength - 1].position;

			for (int i = neededVertsRangeStart; i < neededVertsRangeStart + neededVertsRangeLength; i++)
			{
				if (m_Verts[i].position.x < m_TopLeftPos.x)
				{
					m_TopLeftPos.x = m_Verts[i].position.x;
				}
				if (m_Verts[i].position.y > m_TopLeftPos.y)
				{
					m_TopLeftPos.y = m_Verts[i].position.y;
				}

				if (m_Verts[i].position.x > m_BottomRightPos.x)
				{
					m_BottomRightPos.x = m_Verts[i].position.x;
				}
				if (m_Verts[i].position.y < m_BottomRightPos.y)
				{
					m_BottomRightPos.y = m_Verts[i].position.y;
				}
			}

			m_OverallTextSize = new Vector2(m_BottomRightPos.x - m_TopLeftPos.x, m_TopLeftPos.y - m_BottomRightPos.y);
		}

        var start = neededVertsRangeStart;
        var end = neededVertsRangeStart;

		start = end;
		end = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, effectColor, start, m_Verts.Count, effectDirectionAndDepth.x, effectDirectionAndDepth.y, 0.25f);

		start = end;
		end = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, effectColor, start, m_Verts.Count, effectDirectionAndDepth.x, effectDirectionAndDepth.y, 0.5f);

		start = end;
		end = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, effectColor, start, m_Verts.Count, effectDirectionAndDepth.x, effectDirectionAndDepth.y, 0.75f);

		start = end;
		end = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, effectColor, start, m_Verts.Count, effectDirectionAndDepth.x, effectDirectionAndDepth.y, 1f);
			

		if (onlyInitialCharactersGenerateDepth)
		{
			// this could be improved to not use an additional list
			List<UIVertex> rangeToBeMoved = m_Verts.GetRange(0, initialVertexCount - neededVertsRangeLength);
			m_Verts.RemoveRange(0, initialVertexCount - neededVertsRangeLength);
			m_Verts.InsertRange(m_Verts.Count - neededVertsRangeLength, rangeToBeMoved);
		}


		if (textComponent.material.shader == Shader.Find("Text Effects/Fancy Text"))
		{
			for (int i = 0; i < m_Verts.Count - initialVertexCount; i++)
			{
				UIVertex vert = m_Verts[i];
				vert.uv1 = new Vector2(0, 0);
				m_Verts[i] = vert;
			}
		}

        vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
    }
}