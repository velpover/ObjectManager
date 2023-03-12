using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PersistableObject : MonoBehaviour
{
    public virtual void Save(GameDataWritter writter)
    {
        writter.Write(transform.localPosition);
        writter.Write(transform.localRotation);
        writter.Write(transform.localScale);
    }

    public virtual void Load(GameDataReader reader)
    {
        transform.localPosition = reader.Vector3Reader();
        transform.localRotation = reader.QuaternionReader();
        transform.localScale = reader.Vector3Reader();
    }
}
