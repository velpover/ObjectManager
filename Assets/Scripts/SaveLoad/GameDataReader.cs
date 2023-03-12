using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataReader
{
    BinaryReader reader;

    public int Version { get; }
    public GameDataReader(BinaryReader reader,int version)
    {
        this.Version = version;
        this.reader = reader;
    }

    public int ReadInt()
    {
        return reader.ReadInt32();
    }

    public float ReadFloat()
    {
        return reader.ReadSingle();
    }

    public Quaternion QuaternionReader()
    {   
        Quaternion q = new Quaternion();

        q.x = reader.ReadSingle();
        q.y = reader.ReadSingle();
        q.z = reader.ReadSingle();
        q.w = reader.ReadSingle();

        return q;
    }

    public Vector3 Vector3Reader()
    {   
        Vector3 v = new Vector3();

        v.x = reader.ReadSingle();
        v.y = reader.ReadSingle();
        v.z = reader.ReadSingle();
        
        return v;
    }

    public Color ReadColor()
    {
        Color color = new Color();

        color.r = reader.ReadSingle();
        color.g = reader.ReadSingle();
        color.b = reader.ReadSingle();
        color.a = reader.ReadSingle();

        return color;
    }
}
