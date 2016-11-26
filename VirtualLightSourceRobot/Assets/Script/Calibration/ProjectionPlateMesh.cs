using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ProjectionPlateMesh : MonoBehaviour {

    //public
    public Vector3 dst_topLeft;
    public Vector3 dst_bottomLeft;
    public Vector3 dst_bottomRight;
    public Vector3 dst_topRight;

    public Camera ProjectionCamera;

    public Vector3 src_topLeft;
    public Vector3 src_bottomLeft;
    public Vector3 src_bottomRight;
    public Vector3 src_topRight;

    public Vector3 topLeftofViewPort;
    public Vector3 bottomLeftofViewPort;
    public Vector3 bottomRightofViewPort;
    public Vector3 topRightofViewPort;

    public int Row = 10;
    public int Col = 10;

    private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector2[] _UV;
    private int[] _Triangles;


    //public GameObject PointObjectDst;
    //public GameObject PointObjectSrc;
    public Camera CameraSrc;
    public List<GameObject> PointObjectList;


    // Use this for initialization
    void Start () {
        //メッシュを作る
        this.CreateMesh(this.Col, this.Row);

    }
	
	// Update is called once per frame
	void Update () {
        //頂点変更

        //this.dst_topLeft = ProjectionCamera.ScreenToWorldPoint(topLeftofViewPort);
        //this.dst_bottomLeft = ProjectionCamera.ScreenToWorldPoint(bottomLeftofViewPort);
        //this.dst_bottomRight = ProjectionCamera.ScreenToWorldPoint(bottomRightofViewPort);
        //this.dst_topRight = ProjectionCamera.ScreenToWorldPoint(topRightofViewPort);

        //this.dst_topLeft = this.ConvertUVPointToScreenPoint(topLeftofViewPort);
        //this.dst_bottomLeft = this.ConvertUVPointToScreenPoint(bottomLeftofViewPort);
        //this.dst_bottomRight = this.ConvertUVPointToScreenPoint(bottomRightofViewPort);
        //this.dst_topRight = this.ConvertUVPointToScreenPoint(topRightofViewPort);

        this.dst_topLeft = this.PointObjectList[0].transform.position;
        this.dst_bottomLeft = this.PointObjectList[1].transform.position;
        this.dst_bottomRight = this.PointObjectList[2].transform.position;
        this.dst_topRight = this.PointObjectList[3].transform.position;

        //this.PointObjectList[0].transform.position = this.dst_topLeft;
        //this.PointObjectList[1].transform.position = this.dst_bottomLeft;
        //this.PointObjectList[2].transform.position = this.dst_bottomRight;
        //this.PointObjectList[3].transform.position = this.dst_topRight;

        //this.PointObjectList[4].transform.position = CameraSrc.ScreenToWorldPoint(this.ConvertUVPointToScreenPoint(src_topLeft));
        //this.PointObjectList[5].transform.position = CameraSrc.ScreenToWorldPoint(this.ConvertUVPointToScreenPoint(src_bottomLeft));
        //this.PointObjectList[6].transform.position = CameraSrc.ScreenToWorldPoint(this.ConvertUVPointToScreenPoint(src_bottomRight));
        //this.PointObjectList[7].transform.position = CameraSrc.ScreenToWorldPoint(this.ConvertUVPointToScreenPoint(src_topRight));


        //頂点に変更があったらメッシュ再構築
        this.RefreshData();

    }

    private Vector3 ConvertUVPointToScreenPoint(Vector3 point)
    {
        return new Vector3(point.x * Screen.width, point.y * Screen.height, point.z);
    }

    void CreateMesh(int width, int height)
    {
        int localWidth = width + 1;
        int localHeight = height + 1;

        _Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _Mesh;

        _Vertices = new Vector3[localWidth * localHeight];
        _UV = new Vector2[localWidth * localHeight];
        _Triangles = new int[6 * ((localWidth - 1) * (localHeight - 1))];

        int triangleIndex = 0;
        for (int y = 0; y < localHeight; y++)
        {
            for (int x = 0; x < localWidth; x++)
            {
                int index = (y * localWidth) + x;

                _Vertices[index] = new Vector3(x, -y, 0);
                _UV[index] = new Vector2(((float)x / (float)localWidth), -((float)y / (float)localHeight));

                // Skip the last row/col
                if (x != (localWidth - 1) && y != (localHeight - 1))
                {
                    int topLeft = index;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + localWidth;
                    int bottomRight = bottomLeft + 1;

                    _Triangles[triangleIndex++] = topLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomRight;
                }
            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals();
        _Mesh.RecalculateBounds();
    }

    private void RefreshData()
    {
        int width = this.Col + 1;
        int height = this.Row + 1;

        Vector3 downVec_R = (this.dst_bottomRight - this.dst_topRight) / (this.Row);
        Vector3 downVec_L = (this.dst_bottomLeft - this.dst_topLeft) / (this.Row);

        Vector3 UV_downVec_R = (this.src_bottomRight - this.src_topRight);
        Vector3 UV_downVec_L = (this.src_bottomLeft - this.src_topLeft);

        Vector3 downVec_L_e = downVec_L / downVec_L.magnitude;

        for (int y = 0; y < height; y++)
        {
            Vector3 rightVec = ((this.dst_topRight + downVec_R * y) - (this.dst_topLeft + downVec_L * y)) / (this.Col);
            Vector3 rightVec_e = rightVec / rightVec.magnitude;

            Vector3 UV_rightVec = ((this.src_topRight + UV_downVec_R * y / this.Row) - (this.src_topLeft + UV_downVec_L * y / this.Row));



            for (int x = 0; x < width; x++)
            {
                //各頂点の座標算出
                int index = y * width + x;

                Vector3 pos = new Vector3();
                pos = this.dst_topLeft + downVec_L * y + rightVec * x - this.transform.position ;
                _Vertices[index] = pos;

                _UV[index] = (this.src_topLeft + UV_downVec_L * y / this.Col + UV_rightVec * x / this.Row);


            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals();
        _Mesh.RecalculateBounds();
    }
}
