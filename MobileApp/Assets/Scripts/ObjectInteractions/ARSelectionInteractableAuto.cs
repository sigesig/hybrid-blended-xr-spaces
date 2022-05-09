using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ARSelectionInteractableAuto : ARSelectionInteractable
{
   public bool GestureSelected { get; private set; } = true;

   public override bool IsSelectableBy(IXRSelectInteractor interactor)
   {
      return true;
   }
}
