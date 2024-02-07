using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public enum BlockType
    {
        Red,
        Yellow,
        Green,
        Blue,
        //Black,
        //White,
        //Purple,
        //Orange
    }
    public BlockType _blockType;

    //public Material[] _colorMats;

    //Renderer _renderer;
    MeshFilter _meshfilter;
    public Mesh[] _blockMeshes;


    public void SetInit(BlockType _type)
    {
        _blockType = _type;

        if (_meshfilter == null) _meshfilter = GetComponent<MeshFilter>();
        _meshfilter.sharedMesh = _blockMeshes[(int)_type];
        //if (_renderer == null) _renderer = GetComponent<Renderer>();
        //_renderer.sharedMaterial = Instantiate(_renderer.sharedMaterial);
        //_renderer.sharedMaterial.SetTextureOffset("_BaseMap", new Vector2(0f, 0.025f * (float)_type));
    }



}
