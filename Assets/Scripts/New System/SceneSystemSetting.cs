using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    [CreateAssetMenu(fileName ="SceneSystemSetting",menuName = "System configuration/SceneSystem Setting")]
    public class SceneSystemSetting : ScriptableObject
    {
        public string loadingScene;
    }

}