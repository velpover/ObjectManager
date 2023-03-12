using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    Color color;

    int shapeID = int.MinValue;
    public int MaterialId { get; private set; }

    public int ShapeID {
        get 
        {
            return shapeID;
        }
        set 
        {
            if (shapeID == int.MinValue && value != int.MinValue)
            {
                shapeID = value;
            }
        }
    }

    MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material,int materialID)
    {
        meshRenderer.material = material;
        MaterialId = materialID;
    }

    public void SetColor(Color color)
    {   
        this.color = color;
        meshRenderer.material.color = color;
    }

    public override void Save(GameDataWritter writter)
    { 
        base.Save(writter);
        writter.Write(color);
    }
    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }
}
