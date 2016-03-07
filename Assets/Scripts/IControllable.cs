using UnityEngine;
using System.Collections;

public interface IControllable
{
    void ReceiveControlFrom(IControllable oldController);

    void TransferControlTo(IControllable newController);

    Transform getCameraAnchor();
}
