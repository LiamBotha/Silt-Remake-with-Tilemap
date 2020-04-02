using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    private float leftWidth = 0.4f;
    private float rightWidth = 0.4f;
    private float bottomWidth = 0.4f;

    private float centerXOffset;
    private float centerYOffset;

    [SerializeField] float leftWallDist = 1;
    [SerializeField] float rightWallDist = 1;
    [SerializeField] float floorDist = 1;
    [SerializeField] float xWidth = 1;
    [SerializeField] float yWidth = 1;

    [SerializeField] LayerMask groundLayer = 1;

    public bool OnGround { get; private set; } = false;
    public bool OnLeftWall { get; private set; } = false;
    public bool OnRightWall { get; private set; } = false;

    public bool OnAnyWall { get; private set; } = false;

    // Start is called before the first frame update
    void Awake()
    {
        var bounds = GetComponent<Collider2D>().bounds;
        var offset = GetComponent<Collider2D>().offset;

        leftWidth = bounds.size.y / 1.2f;
        rightWidth = bounds.size.y / 1.2f;
        bottomWidth = bounds.size.x / 1.3f;

        centerXOffset = offset.x; 
        centerYOffset = offset.y; 

        //Get Collider and playerSize for raycasts
        //leftWidth = GetComponent<BoxCollider2D>().size.y / 1.1f;
        //rightWidth = GetComponent<BoxCollider2D>().size.y / 1.1f;
        //bottomWidth = GetComponent<BoxCollider2D>().size.x / 1.2f;

        //centerXOffset = GetComponent<BoxCollider2D>().offset.x;
        //centerYOffset = GetComponent<BoxCollider2D>().offset.y;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if overlapbox hits left or right side
        Vector2 rightPos = (Vector2)transform.position + new Vector2(xWidth + centerXOffset, centerYOffset);
        Vector2 leftPos = (Vector2)transform.position + new Vector2(-xWidth + centerXOffset, centerYOffset);
        Vector2 bottomPos = (Vector2)transform.position + new Vector2(centerXOffset, yWidth + centerYOffset);

        OnRightWall = Physics2D.OverlapBox(rightPos, new Vector2(rightWallDist, rightWidth), 0, groundLayer);
        OnLeftWall = Physics2D.OverlapBox(leftPos, new Vector2(leftWallDist, leftWidth), 0, groundLayer);
        OnGround = Physics2D.OverlapBox(bottomPos, new Vector2(bottomWidth, floorDist), 0, groundLayer);

        OnAnyWall = OnRightWall || OnLeftWall;
    }

    private void OnDrawGizmos()
    {
        Vector2 rightPos = (Vector2)transform.position + new Vector2(xWidth + centerXOffset, -0.06f);
        Vector2 leftPos = (Vector2)transform.position + new Vector2(-xWidth + centerXOffset, -0.06f);
        Vector2 bottomPos = (Vector2)transform.position + new Vector2(centerXOffset, yWidth + centerYOffset);

        ExtDebug.DrawBox(rightPos, new Vector2(rightWallDist / 2, rightWidth / 2), Quaternion.identity, Color.blue);
        ExtDebug.DrawBox(leftPos, new Vector2(leftWallDist / 2, leftWidth / 2), Quaternion.identity, Color.green);
        ExtDebug.DrawBox(bottomPos, new Vector2(bottomWidth / 2, floorDist / 2), Quaternion.identity, Color.cyan);

        //UnityEditor.Handles.Label((Vector2)transform.position + Vector2.up * 0.7f, OnAnyWall ? "On Wall" : "Not On Wall");
        //UnityEditor.Handles.Label((Vector2)transform.position - new Vector2(centerXOffset, centerYOffset), OnGround ? "On Ground" : "Not On Ground");
    }
}
