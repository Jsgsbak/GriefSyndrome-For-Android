using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicEvents : MonoBehaviour
{
    public bool EnableUpdate;
    public bool EnableSlowUpdate;
    public bool EnableFakeLateUpdate;

    public UnityEvent OnAwake;
    public UnityEvent OnStart;
    public UnityEvent OnUpdate;
    public UnityEvent OnSlowUpdate;
    public UnityEvent OnFakeLateUpdate;
    public UnityEvent OnDisabled;
    public UnityEvent OnEnabled;
    public UnityEvent OnDestroy;

    private void Awake()
    {
        OnAwake.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        OnStart.Invoke();
    }

    // Update is called once per frame
    private void OnEnable()
    {

        OnEnabled.Invoke();
    }
}
