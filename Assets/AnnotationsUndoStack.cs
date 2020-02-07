using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnnotationsUndoStack
{
    private readonly List<GameObject> objects = new List<GameObject>();

    public bool CanUndo { get { return objects.Any(); } }

    public void AddObject(GameObject obj)
    {
        objects.Add(obj);
    }

    public void Undo()
    {
        int index = objects.Count - 1;
        GameObject obj = objects[index];
        objects.RemoveAt(index);
        GameObject.Destroy(obj);
    }

    public void DeleteAll()
    {
        GameObject[] toDelete = objects.ToArray();
        objects.Clear();
        for (int i = 0; i < toDelete.Length; i++)
        {
            GameObject.Destroy(toDelete[i]);
        }
    }
}