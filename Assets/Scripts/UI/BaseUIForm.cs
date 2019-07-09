using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有UI的父类
/// </summary>
public class BaseUIForm : MonoBehaviour
{
    public UIType CurrentUIType { get; set; } = new UIType();


    /// <summary>
    /// 显示
    /// </summary>
    public virtual void Display()
    {
        gameObject.SetActive(true);
        if(CurrentUIType.uiFormType == UIFormType.PopUp)
        {
            UIManager.Instance.SetMask(gameObject);
        }
    }
    /// <summary>
    /// 隐藏
    /// </summary>
    public virtual void Hiding()
    {
        gameObject.SetActive(false);
        if (CurrentUIType.uiFormType == UIFormType.PopUp)
        {
            UIManager.Instance.CancelMask();
        }
    }
    /// <summary>
    /// 冰冻
    /// </summary>
    public virtual void Freeze()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 重新显示
    /// </summary>
    public virtual void ReDisplay()
    {
        gameObject.SetActive(true);
        if (CurrentUIType.uiFormType == UIFormType.PopUp)
        {
            UIManager.Instance.SetMask(gameObject);
        }
    }


    //子类方法
    /// <summary>
    /// 打开UI
    /// </summary>
    public void OpenUIForm(string uiName)
    {
        UIManager.Instance.ShowUIForm(uiName);
    }
    /// <summary>
    /// 关闭UI
    /// </summary>
    public void CloseUIForm()
    {
        UIManager.Instance.CloseUIForm(GetType().ToString());
    }

    //-------消息传递------------


}
