using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    //加载的UI缓存
    private Dictionary<string, BaseUIForm> allUIForms;
    //当前UI缓存
    private Dictionary<string, BaseUIForm> currentUIForms;
    //弹窗栈
    private Stack<BaseUIForm> stackUIForms;

    private Transform uiMask;

    //Canvas主体位置，以及其对应的节点位置
    private Transform canvasTransform;
    private Transform normalTransform;
    private Transform popUpTransform;
    private Transform fixedTransform;

    public static UIManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.Find("MainUI").GetComponent<UIManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        allUIForms = new Dictionary<string, BaseUIForm>();
        currentUIForms = new Dictionary<string, BaseUIForm>();
        stackUIForms = new Stack<BaseUIForm>();
        canvasTransform = GameObject.Find("MainUI").transform;
        normalTransform = canvasTransform.Find("Normal");
        popUpTransform = canvasTransform.Find("PopUp");
        //获取uiMask
        uiMask = popUpTransform.GetChild(0);
        fixedTransform = canvasTransform.Find("Fixed");
    }
    /// <summary>
    /// 加载UI到缓存池中
    /// </summary>
    /// <param name="uiName"></param>
    /// <returns></returns>
    private BaseUIForm LoadUIFormToAllUIFormsCache(string uiName)
    {
        BaseUIForm baseUIForm = null;
        allUIForms.TryGetValue(uiName, out baseUIForm);
        if (baseUIForm == null)
        {
            baseUIForm = Resources.Load<BaseUIForm>("UI/" + uiName).GetComponent<BaseUIForm>();

            switch (baseUIForm.CurrentUIType.uiFormType)
            {
                case UIFormType.Normal:
                    baseUIForm.gameObject.transform.SetParent(normalTransform);
                    break;
                case UIFormType.Fixed:
                    baseUIForm.gameObject.transform.SetParent(fixedTransform);
                    break;
                case UIFormType.PopUp:
                    baseUIForm.gameObject.transform.SetParent(popUpTransform);
                    break;
                default:
                    break;
            }
            baseUIForm.gameObject.SetActive(false);
            allUIForms.Add(uiName, baseUIForm);
        }
        
        return baseUIForm;

    }
    /// <summary>
    /// 加载UI到当前UI缓存池中(Normal)
    /// </summary>
    /// <param name="uiName"></param>
    /// <returns></returns>
    private void LoadUIFormToCurrentUIFormCache(string uiName)
    {
        BaseUIForm baseUIForm = null;
        BaseUIForm baseUIFormInAllCache = null;
        //先在当前UI缓存中查找
        currentUIForms.TryGetValue(uiName,out baseUIForm);
        if (baseUIForm != null)
            return;
        //在UI缓存中查找
        allUIForms.TryGetValue(uiName, out baseUIFormInAllCache);
        if(baseUIFormInAllCache != null)
        {
            currentUIForms.Add(uiName, baseUIFormInAllCache);
            baseUIFormInAllCache.Display();
        }
        else
        {
            Debug.LogError("error:该控件不存在");
        }
       
    }

    /// <summary>
    /// 弹窗入栈(Reverse)
    /// </summary>
    /// <param name="uiName"></param>
    private void PushUIFormToStack(string uiName)
    {
        BaseUIForm baseUIForm;
        if (stackUIForms.Count > 0)
        {
            BaseUIForm topUIForm = stackUIForms.Peek();
            topUIForm.Freeze();
        }
        allUIForms.TryGetValue(uiName,out baseUIForm);
        if(baseUIForm != null)
        {
            stackUIForms.Push(baseUIForm);
            baseUIForm.Display();
        }
        else
        {
            Debug.LogError("error:该控件不存在");
        }

    }
    /// <summary>
    /// 显示UI并隐藏其他UI(HideOther)
    /// </summary>
    /// <param name="uiName"></param>
    private void ShowUIFormAndHideOther(string uiName)
    {
        BaseUIForm baseUIForm;
        BaseUIForm baseUIFormInAllCache;

        currentUIForms.TryGetValue(uiName, out baseUIForm);
        if (currentUIForms != null)
            return;

        allUIForms.TryGetValue(uiName, out baseUIFormInAllCache);
        //隐藏其他所有UI
        foreach(BaseUIForm b in currentUIForms.Values)
        {
            b.Hiding();
        }
        foreach(BaseUIForm b in stackUIForms)
        {
            b.Hiding();
        }

        if(baseUIFormInAllCache != null)
        {
            currentUIForms.Add(uiName, baseUIFormInAllCache);
            baseUIFormInAllCache.Display();
        }
        else
        {
            Debug.LogError("error:该控件不存在");
        }
    }
    /// <summary>
    /// 退出UI(Normal)
    /// </summary>
    /// <param name="uiName"></param>
    private void ExitUIForms(string uiName)
    {
        BaseUIForm baseUIForm = null;
        currentUIForms.TryGetValue(uiName, out baseUIForm);

        if (baseUIForm == null)
            return;

        baseUIForm.Hiding();
        currentUIForms.Remove(uiName);
    }
    /// <summary>
    /// 退出UI(Reverse)
    /// </summary>
    private void Reverse()
    {
        if(stackUIForms.Count >= 2)
        {
            BaseUIForm toPop = stackUIForms.Pop();
            toPop.Hiding();
            BaseUIForm top = stackUIForms.Peek();
            top.ReDisplay();
        }
        else if(stackUIForms.Count == 1)
        {
            BaseUIForm toPop = stackUIForms.Pop();
            toPop.Hiding();
        }
    }
    /// <summary>
    /// 退出UI(HideOther)
    /// </summary>
    /// <param name="uiName"></param>
    private void ExitUIFormsAndShowOther(string uiName)
    {
        BaseUIForm baseUIForm = null;
        currentUIForms.TryGetValue(uiName, out baseUIForm);

        if (baseUIForm == null)
            return;
        baseUIForm.Hiding();
        currentUIForms.Remove(uiName);
        foreach(BaseUIForm b in currentUIForms.Values)
        {
            b.ReDisplay();
        }
        foreach(BaseUIForm b in stackUIForms)
        {
            b.ReDisplay();
        }
    }


    /// <summary>
    /// 显示UI
    /// </summary>
    /// <param name="uiName"></param>
    public void ShowUIForm(string uiName)
    {
        BaseUIForm baseUIForm = null;
        if (uiName == null)
            return;
        baseUIForm = LoadUIFormToAllUIFormsCache(uiName);
        if (baseUIForm == null)
            return;

        if (baseUIForm.CurrentUIType.isClear)
        {
            stackUIForms.Clear();
        }

        switch (baseUIForm.CurrentUIType.uIFormShowMode)
        {
            case UIFormShowMode.Normal:
                LoadUIFormToCurrentUIFormCache(uiName);
                break;
            case UIFormShowMode.Reverse:
                PushUIFormToStack(uiName);
                break;
            case UIFormShowMode.HideOther:
                ShowUIFormAndHideOther(uiName);
                break;
            default:
                break;
        }

    }
    /// <summary>
    /// 关闭UI
    /// </summary>
    /// <param name="uiName"></param>
    public void CloseUIForm(string uiName)
    {
        BaseUIForm baseUIForm = null;
        if (uiName == null)
            return;
        allUIForms.TryGetValue(uiName,out baseUIForm);

        switch (baseUIForm.CurrentUIType.uIFormShowMode)
        {
            case UIFormShowMode.Normal:
                ExitUIForms(uiName);
                break;
            case UIFormShowMode.Reverse:
                Reverse();
                break;
            case UIFormShowMode.HideOther:
                ExitUIFormsAndShowOther(uiName);
                break;
            default:
                break;
        }
        
    }

    /// <summary>
    /// 设置遮罩状态
    /// </summary>
    /// <param name="uiForm"></param>
    public void SetMask(GameObject uiForm)
    {
        //顶层窗口下移
        canvasTransform.SetAsLastSibling();
        //遮罩下移
        uiMask.SetAsLastSibling();
        uiMask.gameObject.SetActive(true);
        //显示窗口下移
        uiForm.transform.SetAsLastSibling();

    }

    public void CancelMask()
    {
        canvasTransform.SetAsFirstSibling();
        if (uiMask.gameObject.activeInHierarchy)
        {
            uiMask.gameObject.SetActive(false);
        }

    }
}
