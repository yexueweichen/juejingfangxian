using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class pooldata
//{
//    用来存储抽屉中的对象 记录的是没有使用的对象
//    private stack<gameobject> datastack = new stack<gameobject>();

//    用来记录使用中的对象的 - 改为linkedlist提升性能
//    private linkedlist<gameobject> usedlist = new linkedlist<gameobject>();

//    用于快速查找linkedlist节点的字典
//    private dictionary<gameobject, linkedlistnode<gameobject>> nodedictionary = new dictionary<gameobject, linkedlistnode<gameobject>>();

//    抽屉根对象 用来进行布局管理的对象
//    private gameobject rootobj;

//    获取容器中是否有对象
//    public int count => datastack.count;
//    public int usedcount => usedlist.count;

//    / <summary>
//    / 初始化构造函数
//    / </summary>
//    / <param name = "root" > 柜子（缓存池）父对象</param>
//    / <param name = "name" > 抽屉父对象的名字 </ param >
//    public pooldata(gameobject root, string name, gameobject usedobj)
//    {
//        开启功能时 才会动态创建 建立父子关系
//        if (poolmgr.isopenlayout)
//        {
//            创建抽屉父对象
//           rootobj = new gameobject(name);
//            和柜子父对象建立父子关系
//            rootobj.transform.setparent(root.transform);
//        }

//        创建抽屉时 外部肯定是会动态创建一个对象的
//         我们应该将其记录到 使用中的对象容器中
//        pushusedlist(usedobj);
//    }

//    / <summary>
//    / 从抽屉中弹出数据对象
//    / </summary>
//    / <returns>想要的对象数据</returns>
//    public gameobject pop()
//    {
//        gameobject obj;

//        if (count > 0)
//        {
//            从没有的容器当中取出使用
//           obj = datastack.pop();
//            现在要使用了 应该要用使用中的容器记录它
//            addtousedlist(obj);
//        }
//        else
//        {
//            取第一个节点对象 代表的就是使用时间最长的对象
//            linkedlistnode<gameobject> firstnode = usedlist.first;
//            obj = firstnode.value;

//            从使用着的对象中移除
//            usedlist.removefirst();
//            nodedictionary.remove(obj);

//            由于它还要拿出去用，所以我们应该把它又记录到 使用中的容器中去
//             并且添加到尾部 表示 比较新的开始
//            addtousedlist(obj);
//        }

//        激活对象
//        obj.setactive(true);
//        断开父子关系
//        if (poolmgr.isopenlayout)
//            obj.transform.setparent(null);

//        return obj;
//    }

//    / <summary>
//    / 将物体放入到抽屉对象中
//    / </summary>
//    / <param name = "obj" ></ param >
//    public void push(gameobject obj)
//    {
//        失活放入抽屉的对象
//        obj.setactive(false);
//        放入对应抽屉的根物体中 建立父子关系
//        if (poolmgr.isopenlayout)
//            obj.transform.setparent(rootobj.transform);
//        通过栈记录对应的对象数据
//        datastack.push(obj);
//        这个对象已经不再使用了 应该把它从记录容器中移除
//        removefromusedlist(obj);
//    }

//    / <summary>
//    / 将对象压入到使用中的容器中记录
//    / </summary>
//    / <param name = "obj" ></ param >
//    public void pushusedlist(gameobject obj)
//    {
//        addtousedlist(obj);
//    }

//    / <summary>
//    / 将对象添加到使用中链表（添加到尾部）
//    / </summary>
//    private void addtousedlist(gameobject obj)
//    {
//        linkedlistnode<gameobject> node = usedlist.addlast(obj);
//        nodedictionary[obj] = node;
//    }

//    / <summary>
//    / 从使用中链表中移除对象
//    / </summary>
//    private void removefromusedlist(gameobject obj)
//    {
//        if (nodedictionary.trygetvalue(obj, out linkedlistnode<gameobject> node))
//        {
//            usedlist.remove(node);
//            nodedictionary.remove(obj);
//        }
//    }
//}
