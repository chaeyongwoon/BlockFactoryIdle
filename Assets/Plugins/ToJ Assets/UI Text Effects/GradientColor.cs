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

[AddComponentMenu("UI/ToJ Effects/Gradient Color", 1)]
[RequireComponent(typeof(Text))]
public class GradientColor : BaseMeshEffect
{
	public enum GradientMode
	{
		Local = 0,
		GlobalTextArea = 1,
		GlobalFullRect = 2
	}
	public enum GradientDirection
	{
		Vertical,
		Horizontal
	}
	public enum ColorMode
	{
		Override,
		Additive,
		Multiply
	}

	[SerializeField]
	private GradientMode m_GradientMode = GradientMode.Local;

	[SerializeField]
	private GradientDirection m_GradientDirection = GradientDirection.Vertical;

	[SerializeField]
	private ColorMode m_ColorMode = ColorMode.Override;

	[SerializeField]
	public Color m_FirstColor = Color.white;

	[SerializeField]
	public Color m_SecondColor = Color.black;

	[SerializeField]
	private bool m_UseGraphicAlpha = true;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	protected GradientColor () { }
	
	#if UNITY_EDITOR
	protected override void OnValidate()
	{
		gradientMode = m_GradientMode;
		gradientDirection = m_GradientDirection;

		colorMode = m_ColorMode;
		firstColor = m_FirstColor;
		secondColor = m_SecondColor;

		useGraphicAlpha = m_UseGraphicAlpha;
		base.OnValidate();
	}
	#endif

	public GradientMode gradientMode
	{
		get { return m_GradientMode; }
		set
		{
			m_GradientMode = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public GradientDirection gradientDirection
	{
		get { return m_GradientDirection; }
		set
		{
			m_GradientDirection = value;
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

	public Color firstColor
	{
		get { return m_FirstColor; }
		set
		{
			m_FirstColor = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public Color secondColor
	{
		get { return m_SecondColor; }
		set
		{
			m_SecondColor = value;
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

    public override void ModifyMesh (VertexHelper vh)
	{
		if (!IsActive ())
        {
			return;
		}

		vh.GetUIVertexStream(m_Verts);

		if (m_Verts.Count == 0)
		{
			return;
		}

		if ((gradientMode == GradientMode.GlobalTextArea) || (gradientMode == GradientMode.GlobalFullRect))
		{
			Vector2 topLeftPos = Vector2.zero;
			Vector2 bottomRightPos = Vector2.zero;
			if (gradientMode == GradientMode.GlobalFullRect)
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

			for (int i = 0; i < m_Verts.Count; i++)
            {
				UIVertex uiVertex = m_Verts[i];

				if (gradientDirection == GradientDirection.Vertical)
				{
					Color newColor = Color.Lerp(firstColor, secondColor, (topLeftPos.y - uiVertex.position.y) / overallHeight);
					uiVertex.color = CalculateColor(uiVertex.color, newColor, colorMode);
				}
				else
				{
					Color newColor = Color.Lerp(firstColor, secondColor, (uiVertex.position.x - topLeftPos.x) / overallWidth);
					uiVertex.color = CalculateColor(uiVertex.color, newColor, colorMode);
				}
				if (useGraphicAlpha)
				{
					uiVertex.color.a = (byte)((uiVertex.color.a * m_Verts[i].color.a) / 255);
				}

				m_Verts[i] = uiVertex;
			}
		}
        else
        {
			for (int i = 0; i < m_Verts.Count; i++)
            {
				UIVertex uiVertex = m_Verts[i];

				if (gradientDirection == GradientDirection.Vertical)
				{
					if ((i % 6 == 0) || (i % 6 == 1) || (i % 6 == 5))
					{
						Color newColor = firstColor;
						uiVertex.color = CalculateColor(uiVertex.color, newColor, colorMode);
					}
					else
					{
						Color newColor = secondColor;
						uiVertex.color = CalculateColor(uiVertex.color, newColor, colorMode);
					}
				}
				else
				{
					if ((i % 6 == 0) || (i % 6 == 4) || (i % 6 == 5))
					{
						Color newColor = firstColor;
						uiVertex.color = CalculateColor(uiVertex.color, newColor, colorMode);
					}
					else
					{
						Color newColor = secondColor;
						uiVertex.color = CalculateColor(uiVertex.color, newColor, colorMode);
					}
				}
				if (useGraphicAlpha)
				{
					uiVertex.color.a = (byte)((uiVertex.color.a * m_Verts[i].color.a) / 255);
				}

				m_Verts[i] = uiVertex;
			}
		}

        vh.Clear();
        vh.AddUIVertexTriangleStream(m_Verts);
	}

	private Color CalculateColor (Color initialColor, Color newColor, ColorMode colorMode)
	{
		if (colorMode == ColorMode.Override)
		{
			return newColor;
		}
		else if (colorMode == ColorMode.Additive)
		{
			return initialColor + newColor;
		}
		else if (colorMode == ColorMode.Multiply)
		{
			return initialColor * newColor;
		}
		return newColor;
	}
}