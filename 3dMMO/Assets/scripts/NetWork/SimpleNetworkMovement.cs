using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNetworkMovement : NetworkBehaviour
{
    [Networked] private Vector3 NetworkedPosition { get; set; }

    //public override void FixedUpdateNetwork()
    //{
    //    if (HasInputAuthority)
    //    {
    //        NetworkedPosition = transform.position;
    //    }
    //    else
    //    {
    //        transform.position = Vector3.Lerp(transform.position, NetworkedPosition, 0.2f);
    //    }
    //}
}
