using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Generics.Pattern.SingletonPattern;
using Misc;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private EventManager _xDialogueEventBus;
    private EventManager _xInteractableEventBus;
    private EventManager _xMapEventBus;
    public EventManager XDialogueEventBus { get => _xDialogueEventBus; }
    public EventManager XInteractableEventBus { get => _xInteractableEventBus; }
    public EventManager XMapEventBus { get => _xMapEventBus; }

    protected override void Awake()
    {
        _xDialogueEventBus = new EventManager();
        _xInteractableEventBus = new EventManager();
        _xMapEventBus = new EventManager();
        base.Awake();
    }
}
