using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class helps make copies of one GameObject.
/// For example, you can attach this CloneGenerator component to a GameObject, like a cube.
/// Then in the Unity3D inspector editor, you can change rowCount, direction, direction2, and randomOffset.
/// In the editor and before the game starts, you will see a few "gizmos" showing where the copies will be created.
/// When the game does start, CloneGenerator will make copies of the GameObject it is attached to, in all those places.
/// See the "CloneGeneratorDemo" scene for an example with a cube.
/// You can click on the cube and change the numbers in the "inspector" to the right.
/// </summary>
public class CloneGenerator : MonoBehaviour
{
    public int rowCount = 1;
    public Vector3 direction;
    public Vector3 direction2;
    public Vector3 randomOffset;

    private List<Vector3> GetPoints()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < rowCount; i++)
        {
            if (direction2.sqrMagnitude > 0)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    Vector3 newPoint = new Vector3(direction.x * i + direction2.x * j, direction.y * i + direction2.y * j, direction.z * i + direction2.z * j);
                    points.Add(newPoint + GetRandomOffset());
                }
            }
            else
            {
                Vector3 newPoint =new Vector3(direction.x * i, direction.y * i, direction.z * i);
                points.Add(newPoint + GetRandomOffset());
            }
        }
        points.RemoveAt(0);
        return points;
    }

    private Vector3 GetRandomOffset()
    {
        return new Vector3(Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(-randomOffset.y, randomOffset.y),
            Random.Range(-randomOffset.z, randomOffset.z));
    }

    /// <summary>
    /// Like OnDrawGizmos, this is a special function that Unity3D makes happen once when the game starts.
    /// </summary>
    void Start()
    {
        foreach (Vector3 point in GetPoints())
        {
            GameObject go = Instantiate(gameObject, transform.position + point, transform.rotation) as GameObject;
            go.transform.parent = gameObject.transform.parent;
            Destroy(go.GetComponent<CloneGenerator>());
            go.GetComponent<ChargedParticle>().UpdateColor();
        }
        Destroy(this);
    }

    /// <summary>
    /// In C# you can name your functions whatever you like.
    /// However with C# in Unity3D, some functions have special purposes.
    /// This OnDrawGizmos function happens often, about once every 60 seconds, every time the
    /// Unity3D editor wants to redraw the user interface for the game editor (buttons, lines, graphics).
    /// In this case, this code will draw spheres for the CloneGenerator in 3D space, which represent where
    /// the copied GameObjects will be generated when the game starts.
    /// A "gizmo" is never seen by someone playing your game, it's only ever visible from the Unity3D editor
    /// to help a game developer.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (GetComponent<ChargedParticle>() != null && GetComponent<ChargedParticle>().charge > 0)
            Gizmos.color = Color.green;

        foreach (Vector3 point in GetPoints())
            Gizmos.DrawSphere(gameObject.transform.position + point, 0.2f);

    }
}
