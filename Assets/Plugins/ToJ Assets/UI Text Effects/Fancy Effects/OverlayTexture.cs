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
using System.Text.RegularExpressions;

[AddComponentMenu("UI/ToJ Effects/Overlay Texture", 18)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class OverlayTexture : BaseMeshEffect, IMaterialModifier
{
	public enum TextureMode
	{
		Local = 0,
		GlobalTextArea = 1,
		GlobalFullRect = 2
	}
	public enum ColorMode
	{
		Override,
		Additive,
		Multiply
	}

	[SerializeField]
	private TextureMode m_TextureMode = TextureMode.Local;

	[SerializeField]
	private ColorMode m_ColorMode = ColorMode.Override;

	[SerializeField]
	public Texture2D m_OverlayTexture;

	private bool m_NeedsToSetMaterialDirty = false;

	private Material m_ModifiedMaterial;
	private List<UIVertex> m_Verts = new List<UIVertex>();


	protected OverlayTexture () { }

	#if UNITY_EDITOR
	protected override void OnValidate()
	{
		textureMode = m_TextureMode;
		colorMode = m_ColorMode;
		overlayTexture = m_OverlayTexture;
		base.OnValidate();
	}
	#endif

	protected override void Start ()
	{
		if (graphic != null)
		{
			graphic.SetMaterialDirty();
		}
	}

	public TextureMode textureMode
	{
		get { return m_TextureMode; }
		set
		{
			m_TextureMode = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public ColorMode colorMode
	{
		get { return m_ColorMode; }
		set
		{
			m_ColorMode = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public Texture2D overlayTexture
	{
		get { return m_OverlayTexture; }
		set
		{
			m_OverlayTexture = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}

		vh.GetUIVertexStream(m_Verts);

		int count = m_Verts.Count;
		UIVertex uiVertex;

		if (m_Verts.Count == 0)
		{
			return;
		}

		if ((textureMode == TextureMode.GlobalTextArea) || (textureMode == TextureMode.GlobalFullRect))
		{
			Vector2 topLeftPos = Vector2.zero;
			Vector2 bottomRightPos = Vector2.zero;
			if (textureMode == TextureMode.GlobalFullRect)
			{
				Rect rect = GetComponent<RectTransform>().rect;
				topLeftPos = new Vector2(rect.xMin, rect.yMax);
				bottomRightPos = new Vector2(rect.xMax, rect.yMin);
			}
			else
			{
				topLeftPos = m_Verts[0].position;
				bottomRightPos = m_Verts[m_Verts.Count - 1].position;

				for (int i = 0; i < m_Verts.Count; i++)
				{
					if (m_Verts[i].position.x < topLeftPos.x)
					{
						topLeftPos.x = m_Verts[i].position.x;
					}
					if (m_Verts[i].position.y > topLeftPos.y)
					{
						topLeftPos.y = m_Verts[i].position.y;
					}

					if (m_Verts[i].position.x > bottomRightPos.x)
					{
						bottomRightPos.x = m_Verts[i].position.x;
					}
					if (m_Verts[i].position.y < bottomRightPos.y)
					{
						bottomRightPos.y = m_Verts[i].position.y;
					}
				}
			}

			float overallHeight = topLeftPos.y - bottomRightPos.y;
			float overallWidth = bottomRightPos.x - topLeftPos.x;

			for (int i = 0; i < count; i++)
			{
				uiVertex = m_Verts[i];

				uiVertex.uv1 = new Vector2(1 + (uiVertex.position.x - topLeftPos.x) / overallWidth, 1 + 1f - (topLeftPos.y - uiVertex.position.y) / overallHeight);

				m_Verts[i] = uiVertex;
			}
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				uiVertex = m_Verts[i];

				uiVertex.uv1 = new Vector2(1 + (((i % 6 == 0) || (i % 6 == 5) || (i % 6 == 4)) ? 0 : 1), 1 + (((i % 6 == 2) || (i % 6 == 3) || (i % 6 == 4)) ? 0 : 1));

				m_Verts[i] = uiVertex;
			}
		}

		vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
	}


	void Update ()
	{
		if (!m_NeedsToSetMaterialDirty)
		{
			return;
		}

		if (graphic != null)
		{
			graphic.SetMaterialDirty();
		}
	}

	public virtual Material GetModifiedMaterial (Material baseMaterial)
	{
		if (!IsActive())
		{
			return baseMaterial;
		}

		if (baseMaterial.shader != Shader.Find("Text Effects/Fancy Text"))
		{
			Debug.Log("\"" + gameObject.name + "\"" + " doesn't have the \"Fancy Text\" shader applied. Please use it if you are using the \"Overlay Texture\" effect.");
			return baseMaterial;
		}

		if (m_ModifiedMaterial == null)
		{
			m_ModifiedMaterial = new Material(baseMaterial);
		}
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.name = baseMaterial.name + " with OT";
		m_ModifiedMaterial.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;


		m_ModifiedMaterial.shaderKeywords = baseMaterial.shaderKeywords;
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);

		m_ModifiedMaterial.EnableKeyword("_USEOVERLAYTEXTURE_ON");
		m_ModifiedMaterial.SetTexture("_OverlayTex", overlayTexture);
		m_ModifiedMaterial.SetInt("_OverlayTextureColorMode", (int)colorMode);

		m_NeedsToSetMaterialDirty = true;

		return m_ModifiedMaterial;
	}
}