using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerMovement))]
public class RaycastCollisions: MonoBehaviour {

  private Transform characterTransform;
  private CapsuleCollider capsuleCollider;
  private float capsuleHeight;
  private float capsuleRadius;
  private float angleIncrement;

  private float skinWidth = .0008f;

  public LayerMask collisionMask;

  public int numRays = 10;

  private Vector3 scale;
  public CollisionInfo collisions;

  void Start() {
    capsuleCollider = transform.GetComponent < CapsuleCollider > ();
    characterTransform = this.transform;
    capsuleHeight = capsuleCollider.height;
    capsuleRadius = capsuleCollider.radius;
    angleIncrement = 360 / numRays;
    scale = this.transform.localScale;
  }

  void Move() {

  }

  public void VerticalCollisions(ref Vector3 moveAmount) {
    collisions.Reset();
    float directionY = Mathf.Sign(moveAmount.y);
    float rayLength = Mathf.Abs(moveAmount.y) * Time.deltaTime;

    float y = this.transform.position.y + (capsuleHeight / 2 * directionY) * scale.y;
    //the Y values of the raycast origin
    for (int i = 0; i < numRays; i++) //get x and z coordinates
    {
      float x = this.transform.position.x + Mathf.Cos(i * angleIncrement) * capsuleRadius * scale.x;
      float z = this.transform.position.z + Mathf.Sin(i * angleIncrement) * capsuleRadius * scale.z;
      Vector3 rayOrigin = new Vector3(x, y, z);
      RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.up * directionY, rayLength, collisionMask);

      //iterate through the hits array.  If hit, set moveAmount.y to (hitdistance - skinwidth) * directionY and then set raylenth to the hitDistance.
      Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.red);

      float closestHit = rayLength;
      for (int j = 0; j < hits.Length; j++) 
      {
        collisions.below = true;
        if (hits[j].distance < closestHit) 
        {
          closestHit = hits[j].distance;
        }
      }

      if (closestHit < rayLength) 
      {
        moveAmount.y = (closestHit - skinWidth) * directionY;
        rayLength = closestHit;
      }
    }
  }

  public struct CollisionInfo {
    public bool above, below;

    public bool climbingSlope;
    public bool descendingSlope;
    public float slopeAngle, slopeAngleOld;
    public Vector3 moveAmountOld;
    public int faceDir;
    public bool fallingThroughPlatform;

    public void Reset() {
      above = below = false;
      climbingSlope = false;
      descendingSlope = false;

      slopeAngleOld = slopeAngle;
      slopeAngle = 0;
    }
  }

}