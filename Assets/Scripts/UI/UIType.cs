using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI种类，显示方式
/// </summary>
public class UIType 
{
    //是否清除栈
    public bool isClear;
    public UIFormType uiFormType = UIFormType.Normal;
    public UIFormShowMode uIFormShowMode = UIFormShowMode.Normal;

}

public enum UIFormType
{
    //正常
    Normal,
    //弹窗
    PopUp,
    //固定
    Fixed
}

public enum UIFormShowMode
{
    //正常
    Normal,
    //弹窗类型
    Reverse,
    //隐藏其他
    HideOther,
}