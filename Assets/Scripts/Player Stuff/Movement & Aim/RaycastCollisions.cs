using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof (PlayerMovement))]
public class RaycastCollisions: MonoBehaviour {

  /* #region General Variables */
  private Transform characterTransform;
  private CapsuleCollider capsuleCollider;
  private Vector3 scale;
  public CollisionInfo collisions;
  public LayerMask collisionMask;
  private float capsuleHeight;
  private float capsuleRadius;
  private float angleIncrement;
  private float skinWidth = .0008f;

  /* #endregion */

  /* #region Vertical Check variables */
  public int numVerticalRays = 10;

  /* #endregion */

  /* #region Horizontal Check variables */
  public int HorizontalRayRows = 6;
  public int HorizontalRaysPerRow = 18;
  private float HorizontalRaySpacing;

  private float HorizontalAngleIncrement;
  /* #endregion */

  void Start() {
    capsuleCollider = transform.GetComponent < CapsuleCollider > ();
    characterTransform = this.transform;
    capsuleHeight = capsuleCollider.height;
    capsuleRadius = capsuleCollider.radius;
    angleIncrement = 360 / numVerticalRays;
    HorizontalAngleIncrement = 360 / HorizontalRaysPerRow;
    scale = this.transform.localScale;
    HorizontalRaySpacing = capsuleRadius * scale.y / HorizontalRayRows;
  }
  
  public void Move(ref Vector3 yMove, ref Vector3 xMove, float xSpeed, CharacterController controller)
  {

    //TO DO: REDO THE RAYCAST SYSTEM WITH SPHERECAST.  IDK HOW IT WORKS YET BUT U GOTTA LEARN.
    collisions.Reset();
    //HorizontalCollisions(ref xMove, xSpeed);
    controller.Move(xMove * xSpeed * Time.deltaTime); //Horizontal Movement
    if(yMove.y != 0)
    {
    //VerticalCollisions(ref yMove);
    controller.Move(yMove * Time.deltaTime); //Vertical Movement
    }
  }
  public void VerticalCollisions(ref Vector3 moveAmount) {
    float directionY = Mathf.Sign(moveAmount.y);
    float rayLength = Mathf.Abs(moveAmount.y) * Time.deltaTime;

    float y = this.transform.position.y + (((capsuleHeight / 2)) * directionY) * scale.y;
    //the Y values of the raycast origin
    for (int i = 0; i < numVerticalRays; i++) //get x and z coordinates
    {
      float x = this.transform.position.x + Mathf.Cos(i * angleIncrement) * (capsuleRadius + skinWidth) * scale.x;
      float z = this.transform.position.z + Mathf.Sin(i * angleIncrement) * (capsuleRadius + skinWidth) * scale.z;
      Vector3 rayOrigin = new Vector3(x, y, z);
      RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.up * directionY, rayLength, collisionMask);

      //iterate through the hits array.  If hit, set moveAmount.y to (hitdistance - skinwidth) * directionY and then set raylenth to the hitDistance.
      Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.red);

      float closestHit = rayLength;
      for (int j = 0; j < hits.Length; j++) 
      {
        Debug.Log("Hit Something : " + hits[j]);
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

  public void HorizontalCollisions(ref Vector3 moveAmount, float speed)
  {
    float rayLength = Mathf.Abs(moveAmount.magnitude) + skinWidth;

    float yTopOrigin = transform.position.y + (capsuleHeight/2 - capsuleRadius) * scale.y;
    
    for(int i = 0; i < HorizontalRayRows; i++)
    {
      float yTop = yTopOrigin + i * HorizontalRaySpacing;
      float yBottom = yTop - (capsuleHeight - capsuleRadius) * scale.y;

      for(int j = 0; j < HorizontalRaysPerRow; j++)
      {
        float x = this.transform.position.x + Mathf.Cos(j * angleIncrement) * (capsuleRadius) * scale.x;
        float z = this.transform.position.z + Mathf.Sin(j * angleIncrement) * (capsuleRadius) * scale.z;
        Vector3 rayOriginTop = new Vector3(x, yTop, z);
        Vector3 rayOriginBottom = new Vector3(x, yBottom, z);

        RaycastHit[] hitsTop = Physics.RaycastAll(rayOriginTop, moveAmount, rayLength, collisionMask);
        RaycastHit[] hitsBottom = Physics.RaycastAll(rayOriginBottom, moveAmount, rayLength, collisionMask);

        RaycastHit[] allHits = hitsTop.Concat(hitsBottom).ToArray<RaycastHit>();

        float closestHit = rayLength;
        Vector3 closestPoint = new Vector3();
        for(int k = 0; k < allHits.Length; k++)
        {
          if (allHits[k].distance < closestHit) 
          {
            closestHit = allHits[k].distance;
            closestPoint = allHits[k].point;
          }
        }

        if (closestHit < rayLength) 
        {
          moveAmount.x = (closestPoint.x - rayOriginTop.x - skinWidth);
          moveAmount.z = (closestPoint.z - rayOriginTop.z - skinWidth);
          rayLength = closestHit;
        }
      }
    }
  }
  public struct CollisionInfo {
    public bool above, below;
    public bool climbingSlope;
    public bool descendingSlope;

    public void Reset() {
      above = below = false;
      climbingSlope = false;
      descendingSlope = false;
    }
  }

}