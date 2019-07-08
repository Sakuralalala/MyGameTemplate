using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    [CreateAssetMenu(fileName ="TestSystemSetting",menuName = "System configuration/TestSystem Setting")]
    public class TestSystemSetting : ScriptableObject
    {
        public string testName;
    }
}
