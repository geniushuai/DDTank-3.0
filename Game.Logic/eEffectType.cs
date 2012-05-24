using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic
{
    public enum eEffectType
    {
        /// <summary>
        /// 加敏捷
        /// </summary>
        AddAgilityEffect = 1,

        /// <summary>
        /// 加攻击力
        /// </summary>
        AddAttackEffect = 2,

        /// <summary>
        /// 加血
        /// </summary>
        AddBloodEffect = 3,

        /// <summary>
        /// 加伤害
        /// </summary>
        AddDamageEffect = 4,

        /// <summary>
        /// 加防御
        /// </summary>
        AddDefenceEffect = 5,

        /// <summary>
        /// 加幸运
        /// </summary>
        AddLuckyEffect = 6,

        /// <summary>
        /// 瞄准引导
        /// </summary>
        FatalEffect = 7,

        /// <summary>
        /// 隐身
        /// </summary>
        HideEffect = 8,

        /// <summary>
        /// 冰封
        /// </summary>
        IceFronzeEffect = 9,

        /// <summary>
        /// 装备冰封
        /// </summary>
        IceFronzeEquipEffect = 10,

        /// <summary>
        /// 无敌
        /// </summary>
        InvinciblyEffect = 11,

        /// <summary>
        /// 免坑
        /// </summary>
        NoHoleEffect = 12,

        /// <summary>
        /// 装备免坑
        /// </summary>
        NoHoleEquipEffect = 13,

        /// <summary>
        /// 减伤害
        /// </summary>
        ReduceDamageEffect = 14,

        /// <summary>
        /// 封印
        /// </summary>
        SealEffect = 15,

        /// <summary>
        /// 原子弹
        /// </summary>
        AtomBomb = 16,


        /// <summary>
        /// 穿甲
        /// </summary>
        ArmorPiercer = 17,

        /// <summary>
        /// 装备封印
        /// </summary>
        SealEquipEffect = 18,

        /// <summary>
        /// 立即行动
        /// </summary>
        AddTurnEquipEffect = 19,

        /// <summary>
        /// 激怒
        /// </summary>
        AddDander = 20,

        /// <summary>
        /// 反射伤害
        /// </summary>
        ReflexDamageEquipEffect = 21,


        /// <summary>
        /// 疲劳
        /// </summary>
        ReduceStrengthEffect = 22,

        /// <summary>
        /// 持续伤害
        /// </summary>
        ContinueDamageEffect = 23,

        /// <summary>
        /// 连击
        /// </summary>
        AddBombEquipEffect = 24,

        /// <summary>
        /// 免伤
        /// </summary>
        AvoidDamageEffect = 25,

        /// <summary>
        /// 暴击
        /// </summary>
        MakeCriticalEffect = 26,


        /// <summary>
        /// 吸收伤害
        /// </summary>
        AssimilateDamageEffect = 27,


        /// <summary>
        /// 吸血
        /// </summary>
        AssimilateBloodEffect = 28,


        /// <summary>
        /// 反弹
        /// </summary>
        ReflexDamageEffect = 29,


        /// <summary>
        /// 装备触发疲劳
        /// </summary>
        ReduceStrengthEquipEffect = 31,

        /// <summary>
        /// 装备持续减血
        /// </summary>
        ContinueReduceBloodEquipEffect = 32,

        /// <summary>
        /// 减血
        /// </summary>
        ContinueReduceBloodEffect = 33,

        /// <summary>
        /// 武器锁定角度
        /// </summary>
        LockDirectionEquipEffect = 34,

        /// <summary>
        /// 角度锁定
        /// </summary>
        LockDirectionEffect = 35,


        /// <summary>
        /// 装备持续减伤
        /// </summary>
        ContinueReduceDamageEquipEffect = 30,


        /// <summary>
        /// 持续减伤
        /// </summary>
        ContinueReduceDamageEffect = 36,

    }
}
