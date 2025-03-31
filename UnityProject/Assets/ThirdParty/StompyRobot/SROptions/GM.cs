using System.Collections.Generic;
using System.ComponentModel;
using SRDebugger;
using SRDebugger.Internal;

public partial class SROptions
{
    // [Category("GM-03")]	GM分组，
    // [DisplayName("金币数量")]	自定义属性或方法显示的名字，如果不使用该特性，默认显示属性名/方法名
    // [Sort(0)]	分组内部的排序
    // [NumberRange(0,1000000000000)]	用于限制数值属性范围，使用时请注意int范围
    // [Increment(10000000)]	数值属性左右按键每次变更的数字
    private int _goldCount = 0;

    [Category("GM-03 <color=red>常用</color>"), NumberRange(0, 1000000000000), Increment(10000000), Sort(2), DisplayName("金币数量")]
    public int GoldCount
    {
        get
        {
            //_goldCount = xxxx.GetRes(e_Res.Gold);
            return _goldCount;
        }
        set
        {
            _goldCount = value;
            //ResID = (int)e_Res.Gold;
            //ResCount = _goldCount;
            //todo  你要执行的GM逻辑
        }
    }

    [Category("GM-01 <color=red>GM</color>"), Sort(3), DisplayName("增加体力")]
    public void AddEnergy()
    {
        //todo  你要执行的GM逻辑
    }
}