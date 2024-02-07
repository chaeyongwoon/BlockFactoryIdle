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

[AddComponentMenu("UI/ToJ Effects/Soft Shadow", 3)]
[RequireComponent(typeof(Text))]
public class SoftShadow : Shadow
{
	[SerializeField]
	private float m_BlurSpread = 1;

	[SerializeField]
	private bool m_OnlyInitialCharactersDropShadow = true;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	protected SoftShadow () { }

	#if UNITY_EDITOR
	protected override void OnValidate()
	{
		blurSpread = m_BlurSpread;
		onlyInitialCharactersDropShadow = m_OnlyInitialCharactersDropShadow;
		base.OnValidate();
	}
	#endif

	public float blurSpread
	{
		get { return m_BlurSpread; }
		set
		{
			m_BlurSpread = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public bool onlyInitialCharactersDropShadow
	{
		get { return m_OnlyInitialCharactersDropShadow; }
		set
		{
			m_OnlyInitialCharactersDropShadow = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
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

		/*
		List<UIVertex> neededVerts = new List<UIVertex>();
		if (onlyInitialCharactersDropShadow)
		{
			neededVerts = m_Verts.GetRange(initialVertexCount - textComponent.cachedTextGenerator.characterCountVisible * 6, textComponent.cachedTextGenerator.characterCountVisible * 6);
		}
		else
		{
			neededVerts = m_Verts;
		}
		*/
		int neededVertsRangeStart = 0;
		int neededVertsRangeLength = m_Verts.Count;
		if (m_OnlyInitialCharactersDropShadow)
		{
			neededVertsRangeStart = m_Verts.Count - textComponent.cachedTextGenerator.characterCountVisible * 6;
			neededVertsRangeLength = textComponent.cachedTextGenerator.characterCountVisible * 6;
		}

		Color effectColorEdited = effectColor;
		effectColorEdited.a /= 4;

		var start = neededVertsRangeStart;
		var end = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, effectColorEdited, start, m_Verts.Count, effectDistance.x, effectDistance.y);

		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (!((i == 0) && (j == 0)))
				{
					start = end;
					end = m_Verts.Count;
					ApplyShadowZeroAlloc(m_Verts, effectColorEdited, start, m_Verts.Count, effectDistance.x + i * blurSpread, effectDistance.y + j * blurSpread);
				}
			}
		}

		if (onlyInitialCharactersDropShadow)
		{
			//neededVerts.RemoveRange(neededVerts.Count - textComponent.cachedTextGenerator.characterCountVisible * 6, textComponent.cachedTextGenerator.characterCountVisible * 6);
			//neededVerts.AddRange(m_Verts);

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