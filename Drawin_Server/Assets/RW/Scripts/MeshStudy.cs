/*
 * Copyright (c) 2019 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//[ExecuteInEditMode]
public class MeshStudy : MonoBehaviour
{
    Mesh oMesh;
    Mesh cMesh;
    MeshFilter oMeshFilter;
    int[] triangles;

    //[HideInInspector]
    public Vector3[] vertices;

    [HideInInspector]
    public bool isCloned = false;

    // For Editor
    public float radius = 0.2f;
    public float pull = 0.3f;
    public float handleSize = 0.03f;
    public List<int>[] connectedVertices;
    public List<Vector3[]> allTriangleList;
    public bool moveVertexPoint = true;

    //
    public int planeId;
    public Vector3[] planeVertex = new Vector3[4];
    public bool needUpdate = false;
    void Start()
    {
        InitMesh();
        planeVertex[0] = new Vector3(.5f, .5f, .5f);
        planeVertex[1] = new Vector3(.5f, .5f, -.5f);
        planeVertex[2] = new Vector3(.5f, -.5f, -.5f);
        planeVertex[3] = new Vector3(.5f, -.5f, .5f);

        if (PlayerPrefs.HasKey(planeId + "v0y") && PlayerPrefs.HasKey(planeId + "v0z")
            && PlayerPrefs.HasKey(planeId + "v1y") && PlayerPrefs.HasKey(planeId + "v1z")
            && PlayerPrefs.HasKey(planeId + "v2y") && PlayerPrefs.HasKey(planeId + "v2z")
            && PlayerPrefs.HasKey(planeId + "v3y") && PlayerPrefs.HasKey(planeId + "v3z"))
        {
            planeVertex[0] = new Vector3(.5f, PlayerPrefs.GetFloat(planeId + "v0y"), PlayerPrefs.GetFloat(planeId + "v0z"));
            planeVertex[1] = new Vector3(.5f, PlayerPrefs.GetFloat(planeId + "v1y"), PlayerPrefs.GetFloat(planeId + "v1z"));
            planeVertex[2] = new Vector3(.5f, PlayerPrefs.GetFloat(planeId + "v2y"), PlayerPrefs.GetFloat(planeId + "v2z"));
            planeVertex[3] = new Vector3(.5f, PlayerPrefs.GetFloat(planeId + "v3y"), PlayerPrefs.GetFloat(planeId + "v3z"));
        }
        needUpdate = true;  
    }

    void Update()
    {
        
       if(needUpdate)
        {
            RestVertex(0);
            RestVertex(1);
            RestVertex(2);
            RestVertex(3);
            PlayerPrefs.SetFloat(planeId + "v0y", planeVertex[0].y);
            PlayerPrefs.SetFloat(planeId + "v0z", planeVertex[0].z);
            PlayerPrefs.SetFloat(planeId + "v1y", planeVertex[1].y);
            PlayerPrefs.SetFloat(planeId + "v1z", planeVertex[1].z);
            PlayerPrefs.SetFloat(planeId + "v2y", planeVertex[2].y);
            PlayerPrefs.SetFloat(planeId + "v2z", planeVertex[2].z);
            PlayerPrefs.SetFloat(planeId + "v3y", planeVertex[3].y);
            PlayerPrefs.SetFloat(planeId + "v3z", planeVertex[3].z);
            needUpdate = false;
        }
    }

    public void InitMesh()
    {
        oMeshFilter = GetComponent<MeshFilter>();
        oMesh = oMeshFilter.sharedMesh;

        cMesh = new Mesh();
        cMesh.name = "clone";
        cMesh.vertices = oMesh.vertices;
        cMesh.triangles = oMesh.triangles;
        cMesh.normals = oMesh.normals;
        cMesh.uv = oMesh.uv;
        cMesh.RecalculateNormals();
        oMeshFilter.mesh = cMesh;

        vertices = cMesh.vertices;
        triangles = cMesh.triangles;
        isCloned = true;
        Debug.Log("Init & Cloned");
    }

    public void Reset()
    {
        if (cMesh != null && oMesh != null)
        {
            cMesh.vertices = oMesh.vertices;
            cMesh.triangles = oMesh.triangles;
            cMesh.normals = oMesh.normals;
            cMesh.uv = oMesh.uv;
            oMeshFilter.mesh = cMesh;
            // update local vars..
            vertices = cMesh.vertices;
            triangles = cMesh.triangles;
        }
    }

    public void DoAction(int index, Vector3 localPos)
    {
        // specify methods here
        // PullOneVertex (index, localPos);
        PullSimilarVertices(index, localPos);
    }

    // returns List of int that is related to the targetPt.
    private List<int> FindRelatedVertices(Vector3 targetPt, bool findConnected)
    {
        // list of int
        List<int> relatedVertices = new List<int>();

        int idx = 0;
        Vector3 pos;

        // loop through triangle array of indices
        for (int t = 0; t < triangles.Length; t++)
        {
            // current idx return from tris
            idx = triangles[t];
            // current pos of the vertex
            pos = vertices[idx];
            // if current pos is same as targetPt
            if (pos == targetPt)
            {
                // add to list
                relatedVertices.Add(idx);
                // if find connected vertices
                if (findConnected)
                {
                    // min 
                    // - prevent running out of count
                    if (t == 0)
                    {
                        relatedVertices.Add(triangles[t + 1]);
                    }
                    // max 
                    // - prevent runnign out of count
                    if (t == triangles.Length - 1)
                    {
                        relatedVertices.Add(triangles[t - 1]);
                    }
                    // between 1 ~ max-1 
                    // - add idx from triangles before t and after t 
                    if (t > 0 && t < triangles.Length - 1)
                    {
                        relatedVertices.Add(triangles[t - 1]);
                        relatedVertices.Add(triangles[t + 1]);
                    }
                }
            }
        }
        // return compiled list of int..
        return relatedVertices;
    }

    // Pulling only one vertex pt, results in broken mesh.
    private void PullOneVertex(int index, Vector3 newPos)
    {
        vertices[index] = newPos;
        cMesh.vertices = vertices;
        cMesh.RecalculateNormals();
    }

    private void PullSimilarVertices(int index, Vector3 newPos)
    {
        Vector3 targetVertexPos = vertices[index];
        List<int> relatedVertices = FindRelatedVertices(targetVertexPos, false);
        foreach (int i in relatedVertices)
        {
            vertices[i] = newPos;
        }
        cMesh.vertices = vertices;
        cMesh.RecalculateNormals();
    }

    // To test Reset function
    public void EditMesh()
    {
        vertices[2] = new Vector3(2, 3, 4);
        vertices[3] = new Vector3(1, 2, 4);
        cMesh.vertices = vertices;
        cMesh.RecalculateNormals();
    }

    public void MoveVertex(int idx, Vector3 vec)
    {
        planeVertex[idx] = planeVertex[idx] + vec;
        needUpdate = true;
    }

    public void ApplyMesh()
    {
        cMesh.vertices = vertices;
        cMesh.RecalculateNormals();
    }

    public void RestVertex(int idx)
    {
        //Debug.Log("RestVertex: " + idx);
        switch(idx)
        {
            case 0:
                vertices[2] = planeVertex[0];
                vertices[8] = planeVertex[0];
                vertices[22] = planeVertex[0];
                ApplyMesh();
                break;
            case 1:
                vertices[4] = planeVertex[1];
                vertices[10] = planeVertex[1];
                vertices[21] = planeVertex[1];
                ApplyMesh();
                break;
            case 2:
                vertices[6] = planeVertex[2];
                vertices[12] = planeVertex[2];
                vertices[20] = planeVertex[2];
                ApplyMesh();
                break;
            case 3:
                vertices[0] = planeVertex[3];
                vertices[13] = planeVertex[3];
                vertices[23] = planeVertex[3];
                ApplyMesh();
                break;
            default:
                break;
        }
    }
}