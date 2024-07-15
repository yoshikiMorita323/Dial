using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCriate : MonoBehaviour
{
    public enum MeshTipe
    {
        Line,

        Triangle,

        Sikaku
    }

    public MeshTipe mesuTipe = MeshTipe.Triangle;

    private MeshTipe catchMesutipe = MeshTipe.Triangle;

    private bool change = false;
    // Start is called before the first frame update
    void Start()
    {
        catchMesutipe = mesuTipe;
        //var mesh = new Mesh();

        switch (mesuTipe)
        {
            case MeshTipe.Line:
                LineCriate();
                break;
            case MeshTipe.Triangle:
                TriangleCriate();
                break;
            case MeshTipe.Sikaku:
                QuadCriate();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //var mesh = new Mesh();

        switch (mesuTipe)
        {
            case MeshTipe.Line:
                if(mesuTipe != catchMesutipe)
                {
                    LineCriate();
                    catchMesutipe = mesuTipe;
                    change = true;
                }
                break;
            case MeshTipe.Triangle:
                if (mesuTipe != catchMesutipe)
                {
                    TriangleCriate();
                    catchMesutipe = mesuTipe;
                    change = true;
                }

                break;
            case MeshTipe.Sikaku:
                if (mesuTipe != catchMesutipe)
                {
                    QuadCriate();
                    catchMesutipe = mesuTipe;
                    change = true;
                }
                break;
        }
        if(change)
        {
            change = false;
        }

    }

    private void LineCriate()
    {
        var mesh = new Mesh();

        var vertices
            = new[] { new Vector3(0,0,0),
                      new Vector3(1,0,0),
                      new Vector3(0,1,0)};

        mesh.SetVertices(vertices);

        var indices = new[] { 0, 1, 1, 2, 2, 0 };
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        GetComponent<MeshFilter>().mesh = mesh;

    }
    private void TriangleCriate()
    {
        var mesh = new Mesh();

        // ’¸“_‚ð’è‹`
        var vertices
            = new[] { new Vector3(0,0,0),
                      new Vector3(1,0,0),
                      new Vector3(0,1,0),
                      new Vector3(1,1,0)};

        mesh.SetVertices(vertices);

        var indices = new[] { 0, 2, 1 ,
                              2, 3, 1};
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        GetComponent<MeshFilter>().mesh = mesh;
    }
    private void QuadCriate()
    {
        Mesh[] mesh_1 = new Mesh[2];
        MeshFilter[] filter = new MeshFilter[2];
        for (int i = 1; i <= 2; i++)
        {
            filter[i - 1] = GetComponent<MeshFilter>();
            mesh_1[i - 1] = new Mesh();
            var vertices
                = new[] { new Vector3(0.4f,0,0) * (i + 2),
                      new Vector3(0.6f,0,0) * (i + 2),
                      new Vector3(1,1,0) * (i + 2),
                      new Vector3(0,1,0) * (i + 2)
                };

            mesh_1[i - 1].SetVertices(vertices);

            var indices = new[] { 0, 3, 2, 1 };
            mesh_1[i - 1].SetIndices(indices, MeshTopology.Quads, 0);
            filter[i - 1].mesh = mesh_1[i - 1];
        }

    }


}
