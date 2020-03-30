using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradable
{
    string ModuleName { get; }
    void UpgradeCount(float n);
    bool MaxedOut { get; }
}
