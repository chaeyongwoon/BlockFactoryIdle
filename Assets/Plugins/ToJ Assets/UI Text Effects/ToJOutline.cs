using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/ToJ Outline", 15)]
public class ToJOutline : Shadow
{
	private List<UIVertex> m_Verts = new List<UIVertex>();

	protected ToJOutline ()
    {}

    public override void ModifyMesh (VertexHelper vh)
    {
        if (!IsActive())
			return;

		vh.GetUIVertexStream(m_Verts);

		int initialVertexCount = m_Verts.Count;

        var neededCpacity = m_Verts.Count * 5;
        if (m_Verts.Capacity < neededCpacity)
            m_Verts.Capacity = neededCpacity;

        var start = 0;
        var end = m_Verts.Count;
        ApplyShadowZeroAlloc(m_Verts, effectColor, start, m_Verts.Count, effectDistance.x, effectDistance.y);

        start = end;
        end = m_Verts.Count;
        ApplyShadowZeroAlloc(m_Verts, effectColor, start, m_Verts.Count, effectDistance.x, -effectDistance.y);

        start = end;
        end = m_Verts.Count;
        ApplyShadowZeroAlloc(m_Verts, effectColor, start, m_Verts.Count, -effectDistance.x, effectDistance.y);

        start = end;
        end = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, effectColor, start, m_Verts.Count, -effectDistance.x, -effectDistance.y);


		Text textComponent = GetComponent<Text>();
		if ((textComponent != null) && (textComponent.material.shader == Shader.Find("Text Effects/Fancy Text")))
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
