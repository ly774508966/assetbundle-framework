using UnityEngine;
using System.Collections;

//added by wsh@2016.06.11
//功能：泛型单例类

#region 普通单例类
/// <summary>
/// 普通单例类
/// </summary>
/// <typeparam name="T"></typeparam>
/// where后的称为泛型约束，这里约束泛型参数T必须具有无参的构造函数 
public abstract class Singleton<T>
    where T : class , new()
{
    private static readonly T m_instance = new T();

    // 显示定义静态构造函数告诉C#编译器先初始化静态成员变量，保证多线程安全
    static Singleton()
    {
    }

    /// <summary>
    /// 防止创建实例
    /// </summary>
    protected Singleton()
    {
    }

    /// <summary>
    /// 获取实例
    /// </summary>
    public static T Instance
    {
        get
        {
            return m_instance;
        }
    }
}

#endregion
/// <summary>
/// MonoBehaviour单例抽象类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class UnitySingleton<T> : MonoBehaviour
    where T : Component
{
    /// <summary>
    /// 实例
    /// </summary>
    private static T m_instance;

    /// <summary>
    /// 锁，用来保证多线程安全
    /// </summary>
    private static readonly object m_lock = new object();
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                lock (m_lock)
                {
                    GameObject obj = new GameObject();
                    //TODO(wsh)为了测试，以下注释应该拿掉
                    //obj.hideFlags = HideFlags.HideAndDontSave;
                    m_instance = (T)obj.AddComponent(typeof(T));
                    obj.name = typeof(T).Name;
                }
            }
            return m_instance;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    abstract protected void OnInitialize();

    protected virtual void Awake()
    {
        //场景切换时保存当前对象不被销毁
        //AddComponent语句执行后立即被调用
        DontDestroyOnLoad(this.gameObject);
        OnInitialize();
    }

    /// <summary>
    /// 程序退出时销毁对象
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        if (m_instance != null)
        {
            Destroy(m_instance.gameObject);
        }
    }
}
