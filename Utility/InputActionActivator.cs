using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class InputActionActivator : OnScreenControl
{
    private bool  inProgress     = false;
    private int?   performedFrame = null;

    public void TriggerInputAction(InputActionReference actionRef)
    {
        if (inProgress || (performedFrame.HasValue && Time.frameCount == performedFrame.Value))
        {
            return;
        }
        
        if (actionRef == null)
        {
            Debug.LogError("InputActionActivator: TriggerInputAction: Action is null, aborting, " + gameObject.name);
            return;
        }

        if (string.IsNullOrEmpty(controlPath))
        {
            var asset = actionRef.asset;
            if (asset == null)
            {
                Debug.LogError("InputActionActivator: TriggerInputAction: asset inside action is null, aborting, " + gameObject.name);
                return;
            }

            var bindings = asset.bindings;
            if (bindings == null || !bindings.Any())
            {
                Debug.LogError("InputActionActivator: TriggerInputAction: Action has no bindings, aborting, " + gameObject.name);
                return;
            }
        
            controlPath = bindings.First().effectivePath;
            if (controlPath == null)
            {
                Debug.LogError("InputActionActivator: TriggerInputAction: Failed to set Control Path, aborting, " + gameObject.name);
                return;
            }
        }

        InputControl inputControl = InputSystem.FindControl(controlPath);
        if (inputControl == null)
        {
            Debug.LogError($"InputActionActivator: TriggerInputAction: could not find inputControl for ${controlPath}, aborting, " + gameObject.name);
            return;
        }
        
        switch (inputControl.valueType)
        {
            case not null when inputControl.valueType == typeof(float):
                SendValueToControl(1.0f);
                Debug.Log("Performed " + Time.frameCount);
                StartCoroutine(DisableValueAfterFrame(0.0f));
                break;

            case not null when inputControl.valueType == typeof(double):
                SendValueToControl(1.0);
                Debug.Log("Performed2");
                StartCoroutine(DisableValueAfterFrame(0.0));
                break;

            case not null when inputControl.valueType == typeof(int):
                SendValueToControl(1);
                Debug.Log("Performed3");
                StartCoroutine(DisableValueAfterFrame(0));
                break;

            case not null when inputControl.valueType == typeof(bool):
                SendValueToControl(true);
                Debug.Log("Performed4");
                StartCoroutine(DisableValueAfterFrame(false));
                break;

            default:
                Debug.LogWarning($"InputActionActivator: TriggerInputAction: Value type of button has not been mapped or is invalid, valuetype: ${inputControl.valueType}, " + gameObject.name);
                break;
        }
        
        inProgress     = true;
    }
    


    IEnumerator DisableValueAfterFrame<TValue>(TValue value) where TValue : struct
    {
        //this just skips the current frane
        yield return null;
        Debug.Log("UnPerformed " + Time.frameCount);
        SentDefaultValueToControl();
        inProgress     = false;
        performedFrame = Time.frameCount;
    }


    protected override string controlPathInternal { get; set; }
}